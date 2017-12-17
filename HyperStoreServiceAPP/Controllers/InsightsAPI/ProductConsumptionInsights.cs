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
using HyperStoreServiceAPP.DTO;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class ProductConsumptionInsights
    {
        private class ProductConsumption
        {
            public Guid ProductId;
            public DateTime OrderDate;
            public decimal QuantityPurchased;
        }

        /// <summary>
        /// Returns the average consumption of product on each day of week by analysing order history of product.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public static async Task<MapDay_ProductEstConsumption> GetProductConsumptionTrend(Guid userId, Guid productId)
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
            MapDay_ProductEstConsumption productConsumptionTrend = null;
            product_EstimatedConsumption.TryGetValue(productId, out productConsumptionTrend);
            return productConsumptionTrend;
        }

        private static async Task<float?> GetProductUnitConsumedPerWeek(Guid userId, Guid productId)
        {
            var productEstConsumption = (await GetProductConsumptionTrend(userId, productId))?.ProductEstConsumption;
            if (productEstConsumption == null)
                return null;
            var prodEstConsumptionInWeek = productEstConsumption.Sum(p => p.Value);
            return prodEstConsumptionInWeek;
        }


        /// <summary>
        /// returns true if the product will be fully consumed between start day and end day.
        /// else returns false if product will be fully consumed less than start day or greater than end day. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="currentQuantity"></param>
        /// <param name="startDay"></param>
        /// <param name="endDay"></param>
        /// <returns></returns>
        public static async Task<bool> IsProductConsumed(Guid userId, Guid productId, float currentQuantity, IRange<int?> ConsumptionDayRange)
        {
            var extinctionDate = await GetProductStockCompletionDate(userId, productId, currentQuantity);
            if (extinctionDate == null)
                return true;
            var LBDate = DateTime.Now.AddDays((int)ConsumptionDayRange.LB);
            var UBDate = DateTime.Now.AddDays((int)ConsumptionDayRange.UB);
            if (extinctionDate?.Date >= LBDate.Date && extinctionDate?.Date <= UBDate.Date)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns the date, the product is expected to completely extinct.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="currentQuantity"></param>
        /// <returns></returns>
        public static async Task<DateTime?> GetProductStockCompletionDate(Guid userId, Guid productId, float currentQuantity)
        {
            var unitsConsumedPerWeek = await GetProductUnitConsumedPerWeek(userId, productId);
            if (unitsConsumedPerWeek == null)
                return null;
            var noOfWeeks = Math.Floor((float)currentQuantity / (float)unitsConsumedPerWeek);
            var remainingQuantity = currentQuantity - noOfWeeks * unitsConsumedPerWeek;

            var currentDayIndex = (int)DateTime.Now.DayOfWeek;
            var prodEstConsumption = (await GetProductConsumptionTrend(userId, productId)).ProductEstConsumption;
            int day;
            for (day = 0; day < 7; day++)
            {
                if (remainingQuantity <= 0)
                    break;
                float estConsumption;
                prodEstConsumption.TryGetValue((DayOfWeek)((day + currentDayIndex) % 7), out estConsumption);
                remainingQuantity -= estConsumption;
            }
            return DateTime.Now.AddDays(7 * noOfWeeks + day - 1);
        }

        /// <summary>
        /// This method will run its analysis on all the customer orders and compute the average consumption of the product in each day of week.
        /// After 1 year or so, some linear regression should be applied rather that to compute average.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static async Task<Map_Product_MapDayProductEstConsumption> ComputeProductConsumptionTrend(Guid userId)
        {
            var db = UtilityAPI.RetrieveDBContext(userId);
            var startingOrderDate = DateTime.Now.AddMonths(-1).Date;
            var query = db.OrderProducts.Include(cop => cop.Order)
                         .Where(cop => cop.Order.EntityType == EntityType.Customer &&
                                cop.Order.OrderDate >= startingOrderDate && cop.Order.OrderDate < DbFunctions.TruncateTime(DateTime.Now))
                        .Select(cop => new ProductConsumption()
                        {
                            ProductId = (Guid)cop.ProductId,
                            OrderDate = cop.Order.OrderDate,
                            QuantityPurchased = cop.QuantityPurchased
                        });

            var groups = await query.GroupBy(pc => pc.ProductId).ToListAsync();
            var Product_MapDayProductEstConsumption = groups.Select(productId_cop => AggregateProductConsumptionByDayOfWeek(productId_cop));

            var product_EstimiatedConsumption = new Map_Product_MapDayProductEstConsumption();
            foreach (var group in Product_MapDayProductEstConsumption)
                product_EstimiatedConsumption.Add((Guid)group.Key, group.Value);
            db.Dispose();
            return product_EstimiatedConsumption;
        }

        /// <summary>
        /// Input is the key:productId and collection of ProductConsumption of the given key:productId.
        /// </summary>
        /// <param name="productId_productConsumption"></param>
        /// <returns>(productId, productConsumptionTrend)</returns>
        private static KeyValuePair<Guid?, MapDay_ProductEstConsumption> AggregateProductConsumptionByDayOfWeek(IGrouping<Guid, ProductConsumption> productId_productConsumption)
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
        private static KeyValuePair<DayOfWeek, float> AggregateQuantityOfDayOfWeek(IGrouping<DayOfWeek, ProductConsumption> items)
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

    }
}