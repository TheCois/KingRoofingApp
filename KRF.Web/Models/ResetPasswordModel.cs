using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KRF.Web.Models
{
    public class ResetPasswordModel
    {
        public int UserID { get; set; }
        public string Key { get; set; }
        public bool IsValidKey { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("UserPassword")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordEmailModel
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ResetPasswordUrl { get; set; }

    }
}