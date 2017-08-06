using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal WalletBalance { get; set; }

        public Customer()
        {
            CustomerId = Guid.NewGuid();
            Address = "";
            GSTIN = "";
            MobileNo = "";
            Name = "";
            WalletBalance = 0;
        }

        public List<CustomerOrder> CustomerOrders { get; set; }
        
    }
}