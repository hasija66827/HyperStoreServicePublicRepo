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
using HyperStoreServiceAPP.CustomModels;

namespace HyperStoreServiceAPP.Controllers
{
    public class PersonsController : ApiController, IPerson
    {
        private HyperStoreServiceContext db;

        #region Read
        // GET: api/Suppliers
        [HttpGet]
        [ResponseType(typeof(List<Person>))]
        public async Task<IHttpActionResult> Get(Guid userId, SupplierFilterCriteriaDTO sfc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (sfc == null)
                throw new Exception("Filter Criteria cannot be null");

            db = UtilityAPI.RetrieveDBContext(userId);

            IQueryable<Person> query = db.Persons.Where(s => s.EntityType == sfc.EntityType);
            try
            {
                if (sfc.WalletAmount != null)
                    query = db.Persons.Where(s => s.WalletBalance >= sfc.WalletAmount.LB &&
                                                    s.WalletBalance <= sfc.WalletAmount.UB &&
                                                    s.EntityType == sfc.EntityType);

                if (sfc.SupplierId != null)
                    query = query.Where(s => s.PersonId == sfc.SupplierId);
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch
            {
                throw;
            }
        }

        [HttpGet]
        [ResponseType(typeof(Person))]
        public IHttpActionResult Get(Guid userId, Guid id)
        {
            db = UtilityAPI.RetrieveDBContext(userId);
            var supplier = db.Persons.Find(id);
            return Ok(supplier);
        }


        private async Task<IRange<double>> _GetWalletBalanceRange(EntityType? entityType)
        {
            IRange<double> walletBalanceRange = null;
            if (db.Persons.Count() != 0)
            {
                var minWalletBalance = await db.Persons.Where(p => p.EntityType == entityType).MinAsync(w => w.WalletBalance);
                var maxWalletBalance = await db.Persons.Where(p => p.EntityType == entityType).MaxAsync(w => w.WalletBalance);
                walletBalanceRange = new IRange<double>((double)minWalletBalance, (double)maxWalletBalance);
            }
            return walletBalanceRange;
        }

        [HttpGet]
        [ResponseType(typeof(PersonMetadata))]
        public async Task<IHttpActionResult> GetPersonMetadata(Guid userId, PersonMetadataDTO personMetadataDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db = UtilityAPI.RetrieveDBContext(userId);
            var personMetdata = new PersonMetadata()
            {
                WalletBalanceRange = await _GetWalletBalanceRange(personMetadataDTO.EntityType),
                TotalRecords = db.Persons.Where(p => p.EntityType == personMetadataDTO.EntityType).Count(),
            };
            return Ok(personMetdata);
        }
        #endregion



        // PUT: api/Suppliers/5
        [HttpPut]
        [ResponseType(typeof(Person))]
        public async Task<IHttpActionResult> Put(Guid userId, Guid id, SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (supplierDTO == null)
                throw new Exception("PersonDTO should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            var supplier = await db.Persons.FindAsync(id);
            if (supplier == null)
                throw new Exception(String.Format("Person of id {0} not found", id));
            _UpdatePerson(supplier, supplierDTO);
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

        private void _UpdatePerson(Person person, SupplierDTO supplierDTO)
        {
            person.Address = supplierDTO.Address;
            person.GSTIN = supplierDTO.GSTIN;
            person.LastCalled = supplierDTO.LastCalled;
            person.MobileNo = supplierDTO.MobileNo;
            person.Name = supplierDTO.Name;
            person.Rating = supplierDTO.Rating;
        }

        // POST: api/Suppliers
        [ResponseType(typeof(Person))]
        public async Task<IHttpActionResult> Post(Guid userId, SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (supplierDTO == null)
                throw new Exception("PersonDTO should not have been null");
            db = UtilityAPI.RetrieveDBContext(userId);

            var person = new Person()
            {
                PersonId = Guid.NewGuid(),
                Address = supplierDTO.Address,
                EntityType = supplierDTO.EntityType,
                GSTIN = supplierDTO.GSTIN,
                MobileNo = supplierDTO.MobileNo,
                Name = supplierDTO.Name,
                WalletBalance = 0,
                NetWorth = 0,
                FirstVisited = DateTime.Now,
                LastVisited = DateTime.Now,
            };

            db.Persons.Add(person);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (IsNameExist(person.Name))
                {
                    return BadRequest(String.Format("Person with name {0} already exists.", person.Name));
                }
                else if (IsMobNoExist(person.MobileNo))
                {
                    return BadRequest(String.Format("Person with mobile number {0} already exists.", person.MobileNo));
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtRoute("DefaultApi", new { id = person.PersonId }, person);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && db != null)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IsNameExist(string supplierName)
        {
            return db.Persons.Count(s => s.Name == supplierName) > 0;
        }

        private bool IsMobNoExist(string supplierMobileNo)
        {
            return db.Persons.Count(s => s.MobileNo == supplierMobileNo) > 0;
        }
    }
}