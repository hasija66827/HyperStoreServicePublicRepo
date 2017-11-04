﻿using HyperStoreService.CustomModels;
using HyperStoreService.Models;
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
    public class PriceQuotedBySupplierController : ApiController, IPriceQuotedBySupplier
    {
        private HyperStoreServiceContext db;

        /// <summary>
        /// Finds all the supplier from which the retailer has purchased the given product,
        /// Computes the last/latest purchase price of product.
        /// </summary>
        /// <param name="id">id of the product</param>
        /// <returns>returns the list of Price Quoted By each supplier for the given product</returns>
        [HttpGet]
        [ResponseType(typeof(List<PriceQuotedBySupplier>))]
        public async Task<IHttpActionResult> Get(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var productId = id;
            var query = db.OrderProducts
                                           .Where(sop => sop.ProductId == productId)
                                           .Include(sop => sop.Order)
                                           .Include(sop => sop.Order.Person)
                                           .Where(sop=>sop.Order.EntityType==DTO.EntityType.Supplier)
                                           .Select(s => new PriceQuotedBySupplier()
                                           {
                                               OrderDate = s.Order.OrderDate,
                                               PersonId = s.Order.PersonId,
                                               Person = s.Order.Person,
                                               QuantityPurchased = s.QuantityPurchased,
                                               PurchasePrice = s.PurchasePrice,
                                           });
            var groupPrices_By_SupplierId = await query.GroupBy(s => s.PersonId).ToListAsync();
            var latestPurchasePriceQuotedBySuppliers = groupPrices_By_SupplierId.Select(w_ps => SelectLatestPriceQuoted(w_ps));
            var result = latestPurchasePriceQuotedBySuppliers.OrderBy(pqs => pqs.PurchasePrice);
            return Ok(result);
        }

        /// <summary>
        /// Returns the latest price Quoted by 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static PriceQuotedBySupplier SelectLatestPriceQuoted(IGrouping<Guid, PriceQuotedBySupplier> items)
        {
            //TODO: try to be more efficeint
            DateTime maxOrderDate = DateTime.Now.AddYears(-100);
            PriceQuotedBySupplier ret = new PriceQuotedBySupplier();
            foreach (var item in items)
            {
                if (maxOrderDate < item.OrderDate)
                {
                    maxOrderDate = item.OrderDate;
                    ret = item;
                }
            }
            return ret;
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