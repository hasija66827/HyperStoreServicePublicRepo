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

        public decimal DisplayCostSnapShot { get; set; }
        public float DiscountPerSnapShot { get; set; }
        public float CGSTPerSnapShot { get; set; }
        public float SGSTPerSnapshot { get; set; }
        public float QuantityConsumed { get; set; }
        public decimal NetValue { get; set; }
        /*
         * cop.DisplayCostSnapShot
                ((100 - cop.DiscountPerSnapShot) * (100 + cop.CGSTPerSnapShot + cop.SGSTPerSnapshot) / 10000
                * cop.QuantityConsumed)
         */
        public CustomerOrderProduct() { }

        public Guid CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}