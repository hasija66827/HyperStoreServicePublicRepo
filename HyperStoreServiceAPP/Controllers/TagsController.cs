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
    public class TagsController : ApiController, ITag
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Tags
        public IQueryable<Tag> Get()
        {
            return db.Tags;
        }

        // PUT: api/Tags/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Put(Guid id, Tag tag)
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

        // Post: api/Tags
        [ResponseType(typeof(Tag))]
        public async Task<IHttpActionResult> Post(TagDTO tagDTO)
        {
            if (tagDTO == null)
                return BadRequest("TagDTO should not be null while creating a new tag");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var tag = new Tag()
            {
                TagId = Guid.NewGuid(),
                TagName = tagDTO.TagName,
            };
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TagExists(Guid? id)
        {
            return db.Tags.Count(e => e.TagId == id) > 0;
        }
    }
}