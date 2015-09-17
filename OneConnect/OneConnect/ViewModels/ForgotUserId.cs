using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class ForgotUserId
    {
        public string emailId { get; set; }
        public string captchaResponse { get; set; }
        public string hostName { get; set; }
    }
}