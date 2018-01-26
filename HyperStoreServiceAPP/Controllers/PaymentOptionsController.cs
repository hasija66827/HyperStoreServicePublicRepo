﻿using System;
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
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/PaymentOptions
        [HttpGet]
        public IQueryable<PaymentOption> Get()
        {
            return db.PaymentOptions.OrderBy(p=>p.Name);
        }

        // GET: api/PaymentOptions/5
        [ResponseType(typeof(PaymentOption))]
        public async Task<IHttpActionResult> GetPaymentOption(Guid id)
        {
            PaymentOption paymentOption = await db.PaymentOptions.FindAsync(id);
            if (paymentOption == null)
            {
                return NotFound();
            }

            return Ok(paymentOption);
        }

        // PUT: api/PaymentOptions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPaymentOption(Guid id, PaymentOption paymentOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentOption.PaymentOptionId)
            {
                return BadRequest();
            }

            db.Entry(paymentOption).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentOptionExists(id))
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

        // POST: api/PaymentOptions
        [ResponseType(typeof(PaymentOption))]
        [HttpPost]
        public async Task<IHttpActionResult> Post(PaymentOption paymentOption)
        {
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
        public async Task<IHttpActionResult> DeletePaymentOption(Guid id)
        {
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