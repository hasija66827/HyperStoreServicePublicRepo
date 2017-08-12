using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class ProductTag
    {
        public Guid ProductTagId { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public Product Product;

        [Required]
        public Guid TagId { get; set; }
        public Tag Tag;
    }
}