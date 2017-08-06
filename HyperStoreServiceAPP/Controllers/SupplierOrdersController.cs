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

namespace HyperStoreServiceAPP.Controllers
{
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
        public async Task<IHttpActionResult> PostSupplierOrder(SupplierOrder supplierOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierOrders.Add(supplierOrder);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplierOrderExists(supplierOrder.SupplierOrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = supplierOrder.SupplierOrderId }, supplierOrder);
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

        private bool SupplierOrderExists(Guid id)
        {
            return db.SupplierOrders.Count(e => e.SupplierOrderId == id) > 0;
        }
    }
}