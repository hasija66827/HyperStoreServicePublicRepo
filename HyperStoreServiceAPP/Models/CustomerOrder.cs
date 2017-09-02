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
        public Guid CustomerOrderId { get; set; }

        [Required]
        public string CustomerOrderNo { get; set; }
        
        public DateTime OrderDate { get; set; }

        public decimal TotalQuantity { get; set; }
        public int TotalItems { get; set; }
        public decimal CartAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Tax { get; set; }
        public decimal PayAmount { get; set; }

        // PayingNow = DiscountedBillAmount + AddingMoneyToWallet - UsingWalletAmount

        public bool IsPayingNow { get; set; }
        
        public bool IsUsingWallet { get; set; }
        
        public decimal PayingAmount { get; set; }
        public decimal UsingWalletAmount { get; set; }
        
        public CustomerOrder()
        {
        }

        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}