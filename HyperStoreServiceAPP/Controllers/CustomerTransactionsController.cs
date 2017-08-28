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
    public class CustomerTransactionsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/CustomerTransactions
        public IQueryable<CustomerTransaction> GetCustomerTransactions()
        {
            return db.CustomerTransactions;
        }

        // GET: api/CustomerTransactions/5
        [ResponseType(typeof(CustomerTransaction))]
        public async Task<IHttpActionResult> GetCustomerTransaction(Guid id)
        {
            CustomerTransaction customerTransaction = await db.CustomerTransactions.FindAsync(id);
            if (customerTransaction == null)
            {
                return NotFound();
            }

            return Ok(customerTransaction);
        }

        // POST: api/CustomerTransactions
        [ResponseType(typeof(CustomerTransaction))]
        public async Task<IHttpActionResult> PostCustomerTransaction(CustomerTransaction customerTransaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerTransactions.Add(customerTransaction);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerTransactionExists(customerTransaction.CustomerTransactionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = customerTransaction.CustomerTransactionId }, customerTransaction);
        }

        // DELETE: api/CustomerTransactions/5
        [ResponseType(typeof(CustomerTransaction))]
        public async Task<IHttpActionResult> DeleteCustomerTransaction(Guid id)
        {
            CustomerTransaction customerTransaction = await db.CustomerTransactions.FindAsync(id);
            if (customerTransaction == null)
            {
                return NotFound();
            }

            db.CustomerTransactions.Remove(customerTransaction);
            await db.SaveChangesAsync();

            return Ok(customerTransaction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerTransactionExists(Guid id)
        {
            return db.CustomerTransactions.Count(e => e.CustomerTransactionId == id) > 0;
        }
    }
}