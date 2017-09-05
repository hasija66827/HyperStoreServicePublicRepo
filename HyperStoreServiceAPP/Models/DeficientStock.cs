using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class DeficientStockHit
    {
        public static readonly decimal DeficientStockCriteria = 1;
        public Guid? DeficientStockHitId { get; set; }

        [Required]
        public decimal? QuantitySnapshot { get; set; }

        [Required]
        [Index("IX_ProductIdAndTimeStamp", IsUnique = true, Order = 1)]
        public Guid? ProductId { get; set; }

        [Required]
        [Index("IX_ProductIdAndTimeStamp", IsUnique = true, Order = 2)]
        public DateTime TimeStamp { get; set; }

        public Product Product { get; set; }
    }
}