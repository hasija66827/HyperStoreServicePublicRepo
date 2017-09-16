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

    public class ProductDeficiencyHitTrend
    {
        public DayOfWeek Day { get; set; }
        // Number of days hit of the particular @Day of the week like on all mondays.
        public int TotalDaysHit { get; set; }
        public ProductDeficiencyHitTrend(DayOfWeek day, int numberOfDaysHit)
        {
            this.Day = day;
            this.TotalDaysHit = numberOfDaysHit;
        }
    }

    public class ProductConsumptionDeficientTrend
    {
        public DayOfWeek Day { get; set; }
        public float AvgConsumption { get; set; }
        [Range(0, 1)]
        public float AvgHitRate { get; set; }
        public ProductConsumptionDeficientTrend() { }
    }
}