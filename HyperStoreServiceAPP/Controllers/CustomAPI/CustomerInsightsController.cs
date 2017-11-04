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
        [ResponseType(typeof(IEnumerable<Person>))]
        public IHttpActionResult GetNewCustomer(Guid userId, CustomerInsightsDTO parameter)
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

            var topNewCustomers = newCustomers.Take((int)parameter.NumberOfRecords);

            return Ok(topNewCustomers);
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<Person>))]
        public IHttpActionResult GetNotVisitedCustomer(Guid userId, CustomerInsightsDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (parameter == null)
                return BadRequest("Parameter should not have been null");

            db = UtilityAPI.RetrieveDBContext(userId);
            var filterDate = DateTime.Now.AddDays(Convert.ToDouble(-parameter.NumberOfDays));

            var oldCustomers = db.Persons.Where(c => c.EntityType == DTO.EntityType.Customer &&
                                                    c.LastVisited <= filterDate)
                                                    .OrderByDescending(c => c.NetWorth)
                                            .ToList();

            var limitedNewCustomers = oldCustomers.Take((int)parameter.NumberOfRecords);

            return Ok(limitedNewCustomers);
        }

        protected override void Dispose(bool disposing)
        {
             if (disposing & db!=null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
