using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.CustomModels
{
    public class PersonMetadata
    {
        public IRange<double> WalletBalanceRange;
        public int TotalRecords;
    }

    public class PersonMetadataDTO
    {
        [Required]
        public EntityType? EntityType { get; set; }
    }
}