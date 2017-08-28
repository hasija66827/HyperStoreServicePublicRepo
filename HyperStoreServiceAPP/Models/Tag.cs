using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Tag
    {
        public Guid? TagId { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(24)]
        public string TagName { get; set; }

        public Tag() { }
    }
}