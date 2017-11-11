using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.CartManagementDTO
{
    public class CartDTO
    {
        private Guid? _productId;
        [Required]
        public Guid? ProductId { get { return this._productId; } }

        private Guid? _personId;
        [Required]
        public Guid? PersonId { get { return this._personId; } }

        public CartDTO(Guid? productId, Guid? supplierId)
        {
            _productId = productId;
            _personId = supplierId;
        }
    }
}