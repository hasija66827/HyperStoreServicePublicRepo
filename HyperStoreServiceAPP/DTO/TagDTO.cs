using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{
    public class TagDTO
    {
        [Required]
        [StringLength(24)]
        public string TagName { get; set; }
    }
}