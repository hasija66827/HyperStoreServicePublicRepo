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

    public class ProductDeficiencyHitTrend
    {
        public DayOfWeek Day { get; set; }
        // Number of days hit of the particular @Day of the week.
        public int NumberOfDaysHit { get; set; }
        public ProductDeficiencyHitTrend(DayOfWeek day, int numberOfDaysHit)
        {
            this.Day = day;
            this.NumberOfDaysHit = numberOfDaysHit;
        }
    }

}