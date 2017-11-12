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
        public IHttpActionResult UpdateTheProductsInLiveCart(PersonProductsDTO PersonProductsDTO)
        {
            throw new NotImplementedException();
        }
    }
}
