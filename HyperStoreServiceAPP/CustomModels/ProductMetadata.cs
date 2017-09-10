using HyperStoreServiceAPP.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.CustomModels
{
    public class ProductMetadata
    {
        public IRange<decimal?> DiscountPerRange { get; set; }
        public IRange<decimal> QuantityRange { get; set; }
    }
}