using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupProductDetails
    {
        public int id { set; get; }
        public int groupId { set; get; }
        public int productId { set; get; }
        public string productName { set; get; }
        public string productDescription { set; get; }
        public bool isSubscribed { set; get; }
        public IEnumerable<ProductRoleDetails> productRole { get; set; }

    }
}