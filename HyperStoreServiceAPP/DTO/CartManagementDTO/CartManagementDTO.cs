using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.CartManagementDTO
{
    public class PersonDTO
    {
        private Guid? _personId;
        [Required]
        public Guid? PersonId { get { return this._personId; } }
        public PersonDTO(Guid? personId)
        {
            _personId = personId;
        }
    }

    /// <summary>
    /// DTO to retrieve the product in the cart.
    /// </summary>
    public class ProductCartDTO : PersonDTO
    {
        private Guid? _cartId;
        [Required]
        public Guid? CartId { get { return this._cartId; } }

        public ProductCartDTO(Guid? personId, Guid? cartId) : base(personId)
        {
            _cartId = cartId;
        }
    }

    /// <summary>
    /// DTO to add or remove product from the live cart of the person.
    /// </summary>
    public class AddRemoveProduct_CartDTO : PersonDTO
    {
        private Guid? _productId;
        [Required]
        public Guid? ProductId { get { return this._productId; } }

        public AddRemoveProduct_CartDTO(Guid? personId, Guid? productId) : base(personId)
        {
            _productId = productId;
        }
    }

    /// <summary>
    /// DTO to bookmark the live cart.
    /// </summary>
    public class UpdateLiveCartDTO : PersonDTO
    {
        private Boolean _IsBookMark;
        public Boolean IsBookMark { get { return this._IsBookMark; } }

        private DateTime _deliveryTime;
        public DateTime DeliveryTime { get { return this._deliveryTime; } }

        public UpdateLiveCartDTO(Guid? personId, Boolean isBookMark, DateTime deliveryTime) : base(personId)
        {
            _IsBookMark = isBookMark;
            _deliveryTime = deliveryTime;
        }
    } 
}