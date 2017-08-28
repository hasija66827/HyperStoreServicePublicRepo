using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class ProductConsumptionTrend
    {
        public DayOfWeek Day { get; set; }
        public float Quantity { get; set; }
        public ProductConsumptionTrend(DayOfWeek day, float quantity)
        {
            this.Day = day;
            this.Quantity = quantity;
        }
    }
}