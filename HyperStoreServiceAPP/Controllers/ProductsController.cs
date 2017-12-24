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
using HyperStoreServiceAPP.DTO;
using HyperStoreServiceAPP.Controllers.CustomAPI;

namespace HyperStoreServiceAPP.Controllers
{
    public partial class ProductsController : ApiController, IProduct
    {
        private HyperStoreServiceContext db;

        #region Read
        [HttpGet]
        [ResponseType(typeof(List<Product>))]
        public async Task<IHttpActionResult> Get(Guid userId, ProductFilterCriteriaDTO pfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db = UtilityAPI.RetrieveDBContext(userId);

            var query = await ConstructQuery(pfc);
            var queryResult = await query.ToListAsync();

            if (pfc != null && pfc.FilterProductQDT != null && pfc.FilterProductQDT.ShowInventoryProductsOnly == true)
                queryResult = await FilterProductByConsumptionDayRange(queryResult, userId, pfc.FilterProductQDT.ConsumptionDayRange);

            List<ProductInsight> productInsights = new List<ProductInsight>();
            foreach (var product in queryResult)
            {
                var productInsight = new ProductInsight()
                {
                    Product = product,
                    ProductExtinctionDate = product.TotalQuantity == null ? null : await ProductConsumptionInsights.GetProductStockCompletionDate(userId, (Guid)product.ProductId, (float)product.TotalQuantity),
                    MapDay_ProductEstConsumption = await ProductConsumptionInsights.GetProductConsumptionTrend(userId, (Guid)product.ProductId)
                };
                productInsights.Add(productInsight);
            }
            return Ok(productInsights);
        }

        private async Task<IQueryable<Product>> ConstructQuery(ProductFilterCriteriaDTO pfc)
        {
            IQueryable<Product> query = db.Products;
            if (pfc == null)
                return query;

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
                query = query.Where(p => p.DiscountPer >= discountPerRange.LB && p.DiscountPer <= discountPerRange.UB);
                if (filterProductQDT.ShowInventoryProductsOnly == true)
                {
                    var quantity = filterProductQDT.QuantityRange;
                    query = query.Where(p => p.TotalQuantity >= quantity.LB && p.TotalQuantity <= quantity.UB);
                }
                else
                {
                    query = query.Where(p => p.TotalQuantity == null);
                }
            }
            return query;
        }

        /// <summary>
        /// Filter the inventory product by consumptionDays.
        /// It does not include non inventory product in its result set.
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="userId"></param>
        /// <param name="consumptionDayRange"></param>
        /// <returns></returns>
        private async Task<List<Product>> FilterProductByConsumptionDayRange(List<Product> queryResult, Guid userId, IRange<int?> consumptionDayRange)
        {
            List<Product> products = new List<Product>();
            foreach (var product in queryResult)
            {
                if (product.TotalQuantity != null)
                {
                    var IsProductConsumed = await ProductConsumptionInsights.IsProductConsumed(userId, (Guid)product.ProductId, (float)product.TotalQuantity,
                                                                                                                                    consumptionDayRange);
                    if (IsProductConsumed)
                        products.Add(product);
                }
            }
            return products;
        }

        [HttpGet]
        [ResponseType(typeof(Product))]
        public IHttpActionResult Get(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var product = db.Products.Find(id);
            return Ok(product);
        }

        [HttpGet]
        [ResponseType(typeof(Int32))]
        public IHttpActionResult GetTotalRecordsCount(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.Products.Count());
        }
        #endregion

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
            db = UtilityAPI.RetrieveDBContext(userId);

            var newProductId = Guid.NewGuid();
            Product product = new Product()
            {
                ProductId = newProductId,
                Code = productDTO.Code,
                CGSTPer = productDTO.CGSTPer,
                DiscountPer = productDTO.DiscountPer,
                HSN = productDTO.HSN,
                MRP = productDTO.MRP,
                Name = productDTO.Name,
                SGSTPer = productDTO.SGSTPer,
            };

            if (productDTO.IsNonInventoryProduct == true)
                product.TotalQuantity = null;
            else
                product.TotalQuantity = 0;

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
        [ResponseType(typeof(ProductMetadata))]
        public async Task<IHttpActionResult> GetProductMetadata(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            ProductMetadata productMetadata = null;
            if (db.Products.Count() != 0)
            {
                var minQty = await db.Products.MinAsync(p => p.TotalQuantity);
                var maxQty = await db.Products.MaxAsync(p => p.TotalQuantity);
                var minDiscountPer = await db.Products.MinAsync(p => p.DiscountPer);
                var maxDiscountPer = await db.Products.MaxAsync(p => p.DiscountPer);
                productMetadata = new ProductMetadata()
                {
                    QuantityRange = new IRange<float?>(minQty, maxQty),
                    DiscountPerRange = new IRange<decimal?>(minDiscountPer, maxDiscountPer),
                    DayRange = new IRange<int?>(-1, 30),
                    //TODO: remove constants
                };
            }
            return Ok(productMetadata);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
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