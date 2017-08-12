using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HyperStoreService.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.ComponentModel.DataAnnotations;
using HyperStoreServiceAPP;
namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class ProductConsumed
    {
        [Required]
        public Guid? ProductId;
        public float QuantityConsumed;
    }

    public class OrderDetails
    {
        [Required]
        public Guid? CustomerId;
        [Required]
        public List<ProductConsumed> ProductsConsumed;
        [Required]
        public CustomerOrder CustomerOrder;
        public OrderDetails(Guid? customerId, List<ProductConsumed> productsConsumed, CustomerOrder customerOrder)
        {
            this.CustomerId = customerId;
            this.ProductsConsumed = productsConsumed;
            this.CustomerOrder = customerOrder;
        }
    }

    public class FilterOrderDateRange
    {
        [Required]
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return this._startDate; }
            set { this._startDate = value; }
        }

        [Required]
        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return this._endDate; }
            set { this._endDate = value; }
        }

        public FilterOrderDateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }
    }

    public class CustomerOrderFilterCriteria
        {
        public Customer Customer;
        public string CustomerOrderNo;
        public FilterOrderDateRange DateRange;
      }

    interface PlaceCustomerOrderInt
    {
        Task<IHttpActionResult> PlaceCustomerOrder(OrderDetails orderDetails);
        Task<IHttpActionResult> GetCustomerOrders(CustomerOrderFilterCriteria customerOrderFilterCriteria);
    }

    public class PlaceCustomerOrderController : ApiController, PlaceCustomerOrderInt
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerOrders(CustomerOrderFilterCriteria customerOrderFilterCriteria)
        {
            var selectedCustomer = customerOrderFilterCriteria.Customer;
            var selectedCustomerOrderNo = customerOrderFilterCriteria.CustomerOrderNo;
            var selectedDateRange = customerOrderFilterCriteria.DateRange;

            if (selectedDateRange == null)
                throw new Exception("A Date Range cannot be null");

            var commonQuery = db.Customers
                                .Join(db.CustomerOrders,
                                customer => customer.CustomerId,
                                customerOrder => customerOrder.CustomerId,
                                (customer, customerOrder) => new OrderDetails(customer.CustomerId, null, customerOrder))
                                .Where(order => order.CustomerOrder.OrderDate.Date >= selectedDateRange.StartDate.Date &&
                                                    order.CustomerOrder.OrderDate.Date <= selectedDateRange.EndDate.Date)
                                .OrderByDescending(order => order.CustomerOrder.OrderDate);

            IQueryable<OrderDetails> query=commonQuery;
            if (selectedCustomer!= null)
            {
                query = commonQuery.Where(order => order.CustomerId == selectedCustomer.CustomerId);
            }
            if (selectedCustomerOrderNo != null)
            {
                query = commonQuery.Where(order => order.CustomerOrder.CustomerOrderNo == selectedCustomerOrderNo);
            }
            return Ok(await query.ToListAsync());
        }

        [HttpPut]
        public async Task<IHttpActionResult> PlaceCustomerOrder(OrderDetails orderDetails)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await UpdateProductStockAsync(orderDetails.ProductsConsumed);
                orderDetails.CustomerOrder.UsingWalletAmount = await UpdateWalletBalanceOfCustomer(orderDetails.CustomerId, orderDetails.CustomerOrder);
                CreateNewCustomerOrder(orderDetails.CustomerOrder);
                await AddIntoCustomerOrderProductAsync(orderDetails.ProductsConsumed, orderDetails.CustomerOrder.CustomerOrderId);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (Exception e)
            { throw e; }
            return StatusCode(HttpStatusCode.NoContent);
        }



        private async Task<Boolean> UpdateProductStockAsync(List<ProductConsumed> productsConsumed)
        {
            try
            {
                foreach (var productConsumed in productsConsumed)
                {
                    var x = await UpdateProductStock(productConsumed);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        private async Task<decimal> UpdateWalletBalanceOfCustomer(Guid? customerId, CustomerOrder customerOrder)
        {
            decimal walletAmountToBeDeducted = 0;
            try
            {
                var customer = await db.Customers.FindAsync(customerId);
                if (customer == null)
                    throw new Exception(String.Format("Customer {0} not found", customerId));

                decimal walletAmountToBeAdded = 0;
                if (customerOrder.IsPayingNow)
                {
                    if (customerOrder.IsUsingWallet)
                        walletAmountToBeDeducted = Math.Min(customerOrder.DiscountedAmount, customer.WalletBalance);
                    else
                        walletAmountToBeDeducted = 0;
                    var remainingAmount = customerOrder.DiscountedAmount - walletAmountToBeDeducted;
                    if (customerOrder.PayingAmount < remainingAmount)
                        throw new Exception(String.Format("Customer {0} payment {1} cannot be less than remaining payment {2}", customer.Name, customerOrder.PayingAmount, remainingAmount));
                    walletAmountToBeAdded = customerOrder.PayingAmount - remainingAmount;
                }
                else
                {
                    if (customerOrder.PayingAmount > customerOrder.DiscountedAmount)
                        throw new Exception(String.Format("Customer {0} paying {1} cannot be greater than discountedBillAmount {2} in Pay Later Mode ", customerId, customerOrder.PayingAmount, customerOrder.DiscountedAmount));
                    walletAmountToBeDeducted = customerOrder.DiscountedAmount - customerOrder.PayingAmount;
                }
                customer.WalletBalance -= walletAmountToBeDeducted;
                customer.WalletBalance += walletAmountToBeAdded;
                db.Entry(customer).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                throw e;
            }
            return walletAmountToBeDeducted;
        }

        private void CreateNewCustomerOrder(CustomerOrder customerOrder)
        {
            db.CustomerOrders.Add(customerOrder);
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
                        throw new Exception(String.Format("product with id {0} not found while adding product in customerOrderProduct", productConsumed.ProductId));
                    var customerOrderProduct = new CustomerOrderProduct
                    {
                        CustomerOrderProductId = Guid.NewGuid(),
                        CustomerOrderId = customerOrderId,
                        ProductId = product.ProductId,
                        DiscountPerSnapShot = product.DiscountPer,
                        DisplayCostSnapShot = product.DisplayPrice,
                        QuantityPurchased = productConsumed.QuantityConsumed
                    };
                    db.CustomerOrderProducts.Add(customerOrderProduct);
                }
            }
            catch (Exception e)
            { throw e; }
            return true;
        }

        private async Task<Boolean> UpdateProductStock(ProductConsumed productConsumed)
        {
            try
            {

                var product = await db.Products.FindAsync(productConsumed.ProductId);
                if (product == null)
                    throw new Exception(String.Format("Product with id {0} not found", productConsumed.ProductId));
                if (product.TotalQuantity < productConsumed.QuantityConsumed)
                    throw new Exception(string.Format("Product {0} is deficient by {1} units in stock," +
                        " please update the product in stock", product.Name, productConsumed.QuantityConsumed - product.TotalQuantity));
                product.TotalQuantity -= productConsumed.QuantityConsumed;
                db.Entry(product).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        private bool ProductExists(Guid id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}
