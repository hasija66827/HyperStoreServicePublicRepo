﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Product
    {
        [Required]
        public Guid? ProductId { get; set; }

        [Required]
        public decimal? CGSTPer { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(15)]
        public string Code { get; set; }

        [Required]
        public decimal? DisplayPrice { get; set; }

        [Required]
        public decimal? DiscountPer { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(24)]
        public string Name { get; set; }

        public Int32 RefillTime { get; set; }

        [Required]
        public decimal? SGSTPer { get; set; }
        public decimal Threshold { get; set; }
        public decimal TotalQuantity { get; set; }
    }
}