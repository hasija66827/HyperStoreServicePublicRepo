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
    public class SuppliersController : ApiController, SupplierControllerInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Suppliers
        [HttpGet]
        [ResponseType(typeof(List<Supplier>))]
        public async Task<IHttpActionResult> GetSuppliers(SupplierFilterCriteria sfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            IQueryable<Supplier> query = db.Suppliers;
            if (sfc == null)
                return Ok(await query.ToListAsync());
            try
            {
                if (sfc.WalletAmount != null)
                    query = db.Suppliers.Where(s => s.WalletBalance >= sfc.WalletAmount.LB &&
                                                    s.WalletBalance <= sfc.WalletAmount.UB);

                if (sfc.SupplierId != null)
                    query = query.Where(s => s.SupplierId == sfc.SupplierId);
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch
            {
                throw;
            }
        }

        // GET: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> GetSupplier(Guid id)
        {
            Supplier supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            return Ok(supplier);
        }

        // PUT: api/Suppliers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplier(Guid id, Supplier supplier)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplier.SupplierId)
            {
                return BadRequest();
            }

            db.Entry(supplier).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
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

        // POST: api/Suppliers
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> PostSupplier(SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var supplier = new Supplier()
            {
                SupplierId = Guid.NewGuid(),
                Address = supplierDTO.Address,
                GSTIN = supplierDTO.GSTIN,
                MobileNo = supplierDTO.MobileNo,
                Name = supplierDTO.Name,
                WalletBalance = 0,
            };
            db.Suppliers.Add(supplier);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplierExists(supplier.SupplierId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = supplier.SupplierId }, supplier);
        }

        // DELETE: api/Suppliers/5
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> DeleteSupplier(Guid id)
        {
            Supplier supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }

            db.Suppliers.Remove(supplier);
            await db.SaveChangesAsync();

            return Ok(supplier);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierExists(Guid? id)
        {
            return db.Suppliers.Count(e => e.SupplierId == id) > 0;
        }
    }
}