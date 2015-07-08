using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class AccountInfo
    {
        public string userId { get; set; }
        public string customUserId { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string company { get; set; }
        public string address { get; set; }
        public string contact { get; set; }
        public bool status { get; set; }
    }
}