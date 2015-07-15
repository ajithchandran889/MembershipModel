using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class RecoverPassword
    {
        public string newPassword { get; set; }
        public string recoveryToken { get; set; }
    }
}