using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class ProductRoleDetails
    {
        public int id { get; set; }
        public int productId { get; set; }
        public string roleName { get; set; }
        public string roleDescription { get; set; }
    }
}