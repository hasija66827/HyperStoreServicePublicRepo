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
            decimal deductWalletAmount = 0;
            //TODO: Verify pay amount.
            try
            {
                await UpdateStockOfProductsAsync(orderDetail.ProductsConsumed);
                deductWalletAmount = await ComputeTransactionAmountAsync(orderDetail);

                var customerOrder = CreateNewCustomerOrder(orderDetail, deductWalletAmount);
                var transactionDTO = new CustomerTransactionDTO()
                {
                    CustomerId = orderDetail.CustomerId,
                    TransactionAmount = Math.Abs(deductWalletAmount),
                    IsCredit = deductWalletAmount > 0 ? true : false,
                    Description = customerOrder.CustomerOrderNo,
                };
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);

                var customerOrderTransaction = CreateNewCustomerOrderTransaction(customerOrder, transaction);

                await AddIntoCustomerOrderProductAsync(orderDetail.ProductsConsumed, customerOrder.CustomerOrderId);

                //Data Analytics call
                await UpdateCustomerNetWorthAsync(orderDetail);

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

        private async Task<decimal?> UpdateCustomerNetWorthAsync(CustomerOrderDTO orderDTO)
        {
            var customer = await db.Customers.FindAsync(orderDTO.CustomerId);
            if (customer == null)
                throw new Exception(String.Format("Customer with id {0} not found while updating its networth", orderDTO.CustomerId));
            customer.NetWorth += orderDTO.CustomerBillingSummary.PayAmount;
            db.Entry(customer).State = EntityState.Modified;
            return customer.NetWorth;
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

                var payAmount = (decimal)orderDetails.CustomerBillingSummary.PayAmount;
                if (orderDetails.IsPayingNow == true)
                {
                    if (orderDetails.IsUsingWallet == true)
                        walletAmountToBeDeducted = Math.Min((decimal)payAmount, (decimal)customer.WalletBalance);
                    else
                        walletAmountToBeDeducted = 0;
                    var remainingAmount = payAmount - walletAmountToBeDeducted;
                    if (orderDetails.PayingAmount < remainingAmount)
                        throw new Exception(String.Format("Customer {0} payment {1} cannot be less than payment {2}", customer.Name, orderDetails.PayingAmount, remainingAmount));
                    walletAmountToBeAdded = (decimal)(orderDetails.PayingAmount - remainingAmount);
                }
                else
                {
                    if (orderDetails.PayingAmount > payAmount)
                        throw new Exception(String.Format("Customer {0} paying {1} cannot be greater than discountedBillAmount {2} in Pay Later Mode ",
                            orderDetails.CustomerId, orderDetails.PayingAmount, payAmount));
                    walletAmountToBeDeducted = payAmount - (decimal)orderDetails.PayingAmount;
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
            var CBS = orderDetail.CustomerBillingSummary;
            var customerOrder = new CustomerOrder()
            {
                CustomerOrderId = Guid.NewGuid(),
                CustomerOrderNo = Utility.GenerateCustomerOrderNo(),
                OrderDate = DateTime.Now,
                TotalItems = (int)CBS.TotalItems,
                TotalQuantity = (decimal)CBS.TotalQuantity,
                CartAmount = (decimal)CBS.CartAmount,
                DiscountAmount = (decimal)CBS.DiscountAmount,
                Tax = (decimal)CBS.Tax,
                PayAmount = (decimal)CBS.PayAmount,
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
                    var sellingPrice = (decimal)(product.DisplayPrice * (decimal)((100 - product.DiscountPer) * (100 + product.CGSTPer + product.SGSTPer) / 10000));
                    var customerOrderProduct = new CustomerOrderProduct
                    {
                        CustomerOrderProductId = Guid.NewGuid(),
                        CustomerOrderId = customerOrderId,
                        ProductId = product.ProductId,
                        DiscountPerSnapShot = (decimal)product.DiscountPer,
                        DisplayCostSnapShot = (decimal)product.DisplayPrice,
                        CGSTPerSnapShot = (decimal)product.CGSTPer,
                        SGSTPerSnapshot = (decimal)product.SGSTPer,
                        QuantityConsumed = (decimal)productConsumed.QuantityConsumed,
                        SellingPrice = sellingPrice,
                        NetValue = sellingPrice * (decimal)productConsumed.QuantityConsumed,
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

                if (product.TotalQuantity < DeficientStockHit.DeficientStockCriteria)
                {
                    AddProductToDeficientStockHit(product);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        private void AddProductToDeficientStockHit(Product product)
        {
            if (product == null)
                throw new Exception("Product should not have been null, while adding the product to DeficientStock\n");
            var currentDate = DateTime.Now.Date;
            var IsExist = db.DeficientStockHits.Count(p => p.ProductId == product.ProductId &&
                                                        p.TimeStamp == currentDate) > 0;

            // Preserving Unique key constraints on DeficientStockHit
            if (!IsExist)
            {
                var deficientStock = new DeficientStockHit()
                {
                    DeficientStockHitId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    QuantitySnapshot = product.TotalQuantity,
                    TimeStamp = DateTime.Now.Date,
                };
                db.DeficientStockHits.Add(deficientStock);
            }
        }
    }
}