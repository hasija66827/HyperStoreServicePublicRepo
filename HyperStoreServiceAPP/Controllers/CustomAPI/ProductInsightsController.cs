using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO.InsightsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class ProductInsightsController : ApiController
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(IEnumerable<ProductInsights>))]
        public IHttpActionResult GetProductsHittingZeroStock(Guid userId, ProductInsightsDTO parameter)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (parameter == null)
                return BadRequest("Parameter should not have been null");

            db = UtilityAPI.RetrieveDBContext(userId);
            var filterDate = DateTime.Now.AddDays(Convert.ToDouble(-parameter.NumberOfDays)).Date;

            var productGrps = db.DeficientStockHits.Where(dsh => dsh.TimeStamp > filterDate)
                                                    .GroupBy(dsh => dsh.ProductId).ToList();

            var insightOfProducts = productGrps.Select(pg => CountDaysHitOfTheProduct(pg));

            var insightsOfTopProducts = insightOfProducts.OrderByDescending(p => p.NoOfDaysStockHitZero).Take((int)parameter.NumberOfRecords);
            return Ok(insightsOfTopProducts);
        }

        private ProductInsights CountDaysHitOfTheProduct(IGrouping<Guid?, DeficientStockHit> items)
        {
            return new ProductInsights()
            {
                ProductId = items.Key,
                Product = null,
                NoOfDaysStockHitZero = items.Count()
            };
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
