using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HyperStoreServiceAPP;
using System.Threading.Tasks;
using System.Data.Entity;
using HyperStoreServiceAPP.DTO;

namespace HyperStoreService.Models
{
    public class Transaction
    {
        [Required]
        public EntityType? EntityType { get; set; }

        public Guid TransactionId { get; set; }
        public bool IsCredit { get; set; }
        [Required]
        public string TransactionNo { get; set; }
        public string OrderNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal WalletSnapshot { get; set; }

        public Guid PersonId { get; set; }
        public Person Person { get; set; }

        public Guid? PaymentOptionId { get; set; }
        public PaymentOption PaymentOption { get; set; }

        public Transaction() {
        }
    }
}