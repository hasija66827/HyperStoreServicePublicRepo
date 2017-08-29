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
    public class SupplierTransaction
    {
        public Guid SupplierTransactionId { get; set; }
        public bool IsCredit { get; set; }
        [Required]
        public string TransactionNo { get; set; }
        public string SupplierOrderNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal WalletSnapshot { get; set; }
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public SupplierTransaction() {
        }
    }
}