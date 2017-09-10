using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.CustomModels
{
    public class CustomerPurchaseTrend
    {
        public int TotalQuantityPurchased { get; set; }
        public Product Product { get; set; }
        public decimal NetValue { get; set; }
        public CustomerPurchaseTrend() { }
    }
}