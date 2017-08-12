using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Supplier
    {
        [Required]
        public Guid? SupplierId { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }
        [Required]
        public string MobileNo { get; set; }
        [Required]
        public string Name { get; set; }
        public float WalletBalance { get; set; }

        public Supplier() {
            this.WalletBalance = 0;
        }
    }
}