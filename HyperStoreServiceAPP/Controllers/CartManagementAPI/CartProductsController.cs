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

        [HttpPost]
        public async Task<IHttpActionResult> AddProductInLiveCart(Guid userId, PersonProductDTO personProductsDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (personProductsDTO == null)
                throw new Exception("personproductDTO should not have been null.");

            db = UtilityAPI.RetrieveDBContext(userId);

            var liveCart = _RetrievesLiveCart((Guid)personProductsDTO.PersonId);

            if (liveCart == null)
                throw new Exception("Live cart should not have been null.");

            var IsProductExistInLiveCart = db.CartProducts.Where(cp => cp.CartId == liveCart.CartId &&
                                                                       cp.ProductId == personProductsDTO.ProductId)
                                                           .Count() > 0;

            if (IsProductExistInLiveCart == true)
               return BadRequest("Product already exists in live cart.");

            var cartProduct = new CartProduct()
            {
                CartProductId = Guid.NewGuid(),
                CartId = liveCart.CartId,
                ProductId = personProductsDTO.ProductId,
                PotentielPurchasePrice = 0,
                PotentielQuantityPurhcased = 0,             
            };

            db.CartProducts.Add(cartProduct);
            await db.SaveChangesAsync();

            return Ok(cartProduct);
        }

        public Task<IHttpActionResult> RemoveProductFromLiveCart(Guid userId, PersonProductDTO PersonProductsDTO)
        {
            throw new NotImplementedException();
        }
    }
}
