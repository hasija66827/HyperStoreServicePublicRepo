using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using HyperStoreService.Models;

namespace HyperStoreServiceAPP.Controllers
{
    public class SupplierOrderTransactionsController : ApiController, SupplierOrderTransactionInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        //TODO: Renaming and parameterizing with cclass variable.

        [ResponseType(typeof(List<SupplierOrderTransaction>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactionsOfSupplierOrder(Guid supplierOrderId)
        {
            var query = db.SupplierOrderTransactions.Where(sot => sot.SupplierOrderId == supplierOrderId)
                                                    .Include(sot => sot.Transaction);
            var result = await query.ToListAsync();
            return Ok(result);
        }

        [ResponseType(typeof(List<SupplierOrderTransaction>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetSupplierOrdersOfTransaction(Guid transactionId)
        {
            var query = db.SupplierOrderTransactions.Where(sot => sot.TransactionId == transactionId)
                                                   .Include(sot => sot.SupplierOrder);
            var result = await query.ToListAsync();
            return Ok(result);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}