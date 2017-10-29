using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{

    #region CustomerPurchaseTrend
    public class CustomerPurchaseTrendDTO
    {
        [Required]
        public Guid? CustomerId { get; set; }
        [Required]
        public int? MonthsCount { get; set; }
    }
    #endregion

    #region ProductConsumption Trend
    public class ProductConsumptionTrendDTO
    {
        [Required]
        public Guid? ProductId { get; set; }
        [Required]
        public int? MonthsCount { get; set; }
    }
    #endregion 

   public sealed class DateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;
            var dateRange = value as IRange<DateTime>;
            return dateRange.LB <= dateRange.UB;
        }
    }
    public enum EntityType
    {
        Customer,
        Supplier
    }

}