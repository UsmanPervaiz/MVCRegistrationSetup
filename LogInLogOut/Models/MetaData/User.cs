using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogInLogOut.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        [Display(Name = "First Name")]
        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage = "Only Alphabest Allowed")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fist Name is Required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is Required.")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is Required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Email Address is not Valid.")]
        public string Email { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "Date of Birth Not Provided", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DatOfBirth { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password is Required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password minimum 6 characters.")]
        [ScaffoldColumn(false)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords Do Not Match.")]
        public string ConfirmPassword { get; set; }

        [ScaffoldColumn(false)]
        public bool IsEmailVerified { get; set; }

        [ScaffoldColumn(false)]
        public System.Guid ActivationCode { get; set; }
    }

}