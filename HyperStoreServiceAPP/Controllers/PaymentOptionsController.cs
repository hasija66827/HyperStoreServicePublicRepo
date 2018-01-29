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
    public class PaymentOptionsController : ApiController
    {
        private HyperStoreServiceContext db;

        // GET: api/PaymentOptions
        [HttpGet]
        public IQueryable<PaymentOption> Get(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return db.PaymentOptions.OrderBy(p => p.Name);
        }

        // GET: api/PaymentOptions/5
        [ResponseType(typeof(PaymentOption))]
        public async Task<IHttpActionResult> GetPaymentOption(Guid userId, Guid id)
        {
            if (id == null)
                return BadRequest("PaymentId should not be null");

            db = UtilityAPI.RetrieveDBContext(userId);

            PaymentOption paymentOption = await db.PaymentOptions.FindAsync(id);
            if (paymentOption == null)
            {
                return NotFound();
            }
            return Ok(paymentOption);
        }

        // POST: api/PaymentOptions
        [ResponseType(typeof(PaymentOption))]
        [HttpPost]
        public async Task<IHttpActionResult> Post(Guid userId, PaymentOption paymentOption)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PaymentOptions.Add(paymentOption);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PaymentOptionExists(paymentOption.PaymentOptionId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = paymentOption.PaymentOptionId }, paymentOption);
        }

        // DELETE: api/PaymentOptions/5
        [ResponseType(typeof(PaymentOption))]
        public async Task<IHttpActionResult> DeletePaymentOption(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            PaymentOption paymentOption = await db.PaymentOptions.FindAsync(id);
            if (paymentOption == null)
            {
                return NotFound();
            }

            db.PaymentOptions.Remove(paymentOption);
            await db.SaveChangesAsync();

            return Ok(paymentOption);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentOptionExists(Guid id)
        {
            return db.PaymentOptions.Count(e => e.PaymentOptionId == id) > 0;
        }
    }
}