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
    public class TagsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Tags
        public IQueryable<Tag> GetTags()
        {
            return db.Tags;
        }

        // GET: api/Tags/5
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> GetTag(Guid id)
        {
            Tag tag = await db.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            return Ok(tag);
        }

        // PUT: api/Tags/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTag(Guid id, Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tag.TagId)
            {
                return BadRequest();
            }

            db.Entry(tag).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(id))
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

        // POST: api/Tags
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> PostTag(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tags.Add(tag);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TagExists(tag.TagId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tag.TagId }, tag);
        }

        // DELETE: api/Tags/5
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> DeleteTag(Guid id)
        {
            Tag tag = await db.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            db.Tags.Remove(tag);
            await db.SaveChangesAsync();

            return Ok(tag);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TagExists(Guid id)
        {
            return db.Tags.Count(e => e.TagId == id) > 0;
        }
    }
}