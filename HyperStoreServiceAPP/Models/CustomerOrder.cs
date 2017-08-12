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

        public decimal BillAmount { get; set; }
        public decimal DiscountedAmount { get; set; }

        // PayingNow = DiscountedBillAmount + AddingMoneyToWallet - UsingWalletAmount
        public bool IsPayingNow { get; set; }
        public bool IsUsingWallet { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal UsingWalletAmount { get; set; }// TODO: Should not be set by the customer. Use DTO
        
        public CustomerOrder()
        {
            this.CustomerOrderId = Guid.NewGuid();
            this.CustomerOrderNo = Utility.GenerateCustomerOrderNo();
            this.OrderDate = DateTime.Now;
        }

        [Required]
        public Nullable<Guid> CustomerId;
        public Customer Customer;
        public List<CustomerOrderProduct> CustomerOrderProducts { get; set; }
    }
}