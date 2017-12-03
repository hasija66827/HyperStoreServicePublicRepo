using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace HyperStoreService.HyperStoreService.CustomModels
{
    public class MapDay_ProductEstConsumption
    {
        public Dictionary<DayOfWeek, float> ProductEstConsumption { get; set; }
        public MapDay_ProductEstConsumption() {
            ProductEstConsumption = new Dictionary<DayOfWeek, float>();
        }
    }

    public class AveragaeConsumptionOfProductInDayOfWeek
    {
        public DayOfWeek Day { get; set; }
        public float AvgConsumption { get; set; }
    }
}