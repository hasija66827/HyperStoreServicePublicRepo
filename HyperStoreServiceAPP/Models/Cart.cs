using HyperStoreServiceAPP.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public enum CartStatus
    {
        OrderInitiated,
        OrderCompleted
    }

    public class Cart
    {
        public Guid CartId { get; set; }
        public CartStatus CartStatus { get; set; }
        public Boolean? IsBookmarked { get; set; }
        public DateTime? OrderCompletionDate { get; set; }
        public DateTime? PreferedDeliveryTime { get; set; }

        [Required]
        public Guid? PersonId { get; set; }
        public Person Person { get; set; }        
    }
}