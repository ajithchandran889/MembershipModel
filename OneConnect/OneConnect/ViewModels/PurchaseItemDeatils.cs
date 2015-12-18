using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class PurchaseItemDeatils
    {
        public int productId { get; set; }
        public string userIds { get; set; }
        public DateTime fromDate { get; set; }
        public int subsriptionType { get; set; }
    }
}