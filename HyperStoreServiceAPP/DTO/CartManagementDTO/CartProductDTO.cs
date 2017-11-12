using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.CartManagementDTO
{
    public class PersonProductsDTO
    {
        [Required]
        public Guid? PersonId { get; set; }

        [Required]
        public List<ProductPurchased> ProductsPurchased { get; set; }     
    }
}