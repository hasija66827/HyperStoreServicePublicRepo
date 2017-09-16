using HyperStoreService.Models;
using HyperStoreService.CustomModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class RecommendedProductsController : ApiController, IRecommendedProduct
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id", id of the customer></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<RecommendedProduct>))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            var customerId = id;
            var query = db.CustomerOrderProducts.Include(cop => cop.CustomerOrder)
                                                                .Where(cop => cop.CustomerOrder.CustomerId == id)
                                                                .Include(cop => cop.Product)
                                                                .Select(cop => new RecommendedProduct()
                                                                {
                                                                    ProductId = cop.ProductId,
                                                                    ProductName = cop.Product.Name,
                                                                    LastOrderDate = cop.CustomerOrder.OrderDate

                                                                });

            var groupOrdersByProductId = await query.GroupBy(cop => cop.ProductId).ToListAsync();
            var recommendedProducts = groupOrdersByProductId.Select(cop => GetProductByLatestOrderDate(cop));

            return Ok(recommendedProducts);
        }

        private RecommendedProduct GetProductByLatestOrderDate(IGrouping<Guid?, RecommendedProduct> items)
        {
            return new RecommendedProduct()
            {
                ProductId = items.Key,
                ProductName = items.FirstOrDefault().ProductName,
                LastOrderDate = items.Max(rp => rp.LastOrderDate),
            };
        }
    }
}
