using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HyperStoreServiceAPP.DTO.CartManagementDTO;
using HyperStoreService.Models;
using System.Threading.Tasks;
using System.Data.Entity;

namespace HyperStoreServiceAPP.Controllers.CartManagementAPI
{
    public class CartProductsController : ApiController, ICartProduct
    {
        private HyperStoreServiceContext db;
        public async Task<IHttpActionResult> GetProductsInLiveCart(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var liveCart = _RetrievesLiveCart(id);
            if (liveCart == null)
            {
                liveCart = _CreateLiveCart(id);
                await db.SaveChangesAsync();
            }
            var cartProducts = db.CartProducts.Where(cp => cp.CartId == liveCart.CartId)
                                            .Include(cp => cp.Product);
            return Ok(cartProducts);
        }

        /// <summary>
        /// Retrieves the live cart, if there is no live cart, it will create new live cart.
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        private Cart _RetrievesLiveCart(Guid personId)
        {
            var cart = db.Carts.Where(c => c.CartStatus != CartStatus.OrderCompleted &&
                                            c.PersonId == personId)
                                .FirstOrDefault();
            return cart;
        }

        private Cart _CreateLiveCart(Guid personId)
        {
            var cart = new Cart()
            {
                CartId = Guid.NewGuid(),
                CartStatus = CartStatus.OrderInitiated,
                IsBookmarked = false,
                OrderCompletionDate = null,
                PersonId = personId,
                PreferedDeliveryTime = null,
            };
            db.Carts.Add(cart);
            return cart;

        }

        public IHttpActionResult UpdateTheProductsInLiveCart(Guid userId, PersonProductsDTO PersonProductsDTO)
        {
            throw new NotImplementedException();
        }
    }
}
