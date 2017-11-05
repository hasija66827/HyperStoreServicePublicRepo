using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.CartManagementDTO
{
    public class CartDTO
    {
        [Required]
        Guid? ProductId;

        [Required]
        Guid? SupplierId;
    }
}