using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class OrderProduct
    {
        public Guid OrderProductId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal QuantityPurchased { get; set; }

        public OrderProduct() {
        }

        [Required]
        public Guid? OrderId { get; set; }
        public Order Order { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}