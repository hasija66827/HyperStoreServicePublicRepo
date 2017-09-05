using HyperStoreService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class ProductConsumptionTrendController : ApiController, ProductTrendInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ProductConsumptionTrend>))]
        public async Task<IHttpActionResult> GetProductTrend(ProductConsumptionTrendDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (parameter == null)
                return BadRequest("The parameter should not have been NULL");

            IEnumerable<ProductConsumptionTrend> result;
            try
            {
                var startingOrderDate = DateTime.Now.AddMonths(-(int)parameter.MonthsCount);
                var query = db.CustomerOrderProducts
                            .Include(cop => cop.CustomerOrder)
                            .Where(cop => cop.ProductId == parameter.ProductId
                                            && cop.CustomerOrder.OrderDate >= startingOrderDate);
                var queryResult = await query.ToListAsync();
                var groupByQueryResult = queryResult
                                            .GroupBy(cop => cop.CustomerOrder.OrderDate.DayOfWeek);
                result = groupByQueryResult.Select(c => AggregateQuantity(c));

                var deficiencyHitRate = await ComputeDeficiencyHits(parameter);
            }
            catch
            { throw; }
            return Ok(result);
        }

        private ProductConsumptionTrend AggregateQuantity(IGrouping<DayOfWeek, CustomerOrderProduct> items)
        {
            return new ProductConsumptionTrend(items.Key, items.Sum(cop => (float)cop.QuantityConsumed));
        }

        private async Task<IEnumerable<ProductDeficiencyHitTrend>> ComputeDeficiencyHits(ProductConsumptionTrendDTO parameter)
        {
            var startingTimeStamp = DateTime.Now.AddMonths(-(int)parameter.MonthsCount).Date;
            var xQuery = await db.DeficientStockHits.Where(ds => ds.ProductId == parameter.ProductId &&
                                                        ds.TimeStamp >= startingTimeStamp).ToListAsync();
            var yQuery = xQuery.GroupBy(ds => ds.TimeStamp.DayOfWeek);
            var deficiencyHits = yQuery.Select(ds => CountDaysHitOfTheDay(ds));
            return deficiencyHits;
        }

        private ProductDeficiencyHitTrend CountDaysHitOfTheDay(IGrouping<DayOfWeek, DeficientStockHit> items)
        {
            return new ProductDeficiencyHitTrend(items.Key, items.Count());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
