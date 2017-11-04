using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO.InsightsDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class CustomerInsightsController : ApiController
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(NewCustomerInsights))]
        public IHttpActionResult GetNewCustomers(Guid userId, CustomerInsightsDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (parameter == null)
                return BadRequest("Parameter should not have been null");

            db = UtilityAPI.RetrieveDBContext(userId);

            var filterDate = DateTime.Now.AddDays(Convert.ToDouble(-parameter.NumberOfDays)).Date;

            var newCustomers = db.Persons.Where(c => c.EntityType == DTO.EntityType.Customer &&
                                                    c.FirstVisited > filterDate)
                                            .OrderByDescending(c => c.NetWorth)
                                            .ToList();

            var topNewCustomers = newCustomers.Take((int)parameter.NumberOfRecords).ToList();
            return Ok(new NewCustomerInsights(newCustomers.Count, topNewCustomers));
        }


        [HttpGet]
        [ResponseType(typeof(DetachedCustomerInsights))]
        public IHttpActionResult GetDetachedCustomers(Guid userId, CustomerInsightsDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (parameter == null)
                return BadRequest("Parameter should not have been null");

            db = UtilityAPI.RetrieveDBContext(userId);

            var filterDate = DateTime.Now.AddDays(Convert.ToDouble(-parameter.NumberOfDays));

            var detachedCustomers = db.Persons.Where(c => c.EntityType == DTO.EntityType.Customer &&
                                                        c.LastVisited <= filterDate)
                                                    .OrderByDescending(c => c.NetWorth)
                                            .ToList();

            var topDetachedCustomer = detachedCustomers.Take((int)parameter.NumberOfRecords).ToList();
            var detachedCustomerInsights = new DetachedCustomerInsights(detachedCustomers.Count, topDetachedCustomer);
            return Ok(detachedCustomerInsights);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing & db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
