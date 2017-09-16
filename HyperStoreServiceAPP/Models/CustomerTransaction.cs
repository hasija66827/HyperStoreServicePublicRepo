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
    public class CustomerTransaction
    {
        public Guid CustomerTransactionId { get; set; }

        public bool IsCredit { get; set; }

        public decimal TransactionAmount { get; set; }
        public DateTime TransactionDate { get; set; }

        [Required]
        public string TransactionNo { get; set; }

        //Not making it as foreign key, to reduce the chaos :)
        public string CustomerOrderNo { get; set; }

        public decimal WalletSnapshot { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public CustomerTransaction() { }
    }
}