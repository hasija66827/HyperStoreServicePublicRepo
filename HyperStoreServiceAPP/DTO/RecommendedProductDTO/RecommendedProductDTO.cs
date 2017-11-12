using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.RecommendedProductDTO
{
    public class SetReminderDTO
    {
        private Guid? _productId;
        [Required]
        public Guid? ProductId { get { return this._productId; } }

        private Guid? _personId;
        [Required]
        public Guid? PersonId { get { return this._personId; } }

        private int? _expireInDays;
        [Range(0, 30)]
        public int? ExpireInDays { get { return this._expireInDays; } }

        public SetReminderDTO(Guid? productId, Guid? personId, int? expireInDays)
        {
            _productId = productId;
            _personId = personId;
            _expireInDays = expireInDays;
        }

    }
}