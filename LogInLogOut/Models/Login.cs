using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LogInLogOut.Models
{
    public class Login
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email Is Required")]
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }

        [Required(AllowEmptyStrings =false, ErrorMessage = "Password Is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RemeberMe { get; set; }
    }
}