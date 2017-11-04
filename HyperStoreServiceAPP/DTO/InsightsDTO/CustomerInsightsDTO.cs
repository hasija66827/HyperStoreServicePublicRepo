using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.InsightsDTO
{
    public class CustomerInsightsDTO : InsightsDTO
    {
        public CustomerInsightsDTO(uint numberOfDays, uint numberOfRecords) : base(numberOfDays, numberOfRecords) { }
    }
}