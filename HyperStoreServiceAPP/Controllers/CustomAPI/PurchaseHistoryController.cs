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
    public class PurchaseHistoryController : ApiController, IPurchaseHistory
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        public IHttpActionResult Get(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.PurchaseHistory);
        }

        [HttpGet]
        public IHttpActionResult GetRecommendedProduct(Guid userId, Guid PersonId)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public IHttpActionResult PutReminderForProduct(Guid userId, SetReminderDTO setReminderDTO)
        {
            throw new NotImplementedException();
        }
    }
}
