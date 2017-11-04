﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{
    public class SupplierDTO
    {
        [Required]
        public EntityType? EntityType { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }

        [Required]
        [RegularExpression(@"[987]\d{9}")]
        public string MobileNo { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class SupplierFilterCriteria
    {
        [Required]
        public EntityType? EntityType { get; set; }
        public IRange<decimal> WalletAmount { get; set; }
        public Guid? SupplierId { get; set; }
    }
}