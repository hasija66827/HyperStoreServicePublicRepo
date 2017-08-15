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
        //IQueryable<Product> GetProducts();
        //Task<IHttpActionResult> GetProduct(Guid id);
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
        public List<Guid?> TagIds { get; set; }
    }

    public class IRange<T>
    {
        [Required]
        public T LB { get; set; }
        [Required]
        public T UB { get; set; }
        public IRange(T lb, T ub)
        {
            LB = lb;
            UB = ub;
        }
    }
    public sealed class QuantityRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var quantityRange = value as IRange<float?>;
            return (quantityRange.LB > 0 && quantityRange.LB < quantityRange.UB);
        }
    }

    public sealed class DiscountPerRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var discountPerRange = value as IRange<float?>;
            bool valid = (discountPerRange.LB <= discountPerRange.UB && discountPerRange.LB >= 0 && discountPerRange.UB <= 100);
            return valid;
        }
    }

    public class FilterProductCriteria
    {
        public Guid? ProductId { get; set; }
        public List<Guid?> TagIds { get; set; }
        [Required]
        public FilterProductQDT FilterProductQDT { get; set; }
    }

    public class FilterProductQDT
    {
        [Required]
        [DiscountPerRange]
        public IRange<float?> DiscountPerRange { get; set; }

        [Required]
        [QuantityRange]
        public IRange<float?> QuantityRange { get; set; }

        [Required]
        public bool? IncludeDeficientItemsOnly { get; set; }
    }

    public class ProductsController : ApiController, ProductContInt
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        /*
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
        */
        [HttpGet]
        [ResponseType(typeof(IEnumerable<Product>))]
        public async Task<IHttpActionResult> GetProducts(FilterProductCriteria filterProductCriteria)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (filterProductCriteria == null)
                return BadRequest("Input parameter cannot be null");

            var productId = filterProductCriteria.ProductId;
            var tagIds = filterProductCriteria.TagIds;
            var filterProductQDT = filterProductCriteria.FilterProductQDT;
            var discountPerRange = filterProductQDT.DiscountPerRange;
            var quantity = filterProductQDT.QuantityRange;

            IEnumerable<Product> result;
            try
            {
                IQueryable<Product> query = db.Products;

                if (productId != null)
                    query = query.Where(p => p.ProductId == productId);

                if (filterProductQDT != null)
                {
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
            var newProductId = Guid.NewGuid();
            Product product = new Product()
            {
                ProductId = newProductId,
                TotalQuantity = 0,
                SupplierId = null,
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

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, productDTO);
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
    }
}