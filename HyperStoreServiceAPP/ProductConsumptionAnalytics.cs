using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Product_EstimiatedConsumption = System.Collections.Generic.Dictionary<System.Guid, HyperStoreService.HyperStoreService.CustomModels.ProductConsumptionTrend>;
using HyperStoreServiceAPP.InMemoryStorage;
using HyperStoreService.HyperStoreService.CustomModels;

namespace HyperStoreServiceAPP
{
    public class ProductConsumptionAnalytics
    {
        public static async Task<ProductConsumptionTrend> GetProductConsumptionTrend(Guid userId, Guid productId)
        {
            var cache = CollectionOfCache<Product_EstimiatedConsumption>.GetCache(userId);
            if (cache == null)
            {
                cache = new Cache<Product_EstimiatedConsumption>(userId, 24, ComputeProductConsumptionTrend);
                var IsInserted = CollectionOfCache<Product_EstimiatedConsumption>.InsertCache(userId, cache);
                if (!IsInserted)
                    throw new Exception("Unable to insert into cache.");
            }

            var product_EstimatedConsumption = await cache.GetValue();
            ProductConsumptionTrend productConsumptionTrend;
            product_EstimatedConsumption.TryGetValue(productId, out productConsumptionTrend);
            return productConsumptionTrend;
        }

        private static async Task<Product_EstimiatedConsumption> ComputeProductConsumptionTrend(Guid userId)
        {
            //TODO:Actual Query result
            return new Product_EstimiatedConsumption();
        }

        /// <summary>
        /// Will the propduct consumed fully in the number of days specified.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        public static async Task<bool> WillProductConsumed(Guid userId, Guid productId, int numberOfDays)
        {
            return true;
        }
    }
}