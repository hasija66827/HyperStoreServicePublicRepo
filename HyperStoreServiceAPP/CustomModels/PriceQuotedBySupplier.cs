using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.CustomModels
{
    public class PriceQuotedBySupplier
    {
        public Guid PersonId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal QuantityPurchased { get; set; }
        public decimal PurchasePrice { get; set; }
        public Person Person { get; set; }
    }
}