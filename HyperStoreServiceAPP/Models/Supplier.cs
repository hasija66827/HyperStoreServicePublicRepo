using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Supplier
    {
        public Guid? SupplierId { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Name { get; set; }
        public decimal WalletBalance { get; set; }

        public Supplier() {
        }
    }

    public class SupplierDTO
    {
        public string Address { get; set; }
        public string GSTIN { get; set; }
        [Required]
        [RegularExpression(@"[987]\d{9}")]
        public string MobileNo { get; set; }
        [Required]
        public string Name { get; set; }
    }
}