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
        public OrderDetails() { }
    }

    public class PlaceCustomerOrderController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpPut]
        public async Task<IHttpActionResult> PlaceCustomerOrder(OrderDetails orderDetails)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                await UpdateProductStockAsync(orderDetails.ProductsConsumed);
                CreateNewCustomerOrder(orderDetails.CustomerOrder);
                await AddIntoCustomerOrderProductAsync(orderDetails.ProductsConsumed, orderDetails.CustomerOrder.CustomerOrderId);
                await UpdateWalletBalanceOfCustomer(orderDetails.CustomerId, orderDetails.CustomerOrder);
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

        private async Task<Boolean> UpdateWalletBalanceOfCustomer(Guid? customerId,CustomerOrder customerOrder)
        {
            try
            {
                var customer = await db.Customers.FindAsync(customerId);
                if (customer == null)
                    throw new Exception(String.Format("Customer {0} not found", customerId));
                decimal walletAmountToBeDeducted = 0;
                decimal walletAmountToBeAdded = 0;
                if (customerOrder.IsPayingNow)
                {
                    if (customerOrder.IsUsingWallet)
                        walletAmountToBeDeducted = Math.Min(customerOrder.DiscountedAmount, customer.WalletBalance);
                    else
                        walletAmountToBeDeducted = 0;
                    var remainingAmount = customerOrder.DiscountedAmount - walletAmountToBeDeducted;
                    if (customerOrder.PayingAmount< remainingAmount)
                        throw new Exception(String.Format("Customers {0} payment {1} cannot be less than remaining payment {2}", customerId, customerOrder.PayingAmount, remainingAmount));
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
            return true;
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

        private async Task<Boolean> UpdateProductStock(ProductConsumed productConsumed)
        {
            try
            {

                var product = await db.Products.FindAsync(productConsumed.ProductId);
                if (product == null)
                    throw new Exception(String.Format("Product {0} not found", productConsumed.ProductId));
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
