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
using HyperStoreServiceAPP.DTO;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class CustomerOrdersController : ApiController, ICustomerOrder
    {
        private HyperStoreServiceContext db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerOrderFilterCriteria"></param>
        /// <returns></returns>
        [ResponseType(typeof(List<CustomerOrder>))]
        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid userId, CustomerOrderFilterCriteria customerOrderFilterCriteria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (customerOrderFilterCriteria == null)
                return BadRequest("A filter criteria object should not be null for retrieving list of customer orders");
            db = UtilityAPI.RetrieveDBContext(userId);

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

        [HttpGet]
        [ResponseType(typeof(Int32))]
        public IHttpActionResult GetTotalRecordsCount(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.CustomerOrders.Count());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderDetail"></param>
        /// <returns></returns>
        [ResponseType(typeof(decimal))]
        [HttpPost]
        public async Task<IHttpActionResult> Post(Guid userId, CustomerOrderDTO orderDetail)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (orderDetail == null)
                throw new Exception("OrderDetails should not have been null while placing the customer order");

            if (orderDetail.DueDate < DateTime.Now)
                return BadRequest(String.Format("DueDate {0} cannot be before current Date {1}", orderDetail.DueDate, DateTime.Now.Date));

            var payingAmount = orderDetail.PayingAmount;
            var billAmount = orderDetail.CustomerBillingSummaryDTO.BillAmount;
            if (payingAmount > billAmount)
                return BadRequest(String.Format("Paying Amount {0} should be less than Bill Amount {1}", payingAmount, billAmount));

            db = UtilityAPI.RetrieveDBContext(userId);

            var IsBillingSummaryCorrect = await ValidateBillingSummary(orderDetail);
            if (!IsBillingSummaryCorrect)
                throw new Exception("BillingSummary is not correct.");


            try
            {
                await UpdateStockOfProductsAsync(orderDetail.ProductsConsumed);

                var customerOrder = CreateNewCustomerOrder(orderDetail);
                var transactionDTO = new CustomerTransactionDTO()
                {
                    CustomerId = orderDetail.CustomerId,
                    IsCredit = true,
                    IsCashbackTransaction = false,
                    TransactionAmount = billAmount - payingAmount,
                    Description = customerOrder.CustomerOrderNo,
                };
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);

                var customerOrderTransaction = CreateNewCustomerOrderTransaction(customerOrder, transaction);

                await AddIntoCustomerOrderProductAsync(orderDetail.ProductsConsumed, customerOrder.CustomerOrderId);

                //Data Analytics call
                await UpdateCustomerNetWorthAsync(orderDetail);

                await db.SaveChangesAsync();
                return Ok(transactionDTO.TransactionAmount);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception e)
            { return BadRequest(e.ToString()); }

        }

        private async Task<bool> ValidateBillingSummary(CustomerOrderDTO orderDetail)
        {
            decimal? cartAmount = 0;
            decimal? discountAmount = 0;

            if (orderDetail.CustomerBillingSummaryDTO.TotalItems != orderDetail.ProductsConsumed.Count)
                return false;

            var quantityErrorDiff = orderDetail.CustomerBillingSummaryDTO.TotalQuantity - orderDetail.ProductsConsumed.Sum(p => p.QuantityConsumed);
            if (!Utility.IsErrorAcceptable(quantityErrorDiff))
                return false;

            foreach (var productConsumed in orderDetail.ProductsConsumed)
            {
                var product = await db.Products.FindAsync(productConsumed.ProductId);
                if (product == null)
                    throw new Exception(string.Format("Product with id {0} not found.", productConsumed.ProductId));
                cartAmount += productConsumed.QuantityConsumed * product.MRP;
                discountAmount += productConsumed.QuantityConsumed * product.DiscountPer * product.MRP / 100;
            }

            var cartAmountErrorDiff = cartAmount - orderDetail.CustomerBillingSummaryDTO.CartAmount;
            var discountAmountErrorDiff = discountAmount - orderDetail.CustomerBillingSummaryDTO.DiscountAmount;
            if (!Utility.IsErrorAcceptable(cartAmountErrorDiff) || !Utility.IsErrorAcceptable(discountAmountErrorDiff))
                return false;
            return true;
        }

        private bool Validate_CartAmount_DiscountedAmount(CustomerOrderDTO orderDetail, List<Product> products)
        {
            return true;
        }

        private async Task<decimal?> UpdateCustomerNetWorthAsync(CustomerOrderDTO orderDTO)
        {
            var customer = await db.Customers.FindAsync(orderDTO.CustomerId);
            if (customer == null)
                throw new Exception(String.Format("Customer with id {0} not found while updating its networth", orderDTO.CustomerId));
            customer.NetWorth += orderDTO.CustomerBillingSummaryDTO.BillAmount;
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

    public partial class CustomerOrdersController : ApiController, ICustomerOrder
    {
        private async Task<Boolean> UpdateStockOfProductsAsync(List<ProductConsumedDTO> productsConsumed)
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

        private CustomerOrder CreateNewCustomerOrder(CustomerOrderDTO orderDetail)
        {
            var CBS = orderDetail.CustomerBillingSummaryDTO;
            var customerOrder = new CustomerOrder()
            {
                CustomerId = (Guid)orderDetail.CustomerId,
                CustomerOrderId = Guid.NewGuid(),
                CustomerOrderNo = Utility.GenerateCustomerOrderNo(),
                DueDate = (DateTime)orderDetail.DueDate,
                InterestRate = (decimal)orderDetail.IntrestRate,
                OrderDate = DateTime.Now,
                CartAmount = (decimal)CBS.CartAmount,
                DiscountAmount = (decimal)CBS.DiscountAmount,
                Tax = (decimal)CBS.Tax,
                BillAmount = (decimal)CBS.BillAmount,
                SettledPayedAmount = (decimal)orderDetail.PayingAmount,
                PayedAmount = (decimal)orderDetail.PayingAmount,
                TotalItems = (int)CBS.TotalItems,
                TotalQuantity = (decimal)CBS.TotalQuantity,
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

        private async Task<Boolean> AddIntoCustomerOrderProductAsync(List<ProductConsumedDTO> productsConsumed, Guid customerOrderId)
        {
            try
            {
                foreach (var productConsumed in productsConsumed)
                {
                    // Adding each product consumed in the order into the Entity CustomerOrderProduct.
                    var product = await db.Products.FindAsync(productConsumed.ProductId);
                    if (product == null)
                        throw new Exception(String.Format("product with id {0} not found while adding product in CustomerOrderProduct", productConsumed.ProductId));
                    var valueIncTax = product.MRP * (100 - product.DiscountPer) / 100;
                    var customerOrderProduct = new CustomerOrderProduct
                    {
                        CustomerOrderProductId = Guid.NewGuid(),
                        CustomerOrderId = customerOrderId,
                        ProductId = product.ProductId,
                        DiscountPerSnapShot = (decimal)product.DiscountPer,
                        MRPSnapShot = (decimal)product.MRP,
                        CGSTPerSnapShot = (decimal)product.CGSTPer,
                        SGSTPerSnapshot = (decimal)product.SGSTPer,
                        QuantityConsumed = (decimal)productConsumed.QuantityConsumed,
                        ValueIncTaxSnapShot = (decimal)valueIncTax,
                        NetValue = (decimal)(valueIncTax * productConsumed.QuantityConsumed),
                    };
                    db.CustomerOrderProducts.Add(customerOrderProduct);
                }
            }
            catch (Exception e)
            { throw e; }
            return true;
        }

        private async Task<Boolean> UpdateProductStockAsync(ProductConsumedDTO productConsumed)
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