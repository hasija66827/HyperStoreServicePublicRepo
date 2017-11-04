using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO.InsightsDTO;
using PriorityQueueDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class ProductInsightsController : ApiController
    {
        private HyperStoreServiceContext db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="parameter"></param>
        /// <returns>The collection of product with the number of days it had hit the zero stock.</returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<SusceptibleProductsInsight>))]
        public IHttpActionResult GetSusceptibleProducts(Guid userId, SusceptibleProductsInsightDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (parameter == null)
                return BadRequest("Parameter should not have been null");

            db = UtilityAPI.RetrieveDBContext(userId);
            var filterDate = DateTime.Now.AddDays(Convert.ToDouble(-parameter.NumberOfDays)).Date;

            var susceptibleProductGrps = db.DeficientStockHits.Where(dsh => dsh.TimeStamp > filterDate)
                                                    .GroupBy(dsh => dsh.ProductId).ToList();

            var susceptibleProducts = susceptibleProductGrps.Select(pg => CountDaysHitOfTheProduct(pg));

            var compareDays = Comparer<int>.Create((p1, p2) => p2 - p1);
            var priorityQueue = new PriorityQueue<int, Product>(susceptibleProducts, compareDays);
            var topSusceptibleProducts = priorityQueue.DequeRange((int)parameter.NumberOfRecords);

            var susceptibleProductsInsight= new SusceptibleProductsInsight(susceptibleProductGrps.Count, topSusceptibleProducts);

            return Ok(susceptibleProductsInsight);
        }

        private KeyValuePair<int, Product> CountDaysHitOfTheProduct(IGrouping<Guid?, DeficientStockHit> item)
        {
            var product = new Product() { ProductId = (Guid)item.Key };            
            return new KeyValuePair<int, Product>(item.Count(), product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
