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
    public class CustomersController : ApiController, CustomerInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();
        // GET: api/Customers/5
        [HttpGet]
        [ResponseType(typeof(List<Customer>))]
        public async Task<IHttpActionResult> GetCustomers(CustomerFilterCriteria cfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            IQueryable<Customer> query = db.Customers;
            if (cfc == null)
                return Ok(await query.ToListAsync());
            try
            {
                if (cfc.WalletAmount != null)
                    query = db.Customers.Where(s => s.WalletBalance >= cfc.WalletAmount.LB &&
                                                    s.WalletBalance <= cfc.WalletAmount.UB);

                if (cfc.CustomerId != null)
                    query = query.Where(c => c.CustomerId == cfc.CustomerId);
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch
            {
                throw;
            }
        }

        // PUT: api/Customers/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomer(Guid id, Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            db.Entry(customer).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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

        // POST: api/Customers
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> PostCustomer(CustomerDTO customerDTO)
        {
            if (customerDTO == null)
                throw new Exception("CustomerDTO should not be null");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = new Customer()
            {
                CustomerId = Guid.NewGuid(),
                Address = customerDTO.Address,
                GSTIN = customerDTO.GSTIN,
                MobileNo = customerDTO.MobileNo,
                Name = customerDTO.Name,
                NetWorth = 0,
                WalletBalance = customerDTO.WalletBalance,
            };
            db.Customers.Add(customer);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customers/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> DeleteCustomer(Guid id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            db.Customers.Remove(customer);
            await db.SaveChangesAsync();

            return Ok(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerExists(Guid? id)
        {
            return db.Customers.Count(e => e.CustomerId == id) > 0;
        }
    }
}