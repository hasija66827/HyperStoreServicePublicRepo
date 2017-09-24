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

        public decimal MRPSnapShot { get; set; }
        public decimal DiscountPerSnapShot { get; set; }
        public decimal CGSTPerSnapShot { get; set; }
        public decimal SGSTPerSnapshot { get; set; }
        public decimal QuantityConsumed { get; set; }
        public decimal ValueIncTaxSnapShot { get; set; }//derived attr
        public decimal NetValue { get; set; }//derived attri

        public CustomerOrderProduct() { }

        public Guid CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}