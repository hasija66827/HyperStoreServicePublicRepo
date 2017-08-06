using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string UserDefinedCode { get; set; }
        public bool IsInventoryItem { get; set; }
        public Int32 Threshold { get; set; }
        public Int32 RefillTime { get; set; }
        public float DisplayPrice { get; set; }
        public float DiscountPer { get; set; }
        public Int32 TotalQuantity { get; set; }
        public float SGSTPer { get; set; }
        public float CGSTPer { get; set; }

        public Product() { }

        public List<SupplierOrderProduct> SupplierOrderProducts { get; set; }
        public List<CustomerOrderProduct> CustomerOrderProducts { get; set; }
        public List<ProductTag> ProductTags { get; set; }

        //SupplierId is used by Retailer to mark the product to be prurchased from Wholeseller.
        [Required]
        public Guid? SupplierId;
        public Supplier Supplier;
    }
}