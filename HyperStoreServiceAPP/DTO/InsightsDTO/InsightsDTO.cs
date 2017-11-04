using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO.InsightsDTO
{
    public class InsightsDTO
    {
        private uint? _numberOfDays;
        [Required]
        public uint? NumberOfDays { get { return this._numberOfDays; } }

        private uint? _numberOfRecords;
        [Required]
        public uint? NumberOfRecords { get { return this._numberOfRecords; } }

        public InsightsDTO(uint numberOfDays, uint numberOfRecords)
        {
            _numberOfDays = numberOfDays;
            _numberOfRecords = numberOfRecords;
        }
    }
}