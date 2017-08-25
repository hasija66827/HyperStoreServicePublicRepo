using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public decimal? CGSTPer { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal? DisplayPrice { get; set; }
        [Required]
        public decimal? DiscountPer { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32 RefillTime { get; set; }
        public decimal? SGSTPer { get; set; }
        public decimal Threshold { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}