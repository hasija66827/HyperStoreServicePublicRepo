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
    public class BusinessInsightsController : ApiController
    {
        private HyperStoreServiceContext db;

        [HttpGet]
        [ResponseType(typeof(BusinessInsight))]
        public IHttpActionResult GetBusinessInsight(Guid userId, BusinessInsightDTO parameter)
        {
            db = UtilityAPI.RetrieveDBContext(userId);

            if (parameter == null)
                return BadRequest("Parameter should not have been null");

            var orderGroups = db.Orders.Where(o => DbFunctions.TruncateTime(o.OrderDate) >= parameter.DateRange.LB.Date &&
                                                    DbFunctions.TruncateTime(o.OrderDate) <= parameter.DateRange.UB.Date)
                                        .GroupBy(o => DbFunctions.TruncateTime(o.OrderDate)).ToList();

            var transactionGroups = db.Transactions.Where(t => DbFunctions.TruncateTime(t.TransactionDate) >= parameter.DateRange.LB.Date &&
                                                                 DbFunctions.TruncateTime(t.TransactionDate) <= parameter.DateRange.UB.Date)
                                                    .GroupBy(t => DbFunctions.TruncateTime(t.TransactionDate)).ToList();

            var businessInsight = new BusinessInsight();
            businessInsight.OrderInsight = orderGroups.Select(orderGrp => ComputeSalesOrderInsights(orderGrp)).ToList();
            businessInsight.TransactionInsight = transactionGroups.Select(transactionGrp => ComputeExplicitTransactionInsights(transactionGrp)).ToList();
            return Ok(businessInsight);
        }

        private TransactionInsight ComputeExplicitTransactionInsights(IGrouping<DateTime?, Transaction> transactionGrp)
        {
            var transactionInsights = new TransactionInsight();

            transactionInsights.Date = transactionGrp.Key;

            transactionInsights.MoneyIn = transactionGrp.Where(t => t.EntityType == EntityType.Customer &&
                                                                    t.IsCredit == true)
                                                                    .Sum(t => (decimal?)t.TransactionAmount);

            transactionInsights.MoneyOut = transactionGrp.Where(t => t.EntityType == EntityType.Supplier &&
                                                                    t.IsCredit == false)
                                                                    .Sum(t => (decimal?)t.TransactionAmount);
            return transactionInsights;
        }

        private OrderInsight ComputeSalesOrderInsights(IGrouping<DateTime?, Order> orderGrp)
        {
            var salesOrderInsights = new OrderInsight();

            salesOrderInsights.Date = orderGrp.Key;

            salesOrderInsights.MoneyIn = orderGrp.Where(ord => ord.EntityType == EntityType.Customer)
                                                        .Sum(ord => (decimal?)ord.PayedAmount);
            salesOrderInsights.MoneyOut = orderGrp.Where(ord => ord.EntityType == EntityType.Supplier)
                                                        .Sum(ord => (decimal?)ord.PayedAmount);

            salesOrderInsights.TotalSales = orderGrp.Where(ord => ord.EntityType == EntityType.Customer)
                                                        .Sum(ord => (decimal?)ord.BillAmount);
            salesOrderInsights.TotalPurchase = orderGrp.Where(ord => ord.EntityType == EntityType.Supplier)
                                                        .Sum(ord => (decimal?)ord.BillAmount);

            return salesOrderInsights;
        }
    }
}
