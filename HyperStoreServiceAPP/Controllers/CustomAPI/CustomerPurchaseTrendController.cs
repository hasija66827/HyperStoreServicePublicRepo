using HyperStoreService.CustomModels;
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
    public class CustomerPurchaseTrendController : ApiController, ICustomerPurchaseTrend
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(IEnumerable<CustomerPurchaseTrend>))]
        public async Task<IHttpActionResult> Get(Guid userId, CustomerPurchaseTrendDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (parameter == null)
                return BadRequest("CustomerPurchaseTrend parameter should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            IEnumerable<CustomerPurchaseTrend> queryResult;
            try
            {
                var startingOrderDate = DateTime.Now.AddMonths(-(int)parameter.MonthsCount);
                var query = db.OrderProducts
                       .Include(cop => cop.Order)
                        .Where(cop => cop.Order.PersonId == parameter.CustomerId &&
                                      cop.Order.OrderDate >= startingOrderDate)
                        .Include(cop => cop.Product)
                        .Select(cop => new CustomerPurchaseTrend()
                        {
                            Product = cop.Product,
                            NetValue = cop.PurchasePrice*cop.QuantityPurchased,
                            TotalQuantityPurchased = (int)cop.QuantityPurchased
                        }
                        );
                var groupByQueryResult = await query.GroupBy(c => c.Product.ProductId).ToListAsync();
                queryResult = groupByQueryResult.Select(c => AggregateAttributes(c));
            }
            catch (Exception e)
            { throw e; }
            return Ok(queryResult);
        }

        private CustomerPurchaseTrend AggregateAttributes(IGrouping<Guid?, CustomerPurchaseTrend> items)
        {
            return new CustomerPurchaseTrend()
            {
                Product = items.Select(p => p.Product).FirstOrDefault(),
                NetValue = items.Sum(p => p.NetValue),
                TotalQuantityPurchased = items.Sum(p => p.TotalQuantityPurchased)
            };
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
