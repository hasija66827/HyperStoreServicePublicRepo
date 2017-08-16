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
        public decimal DisplayCostSnapShot { get; set; }
        public float QuantityConsumed { get; set; }

        public CustomerOrderProduct() { }

        public Guid CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}