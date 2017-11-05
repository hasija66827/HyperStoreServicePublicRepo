using HyperStoreService.Models;
using HyperStoreServiceAPP.DTO.InsightsDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace HyperStoreServiceAPP.Controllers.CustomAPI
{
    public class SalesInsightsController : ApiController
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(SalesInsight))]
        public IHttpActionResult GetSalesInsights(Guid userId, SalesInsightsDTO parameter)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            var orderGroups = db.Orders.Where(o => o.OrderDate >= parameter.DateRange.LB.Date &&
                                                    o.OrderDate <= parameter.DateRange.UB.Date)
                                        .GroupBy(o => DbFunctions.TruncateTime(o.OrderDate)).ToList();

            var transactionGroups = db.Transactions.Where(t => t.TransactionDate >= parameter.DateRange.LB.Date&&
                                                                t.TransactionDate<=parameter.DateRange.UB.Date)
                                                    .GroupBy(t => DbFunctions.TruncateTime(t.TransactionDate)).ToList();

            var salesInsight = new SalesInsight();
            salesInsight.SalesOrderInsight = orderGroups.Select(orderGrp => ComputeSalesOrderInsights(orderGrp)).ToList();
            salesInsight.TransactionInsight = transactionGroups.Select(transactionGrp => ComputeExplicitTransactionInsights(transactionGrp)).ToList();
            return Ok(salesInsight);
        }

        private TransactionInsight ComputeExplicitTransactionInsights(IGrouping<DateTime?, Transaction> transactionGrp)
        {
            var transactionInsights = new TransactionInsight();

            transactionInsights.Date = transactionGrp.Key;

            transactionInsights.MoneyIn = transactionGrp.Where(t => t.EntityType == DTO.EntityType.Customer &&
                                                                    t.IsCredit == true)
                                                                    .Sum(t => (decimal?)t.TransactionAmount);

            transactionInsights.MoneyOut = transactionGrp.Where(t => t.EntityType == DTO.EntityType.Supplier &&
                                                                    t.IsCredit == false)
                                                                    .Sum(t => (decimal?)t.TransactionAmount);
            return transactionInsights;
        }

        private SalesOrderInsight ComputeSalesOrderInsights(IGrouping<DateTime?, Order> orderGrp)
        {
            var salesOrderInsights = new SalesOrderInsight();

            salesOrderInsights.Date = orderGrp.Key;

            salesOrderInsights.MoneyIn = orderGrp.Where(ord => ord.EntityType == DTO.EntityType.Customer)
                                                        .Sum(ord => (decimal?)ord.PayedAmount);
            salesOrderInsights.MoneyOut = orderGrp.Where(ord => ord.EntityType == DTO.EntityType.Supplier)
                                                        .Sum(ord => (decimal?)ord.PayedAmount);

            salesOrderInsights.TotalSales = orderGrp.Where(ord => ord.EntityType == DTO.EntityType.Customer)
                                                        .Sum(ord => (decimal?)ord.BillAmount);
            salesOrderInsights.TotalPurchase = orderGrp.Where(ord => ord.EntityType == DTO.EntityType.Supplier)
                                                        .Sum(ord => (decimal?)ord.BillAmount);

            return salesOrderInsights;
        }
    }
}
