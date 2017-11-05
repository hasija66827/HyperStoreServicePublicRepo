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

        private Guid? _supplierId;
        [Required]
        public Guid? SupplierId { get { return this._supplierId; } }

        public CartDTO(Guid? productId, Guid? supplierId)
        {
            _productId = productId;
            _supplierId = supplierId;
        }
    }

    public class AddProductToCartDTO : CartDTO
    {
        private decimal? _quantityPurchased;
        [Required]
        public decimal? QuantityPurchased { get { return this._quantityPurchased; } }
        public AddProductToCartDTO(Guid? productId, Guid? supplierId, decimal? quantityPurchased) : base(productId, supplierId)
        {
            this._quantityPurchased = quantityPurchased;
        }
    }
}