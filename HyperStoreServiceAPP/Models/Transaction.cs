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
        [Required]
        public Guid? TransactionId { get; set; }
        [Required]
        public string TransactionNo { get; set; }
        [Required]
        public float? CreditAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        [Required]
        public float? WalletSnapshot { get; set; }

        public Transaction() {
            this.TransactionDate = DateTime.Now;
        }

        public void SetDefaultValue()
        {
            this.TransactionId = Guid.NewGuid();
            this.TransactionNo = Utility.GenerateSupplierTransactionNo();
            this.TransactionDate = DateTime.Now;
        }
    }
}