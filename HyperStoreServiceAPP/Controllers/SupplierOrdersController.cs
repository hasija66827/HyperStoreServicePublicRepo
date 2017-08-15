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
            //TODO: Verify bill amount.
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = orderDetail.BillAmount }, orderDetail);
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