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
    public partial class SupplierTransactionsController : ApiController, SupplierTransactionControllerInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Transactions/5
        [ResponseType(typeof(List<SupplierTransaction>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions(SupplierTransactionFilterCriteria transactionFilterCriteria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var supplierId = transactionFilterCriteria.SupplierId;
            var transactions = await db.SupplierTransactions.Where(t => t.SupplierId == supplierId)
                                                    .OrderByDescending(t => t.TransactionDate).ToListAsync();
            return Ok(transactions);
        }

        // POST: api/Transactions
        [ResponseType(typeof(SupplierTransaction))]
        [HttpPost]
        public async Task<IHttpActionResult> PostTransaction(SupplierTransactionDTO transactionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var transaction = await transactionDTO.CreateNewTransactionAsync(db);
                var settleUpOrders = SettleUpOrders(transaction);
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

        private bool TransactionExists(Guid? id)
        {
            return db.SupplierTransactions.Count(e => e.SupplierTransactionId == id) > 0;
        }
    }

    public partial class SupplierTransactionsController : ApiController, SupplierTransactionControllerInterface
    {
        private List<SupplierOrder> SettleUpOrders(SupplierTransaction transaction)
        {
            List<SupplierOrder> settleUpSupplierOrder = new List<SupplierOrder>();
            if (transaction.IsCredit)
                throw new Exception(String.Format("While settling up the orders transaction {0} cannot be of type credit", transaction.SupplierTransactionId));
            var partiallyPaidOrders = db.SupplierOrders.Where(so => so.SupplierId == transaction.SupplierId &&
                                                                   so.BillAmount - so.PaidAmount > 0)
                                                       .OrderBy(wo => wo.OrderDate);
            var debitTransactionAmount = transaction.TransactionAmount;
            foreach (var partiallyPaidOrder in partiallyPaidOrders)
            {
                if (debitTransactionAmount <= 0)
                    break;
                var remainingAmount = partiallyPaidOrder.BillAmount - partiallyPaidOrder.PaidAmount;
                if (remainingAmount < 0)
                    throw new Exception(string.Format("Supplier OrderNo {0}, Amount remaining to be paid: {1} cannot be less than zero", partiallyPaidOrder.SupplierOrderNo, remainingAmount));
                decimal payingAmountForOrder = Math.Min(remainingAmount, debitTransactionAmount);
                debitTransactionAmount -= payingAmountForOrder;
                var IsOrderSettleUp = SettleUpOrder(partiallyPaidOrder, payingAmountForOrder);
                settleUpSupplierOrder.Add(partiallyPaidOrder);
                db.SupplierOrderTransactions.Add(new SupplierOrderTransaction
                {
                    SupplierOrderTransactionId = Guid.NewGuid(),
                    SupplierOrderId = partiallyPaidOrder.SupplierOrderId,
                    TransactionId = transaction.SupplierTransactionId,
                    PaidAmount = payingAmountForOrder,
                    IsPaymentComplete = IsOrderSettleUp
                });
            }
            return settleUpSupplierOrder;
        }

        private bool SettleUpOrder(SupplierOrder supplierOrder, decimal settleUpAmount)
        {
            supplierOrder.PaidAmount += settleUpAmount;
            db.Entry(supplierOrder).State = EntityState.Modified;
            if (supplierOrder.PaidAmount == supplierOrder.BillAmount)
                return true;
            return false;
        }
    }
}