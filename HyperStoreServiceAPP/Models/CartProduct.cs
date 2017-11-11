using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class CartProduct
    {
        public Guid CartProductId { get; set; }

        public decimal PotentielQuantityPurhcased { get; set; }
        public decimal PotentielPurchasePrice { get; set; }

        [Required]
        public Guid? CartId { get; set; }
        public Cart Cart { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}