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
    public class CustomerTransactionsController : ApiController, CustomerTransactionInterface
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/CustomerTransactions/5
        [ResponseType(typeof(List<CustomerTransaction>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetTransactions(CustomerTransactionFilterCriteria transactionFilterCriteria)
        {
            if (transactionFilterCriteria == null)
                return BadRequest("TransactionFilterCriteria cannont be null while retreiving the transaction for Customer");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var customerId = transactionFilterCriteria.CustomerId;
            var transactions = await db.CustomerTransactions.Where(t => t.CustomerId == customerId)
                                                          .OrderByDescending(t => t.TransactionDate).ToListAsync();
            Customer customer;
            if (transactions == null || transactions.Count() == 0)
            {
                customer = await db.Customers.FindAsync(customerId);
                if (customer == null)
                    return BadRequest(String.Format("Customer of id {0} does not exists", customerId));
            }
            return Ok(transactions);
        }

        // POST: api/CustomerTransactions
        [ResponseType(typeof(CustomerTransaction))]
        [HttpPost]
        public async Task<IHttpActionResult> PostCustomerTransaction(CustomerTransactionDTO transactionDTO)
        {
            if (transactionDTO == null)
                return BadRequest("TransactionDTO cannot be null, on creating transaction for customer");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (transactionDTO.IsCredit == true)
                throw new Exception("Currently transaction of type credit is not supported through this API");

            var transaction = await transactionDTO.CreateNewTransactionAsync(db);
            await db.SaveChangesAsync();
            return (Ok(transaction));
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