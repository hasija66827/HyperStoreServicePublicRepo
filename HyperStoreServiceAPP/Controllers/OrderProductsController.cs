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
    public class OrderProductsController : ApiController, ISupplierOrderProduct
    {
        private HyperStoreServiceContext db ;

        // GET: api/SupplierOrderProducts/5
        [ResponseType(typeof(List<OrderProduct>))]
        public async Task<IHttpActionResult> Get(Guid userId, Guid? id)
        {
            if (id == null)
                return BadRequest("OrderId should not be null");
            db = UtilityAPI.RetrieveDBContext(userId);

            List<OrderProduct> queryResult;
            try
            {
                var query = db.OrderProducts
                                .Where(sop => sop.OrderId == id)
                                .Include(sop => sop.Product);
                queryResult = await query.ToListAsync();
            }
            catch (Exception e)
            { throw e; }
            return Ok(queryResult);
        }

        // PUT: api/SupplierOrderProducts/5

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