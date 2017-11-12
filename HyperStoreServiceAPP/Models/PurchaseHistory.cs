using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class PurchaseHistory
    {
        public Guid PurchaseHistoryId { get; set; }

        public DateTime LatestPurchaseDate { get; set; }

        public int? ExpiryDays { get; set; }

        [Required]
        [Index("IX_PersonIdAndProductId", IsUnique = true, Order = 1)]
        public Guid? PersonId { get; set; }
        public Person Person { get; set; }

        [Required]
        [Index("IX_PersonIdAndProductId", IsUnique = true, Order = 2)]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}