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
    public class CustomerPurchaseTrend
    {
        public int TotalQuantityPurchased { get; set; }
        public Product Product { get; set; }

        public CustomerPurchaseTrend()
        {
        }

        public CustomerPurchaseTrend(Product product, int totalQuantityPurchased)
        {
            this.Product = product;
            this.TotalQuantityPurchased = totalQuantityPurchased;
        }
    }

    public class CustomerPurchaseTrendParameter
    {
        [Required]
        public Guid? CustomerId { get; set; }
        [Required]
        public int? MonthsCount { get; set; }
    }

    public class CustomerPurchaseTrendController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        [HttpGet]
        [ResponseType(typeof(IEnumerable<CustomerPurchaseTrend>))]
        public async Task<IHttpActionResult> CustomerPurchaseTrend(CustomerPurchaseTrendParameter parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (parameter == null)
                throw new Exception("CustomerPurchaseTrend parameter should not have been null");
            IEnumerable<CustomerPurchaseTrend> queryResult;
            try
            {
                var startingOrderDate = DateTime.Now.AddMonths(-(int)parameter.MonthsCount);
                var query = db.CustomerOrderProducts
                       .Include(cop => cop.CustomerOrder)
                        .Where(cop => cop.CustomerOrder.CustomerId == parameter.CustomerId &&
                                      cop.CustomerOrder.OrderDate >= startingOrderDate)
                        .Include(cop => cop.Product)
                        .Select(cop => new CustomerPurchaseTrend()
                        {
                            TotalQuantityPurchased = (int)cop.QuantityPurchased,
                            Product = cop.Product
                        }
                        );
                var groupByQuery = await query.GroupBy(c => c.Product.ProductId).ToListAsync();
                queryResult = groupByQuery.Select(c => AggregateQuantity(c));
            }
            catch (Exception e)
            { throw e; }
            return Ok(queryResult);
        }

        private CustomerPurchaseTrend AggregateQuantity(IGrouping<Guid?, CustomerPurchaseTrend> items)
        {
            return new CustomerPurchaseTrend(items.Select(p => p.Product).FirstOrDefault(), items.Sum(p => p.TotalQuantityPurchased));
        }
    }
}
