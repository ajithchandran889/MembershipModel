using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class ProductPriceDetails
    {
        public int typeId { get; set; }
        public string typeName { get; set; }
        public string displayName { get; set; }
        public int productId { get; set; }
        public double price { get; set; }
        public bool isPerUser { get; set; }
    }
}