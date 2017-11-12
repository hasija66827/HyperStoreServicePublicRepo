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
        public Guid? ProductID { get { return this._productId; } }
        private Guid? _personId;
        public Guid? PersonId { get { return this._personId; } }
        private int? reminderInDays;
    }
}