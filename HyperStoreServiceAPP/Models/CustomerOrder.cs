using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class CustomerOrder
    {
        public Guid CustomerOrderId { get; set; }
        public string CustomerOrderNo { get; set; }
        public DateTime OrderDate { get; set; }

        public float BillAmount { get; set; }
        public float DiscountedAmount { get; set; }

        // PayingNow = DiscountedBillAmount + AddingMoneyToWallet - UsingWalletAmount
        public bool IsPaidNow { get; set; }
        public float PayingNow { get; set; }
        public float AddingMoneyToWallet { get; set; }

        public bool IsUseWallet { get; set; }
        public float UsingWalletAmount { get; set; }

        // DiscountedBillAmt = PartiallyPaid + PayingLater
        public float PartiallyPaid { get; set; }
        public float PayingLater { get; set; }

        public CustomerOrder() { }

        [Required]
        public Nullable<Guid> CustomerId;
        public Customer Customer;
        public List<CustomerOrderProduct> CustomerOrderProducts { get; set; }
    }
}