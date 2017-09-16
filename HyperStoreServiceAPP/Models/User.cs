using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HyperStoreService.Models
{
    public class User
    {
        #region UserInformation
        public Guid UserId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailId { get; set; }

        public DateTime DateOfBirth { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(10)]
        public string MobileNo { get; set; }

        [Required]
        public string Password { get; set; }
        #endregion

        #region BusinessInformation
        [Required]
        public string BusinessName { get; set; }

        [Required]
        public string BusinessType { get; set; }

        [Required]
        public string GSTIN { get; set; }

        [Required]
        public string AddressLine { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PinCode { get; set; }


        [Required]
        public string State { get; set; }
        
        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }
        #endregion

        #region UserStatistics
        [Required]
        public string DeviceId { get; set; }
        public int NumberOfSMS { get; set; }
        #endregion
    }
}