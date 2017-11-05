using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO.CartManagementDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
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
        public async Task<IHttpActionResult> AddProductToCart(Guid userId, AddProductToCartDTO cartDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db = UtilityAPI.RetrieveDBContext(userId);

            try
            {
                if (cartDTO == null)
                    return BadRequest("CartDTO should not have been null");

                var product = await db.Products.FindAsync(cartDTO.ProductId);
                if (product == null)
                    throw new Exception(String.Format("Product with id {0} not found", cartDTO.ProductId));

                _AddProductToSupplierBucket(product, cartDTO.SupplierId, cartDTO.QuantityPurchased);

                db.Entry(product).State = EntityState.Modified;

                await db.SaveChangesAsync();
                return Ok(product);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Supplier Id might not exist\n" + ex.Message);
            }
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
                    _AddProductToSupplierBucket(product, product.LatestSupplierId, 0);
                    db.Entry(product).State = EntityState.Modified;
                    count++;
                }
            }

            await db.SaveChangesAsync();
            return Ok(count);
        }


        [HttpPost]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> RemoveProductFromCart(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            var product = await db.Products.FindAsync(id);
            if (product == null)
                throw new Exception(String.Format("Product with id {0} not found", id));

            if (product.PotentielSupplierId == null)
                throw new Exception("Potentiel supplier of the product is already null");

            product.PotentielSupplierId = null;

            db.Entry(product).State = EntityState.Modified;

            await db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPost]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PurchaseProductInCart(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            var product = await db.Products.FindAsync(id);
            if (product == null)
                throw new Exception(String.Format("Product with id {0} not found", id));

            if (product.PotentielSupplierId == null)
                throw new Exception("Product should have been added to cart before purchasing it");

            if (product.IsPurchased == true)
                throw new Exception("Product is already in purchase state");

            product.IsPurchased = true;
            db.Entry(product).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPost]
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> UnPurchaseProductInCart(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            var product = await db.Products.FindAsync(id);
            if (product == null)
                throw new Exception(String.Format("Product with id {0} not found", id));

            if (product.PotentielSupplierId == null)
                throw new Exception("Product should have been added to cart before unpurchasing it");

            if (product.IsPurchased == false)
                throw new Exception("Product is already in unpurchase state");

            product.IsPurchased = false;
            db.Entry(product).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> EmptyShoppingCart(Guid userId, Guid id)
        {
            if (id == null)
                throw new Exception("supplierId should not have been null");
            int count = 0;
            db = UtilityAPI.RetrieveDBContext(userId);

            var products = db.Products.Where(p => p.PotentielSupplierId == id);
            foreach (var product in products)
            {
                product.PotentielSupplierId = null;
                db.Entry(product).State = EntityState.Modified;
                count++;
            }

            await db.SaveChangesAsync();
            return (Ok(count));
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> EmptyAllShoppingCart(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            int count = 0;

            var products = db.Products.Where(p => p.PotentielSupplierId != null);
            foreach (var product in products)
            {
                product.PotentielSupplierId = null;
                db.Entry(product).State = EntityState.Modified;
                count++;
            }

            await db.SaveChangesAsync();
            return (Ok(count));
        }

        private void _AddProductToSupplierBucket(Product product, Guid? supplierId, decimal? quantityPurchased)
        {
            if (supplierId == null)
                throw new Exception("SupplierId should not be null while adding product to supplier bucket.");

            if (product.PotentielSupplierId != supplierId)
            {
                product.PotentielSupplierId = supplierId;
                product.IsPurchased = false;
                product.PotentielPurchasePrice = 0;
            }
            product.PotentielQuantityPurhcased = quantityPurchased;
        }
    }
}
