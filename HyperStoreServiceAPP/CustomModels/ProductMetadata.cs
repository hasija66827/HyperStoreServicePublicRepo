using HyperStoreServiceAPP.Controllers;
using HyperStoreServiceAPP.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.CustomModels
{
    public class ProductMetadata
    {
        public IRange<decimal?> DiscountPerRange { get; set; }
        public IRange<float?> QuantityRange { get; set; }
        public IRange<int?> DayRange { get; set; }
    }
}