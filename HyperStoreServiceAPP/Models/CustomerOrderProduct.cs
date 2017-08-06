using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class CustomerOrderProduct
    {
        public Guid CustomerOrderProductId { get; set; }
        public float DiscountPerSnapShot { get; set; }
        public float DisplayCostSnapShot { get; set; }
        public int QuantityPurchased { get; set; }

        public CustomerOrderProduct() { }

        [Required]
        public Nullable<Guid> CustomerOrderId;
        public CustomerOrder CustomerOrder;

        [Required]
        public Nullable<Guid> ProductId;
        public Product Product;
    }
}