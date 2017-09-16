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
using HyperStoreServiceAPP.DTO;

namespace HyperStoreServiceAPP.Controllers
{
    public enum EAuthentication
    {
        NotAuthenticated,
        OneFactorAuthenticated,
        TwoFactorAuthenticated
    }
    public class UsersController : ApiController
    {
        private HyperStoreServiceContext db = new HyperStoreServiceContext();

        // GET: api/Users
        [HttpGet]
        public IQueryable<User> Get()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> GetUser(Guid id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(Guid id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserId)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [ResponseType(typeof(EAuthentication))]
        public IHttpActionResult AuthenticateUser(AuthenticateUserDTO authenticateUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (authenticateUserDTO == null)
                return BadRequest("userDTO should not have been null");

            EAuthentication eauthenitcation = EAuthentication.NotAuthenticated;
            var user = db.Users.Where(u => u.MobileNo == authenticateUserDTO.MobileNo).FirstOrDefault();
            if (user != null)
            {
                if (string.Compare(user.Password, authenticateUserDTO.Password) == 0)
                {
                    eauthenitcation = EAuthentication.OneFactorAuthenticated;
                    if (string.Compare(user.DeviceId, authenticateUserDTO.DeviceId) == 0)
                        eauthenitcation = EAuthentication.TwoFactorAuthenticated;
                }
            }
            var result = new List<EAuthentication>();
            result.Add(eauthenitcation);
            return Ok(result);
        }

        // POST: api/Users
        [HttpPost]
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> Post(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userDTO == null)
                return BadRequest("userDTO should not have been null");

            var user = new User()
            {
                UserId = Guid.NewGuid(),
                NumberOfSMS = 400,
                AddressLine = userDTO.BI.AddressLine,
                BusinessName = userDTO.BI.BusinessName,
                BusinessType = userDTO.BI.BusinessType,
                City = userDTO.BI.City,
                DateOfBirth = userDTO.PI.DateOfBirth,
                EmailId = userDTO.PI.EmailId,
                FirstName = userDTO.PI.FirstName,
                GSTIN = userDTO.BI.GSTIN,
                LastName = userDTO.PI.LastName,
                Latitude = userDTO.BI.Cordinates.Latitude,
                Longitude = userDTO.BI.Cordinates.Longitude,
                MobileNo = userDTO.PI.MobileNo,
                Password = userDTO.PI.Password,
                PinCode = userDTO.BI.PinCode,
                State = userDTO.BI.State,
                DeviceId = userDTO.DeviceId,
            };
            db.Users.Add(user);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(userDTO.PI.MobileNo))
                    return BadRequest(String.Format("The mobileNo {0} you entered already exist with another account. Please try with another number", userDTO.PI.MobileNo));
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public async Task<IHttpActionResult> DeleteUser(Guid id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(string mobileNo)
        {
            return db.Users.Count(e => e.MobileNo == mobileNo) > 0;
        }
    }
}