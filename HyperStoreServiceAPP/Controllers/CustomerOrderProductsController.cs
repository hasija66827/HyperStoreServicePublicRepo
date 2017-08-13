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
    public class CustomerOrderProductsController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/CustomerOrderProducts
        public IQueryable<CustomerOrderProduct> GetCustomerOrderProducts()
        {
            return db.CustomerOrderProducts;
        }

        // GET: api/CustomerOrderProducts/5
        [ResponseType(typeof(CustomerOrderProduct))]
        public async Task<IHttpActionResult> GetCustomerOrderProduct(Guid id)
        {
            CustomerOrderProduct customerOrderProduct = await db.CustomerOrderProducts.FindAsync(id);
            if (customerOrderProduct == null)
            {
                return NotFound();
            }

            return Ok(customerOrderProduct);
        }
        /*
        // PUT: api/CustomerOrderProducts/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomerOrderProduct(Guid id, CustomerOrderProduct customerOrderProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerOrderProduct.CustomerOrderProductId)
            {
                return BadRequest();
            }

            db.Entry(customerOrderProduct).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerOrderProductExists(id))
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
        
        // POST: api/CustomerOrderProducts
        [ResponseType(typeof(CustomerOrderProduct))]
        public async Task<IHttpActionResult> PostCustomerOrderProduct(CustomerOrderProduct customerOrderProduct)
        {
            customerOrderProduct.CustomerOrderProductId = Guid.NewGuid();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerOrderProducts.Add(customerOrderProduct);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerOrderProductExists(customerOrderProduct.CustomerOrderProductId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = customerOrderProduct.CustomerOrderProductId }, customerOrderProduct);
        }

        // DELETE: api/CustomerOrderProducts/5
        [ResponseType(typeof(CustomerOrderProduct))]
        public async Task<IHttpActionResult> DeleteCustomerOrderProduct(Guid id)
        {
            CustomerOrderProduct customerOrderProduct = await db.CustomerOrderProducts.FindAsync(id);
            if (customerOrderProduct == null)
            {
                return NotFound();
            }

            db.CustomerOrderProducts.Remove(customerOrderProduct);
            await db.SaveChangesAsync();

            return Ok(customerOrderProduct);
        }
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerOrderProductExists(Guid? id)
        {
            return db.CustomerOrderProducts.Count(e => e.CustomerOrderProductId == id) > 0;
        }
    }
}