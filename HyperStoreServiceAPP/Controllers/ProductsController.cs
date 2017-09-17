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
using HyperStoreServiceAPP.CustomModels;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class ProductsController : ApiController, IProduct
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        [ResponseType(typeof(List<Product>))]
        public async Task<IHttpActionResult> Get(Guid userId, ProductFilterCriteria pfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            IQueryable<Product> query = db.Products;
            if (pfc == null)
                return Ok(await query.ToListAsync());

            IEnumerable<Product> result;
            try
            {
                var productId = pfc.ProductId;
                if (productId != null)
                {
                    query = query.Where(p => p.ProductId == productId);
                }

                var tagIds = pfc.TagIds;
                if (tagIds != null && tagIds.Count() != 0)
                {
                    var productIds_tag = await RetrieveProductIdsAsync(tagIds);
                    query = query.Where(p => productIds_tag.Contains(p.ProductId));
                }

                var filterProductQDT = pfc.FilterProductQDT;
                if (filterProductQDT != null)
                {
                    var discountPerRange = filterProductQDT.DiscountPerRange;
                    var quantity = filterProductQDT.QuantityRange;
                    query = query.Where(p => p.DiscountPer >= discountPerRange.LB &&
                                                                p.DiscountPer <= discountPerRange.UB &&
                                                                p.TotalQuantity >= quantity.LB &&
                                                                p.TotalQuantity <= quantity.UB);
                    if (filterProductQDT.IncludeDeficientItemsOnly == true)
                        query = query.Where(p => p.TotalQuantity <= p.Threshold);
                }
                result = await query.ToListAsync();
            }
            catch (Exception e)
            { throw e; }
            return Ok(result);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(Guid userId, Guid id, Product product)
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

        // Post: api/Products
        [ResponseType(typeof(Product))]
        public async Task<IHttpActionResult> Post(Guid userId, ProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (productDTO == null)
                return BadRequest("ProductDTO cannot be null");

            var newProductId = Guid.NewGuid();
            Product product = new Product()
            {
                ProductId = newProductId,
                TotalQuantity = 0,
                Code = productDTO.Code,
                CGSTPer = productDTO.CGSTPer,
                DiscountPer = productDTO.DiscountPer,
                DisplayPrice = productDTO.DisplayPrice,
                Name = productDTO.Name,
                RefillTime = productDTO.RefillTime,
                SGSTPer = productDTO.SGSTPer,
                Threshold = productDTO.Threshold
            };

            try
            {
                db.Products.Add(product);
                if (productDTO.TagIds != null && productDTO.TagIds.Count != 0)
                {
                    IEnumerable<ProductTag> productTags = productDTO.TagIds.Select(tagId => new ProductTag
                    {
                        ProductTagId = Guid.NewGuid(),
                        ProductId = newProductId,
                        TagId = tagId
                    });
                    db.ProductTags.AddRange(productTags);
                }
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductExists(product.ProductId))
                {
                    return Conflict();
                }
                var x = await TagExists(productDTO.TagIds);
                if (x != null)
                {
                    return BadRequest(String.Format("Tag with id {0} does not exists", x));
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, product);
        }

        [HttpGet]
        [ResponseType(typeof(List<ProductMetadata>))]
        public async Task<IHttpActionResult> GetProductMetadata(Guid userId)
        {
            var minQty = await db.Products.MinAsync(p => p.TotalQuantity);
            var maxQty = await db.Products.MaxAsync(p => p.TotalQuantity);
            var minDiscountPer = await db.Products.MinAsync(p => p.DiscountPer);
            var maxDiscountPer = await db.Products.MaxAsync(p => p.DiscountPer);
            var productMetadata = new ProductMetadata()
            {
                QuantityRange = new IRange<decimal>(minQty, maxQty),
                DiscountPerRange = new IRange<decimal?>(minDiscountPer, maxDiscountPer)
            };
            // Need to send the list as the client is expecting that.
            List<ProductMetadata> result = new List<ProductMetadata>();
            result.Add(productMetadata);
            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }

    public partial class ProductsController : ApiController, IProduct
    {

        private bool ProductExists(Guid? id)
        {
            return db.Products.Count(e => e.ProductId == id) > 0;
        }

        private async Task<Guid?> TagExists(List<Guid?> tagIds)
        {
            foreach (var tagId in tagIds)
            {
                var x = await db.Tags.CountAsync(t => t.TagId == tagId) > 0;
                if (!x)
                    return tagId;
            }
            return null;
        }

        /// <summary>
        /// Retrieves list of distinct product Id having atleast one tag in tagIds.
        /// </summary>
        /// <param name="tagIds"></param>
        /// <returns></returns>
        private async Task<List<Guid?>> RetrieveProductIdsAsync(List<Guid?> tagIds)
        {
            var result = db.ProductTags
                            .Where(pt => tagIds.Contains(pt.TagId))
                            .Select(pt => pt.ProductId)
                             .Distinct();
            return await result.ToListAsync();
        }
    }
}