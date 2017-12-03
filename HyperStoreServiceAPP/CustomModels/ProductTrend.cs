using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace HyperStoreService.HyperStoreService.CustomModels
{
    public class ProductConsumptionTrend
    {
        public DayOfWeek Day { get; set; }
        public float TotalQuantityConsumed { get; set; }
        public ProductConsumptionTrend(DayOfWeek day, float quantity)
        {
            this.Day = day;
            this.TotalQuantityConsumed = quantity;
        }
    }

    public class AveragaeConsumptionOfProductInDayOfWeek
    {
        public DayOfWeek Day { get; set; }
        public float AvgConsumption { get; set; }
    }
}