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
    public class ProductTrend
    {
        public DayOfWeek Day { get; set; }
        public float Quantity { get; set; }
        public ProductTrend(DayOfWeek day, float quantity)
        {
            this.Day = day;
            this.Quantity = quantity;
        }
    }

    public class ProductTrendParameter
    {
        [Required]
        public Guid? ProductId { get; set; }
        [Required]
        public int? MonthsCount { get; set; }
    }

    public class PrductConsumptionTrendController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ProductTrend>))]
        public async Task<IHttpActionResult> GetProductTrend(ProductTrendParameter parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (parameter == null)
                return BadRequest("The parameter should not have been NULL");

            IEnumerable<ProductTrend> result;
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
                result=groupByQueryResult.Select(c => AggregateQuantity(c));
            }
            catch
            { throw; }
            return Ok(result);
        }

        private ProductTrend AggregateQuantity(IGrouping<DayOfWeek, CustomerOrderProduct> items)
        {
            return new ProductTrend(items.Key, items.Sum(cop => (float)cop.QuantityPurchased));
        }
    }
}
