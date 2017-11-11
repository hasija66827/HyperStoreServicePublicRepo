using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class RecommendedProduct
    {
        public Guid RecommendedProductId { get; set; }

        public DateTime LatestPurchaseDate { get; set; }

        public int? ExpiryDays { get; set; }

        [Required]
        public Guid? PersonId { get; set; }
        public Person Person { get; set; }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}