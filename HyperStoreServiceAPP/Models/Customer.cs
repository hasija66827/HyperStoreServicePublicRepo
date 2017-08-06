using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Customer
    {
        public Guid CustomerId { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        public string MobileNo { get; set; }
        public string Name { get; set; }
        public float WalletBalance { get; set; }

        public Customer()
        { }

        public List<CustomerOrder> CustomerOrders { get; set; }
        
    }
}