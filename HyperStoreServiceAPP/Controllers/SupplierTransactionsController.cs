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
using HyperStoreServiceAPP.DTO;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class SupplierTransactionsController : ApiController, ISupplierTransaction
    {
        private HyperStoreServiceContext db ;

        // GET: api/Transactions/5
        [ResponseType(typeof(List<SupplierTransaction>))]
        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid userId, SupplierTransactionFilterCriteria transactionFilterCriteria)
        {
            if (transactionFilterCriteria == null)
                return BadRequest("TransactionFilterCriteria cannont be null while retreiving the transaction for supplier");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            db = UtilityAPI.RetrieveDBContext(userId);

            var supplierId = transactionFilterCriteria.SupplierId;
            var transactions = await db.SupplierTransactions.Where(t => t.SupplierId == supplierId)
                                                             .OrderByDescending(t => t.TransactionDate).ToListAsync();
            Supplier supplier;
            if (transactions == null || transactions.Count() == 0)
            {
                supplier = await db.Suppliers.FindAsync(supplierId);
                if (supplier == null)
                    return BadRequest(String.Format("Supplier of id {0} does not exists", supplierId));
            }
            return Ok(transactions);
        }

        // POST: api/Transactions
        [ResponseType(typeof(SupplierTransaction))]
        [HttpPost]
        public async Task<IHttpActionResult> Post(Guid userId, SupplierTransactionDTO transactionDTO)
        {
            if (transactionDTO == null)
                return BadRequest("TransactionDTO cannot be null, on creating transaction for supplier");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            db = UtilityAPI.RetrieveDBContext(userId);

            try
            {
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);
                await db.SaveChangesAsync();
                return Ok(transaction);
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}