using HyperStoreService.HyperStoreService.CustomModels;
using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO;
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
    public class ProductConsumptionTrendController : ApiController, IProductTrend
    {
        //Average number of THE day in a month
        private readonly float AvgNoOfTheDayInMonth = 4.2f;
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(IEnumerable<AveragaeConsumptionOfProductInDayOfWeek>))]
        public async Task<IHttpActionResult> Get(Guid userId, ProductConsumptionTrendDTO parameter)
        {
            //TODO: check if the product exist
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (parameter == null)
                return BadRequest("The parameter should not have been NULL");
            db = UtilityAPI.RetrieveDBContext(userId);

            try
            {
                var productConsumptionTrends = await ComputeProductConsumptionTrend(parameter);
                var result = ComputeProductConsumptionDeficientTrend(productConsumptionTrends, parameter.MonthsCount);
                return Ok(result);
            }
            catch
            { throw; }
        }

        private IEnumerable<AveragaeConsumptionOfProductInDayOfWeek> ComputeProductConsumptionDeficientTrend(IEnumerable<ProductConsumptionTrend> productConsumptionTrends, int? monthsCount)
        {
            List<AveragaeConsumptionOfProductInDayOfWeek> result = new List<AveragaeConsumptionOfProductInDayOfWeek>(7);
            float noOfTheDay = (float)(AvgNoOfTheDayInMonth * monthsCount);
            for (var index = 0; index < 7; index++)
            {
                result.Add(new AveragaeConsumptionOfProductInDayOfWeek() {
                    Day = (DayOfWeek)index,
                    AvgConsumption = 0,
                });
            }

            foreach (var item in productConsumptionTrends)
            {
                Int32 index = Convert.ToInt16(item.Day);
                result[index].AvgConsumption = item.TotalQuantityConsumed / noOfTheDay;
            }
            return result;
        }

        private async Task<IEnumerable<ProductConsumptionTrend>> ComputeProductConsumptionTrend(ProductConsumptionTrendDTO parameter)
        {
            var startingOrderDate = DateTime.Now.AddMonths(-(int)parameter.MonthsCount);
            var query = db.OrderProducts
                        .Include(cop => cop.Order)
                        .Where(cop => cop.ProductId == parameter.ProductId
                                        && cop.Order.OrderDate >= startingOrderDate);
            var queryResult = await query.ToListAsync();
            var groupByQueryResult = queryResult
                                        .GroupBy(cop => cop.Order.OrderDate.DayOfWeek);
            var productConsumptionTrends = groupByQueryResult.Select(c => AggregateSumQuantity(c));
            return productConsumptionTrends;
        }

        private ProductConsumptionTrend AggregateSumQuantity(IGrouping<DayOfWeek, OrderProduct> items)
        {
            return new ProductConsumptionTrend(items.Key, items.Sum(cop => (float)cop.QuantityPurchased));
        }

        protected override void Dispose(bool disposing)
        {
             if (disposing && db!=null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
