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
            if (userId == Guid.Empty)
                return new HyperStoreServiceContext("name=AHyperStoreServiceContext");
            else
                return new HyperStoreServiceContext("name=HyperStoreServiceContext");

            //TODO: retrieve the db name from a loginsign up module.
        }
    }
}