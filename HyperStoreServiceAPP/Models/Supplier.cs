using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Supplier
    {
        public Guid SupplierId { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Name { get; set; }
        public float WalletBalance { get; set; }

        public Supplier() { }

        public List<SupplierOrder> SupplierOrders { get; set; }
        //This is used by Retailer to mark the product to be prurchased from Wholeseller.
        public List<Product> Products { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}