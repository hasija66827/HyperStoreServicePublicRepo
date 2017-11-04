using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.InsightsDTO
{
    public class ProductInsightsDTO : InsightsDTO
    {
        public ProductInsightsDTO(uint numberOfDays, uint numberOfRecords) : base(numberOfDays, numberOfRecords) { }
    }

    class ProductInsights
    {
        public Guid? ProductId;
        public Product Product { get; set; }
        public int? NoOfDaysStockHitZero { get; set; }
    }
}