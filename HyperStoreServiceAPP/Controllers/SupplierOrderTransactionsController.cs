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
    public class SupplierOrderTransactionsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/SupplierOrderTransactions
        public IQueryable<SupplierOrderTransaction> GetSupplierOrderTransactions()
        {
            return db.SupplierOrderTransactions;
        }

        // GET: api/SupplierOrderTransactions/5
        [ResponseType(typeof(SupplierOrderTransaction))]
        public async Task<IHttpActionResult> GetSupplierOrderTransaction(Guid id)
        {
            SupplierOrderTransaction supplierOrderTransaction = await db.SupplierOrderTransactions.FindAsync(id);
            if (supplierOrderTransaction == null)
            {
                return NotFound();
            }

            return Ok(supplierOrderTransaction);
        }

        // PUT: api/SupplierOrderTransactions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSupplierOrderTransaction(Guid id, SupplierOrderTransaction supplierOrderTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != supplierOrderTransaction.SupplierOrderTransactionId)
            {
                return BadRequest();
            }

            db.Entry(supplierOrderTransaction).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierOrderTransactionExists(id))
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

        // POST: api/SupplierOrderTransactions
        [ResponseType(typeof(SupplierOrderTransaction))]
        public async Task<IHttpActionResult> PostSupplierOrderTransaction(SupplierOrderTransaction supplierOrderTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SupplierOrderTransactions.Add(supplierOrderTransaction);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplierOrderTransactionExists(supplierOrderTransaction.SupplierOrderTransactionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = supplierOrderTransaction.SupplierOrderTransactionId }, supplierOrderTransaction);
        }

        // DELETE: api/SupplierOrderTransactions/5
        [ResponseType(typeof(SupplierOrderTransaction))]
        public async Task<IHttpActionResult> DeleteSupplierOrderTransaction(Guid id)
        {
            SupplierOrderTransaction supplierOrderTransaction = await db.SupplierOrderTransactions.FindAsync(id);
            if (supplierOrderTransaction == null)
            {
                return NotFound();
            }

            db.SupplierOrderTransactions.Remove(supplierOrderTransaction);
            await db.SaveChangesAsync();

            return Ok(supplierOrderTransaction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SupplierOrderTransactionExists(Guid? id)
        {
            return db.SupplierOrderTransactions.Count(e => e.SupplierOrderTransactionId == id) > 0;
        }
    }
}