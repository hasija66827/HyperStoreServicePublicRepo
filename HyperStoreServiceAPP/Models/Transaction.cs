using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public string TransactionNo { get; set; }
        public float CreditAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public float WalletSnapshot { get; set; }

        public Transaction() { }

        [Required]
        public Guid SupplierId;
        public Supplier Supplier;

        public List<SupplierOrderTransaction> SupplierOrderTransactions { get; set; }
    }
}