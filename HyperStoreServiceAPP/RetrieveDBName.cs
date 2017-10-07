using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP
{
    public class UtilityAPI
    {
        public static HyperStoreServiceContext RetrieveDBContext(Guid userId)
        {
                return new HyperStoreServiceContext("name=HyperStoreServiceContext");
        }
    }
}