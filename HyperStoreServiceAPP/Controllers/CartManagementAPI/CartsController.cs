  using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HyperStoreServiceAPP.DTO.CartManagementDTO;

namespace HyperStoreServiceAPP.Controllers.CartManagementAPI
{
    public class CartsController : ApiController, ICart
    {
        private HyperStoreServiceContext db;

        public IHttpActionResult CompleteTheOrderInLiveCart(Guid userId)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult UpdateLiveCartMetadata(UpdateLiveCartDTO updateLiveCartDTO)
        {
            throw new NotImplementedException();
        }
    }
}
