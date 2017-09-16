using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HyperStoreServiceAPP.DTO
{
    public class PersonalInformationDTO
    {
        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [MaxLength(9, ErrorMessage = "Try first name with atmost 9 charecters.")]
        [RegularExpression(@"[a-zA-Z]{1,9}[\s]{0,1}", ErrorMessage = "First name is Invalid")]
        public string FirstName { get; set; }

        [MaxLength(9, ErrorMessage = "Try last name with atmost 9 charecters.")]
        [RegularExpression(@"[a-zA-Z]{1,9}", ErrorMessage = "Last name is Invalid")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Email Id is Invalid")]
        public string EmailId { get; set; }

        public DateTime DateOfBirth { get; set; }

        [RegularExpression(@"[987]\d{9}", ErrorMessage = "{0} is Invalid.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Short passwords are easy to guess. Try one with at least 6 characters.")]
        public string Password { get; set; }
    }

    public class BusinessInformationDTO
    {
        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [MaxLength(25, ErrorMessage = "Try Business name with atmost 25 charecters.")]
        [RegularExpression(@"[a-zA-Z\s]{1,25}", ErrorMessage = "Business name is Invalid")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [MaxLength(25, ErrorMessage = "Try address with atmost 25 charecters.")]
        public string AddressLine { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [RegularExpression(@"\d{2}[a-zA-Z]{5}\d{4}[a-zA-Z]{1}\d{1}[zZ][a-zA-Z0-9]", ErrorMessage = "{0} is Invalid.")]
        public string GSTIN { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [MaxLength(15, ErrorMessage = "Try city with atmost 15 charecters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [MaxLength(6, ErrorMessage = "Try pincode with 6 digits.")]
        [RegularExpression(@"[1-9]\d{5}", ErrorMessage = "Try pincode with 6 digits.")]
        public string PinCode { get; set; }

        public string BusinessType { get; set; }

        public string State { get; set; }

        public Cordinates Cordinates { get; set; }
    }

    public class UserDTO
    {
        [Required]
        public PersonalInformationDTO PI { get; set; }
        [Required]
        public BusinessInformationDTO BI { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        public string DeviceId { get; set; }

    }

    public class Cordinates
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class AuthenticateUserDTO
    {
        [RegularExpression(@"[987]\d{9}", ErrorMessage = "{0} is Invalid.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "You can't leave this empty.", AllowEmptyStrings = false)]
        public string DeviceId { get; set; }

    }
}