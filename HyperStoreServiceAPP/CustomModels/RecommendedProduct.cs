using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.CustomModels
{
    public class RecommendedProductForSupplier
    {
        public Product Product { get; set; }
        public double DeficientByNumber { get; set; }
    }

    public class RecommendedProductForCustomer
    {
        public Product Product { get; set; }
        public double? ExpiredByDays { get; set; }
    }

}