using HyperStoreService.Models;
using HyperStoreService.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HyperStoreServiceAPP.DTO.RecommendedProductDTO;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class RecommendedProductsController : ApiController, IRecommendedProduct
    {
        private HyperStoreServiceContext db;

        public IHttpActionResult GetRecommendedProduct(Guid userId, Guid PersonId)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult PutReminderForProduct(Guid userId, SetReminderDTO setReminderDTO)
        {
            throw new NotImplementedException();
        }
    }
}
