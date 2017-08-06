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
        public float CGSTPer { get; set; }
        [Required]
        public string Code { get; set; }
        public float DisplayPrice { get; set; }
        public float DiscountPer { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32 RefillTime { get; set; }
        public float SGSTPer { get; set; }
        public Int32 Threshold { get; set; }
        public Int32 TotalQuantity { get; set; }
        
        public Product() {
            this.ProductId = Guid.NewGuid();
            this.CGSTPer = 0;
            this.Code = null;
            this.DisplayPrice = 0;
            this.DiscountPer = 0;
            this.Name = null;
            this.RefillTime = 0;
            this.SGSTPer = 0;
            this.Threshold = 0;
            this.TotalQuantity = 0;
            this.SupplierId = null;
        }

        public List<SupplierOrderProduct> SupplierOrderProducts { get; set; }
        public List<CustomerOrderProduct> CustomerOrderProducts { get; set; }
        public List<ProductTag> ProductTags { get; set; }

        //SupplierId is used by Retailer to mark the product to be prurchased from Wholeseller.
        [Required]
        public Guid? SupplierId;
        public Supplier Supplier;
    }
}