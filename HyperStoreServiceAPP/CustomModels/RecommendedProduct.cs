using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.CustomModels
{
    public class DumpRecommendedProduct
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime LastOrderDate { get; set; }
    }
}