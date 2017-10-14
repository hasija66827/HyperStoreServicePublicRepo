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
        private HyperStoreServiceContext db;

        // GET: api/Customers/5
        [HttpGet]
        [ResponseType(typeof(List<Customer>))]
        public async Task<IHttpActionResult> Get(Guid userId, CustomerFilterCriteria cfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            db = UtilityAPI.RetrieveDBContext(userId);

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

        [HttpGet]
        [ResponseType(typeof(Customer))]
        public IHttpActionResult Get(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var customer = db.Customers.Find(id);
            return Ok(customer);
        }

        [HttpGet]
        [ResponseType(typeof(Int32))]
        public IHttpActionResult GetTotalRecordsCount(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.Customers.Count());
        }

        // PUT: api/Customers/5
        [HttpPut]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> Put(Guid userId, Guid id, CustomerDTO customerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (customerDTO == null)
            {
                throw new Exception("CustomerDTO should not have been null");
            }
            db = UtilityAPI.RetrieveDBContext(userId);

            var customer = await db.Customers.FindAsync(id);
            if (customer == null)
                throw new Exception(String.Format("Customer of id {0} not found while updating the customer", id));
           _UpdateCustomer(customer, customerDTO);
            db.Entry(customer).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(customer);
        }

        private void _UpdateCustomer(Customer customer, CustomerDTO customerDTO)
        {
            customer.Address = customerDTO.Address;
            customer.MobileNo = customerDTO.MobileNo;
            customer.Name = customerDTO.Name;
            customer.GSTIN = customerDTO.GSTIN;           
        }

        // POST: api/Customers
        [HttpPost]
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> Post(Guid userId, CustomerDTO customerDTO)
        {
            if (customerDTO == null)
                throw new Exception("CustomerDTO should not be null");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db = UtilityAPI.RetrieveDBContext(userId);

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
                if (IsNameExist(customer.Name))
                {
                    return BadRequest(String.Format("Customer with name {0} already exists.", customer.Name));
                }
                else if (IsMobNoExist(customer.MobileNo))
                {
                    return BadRequest(String.Format("Customer with mobile number {0} already exists.", customer.MobileNo));
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = customer.CustomerId }, customer);
        }

        [HttpGet]
        [ResponseType(typeof(IRange<decimal?>))]
        public async Task<IHttpActionResult> GetWalletBalanceRange(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            IRange<decimal?> walletBalanceRange = null;
            if (db.Customers.Count() != 0)
            {
                var minWalletBalance = await db.Customers.MinAsync(w => w.WalletBalance);
                var maxWalletBalance = await db.Customers.MaxAsync(w => w.WalletBalance);
                walletBalanceRange = new IRange<decimal?>(minWalletBalance, maxWalletBalance);

            }
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

        private bool IsNameExist(string customerName)
        {
            return db.Customers.Count(c => c.Name == customerName) > 0;
        }

        private bool IsMobNoExist(string customerMobileNo)
        {
            return db.Customers.Count(c => c.MobileNo == customerMobileNo) > 0;
        }

    }
}