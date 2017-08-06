using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class SupplierOrderProduct
    {
        public Guid SupplierOrderProductId { get; set; }
        public int QuantityPurchased { get; set; }
        public float PurchasePrice { get; set; }

        public SupplierOrderProduct() { }

        [Required]
        public Nullable<Guid> SupplierOrderId;
        public SupplierOrder SupplierOrder;

        [Required]
        public Nullable<Guid> ProductId;
        public virtual Product Product { get; set; }
    }
}