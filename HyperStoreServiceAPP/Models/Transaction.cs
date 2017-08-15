using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HyperStoreServiceAPP;
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
    }
}