using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{
    public class SupplierTransactionFilterCriteria
    {
        [Required]
        public Guid? SupplierId { get; set; }
    }

    public class SupplierTransactionDTO
    {
        [Required]
        public bool? IsCredit { get; set; }

        [Required]
        public Guid? SupplierId { get; set; }

        [Required]
        [Range(0, 98765432198765)]
        public decimal? TransactionAmount { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// 1. Updates the wallet balance of the supplier.
        /// 2. Creates a transaction entity associated with the supplier.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>Retruns the newly creaated transaction with the customer included in it.</returns>
        public async Task<Transaction> CreateNewTransactionAsync(HyperStoreServiceContext db)
        {
            var supplier = await db.Persons.FindAsync(this.SupplierId);
            var walletSnapshot = this.UpdateWalletBalanceAsync(db, supplier);
            var transaction = this.AddNewTransaction(db, (decimal)walletSnapshot);
            List<Order> settleUpOrders;
            if (transaction.IsCredit == false && supplier.EntityType == EntityType.Supplier ||
                transaction.IsCredit == true && supplier.EntityType == EntityType.Customer)
                settleUpOrders = SettleUpOrders(transaction, db);
            return transaction;
        }

        /// <summary>
        /// Updates the wallet balance of the supplier.
        /// Positive Balance always means that the user owes the supplier that much amount.
        /// </summary>
        /// <param name="db"></param>
        /// <returns>The wallet snapshot which was before this function updates it.</returns>
        private decimal UpdateWalletBalanceAsync(HyperStoreServiceContext db, Person supplier)
        {
            Guid supplierId = (Guid)this.SupplierId;
            decimal transactionAmount = (decimal)this.TransactionAmount;
            bool IsCredit = (bool)this.IsCredit;
            if (supplier == null)
                throw new Exception(String.Format("Supplier with id {0} not found while updating its wallet balance", this.SupplierId));
            var walletSnapshot = supplier.WalletBalance;
            if (IsCredit == true)
                supplier.WalletBalance += transactionAmount;
            else
                supplier.WalletBalance -= transactionAmount;
            db.Entry(supplier).State = EntityState.Modified;
            return walletSnapshot;
        }

        /// <summary>
        /// Creates a transaction entity associated with supplier.
        /// </summary>
        /// <param name="walletSnapshot"></param>
        /// <param name="db"></param>
        /// <returns>Retruns the newly creaated transaction with the customer included in it.</returns>
        private Transaction AddNewTransaction(HyperStoreServiceContext db, decimal walletSnapshot)
        {
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                TransactionNo = Utility.GenerateSupplierTransactionNo(),
                TransactionDate = DateTime.Now,
                TransactionAmount = (decimal)this.TransactionAmount,
                OrderNo = this.Description,
                IsCredit = (bool)this.IsCredit,
                PersonId = (Guid)this.SupplierId,
                WalletSnapshot = walletSnapshot
            };
            db.Transactions.Add(transaction);
            return transaction;
        }

        private List<Order> SettleUpOrders(Transaction transaction, HyperStoreServiceContext db)
        {
            List<Order> settleUpSupplierOrder = new List<Order>();
            var partiallyPaidOrders = db.Orders.Where(so => so.PersonId == transaction.PersonId &&
                                                                   so.BillAmount - so.SettledPayedAmount > 0)
                                                       .OrderBy(wo => wo.OrderDate);
            var debitTransactionAmount = transaction.TransactionAmount;
            foreach (var partiallyPaidOrder in partiallyPaidOrders)
            {
                if (debitTransactionAmount <= 0)
                    break;
                var remainingAmount = partiallyPaidOrder.BillAmount - partiallyPaidOrder.SettledPayedAmount;
                if (remainingAmount < 0)
                    throw new Exception(string.Format("Supplier OrderNo {0}, Amount remaining to be paid: {1} cannot be less than zero", partiallyPaidOrder.OrderNo, remainingAmount));
                decimal payingAmountForOrder = Math.Min(remainingAmount, debitTransactionAmount);
                debitTransactionAmount -= payingAmountForOrder;
                var IsOrderSettleUp = SettleUpOrder(partiallyPaidOrder, payingAmountForOrder, db);
                settleUpSupplierOrder.Add(partiallyPaidOrder);
                db.OrderTransactions.Add(new OrderTransaction
                {
                    OrderTransactionId = Guid.NewGuid(),
                    OrderId = partiallyPaidOrder.OrderId,
                    TransactionId = transaction.TransactionId,
                    PaidAmount = payingAmountForOrder,
                    IsPaymentComplete = IsOrderSettleUp
                });
            }
            return settleUpSupplierOrder;
        }

        private bool SettleUpOrder(Order supplierOrder, decimal settleUpAmount, HyperStoreServiceContext db)
        {
            supplierOrder.SettledPayedAmount += settleUpAmount;
            db.Entry(supplierOrder).State = EntityState.Modified;
            if (supplierOrder.SettledPayedAmount == supplierOrder.BillAmount)
                return true;
            return false;
        }
    }
}