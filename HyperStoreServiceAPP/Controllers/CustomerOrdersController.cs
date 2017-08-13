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
    public class FilterOrderDateRange
    {
        [Required]
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return this._startDate; }
            set { this._startDate = value; }
        }

        [Required]
        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return this._endDate; }
            set { this._endDate = value; }
        }

        public FilterOrderDateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }
    }

    public class CustomerOrderFilterCriteria
    {
        public Guid? CustomerId;
        public string CustomerOrderNo;
        [Required]
        public FilterOrderDateRange DateRange;
    }

    interface CustomerOrderInt
    {
        Task<IHttpActionResult> GetCustomerOrders(CustomerOrderFilterCriteria customerOrderFilterCriteria);
    }


    public class CustomerOrdersController : ApiController, CustomerOrderInt
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        /*
        // GET: api/CustomerOrders
        public IQueryable<CustomerOrder> GetCustomerOrders()
        {
            return db.CustomerOrders;
        }
        
        // GET: api/CustomerOrders/5
        [ResponseType(typeof(CustomerOrder))]
        public async Task<IHttpActionResult> GetCustomerOrder(Guid id)
        {
            CustomerOrder customerOrder = await db.CustomerOrders.FindAsync(id);
            if (customerOrder == null)
            {
                return NotFound();
            }

            return Ok(customerOrder);
        }
        */
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomerOrders(CustomerOrderFilterCriteria customerOrderFilterCriteria)
        {
            try
            {
                var selectedCustomerId = customerOrderFilterCriteria.CustomerId;
                var selectedCustomerOrderNo = customerOrderFilterCriteria.CustomerOrderNo;
                var selectedDateRange = customerOrderFilterCriteria.DateRange;

                if (selectedDateRange == null)
                    throw new Exception("A Date Range cannot be null");
                if (selectedDateRange.StartDate > selectedDateRange.EndDate)
                    throw new Exception(String.Format("Start Date {0} cannot be ahead of EndDate {1}", selectedDateRange.StartDate, selectedDateRange.EndDate));
                var commonQuery = db.CustomerOrders
                                    .Where(order => order.OrderDate >= selectedDateRange.StartDate.Date &&
                                                        order.OrderDate <= selectedDateRange.EndDate.Date);

                IQueryable<CustomerOrder> query = commonQuery;
                if (selectedCustomerId != null)
                {
                    query = commonQuery.Where(order => order.CustomerId == selectedCustomerId);
                }
                if (selectedCustomerOrderNo != null)
                {
                    query = commonQuery.Where(order => order.CustomerOrderNo == selectedCustomerOrderNo);
                }
                var queryResult = query.OrderByDescending(order => order.OrderDate);
                return Ok(await queryResult.ToListAsync());
            }
            catch (Exception e)
            { throw e; }
        }
        /*
        // PUT: api/CustomerOrders/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCustomerOrder(Guid id, CustomerOrder customerOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerOrder.CustomerOrderId)
            {
                return BadRequest();
            }

            db.Entry(customerOrder).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerOrderExists(id))
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

        // POST: api/CustomerOrders
        [ResponseType(typeof(CustomerOrder))]
        public async Task<IHttpActionResult> PostCustomerOrder(CustomerOrder customerOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerOrders.Add(customerOrder);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerOrderExists(customerOrder.CustomerOrderId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = customerOrder.CustomerOrderId }, customerOrder);
        }

        // DELETE: api/CustomerOrders/5
        [ResponseType(typeof(CustomerOrder))]
        public async Task<IHttpActionResult> DeleteCustomerOrder(Guid id)
        {
            CustomerOrder customerOrder = await db.CustomerOrders.FindAsync(id);
            if (customerOrder == null)
            {
                return NotFound();
            }

            db.CustomerOrders.Remove(customerOrder);
            await db.SaveChangesAsync();

            return Ok(customerOrder);
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

        private bool CustomerOrderExists(Guid? id)
        {
            return db.CustomerOrders.Count(e => e.CustomerOrderId == id) > 0;
        }
    }
}