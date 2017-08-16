using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HyperStoreServiceAPP;
using System.Threading.Tasks;
using System.Data.Entity;

namespace HyperStoreService.Models
{
    public class Transaction
    {
        public Guid? TransactionId { get; set; }
        public bool IsCredit { get; set; }
        [Required]
        public string TransactionNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal WalletSnapshot { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public Transaction() {
        }
    }

    public class TransactionDTO
    {
        [Required]
        public bool? IsCredit { get; set; }
        [Required]
        public Guid? SupplierId { get; set; }
        [Required]
        public decimal? TransactionAmount { get; set; }

        public async Task<Transaction> CreateNewTransaction(HyperStoreServiceContext db)
        {
            var walletSnapshot = await this.UpdateSupplierWalletBalance(db);
            if (walletSnapshot == null)
                throw new Exception(String.Format("Supplier with id {0} not found", this.SupplierId));
            var transaction = this.AddNewTransaction((decimal)walletSnapshot, db);
            //if is credit==false then do settle up the orders.
            return transaction;
        }

        private async Task<decimal?> UpdateSupplierWalletBalance(HyperStoreServiceContext db)
        {
            Guid supplierId = (Guid)this.SupplierId;
            decimal transactionAmount = (decimal)this.TransactionAmount;
            bool IsCredit = (bool)this.IsCredit;
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

        private Transaction AddNewTransaction(decimal walletSnapshot, HyperStoreServiceContext db)
        {
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                TransactionNo = Utility.GenerateSupplierTransactionNo(),
                TransactionDate = DateTime.Now,
                TransactionAmount = (decimal)this.TransactionAmount,
                IsCredit = (bool)this.IsCredit,
                SupplierId = (Guid)this.SupplierId,
                WalletSnapshot = walletSnapshot
            };
            db.Transactions.Add(transaction);
            return transaction;
        }

    }
}