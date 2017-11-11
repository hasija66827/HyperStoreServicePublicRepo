using HyperStoreServiceAPP.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Person
    {
        [Required]
        public EntityType? EntityType { get; set; }
        public Guid PersonId { get; set; }
        public string Address { get; set; }
        public string GSTIN { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(10)]
        public string MobileNo { get; set; }

        [Required]
        [StringLength(24)]
        public string Name { get; set; }
        public decimal WalletBalance { get; set; }

        public decimal? NetWorth { get; set; }
        public DateTime FirstVisited { get; set; }
        public DateTime LastVisited { get; set; } 
        public DateTime PreferedTimeToContact { get; set; }
        public Person() {
        }
    }
}