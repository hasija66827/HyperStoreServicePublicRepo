using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class Tag
    {
        [Required]
        public Guid? TagId { get; set; }
        [Required]
        public string TagName { get; set; }

        public Tag() { }
    }
}