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
        [Required]
        public float? QuantityConsumed;
    }

    public class CustomerOrderDetails
    {
        [Required]
        public List<ProductConsumed> ProductsConsumed;
        [Required]
        public Guid? CustomerId { get; set; }
        [Required]
        public decimal? BillAmount { get; set; }
        [Required]
        public decimal? DiscountedAmount { get; set; }
        [Required]
        public bool? IsPayingNow { get; set; }
        [Required]
        public bool? IsUsingWallet { get; set; }
        [Required]
        public decimal? PayingAmount { get; set; }
    }

    interface PlaceCustomerOrderInt
    {
        Task<IHttpActionResult> PlaceCustomerOrder(CustomerOrderDetails orderDetails);
    }

    public class PlaceCustomerOrderController : ApiController, PlaceCustomerOrderInt
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpPut]
        public async Task<IHttpActionResult> PlaceCustomerOrder(CustomerOrderDetails orderDetails)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (orderDetails == null)
                return BadRequest("OrderDetails should not have been null while placing the customer order");
            if (orderDetails.BillAmount < orderDetails.DiscountedAmount)
                return BadRequest(string.Format("Bill Amount {0} cannot be less than Discounted Amount {1}",
                                                    orderDetails.BillAmount, orderDetails.DiscountedAmount));
            //TODO: Verify bill amount, if it is given else assign it to bill amount.
            try
            {
                await UpdateProductStockAsync(orderDetails.ProductsConsumed);
                var usingWalletAmount = await UpdateWalletBalanceOfCustomer(orderDetails);
                var customerOrder = new CustomerOrder()
                {
                    CustomerOrderId = Guid.NewGuid(),
                    CustomerOrderNo = Utility.GenerateCustomerOrderNo(),
                    OrderDate = DateTime.Now,
                    BillAmount = orderDetails.BillAmount,
                    DiscountedAmount = orderDetails.DiscountedAmount,
                    IsPayingNow = orderDetails.IsPayingNow,
                    IsUsingWallet = orderDetails.IsUsingWallet,
                    PayingAmount = orderDetails.PayingAmount,
                    UsingWalletAmount = usingWalletAmount,
                    CustomerId = orderDetails.CustomerId
                };
                CreateNewCustomerOrder(customerOrder);
                await AddIntoCustomerOrderProductAsync(orderDetails.ProductsConsumed, customerOrder.CustomerOrderId);
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

        private async Task<decimal> UpdateWalletBalanceOfCustomer(CustomerOrderDetails orderDetails)
        {
            decimal walletAmountToBeDeducted = 0;

            try
            {
                var customer = await db.Customers.FindAsync(orderDetails.CustomerId);
                if (customer == null)
                    throw new Exception(String.Format("Customer {0} not found", orderDetails.CustomerId));

                decimal walletAmountToBeAdded = 0;
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

        private async Task<Boolean> AddIntoCustomerOrderProductAsync(List<ProductConsumed> productsConsumed, Guid? customerOrderId)
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
                product.TotalQuantity -= (float)productConsumed.QuantityConsumed;
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
