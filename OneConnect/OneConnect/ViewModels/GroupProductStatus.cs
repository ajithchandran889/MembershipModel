using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupProductStatus
    {
        public int productId { get; set; }
        public int groupId { get; set; }
        public bool isSubscribe { get; set; }

    }
}