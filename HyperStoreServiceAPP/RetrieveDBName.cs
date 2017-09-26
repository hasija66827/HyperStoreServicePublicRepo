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
            //9977752717@abc123
            if (userId == new Guid("7f010986-2bf2-4564-bcba-26109c9e1b22"))
                return new HyperStoreServiceContext("name=HyperStoreServiceContext");
            else
                return new HyperStoreServiceContext("name=HyperStoreServiceContext");

            //TODO: retrieve the db name from a loginsign up module.
        }
    }
}