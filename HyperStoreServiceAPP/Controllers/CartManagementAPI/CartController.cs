using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO.CartManagementDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CartManagementAPI
{
    public class CartController : ApiController, ICart
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(List<Product>))]
        public IHttpActionResult GetProductsInCart(Guid userId, Guid? id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            if (id == null)
                return BadRequest("Supplier Id should not have been null");

            var products = db.Products.Where(p => p.PotentielSupplierId == id);
            return Ok(products);
        }

        [HttpPost]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> AddProductToCart(Guid userId, CartDTO cartDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db = UtilityAPI.RetrieveDBContext(userId);
            if (cartDTO == null)
                return BadRequest("CartDTO should not have been null");

            var product = await db.Products.FindAsync(cartDTO.ProductId);

            _AddProductToSupplierBucket(product, cartDTO.SupplierId);

            db.Entry(product).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return Ok(product);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> AddLeftOverDeficientProductsToCart(Guid userId)
        {
            int count = 0;
            db = UtilityAPI.RetrieveDBContext(userId);
            var products = db.Products.Where(p => p.TotalQuantity <= p.Threshold);
            foreach (var product in products)
            {
                if (product.PotentielSupplierId == null && product.LatestSupplierId != null)
                {
                    _AddProductToSupplierBucket(product, product.LatestSupplierId);
                    db.Entry(product).State = EntityState.Modified;
                    count++;
                }
            }

            await db.SaveChangesAsync();
            return Ok(count);
        }

        private void _AddProductToSupplierBucket(Product product, Guid? supplierId)
        {
            if (supplierId == null)
                throw new Exception("SupplierId should not be null while adding product to supplier bucket.");

            if (product.PotentielSupplierId == supplierId)
            {
                product.PotentielQuantityPurhcased += 1;
            }
            else
            {
                product.PotentielSupplierId = supplierId;
                product.PotentielQuantityPurhcased = 0;
                product.IsPurchased = false;
                product.PotentielPurchasePrice = 0;
            }
        }
    }
}
