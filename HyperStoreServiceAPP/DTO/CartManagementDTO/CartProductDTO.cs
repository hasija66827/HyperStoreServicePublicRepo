using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.CartManagementDTO
{
    public class PersonProductDTO
    {
        [Required]
        public Guid? PersonId { get; set; }

        [Required]
        public Guid? ProductId { get; set; }

    }
}