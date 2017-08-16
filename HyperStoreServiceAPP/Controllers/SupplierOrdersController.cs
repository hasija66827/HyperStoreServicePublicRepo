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
    public class ProductPurchased
    {
        [Required]
        public Guid? ProductId { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float? QuantityPurchased { get; set; }
        [Required]
        public decimal? PurchasePricePerUnit { get; set; }
    }

    public class SupplierOrderDTO
    {
        [Required]
        public List<ProductPurchased> ProductsPurchased { get; set; }
        [Required]
        public Guid? SupplierId { get; set; }
        [Required]
        public decimal? BillAmount { get; set; }
        [Required]
        public decimal? PaidAmount { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        [Required]
        [Range(0, 100)]
        public float IntrestRate { get; set; }
    }

    public class SupplierOrdersController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/SupplierOrders
        public IQueryable<SupplierOrder> GetSupplierOrders()
        {
            return db.SupplierOrders;
        }

        // GET: api/SupplierOrders/5
        [ResponseType(typeof(SupplierOrder))]
        public async Task<IHttpActionResult> GetSupplierOrder(Guid id)
        {
            SupplierOrder supplierOrder = await db.SupplierOrders.FindAsync(id);
            if (supplierOrder == null)
            {
                return NotFound();
            }

            return Ok(supplierOrder);
        }

        // PUT: api/SupplierOrders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplierOrder(Guid id, SupplierOrder supplierOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplierOrder.SupplierOrderId)
            {
                return BadRequest();
            }

            db.Entry(supplierOrder).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/SupplierOrders
        [ResponseType(typeof(SupplierOrder))]
        public async Task<IHttpActionResult> PostSupplierOrder(SupplierOrderDTO orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (orderDetail == null)
                return BadRequest("OrderDetails should not have been null while placing the supplier order");
            if (orderDetail.PaidAmount > orderDetail.BillAmount)
                return BadRequest(String.Format("PaidAmount {0} should be less than Bill Amount {1}", orderDetail.PaidAmount, orderDetail.BillAmount));
            if (orderDetail.DueDate < DateTime.Now)
                return BadRequest(String.Format("DueDate{0} cannot be before current Date {1}", orderDetail.DueDate, DateTime.Now));
            //TODO: Verify bill amount.
            try
            {
                await UpdateStockOfProductsAsync(orderDetail.ProductsPurchased);
                var IsUpdated = await UpdateWalletBalanceOfSupplierAsync(orderDetail);
                var supplierOrderId = CreateNewSupplierOrder(orderDetail);
                await AddIntoSupplierOrderProductAsync(orderDetail.ProductsPurchased, supplierOrderId);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = orderDetail.BillAmount }, orderDetail);
        }

        private async Task<bool> UpdateWalletBalanceOfSupplierAsync(SupplierOrderDTO orderDetails)
        {
            try
            {
                var supplier = await db.Suppliers.FindAsync(orderDetails.SupplierId);
                if (supplier == null)
                    throw new Exception(String.Format("Supplier with id {0} not found", orderDetails.SupplierId));
                var usingWalletAmount = orderDetails.BillAmount - orderDetails.PaidAmount;
                supplier.WalletBalance += (decimal)usingWalletAmount;
                db.Entry(supplier).State = EntityState.Modified;
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Guid CreateNewSupplierOrder(SupplierOrderDTO orderDetail)
        {
            var supplierOrder = new SupplierOrder
            {
                SupplierOrderId = Guid.NewGuid(),
                BillAmount = (decimal)orderDetail.BillAmount,
                DueDate = (DateTime)orderDetail.DueDate,
                OrderDate = DateTime.Now,
                PaidAmount = (decimal)orderDetail.PaidAmount,
                SupplierOrderNo = Utility.GenerateSupplierOrderNo(),
                SupplierId = (Guid)orderDetail.SupplierId
            };
            db.SupplierOrders.Add(supplierOrder);
            return supplierOrder.SupplierOrderId;
        }

        private async Task<bool> AddIntoSupplierOrderProductAsync(List<ProductPurchased> productsPurchased, Guid supplierOrderId)
        {
            foreach (var productPurchased in productsPurchased)
            {
                // Adding each product purchased in the order into the Entity SupplierOrderProduct.
                var product = await db.Products.FindAsync(productPurchased.ProductId);
                if (product == null)
                    throw new Exception(String.Format("product with id {0} not found while adding product in SupplierOrderProduct", productPurchased.ProductId));
                var supplierOrderProduct = new SupplierOrderProduct
                {
                    SupplierOrderProductId = Guid.NewGuid(),
                    PurchasePrice = (decimal)productPurchased.PurchasePricePerUnit,
                    QuantityPurchased = (float)productPurchased.QuantityPurchased,
                    SupplierOrderId = supplierOrderId,
                    ProductId = productPurchased.ProductId
                };
                db.SupplierOrderProducts.Add(supplierOrderProduct);
            }
            return true;
        }

        private async Task<Boolean> UpdateStockOfProductsAsync(List<ProductPurchased> productsPurchased)
        {
            try
            {
                foreach (var productPurchased in productsPurchased)
                {
                    var x = await UpdateProductStockAsync(productPurchased);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }

        private async Task<Boolean> UpdateProductStockAsync(ProductPurchased productPurchased)
        {
            var product = await db.Products.FindAsync(productPurchased.ProductId);
            if (product == null)
                throw new Exception(String.Format("Product with id {0} not found", productPurchased.ProductId));
            product.TotalQuantity += (float)productPurchased.QuantityPurchased;
            db.Entry(product).State = EntityState.Modified;
            return true;
        }

        // DELETE: api/SupplierOrders/5
        [ResponseType(typeof(SupplierOrder))]
        public async Task<IHttpActionResult> DeleteSupplierOrder(Guid id)
        {
            SupplierOrder supplierOrder = await db.SupplierOrders.FindAsync(id);
            if (supplierOrder == null)
            {
                return NotFound();
            }

            db.SupplierOrders.Remove(supplierOrder);
            await db.SaveChangesAsync();

            return Ok(supplierOrder);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierOrderExists(Guid? id)
        {
            return db.SupplierOrders.Count(e => e.SupplierOrderId == id) > 0;
        }
    }
}