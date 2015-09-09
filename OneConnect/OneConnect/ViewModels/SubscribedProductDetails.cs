using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class SubscribedProductDetails
    {
        public string productName { get; set; }
        public string productDescription { get; set; }
        public string productImageUrl { get; set; }
        public string subscriptionType { get; set; }
        public DateTime? toDate { get; set; }
        public DateTime? fromDate { get; set; }
    }
}