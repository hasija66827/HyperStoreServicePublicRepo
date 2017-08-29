using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HyperStoreService.Models;
using System.ComponentModel.DataAnnotations;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class CustomerOrdersController : ApiController, CustomerOrderInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderFilterCriteria"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<CustomerOrder>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerOrders(CustomerOrderFilterCriteria customerOrderFilterCriteria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (customerOrderFilterCriteria == null)
                return BadRequest("A filter criteria object should not be null for retrieving list of customer orders");
            var selectedCustomerId = customerOrderFilterCriteria.CustomerId;
            var selectedCustomerOrderNo = customerOrderFilterCriteria.CustomerOrderNo;
            var selectedDateRange = customerOrderFilterCriteria.OrderDateRange;

            List<CustomerOrder> result;
            try
            {
                var query = db.CustomerOrders
                                    .Where(order => order.OrderDate >= selectedDateRange.LB.Date &&
                                                    order.OrderDate <= selectedDateRange.UB.Date)
                                     .Include(co => co.Customer);
                if (selectedCustomerId != null)
                {
                    query = query.Where(order => order.CustomerId == selectedCustomerId);
                }
                if (selectedCustomerOrderNo != null)
                {
                    query = query.Where(order => order.CustomerOrderNo == selectedCustomerOrderNo);
                }
                result = await query.OrderByDescending(order => order.OrderDate).ToListAsync();
            }
            catch (Exception e)
            { throw e; }
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        [ResponseType(typeof(decimal))]
        [HttpPost]
        public async Task<IHttpActionResult> PlaceCustomerOrder(CustomerOrderDTO orderDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (orderDetail == null)
                return BadRequest("OrderDetails should not have been null while placing the customer order");
            if (orderDetail.BillAmount < orderDetail.DiscountedAmount)
                return BadRequest(string.Format("Bill Amount {0} cannot be less than Discounted Amount {1}",
                                                    orderDetail.BillAmount, orderDetail.DiscountedAmount));
            decimal deductWalletAmount = 0;
            //TODO: Verify bill amount.
            try
            {
                await UpdateStockOfProductsAsync(orderDetail.ProductsConsumed);
                deductWalletAmount = await ComputeTransactionAmountAsync(orderDetail);
                var transactionDTO = new CustomerTransactionDTO()
                {
                    CustomerId = orderDetail.CustomerId,
                    TransactionAmount = Math.Abs(deductWalletAmount),
                    IsCredit = deductWalletAmount > 0 ? true : false,
                };
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);
                var customerOrder = CreateNewCustomerOrder(orderDetail, deductWalletAmount);
                var customerOrderTransaction = CreateNewCustomerOrderTransaction(customerOrder, transaction);
                await AddIntoCustomerOrderProductAsync(orderDetail.ProductsConsumed, customerOrder.CustomerOrderId);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception e)
            { return BadRequest(e.ToString()); }
            return Ok(deductWalletAmount);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public partial class CustomerOrdersController : ApiController, CustomerOrderInterface
    {
        private async Task<Boolean> UpdateStockOfProductsAsync(List<ProductConsumed> productsConsumed)
        {
            try
            {
                foreach (var productConsumed in productsConsumed)
                {
                    var x = await UpdateProductStockAsync(productConsumed);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        private async Task<decimal> ComputeTransactionAmountAsync(CustomerOrderDTO orderDetails)
        {
            decimal walletAmountToBeDeducted = 0;
            decimal walletAmountToBeAdded = 0;
            try
            {
                var customer = await db.Customers.FindAsync(orderDetails.CustomerId);
                if (customer == null)
                    throw new Exception(String.Format("Customer {0} not found", orderDetails.CustomerId));

                if (orderDetails.IsPayingNow == true)
                {
                    if (orderDetails.IsUsingWallet == true)
                        walletAmountToBeDeducted = Math.Min((decimal)orderDetails.DiscountedAmount, (decimal)customer.WalletBalance);
                    else
                        walletAmountToBeDeducted = 0;
                    var remainingAmount = orderDetails.DiscountedAmount - walletAmountToBeDeducted;
                    if (orderDetails.PayingAmount < remainingAmount)
                        throw new Exception(String.Format("Customer {0} payment {1} cannot be less than payment {2}", customer.Name, orderDetails.PayingAmount, remainingAmount));
                    walletAmountToBeAdded = (decimal)(orderDetails.PayingAmount - remainingAmount);
                }
                else
                {
                    if (orderDetails.PayingAmount > orderDetails.DiscountedAmount)
                        throw new Exception(String.Format("Customer {0} paying {1} cannot be greater than discountedBillAmount {2} in Pay Later Mode ", orderDetails.CustomerId, orderDetails.PayingAmount, orderDetails.DiscountedAmount));
                    walletAmountToBeDeducted = (decimal)orderDetails.DiscountedAmount - (decimal)orderDetails.PayingAmount;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return walletAmountToBeDeducted - walletAmountToBeAdded;
        }

        private CustomerOrder CreateNewCustomerOrder(CustomerOrderDTO orderDetail, decimal usingWalletAmount)
        {
            var customerOrder = new CustomerOrder()
            {
                CustomerOrderId = Guid.NewGuid(),
                CustomerOrderNo = Utility.GenerateCustomerOrderNo(),
                OrderDate = DateTime.Now,
                BillAmount = (decimal)orderDetail.BillAmount,
                DiscountedAmount = (decimal)orderDetail.DiscountedAmount,
                IsPayingNow = (bool)orderDetail.IsPayingNow,
                IsUsingWallet = (bool)orderDetail.IsUsingWallet,
                PayingAmount = (decimal)orderDetail.PayingAmount,
                UsingWalletAmount = usingWalletAmount,
                CustomerId = (Guid)orderDetail.CustomerId
            };
            db.CustomerOrders.Add(customerOrder);
            return customerOrder;
        }

        private CustomerOrderTransaction CreateNewCustomerOrderTransaction(CustomerOrder customerOrder, CustomerTransaction transaction)
        {
            var customerOrderTransaction = new CustomerOrderTransaction
            {
                CustomerOrderTransactionId = Guid.NewGuid(),
                TransactionId = transaction.CustomerTransactionId,
                CustomerOrderId = customerOrder.CustomerOrderId,
            };
            db.CustomerOrderTransactions.Add(customerOrderTransaction);
            return customerOrderTransaction;
        }

        private async Task<Boolean> AddIntoCustomerOrderProductAsync(List<ProductConsumed> productsConsumed, Guid customerOrderId)
        {
            try
            {
                foreach (var productConsumed in productsConsumed)
                {
                    // Adding each product consumed in the order into the Entity CustomerOrderProduct.
                    var product = await db.Products.FindAsync(productConsumed.ProductId);
                    if (product == null)
                        throw new Exception(String.Format("product with id {0} not found while adding product in CustomerOrderProduct", productConsumed.ProductId));
                    var customerOrderProduct = new CustomerOrderProduct
                    {
                        CustomerOrderProductId = Guid.NewGuid(),
                        CustomerOrderId = customerOrderId,
                        ProductId = product.ProductId,
                        DiscountPerSnapShot = (float)product.DiscountPer,
                        DisplayCostSnapShot = (decimal)product.DisplayPrice,
                        CGSTPerSnapShot = (float)product.CGSTPer,
                        SGSTPerSnapshot = (float)product.SGSTPer,
                        QuantityConsumed = (float)productConsumed.QuantityConsumed,
                        NetValue = (decimal)(product.DisplayPrice
                                                    * (decimal)((100 - product.DiscountPer) * (100 + product.CGSTPer + product.SGSTPer) / 10000
                                                    * productConsumed.QuantityConsumed))
                    };
                    db.CustomerOrderProducts.Add(customerOrderProduct);
                }
            }
            catch (Exception e)
            { throw e; }
            return true;
        }

        private async Task<Boolean> UpdateProductStockAsync(ProductConsumed productConsumed)
        {
            try
            {
                var product = await db.Products.FindAsync(productConsumed.ProductId);
                if (product == null)
                    throw new Exception(String.Format("Product with id {0} not found", productConsumed.ProductId));
                if (product.TotalQuantity < productConsumed.QuantityConsumed)
                    throw new Exception(string.Format("Product {0} is deficient by {1} units in stock," +
                        " please update the product in stock", product.Name, productConsumed.QuantityConsumed - product.TotalQuantity));
                product.TotalQuantity -= (decimal)productConsumed.QuantityConsumed;
                db.Entry(product).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
    }
}