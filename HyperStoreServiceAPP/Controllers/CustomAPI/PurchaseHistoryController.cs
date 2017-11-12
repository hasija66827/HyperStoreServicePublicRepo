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
using HyperStoreServiceAPP.DTO.RecommendedProductDTO;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class PurchaseHistoryController : ApiController, IPurchaseHistory
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        public IHttpActionResult Get(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.PurchaseHistory);
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetRecommendedProduct(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            var person = await db.Persons.FindAsync(id);

            if (person.EntityType == DTO.EntityType.Supplier)
                return Ok(_GetRecommendedProductForSupplier(person.PersonId));
            else
                return Ok(_GetRecommendedProductForCustomer(person.PersonId));
        } 

        [HttpPut]
        public async Task<IHttpActionResult> SetReminderForProduct(Guid userId, SetReminderDTO setReminderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (setReminderDTO == null)
                throw new Exception("SetReminderDTO should not have been null");

            db = UtilityAPI.RetrieveDBContext(userId);

            var purchaseHistory = db.PurchaseHistory.Where(ph => ph.PersonId == setReminderDTO.PersonId
                                                                && ph.ProductId == setReminderDTO.ProductId).FirstOrDefault();
            if (purchaseHistory == null)
                throw new Exception(String.Format("Product {0} corresponding to person {1} not found", setReminderDTO.ProductId, setReminderDTO.PersonId));

            purchaseHistory.ExpiryDays = setReminderDTO.ExpireInDays;
            db.Entry(purchaseHistory).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return Ok(true);
        }


        private IQueryable<RecommendedProductForCustomer> _GetRecommendedProductForCustomer(Guid customerId)
        {
            var recommendedProduct = db.PurchaseHistory.Where(ph => ph.PersonId == customerId)
                                                       .Include(ph => ph.Product)
                                                       .Select(ph => new RecommendedProductForCustomer()
                                                       {
                                                           Product = ph.Product,
                                                           ExpiredByDays = DbFunctions.DiffDays(DateTime.Now, ph.LatestPurchaseDate) - ph.ExpiryDays
                                                       }).OrderByDescending(rpc => rpc.ExpiredByDays);
            return recommendedProduct;
        }

        private IQueryable<RecommendedProductForSupplier> _GetRecommendedProductForSupplier(Guid supplierId)
        {
            var recommendedProduct = db.PurchaseHistory.Where(ph => ph.PersonId == supplierId)
                                                        .Include(ph => ph.Product)
                                                        .Where(ph => ph.Product.TotalQuantity <= ph.Product.Threshold)
                                                        .Select(ph => new RecommendedProductForSupplier()
                                                        {
                                                            Product = ph.Product,
                                                            DeficientByNumber = (double)(ph.Product.Threshold - ph.Product.TotalQuantity)
                                                        }).OrderByDescending(rpc => rpc.DeficientByNumber);
            return recommendedProduct;
        }

    }
}
