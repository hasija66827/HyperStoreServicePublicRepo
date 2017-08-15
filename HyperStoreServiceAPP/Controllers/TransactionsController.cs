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
    public class TransactionsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Transactions
        public IQueryable<Transaction> GetTransactions()
        {
            return db.Transactions;
        }

        // GET: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> GetTransaction(Guid id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // PUT: api/Transactions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTransaction(Guid id, Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != transaction.TransactionId)
            {
                return BadRequest();
            }

            db.Entry(transaction).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionExists(id))
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

        // POST: api/Transactions
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> PostTransaction(TransactionDTO transactionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var walletSnapshot = await UpdateSupplierWalletBalance(transactionDTO);
            if (walletSnapshot == null)
                return BadRequest(String.Format("Supplier with id {0} not found", transactionDTO.SupplierId));
            var transaction = CreateNewTransaction(transactionDTO, (decimal)walletSnapshot);
            //TODO: SettleUP orders
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;

            }
            return CreatedAtRoute("DefaultApi", new { id = transaction.TransactionId }, transaction);
        }

        private async Task<decimal?> UpdateSupplierWalletBalance(TransactionDTO transactionDTO)
        {
            Guid supplierId = (Guid)transactionDTO.SupplierId;
            decimal transactionAmount = (decimal)transactionDTO.TransactionAmount;
            bool IsCredit = (bool)transactionDTO.IsCredit;
            var supplier = await db.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                return null;
            var walletSnapshot = supplier.WalletBalance;
            if (IsCredit == true)
                supplier.WalletBalance += transactionAmount;
            else
                supplier.WalletBalance -= transactionAmount;
            db.Entry(supplier).State = EntityState.Modified;
            return walletSnapshot;
        }

        private Transaction CreateNewTransaction(TransactionDTO transactionDTO, decimal walletSnapshot)
        {
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                TransactionNo = Utility.GenerateSupplierTransactionNo(),
                TransactionDate = DateTime.Now,
                TransactionAmount = (decimal)transactionDTO.TransactionAmount,
                IsCredit = (bool)transactionDTO.IsCredit,
                SupplierId = (Guid)transactionDTO.SupplierId,
                WalletSnapshot = walletSnapshot
            };
            db.Transactions.Add(transaction);
            return transaction;
        }

        // DELETE: api/Transactions/5
        [ResponseType(typeof(Transaction))]
        public async Task<IHttpActionResult> DeleteTransaction(Guid id)
        {
            Transaction transaction = await db.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            db.Transactions.Remove(transaction);
            await db.SaveChangesAsync();

            return Ok(transaction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TransactionExists(Guid? id)
        {
            return db.Transactions.Count(e => e.TransactionId == id) > 0;
        }
    }
}