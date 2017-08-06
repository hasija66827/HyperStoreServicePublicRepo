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

namespace HyperStoreServiceAPP.Controllers
{
    public class ProductTagsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/ProductTags
        public IQueryable<ProductTag> GetProductTags()
        {
            return db.ProductTags;
        }

        // GET: api/ProductTags/5
        [ResponseType(typeof(ProductTag))]
        public async Task<IHttpActionResult> GetProductTag(Guid id)
        {
            ProductTag productTag = await db.ProductTags.FindAsync(id);
            if (productTag == null)
            {
                return NotFound();
            }

            return Ok(productTag);
        }

        // PUT: api/ProductTags/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProductTag(Guid id, ProductTag productTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productTag.ProductTagId)
            {
                return BadRequest();
            }

            db.Entry(productTag).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTagExists(id))
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

        // POST: api/ProductTags
        [ResponseType(typeof(ProductTag))]
        public async Task<IHttpActionResult> PostProductTag(ProductTag productTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProductTags.Add(productTag);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProductTagExists(productTag.ProductTagId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = productTag.ProductTagId }, productTag);
        }

        // DELETE: api/ProductTags/5
        [ResponseType(typeof(ProductTag))]
        public async Task<IHttpActionResult> DeleteProductTag(Guid id)
        {
            ProductTag productTag = await db.ProductTags.FindAsync(id);
            if (productTag == null)
            {
                return NotFound();
            }

            db.ProductTags.Remove(productTag);
            await db.SaveChangesAsync();

            return Ok(productTag);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductTagExists(Guid id)
        {
            return db.ProductTags.Count(e => e.ProductTagId == id) > 0;
        }
    }
}