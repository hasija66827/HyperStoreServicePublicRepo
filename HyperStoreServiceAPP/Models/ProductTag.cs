using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class ProductTag
    {
        [Required]
        public Guid? ProductTagId { get; set; }

        public ProductTag() { }

        [Required]
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }

        [Required]
        public Guid? TagId { get; set; }
        public Tag Tag { get; set; }
    }
}