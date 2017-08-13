using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HyperStoreService.Models;
using System.ComponentModel.DataAnnotations;

namespace HyperStoreServiceAPP.Controllers
{
    interface ProductContInt
    {
        IQueryable<Product> GetProducts();
        Task<IHttpActionResult> GetProduct(Guid id);
        Task<IHttpActionResult> PutProduct(Guid id, Product product);
        Task<IHttpActionResult> PostProduct(ProductDTO product);
        Task<IHttpActionResult> DeleteProduct(Guid id);

    }
    public class ProductDTO
    {
        public float? CGSTPer { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public float? DisplayPrice { get; set; }
        public float DiscountPer { get; set; }
        [Required]
        public string Name { get; set; }
        public Int32 RefillTime { get; set; }
        public float? SGSTPer { get; set; }
        public Int32 Threshold { get; set; }
    }

    public class ProductsController : ApiController, ProductContInt
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Products
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> GetProduct(Guid id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProduct(Guid id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ProductId)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> PostProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Product product = new Product()
            {
                ProductId = Guid.NewGuid(),
                TotalQuantity = 0,
                SupplierId = null,
                Code = productDTO.Code,
                CGSTPer =productDTO.CGSTPer,
                DiscountPer=productDTO.DiscountPer,
                DisplayPrice=productDTO.DisplayPrice,
                Name=productDTO.Name,
                RefillTime=productDTO.RefillTime,
                SGSTPer=productDTO.SGSTPer,
                Threshold=productDTO.Threshold
            };
            db.Products.Add(product);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> DeleteProduct(Guid id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(Guid? id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }
    }
}