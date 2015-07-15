using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class Login
    {
        [Display(Name = "User ID")]
        [Required]
        public string userId { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required]
        public string password { get; set; }
    }
}