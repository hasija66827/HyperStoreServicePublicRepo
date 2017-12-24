using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class PaymentOption
    {
        public Guid PaymentOptionId { get; set; }
        public string Name { get; set; }
        //Offers Image
    }
}