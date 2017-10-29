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
using HyperStoreServiceAPP.DTO;

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

            if (sfc == null)
                throw new Exception("Filter Criteria cannot be null");

            db = UtilityAPI.RetrieveDBContext(userId);

            IQueryable<Supplier> query = db.Suppliers.Where(s => s.EntityType == sfc.EntityType);
            try
            {
                if (sfc.WalletAmount != null)
                    query = db.Suppliers.Where(s => s.WalletBalance >= sfc.WalletAmount.LB &&
                                                    s.WalletBalance <= sfc.WalletAmount.UB &&
                                                    s.EntityType == sfc.EntityType);

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
                throw new Exception("PersonDTO should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            var supplier = await db.Suppliers.FindAsync(id);
            if (supplier == null)
                throw new Exception(String.Format("Person of id {0} not found", id));
            _UpdateSupplier(supplier, supplierDTO);
            db.Entry(supplier).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok(supplier);
        }

        private void _UpdateSupplier(Supplier supplier, SupplierDTO supplierDTO)
        {
            supplier.Address = supplierDTO.Address;
            supplier.GSTIN = supplierDTO.GSTIN;
            supplier.MobileNo = supplierDTO.MobileNo;
            supplier.Name = supplierDTO.Name;
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
                throw new Exception("PersonDTO should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            var supplier = new Supplier()
            {
                SupplierId = Guid.NewGuid(),
                Address = supplierDTO.Address,
                EntityType = supplierDTO.EntityType,
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
                    return BadRequest(String.Format("Person with name {0} already exists.", supplier.Name));
                }
                else if (IsMobNoExist(supplier.MobileNo))
                {
                    return BadRequest(String.Format("Person with mobile number {0} already exists.", supplier.MobileNo));
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = supplier.SupplierId }, supplier);
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