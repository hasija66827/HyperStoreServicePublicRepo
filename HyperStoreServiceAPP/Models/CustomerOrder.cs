using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using HyperStoreServiceAPP;
namespace HyperStoreService.Models
{
    public class CustomerOrder
    {
        public Guid? CustomerOrderId { get; set; }//TODO: Should not be nullable in database

        public string CustomerOrderNo { get; set; }//TODO: Should not be nullable in database
        
        public DateTime OrderDate { get; set; } 

        [Required]
        public decimal? BillAmount { get; set; }
        [Required]
        public decimal? DiscountedAmount { get; set; }

        // PayingNow = DiscountedBillAmount + AddingMoneyToWallet - UsingWalletAmount
        [Required]
        public bool? IsPayingNow { get; set; }
        [Required]
        public bool? IsUsingWallet { get; set; }
        [Required]
        public decimal? PayingAmount { get; set; }
        public decimal UsingWalletAmount { get; set; }
        
        public CustomerOrder()
        {
        }

        [Required]
        public Guid? CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}