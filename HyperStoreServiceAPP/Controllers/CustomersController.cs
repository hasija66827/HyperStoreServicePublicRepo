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
    public class CustomersController : ApiController, ICustomer
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();
        // GET: api/Customers/5
        [HttpGet]
        [ResponseType(typeof(List<Customer>))]
        public async Task<IHttpActionResult> Get(CustomerFilterCriteria cfc)
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
        [HttpPut]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> Put(Guid id, CustomerDTO customerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (customerDTO == null)
            {
                throw new Exception("CustomerDTO should not have been null");
            }
            var customer = await db.Customers.FindAsync(id);
            if (customer == null)
                throw new Exception(String.Format("Customer of id {0} not found while updating the customer", id));
            var updatedCustomer = _UpdateCustomer(customer, customerDTO);
            db.Entry(updatedCustomer).State = EntityState.Modified;
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
            return Ok(updatedCustomer);
        }

        private Customer _UpdateCustomer(Customer customer, CustomerDTO customerDTO)
        {
            var updatedCustomer = customer;
            updatedCustomer.Address = customerDTO.Address;
            updatedCustomer.MobileNo = customerDTO.MobileNo;
            updatedCustomer.Name = customerDTO.Name;
            updatedCustomer.GSTIN = customerDTO.GSTIN;
            return updatedCustomer;
        }

        // POST: api/Customers
        [HttpPost]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> Post(CustomerDTO customerDTO)
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
                WalletBalance = 0,
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

        [HttpGet]
        [ResponseType(typeof(IRange<decimal>))]
        public async Task<IHttpActionResult> GetWalletBalanceRange()
        {
            var minWalletBalance = await db.Customers.MinAsync(w => w.WalletBalance);
            var maxWalletBalance = await db.Customers.MaxAsync(w => w.WalletBalance);

            var walletBalanceRange = new IRange<decimal?>(minWalletBalance,maxWalletBalance);
            return Ok(walletBalanceRange);
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