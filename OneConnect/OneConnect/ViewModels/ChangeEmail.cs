﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class ChangeEmail
    {
        public string userId { get; set; }
        public string oldEmailId { get; set; }
        public string password { get; set; }
        public string emailId { get; set; }
        public string hostName { get; set; }

    }
}