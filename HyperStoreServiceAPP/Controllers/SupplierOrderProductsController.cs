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
    public class SupplierOrderProductsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/SupplierOrderProducts
        public IQueryable<SupplierOrderProduct> GetSupplierOrderProducts()
        {
            return db.SupplierOrderProducts;
        }

        // GET: api/SupplierOrderProducts/5
        [ResponseType(typeof(List<SupplierOrderProduct>))]
        public async Task<IHttpActionResult> GetSupplierOrderProduct(Guid? id)
        {
            if (id == null)
                return BadRequest("SupplierOrderId should not be null");

            List<SupplierOrderProduct> queryResult;
            try
            {
                var query = db.SupplierOrderProducts
                                .Where(sop => sop.SupplierOrderId == id)
                                .Include(sop => sop.Product);
                queryResult = await query.ToListAsync();
            }
            catch (Exception e)
            { throw e; }
            return Ok(queryResult);
        }

        // PUT: api/SupplierOrderProducts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplierOrderProduct(Guid id, SupplierOrderProduct supplierOrderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplierOrderProduct.SupplierOrderProductId)
            {
                return BadRequest();
            }

            db.Entry(supplierOrderProduct).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierOrderProductExists(id))
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

        // POST: api/SupplierOrderProducts
        [ResponseType(typeof(SupplierOrderProduct))]
        public async Task<IHttpActionResult> PostSupplierOrderProduct(SupplierOrderProduct supplierOrderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierOrderProducts.Add(supplierOrderProduct);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplierOrderProductExists(supplierOrderProduct.SupplierOrderProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = supplierOrderProduct.SupplierOrderProductId }, supplierOrderProduct);
        }

        // DELETE: api/SupplierOrderProducts/5
        [ResponseType(typeof(SupplierOrderProduct))]
        public async Task<IHttpActionResult> DeleteSupplierOrderProduct(Guid id)
        {
            SupplierOrderProduct supplierOrderProduct = await db.SupplierOrderProducts.FindAsync(id);
            if (supplierOrderProduct == null)
            {
                return NotFound();
            }

            db.SupplierOrderProducts.Remove(supplierOrderProduct);
            await db.SaveChangesAsync();

            return Ok(supplierOrderProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierOrderProductExists(Guid? id)
        {
            return db.SupplierOrderProducts.Count(e => e.SupplierOrderProductId == id) > 0;
        }
    }
}