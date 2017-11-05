using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Product
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        public decimal? CGSTPer { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(15)]
        public string Code { get; set; }

        [Required]
        public decimal? MRP { get; set; }

        [Required]
        public decimal? DiscountPer { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(24)]
        public string Name { get; set; }

        public Int32? HSN { get; set; }

        [Required]
        public decimal? SGSTPer { get; set; }
        [Required]
        public decimal? Threshold { get; set; }
        public decimal TotalQuantity { get; set; }

        public Guid? LatestSupplierId { get; set; }
        [ForeignKey("LatestSupplierId")]
        public Person LatestSupplier { get; set; }

        public Guid? PotentielSupplierId { get; set; }
        [ForeignKey("PotentielSupplierId")]
        public Person PotentielSupplier { get; set; }

        public decimal? PotentielQuantityPurhcased { get; set; }
        public decimal? PotentielPurchasePrice { get; set; }

        public bool? IsPurchased { get; set; }
    }
}