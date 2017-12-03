using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Map_Product_MapDayProductEstConsumption = System.Collections.Generic.Dictionary<System.Guid, HyperStoreService.HyperStoreService.CustomModels.MapDay_ProductEstConsumption>;
using HyperStoreServiceAPP.InMemoryStorage;
using HyperStoreService.HyperStoreService.CustomModels;
using System.Data.Entity;
using HyperStoreService.Models;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class ProductConsumptionInsightsController : ApiController
    {
        private class ProductConsumption
        {
            public Guid ProductId;
            public DateTime OrderDate;
            public decimal QuantityPurchased;
        }

        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(MapDay_ProductEstConsumption))]
        public async Task<IHttpActionResult> Get(Guid userId, Guid id)
        {
            var cache = CollectionOfCache<Map_Product_MapDayProductEstConsumption>.GetCache(userId);
            if (cache == null)
            {
                cache = new Cache<Map_Product_MapDayProductEstConsumption>(userId, RefreshInterval.ProductEstimatedConsumption, ComputeProductConsumptionTrend);
                var IsInserted = CollectionOfCache<Map_Product_MapDayProductEstConsumption>.InsertCache(userId, cache);
                if (!IsInserted)
                    throw new Exception("Unable to insert into cache.");
            }

            var product_EstimatedConsumption = await cache.GetValue();
            MapDay_ProductEstConsumption productConsumptionTrend;
            product_EstimatedConsumption.TryGetValue(id, out productConsumptionTrend);
            return Ok(productConsumptionTrend);
        }

        /// <summary>
        /// This method will run its analysis on all the customer orders and compute the average consumption of the product in each day of week.
        /// After 1 year or so, some linear regression should be applied rather that to compute average.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<Map_Product_MapDayProductEstConsumption> ComputeProductConsumptionTrend(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var startingOrderDate = DateTime.Now.AddMonths(-2).Date;
            var query = db.OrderProducts.Include(cop => cop.Order)
                         .Where(cop => cop.Order.EntityType == DTO.EntityType.Customer &&
                                cop.Order.OrderDate >= startingOrderDate && cop.Order.OrderDate < DbFunctions.TruncateTime(DateTime.Now))
                        .Select(cop => new ProductConsumption()
                        {
                            ProductId = (Guid)cop.ProductId,
                            OrderDate = cop.Order.OrderDate,
                            QuantityPurchased = cop.QuantityPurchased
                        });

            var groups = await query.GroupBy(pc => pc.ProductId).ToListAsync();
            var x = groups.Select(productId_cop => AggregateProductConsumptionByDayOfWeek(productId_cop));

            var product_EstimiatedConsumption = new Map_Product_MapDayProductEstConsumption();
            foreach (var group in x)
                product_EstimiatedConsumption.Add((Guid)group.Key, group.Value);
            db.Dispose();
            return product_EstimiatedConsumption;
        }

        /// <summary>
        /// Input is the key:productId and collection of ProductConsumption of the given key:productId.
        /// </summary>
        /// <param name="productId_productConsumption"></param>
        /// <returns>(productId, productConsumptionTrend)</returns>
        private KeyValuePair<Guid?, MapDay_ProductEstConsumption> AggregateProductConsumptionByDayOfWeek(IGrouping<Guid, ProductConsumption> productId_productConsumption)
        {
            var groups = productId_productConsumption.GroupBy(pc => pc.OrderDate.DayOfWeek)
                                                        .Select(items => AggregateQuantityOfDayOfWeek(items));

            var mapDay_ProductEstConsumption = new MapDay_ProductEstConsumption();

            foreach (var group in groups)
                mapDay_ProductEstConsumption.ProductEstConsumption.Add(group.Key, group.Value);

            return new KeyValuePair<Guid?, MapDay_ProductEstConsumption>(productId_productConsumption.Key, mapDay_ProductEstConsumption);
        }

        /// <summary>
        /// Input is the Day of Week and collection of ProductConsumption of the given key:Day Of Week.
        /// </summary>
        /// <param name="items"></param>
        /// <returns>(DayOfWeek, AverageConsumptionOfProduct)</returns>
        private KeyValuePair<DayOfWeek, float> AggregateQuantityOfDayOfWeek(IGrouping<DayOfWeek, ProductConsumption> items)
        {
            var distinctNumberOfTheDay = items.Distinct(new ProductConsumptionEqualityComparer()).Count();
            return new KeyValuePair<DayOfWeek, float>(items.Key, items.Sum(pc => ((float)pc.QuantityPurchased / distinctNumberOfTheDay)));
        }

        private class ProductConsumptionEqualityComparer : IEqualityComparer<ProductConsumption>
        {
            public bool Equals(ProductConsumption a, ProductConsumption b)
            {
                if (b == null && a == null)
                    return true;
                else if (a == null | b == null)
                    return false;
                else if (a.OrderDate.Date == b.OrderDate.Date)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(ProductConsumption pc)
            {
                var hCode = pc.OrderDate.Date;
                return hCode.GetHashCode();
            }
        }

        /// <summary>
        /// Will the propduct consumed fully in the number of days specified.
        /// This will help us to place the order to supplier according to the consumption rate of our product.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        public async Task<bool> WillProductBeConsumed(Guid userId, Guid productId, int numberOfDays)
        {
            return true;
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