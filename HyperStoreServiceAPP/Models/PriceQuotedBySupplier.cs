using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class PriceQuotedBySupplier
    {
        public Guid SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal QuantityPurchased { get; set; }
        public decimal PurchasePrice { get; set; }
        public Supplier Supplier { get; set; }
    }
}