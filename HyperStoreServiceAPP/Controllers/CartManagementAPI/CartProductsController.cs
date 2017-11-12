using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HyperStoreServiceAPP.DTO.CartManagementDTO;

namespace HyperStoreServiceAPP.Controllers.CartManagementAPI
{
    public class CartProductsController : ApiController, ICartProduct
    {
        public IHttpActionResult GetTheProductInLiveCart(Guid userId, Guid id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult UpdateTheProductsInLiveCart(Guid userId, PersonProductsDTO PersonProductsDTO)
        {
            throw new NotImplementedException();
        }
    }
}
