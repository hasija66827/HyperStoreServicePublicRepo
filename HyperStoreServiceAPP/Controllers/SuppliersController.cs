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
using System.ComponentModel.DataAnnotations;

namespace HyperStoreServiceAPP.Controllers
{
    public class SuppliersController : ApiController, ISupplier
    {
        private HyperStoreServiceContext db;

        #region Read
        // GET: api/Suppliers
        [HttpGet]
        [ResponseType(typeof(List<Supplier>))]
        public async Task<IHttpActionResult> Get(Guid userId, SupplierFilterCriteria sfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            db = UtilityAPI.RetrieveDBContext(userId);

            IQueryable<Supplier> query = db.Suppliers;
            if (sfc == null)
                return Ok(await query.ToListAsync());
            try
            {
                if (sfc.WalletAmount != null)
                    query = db.Suppliers.Where(s => s.WalletBalance >= sfc.WalletAmount.LB &&
                                                    s.WalletBalance <= sfc.WalletAmount.UB);

                if (sfc.SupplierId != null)
                    query = query.Where(s => s.SupplierId == sfc.SupplierId);
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [ResponseType(typeof(Supplier))]
        public IHttpActionResult Get(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var supplier = db.Suppliers.Find(id);
            return Ok(supplier);
        }

        [HttpGet]
        [ResponseType(typeof(Int32))]
        public IHttpActionResult GetTotalRecordsCount(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            return Ok(db.Suppliers.Count());
        }
        #endregion

        // PUT: api/Suppliers/5
        [HttpPut]
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> Put(Guid userId, Guid id, SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (supplierDTO == null)
                throw new Exception("SupplierDTO should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            var supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
                throw new Exception(String.Format("Supplier of id {0} not found", id));
            var updatedSupplier = _UpdateSupplier(supplier, supplierDTO);
            db.Entry(updatedSupplier).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(updatedSupplier);
        }

        private Supplier _UpdateSupplier(Supplier supplier, SupplierDTO supplierDTO)
        {
            var updatedSupplier = supplier;
            updatedSupplier.Address = supplierDTO.Address;
            updatedSupplier.GSTIN = supplierDTO.GSTIN;
            updatedSupplier.MobileNo = supplierDTO.MobileNo;
            updatedSupplier.Name = supplierDTO.Name;
            return updatedSupplier;
        }

        // POST: api/Suppliers
        [ResponseType(typeof(Supplier))]
        public async Task<IHttpActionResult> Post(Guid userId, SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (supplierDTO == null)
                return BadRequest("SupplierDTO should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            var supplier = new Supplier()
            {
                SupplierId = Guid.NewGuid(),
                Address = supplierDTO.Address,
                GSTIN = supplierDTO.GSTIN,
                MobileNo = supplierDTO.MobileNo,
                Name = supplierDTO.Name,
                WalletBalance = 0,
            };
            db.Suppliers.Add(supplier);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IsNameExist(supplier.Name))
                {
                    return BadRequest(String.Format("Supplier with name {0} already exists.", supplier.Name));
                }
                else if (IsMobNoExist(supplier.MobileNo))
                {
                    return BadRequest(String.Format("Supplier with mobile number {0} already exists.", supplier.MobileNo));
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = supplier.SupplierId }, supplier);
        }

        [HttpGet]
        [ResponseType(typeof(IRange<decimal?>))]
        public async Task<IHttpActionResult> GetWalletBalanceRange(Guid userId)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            IRange<decimal?> walletBalanceRange = null;
            if (db.Suppliers.Count() != 0)
            {
                var minWalletBalance = await db.Suppliers.MinAsync(w => w.WalletBalance);
                var maxWalletBalance = await db.Suppliers.MaxAsync(w => w.WalletBalance);
                walletBalanceRange = new IRange<decimal?>(minWalletBalance, maxWalletBalance);
            }
            return Ok(walletBalanceRange);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IsNameExist(string supplierName)
        {
            return db.Suppliers.Count(s => s.Name == supplierName) > 0;
        }

        private bool IsMobNoExist(string supplierMobileNo)
        {
            return db.Suppliers.Count(s => s.MobileNo == supplierMobileNo) > 0;
        }
    }
}