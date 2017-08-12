using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class SupplierOrderProduct
    {
        [Required]
        public Guid? SupplierOrderProductId { get; set; }
        [Required]
        public int? QuantityPurchased { get; set; }
        [Required]
        public float? PurchasePrice { get; set; }

        public SupplierOrderProduct() {
        }

        [Required]
        public Guid? SupplierOrderId { get; set; }
        public SupplierOrder SupplierOrder { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}