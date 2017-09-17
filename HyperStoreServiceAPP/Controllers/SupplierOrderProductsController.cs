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
    public class SupplierOrderProductsController : ApiController, ISupplierOrderProduct
    {
        private HyperStoreServiceContext db ;

        // GET: api/SupplierOrderProducts/5
        [ResponseType(typeof(List<SupplierOrderProduct>))]
        public async Task<IHttpActionResult> Get(Guid userId, Guid? id)
        {
            if (id == null)
                return BadRequest("SupplierOrderId should not be null");
            db = UtilityAPI.RetrieveDBContext(userId);

            List<SupplierOrderProduct> queryResult;
            try
            {
                var query = db.SupplierOrderProducts
                                .Where(sop => sop.SupplierOrderId == id)
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