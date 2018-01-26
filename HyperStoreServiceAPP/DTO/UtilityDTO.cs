﻿using HyperStoreService.Models;
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

   
   
}