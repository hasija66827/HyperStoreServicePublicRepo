using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Product
    {
        [Required]
        public Guid? ProductId { get; set; }
        public float CGSTPer { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public float? DisplayPrice { get; set; }
        public float DiscountPer { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32 RefillTime { get; set; }
        public float SGSTPer { get; set; }
        public Int32 Threshold { get; set; }
        public float TotalQuantity { get; set; }
        
        public Product() {
            this.CGSTPer = 0;
            this.DiscountPer = 0;
            this.RefillTime = 0;
            this.SGSTPer = 0;
            this.Threshold = 0;
            this.TotalQuantity = 0;
        }


        //SupplierId is used by Retailer to mark the product to be prurchased from Wholeseller.
        public Guid? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}