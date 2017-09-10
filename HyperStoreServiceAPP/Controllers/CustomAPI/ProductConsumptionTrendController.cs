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
        //Average number of THE day in a month
        private readonly float AvgNoOfTheDayInMonth = 4.2f;
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ProductConsumptionDeficientTrend>))]
        public async Task<IHttpActionResult> Get(ProductConsumptionTrendDTO parameter)
        {
            //TODO: check if the product exist
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (parameter == null)
                return BadRequest("The parameter should not have been NULL");

            try
            {
                var productConsumptionTrends = await ComputeProductConsumptionTrend(parameter);
                var productDeficiencyHitTrends = await ComputeDeficiencyHits(parameter);
                var result = ComputeProductConsumptionDeficientTrend(productConsumptionTrends, productDeficiencyHitTrends, parameter.MonthsCount);
                return Ok(result);
            }
            catch
            { throw; }
        }

        private IEnumerable<ProductConsumptionDeficientTrend> ComputeProductConsumptionDeficientTrend(IEnumerable<ProductConsumptionTrend> productConsumptionTrends, IEnumerable<ProductDeficiencyHitTrend> productDeficiencyHitTrends, int? monthsCount)
        {
            List<ProductConsumptionDeficientTrend> result = new List<ProductConsumptionDeficientTrend>(7);
            float noOfTheDay = (float)(AvgNoOfTheDayInMonth * monthsCount);
            for (var index = 0; index < 7; index++)
            {
                result.Add(new ProductConsumptionDeficientTrend() {
                    Day = (DayOfWeek)index,
                    AvgConsumption = 0,
                    AvgHitRate = 0,
                });
            }

            foreach (var item in productConsumptionTrends)
            {
                Int32 index = Convert.ToInt16(item.Day);
                result[index].AvgConsumption = item.TotalQuantityConsumed / noOfTheDay;
            }

            foreach (var item in productDeficiencyHitTrends)
            {
                Int32 index = Convert.ToInt16(item.Day);
                result[index].AvgHitRate = Math.Min(1, item.TotalDaysHit / noOfTheDay);
            }
            return result;
        }

        private async Task<IEnumerable<ProductConsumptionTrend>> ComputeProductConsumptionTrend(ProductConsumptionTrendDTO parameter)
        {
            var startingOrderDate = DateTime.Now.AddMonths(-(int)parameter.MonthsCount);
            var query = db.CustomerOrderProducts
                        .Include(cop => cop.CustomerOrder)
                        .Where(cop => cop.ProductId == parameter.ProductId
                                        && cop.CustomerOrder.OrderDate >= startingOrderDate);
            var queryResult = await query.ToListAsync();
            var groupByQueryResult = queryResult
                                        .GroupBy(cop => cop.CustomerOrder.OrderDate.DayOfWeek);
            var productConsumptionTrends = groupByQueryResult.Select(c => AggregateSumQuantity(c));
            return productConsumptionTrends;
        }

        private ProductConsumptionTrend AggregateSumQuantity(IGrouping<DayOfWeek, CustomerOrderProduct> items)
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
