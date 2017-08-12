using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class CustomerOrderProduct
    {
        [Required]
        public Guid? CustomerOrderProductId { get; set; }
        [Required]
        public float? DiscountPerSnapShot { get; set; }
        [Required]
        public float? DisplayCostSnapShot { get; set; }
        [Required]
        public float? QuantityPurchased { get; set; }

        public CustomerOrderProduct() { }

        [Required]
        public Guid? CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}