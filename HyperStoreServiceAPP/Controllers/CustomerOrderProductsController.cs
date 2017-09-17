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
    public class CustomerOrderProductsController : ApiController, ICustomerOrderDetail
    {
        private HyperStoreServiceContext db ;

        /// <summary>
        /// Returns the order detail of the customer order.
        /// </summary>
        /// <param name="id">CustomerOrderId</param>
        /// <returns></returns>
        [ResponseType(typeof(List<CustomerOrderProduct>))]
        [HttpGet]
        public async Task<IHttpActionResult> Get(Guid userId, Guid? id)
        {
            if (id == null)
                throw new Exception("CustomerOrderId should not have been NULL");
            db = UtilityAPI.RetrieveDBContext(userId);

            List<CustomerOrderProduct> queryResult;
            try
            {
                var query = db.CustomerOrderProducts
                             .Where(cop => cop.CustomerOrderId == id)
                             .Include(cop => cop.Product);
                queryResult = await query.ToListAsync();
            }
            catch (Exception e)
            { throw e; }

            return (Ok(queryResult));
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