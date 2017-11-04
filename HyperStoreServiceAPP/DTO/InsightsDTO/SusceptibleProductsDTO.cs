﻿using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.InsightsDTO
{
    public class SusceptibleProductsInsightDTO : InsightsDTO
    {
        public SusceptibleProductsInsightDTO(uint numberOfDays, uint numberOfRecords) : base(numberOfDays, numberOfRecords) { }
    }

    public class SusceptibleProductsInsight {
        private int _totalSusceptibleProducts;
        public int TotalSusceptibleProducts { get { return this._totalSusceptibleProducts; } }
        private List<KeyValuePair<int, Product>> _susceptibleProducts;
        public List<KeyValuePair<int, Product>> SusceptibleProducts { get { return this._susceptibleProducts; } }
        public SusceptibleProductsInsight(int totalSusceptibleProducts, List<KeyValuePair<int, Product>> susceptibleProducts) {
            _totalSusceptibleProducts = totalSusceptibleProducts;
            _susceptibleProducts = susceptibleProducts;
        }
    }
}