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
        private HyperStoreServiceContext db;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id", id of the customer></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<DumpRecommendedProduct>))]
        public async Task<IHttpActionResult> Get(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            var customerId = id;
            var query = db.OrderProducts.Include(cop => cop.Order)
                                                                .Where(cop => cop.Order.PersonId == id)
                                                                .Include(cop => cop.Product)
                                                                .Select(cop => new DumpRecommendedProduct()
                                                                {
                                                                    ProductId = cop.ProductId,
                                                                    ProductName = cop.Product.Name,
                                                                    LastOrderDate = cop.Order.OrderDate
                                                                });

            var groupOrdersByProductId = await query.GroupBy(cop => cop.ProductId).ToListAsync();
            var recommendedProducts = groupOrdersByProductId.Select(cop => GetProductByLatestOrderDate(cop));

            return Ok(recommendedProducts);
        }

        private DumpRecommendedProduct GetProductByLatestOrderDate(IGrouping<Guid?, DumpRecommendedProduct> items)
        {
            return new DumpRecommendedProduct()
            {
                ProductId = items.Key,
                ProductName = items.FirstOrDefault().ProductName,
                LastOrderDate = items.Max(rp => rp.LastOrderDate),
            };
        }
    }
}
