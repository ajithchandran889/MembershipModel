using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace OneConnect.ViewModels
{
    public class Register
    {
        [Display(Name="Email")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string emailId { get; set; }
        [Display(Name="Password")]
        [DataType(DataType.Password)]
        [Required]
        public string password { get; set; }
        public string captchaResponse { get; set; }
        public string hostName { get; set; }
    }
}