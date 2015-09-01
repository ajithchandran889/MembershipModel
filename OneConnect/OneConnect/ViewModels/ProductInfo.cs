using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class ProductInfo
    {
        public int productId { get; set; }
        public string productImageUrl { get; set; }
        public string productName { get; set; }
        public string productDescription { get;set; }
        public decimal? goldMonthly { get; set; }
        public decimal? goldYearly { get; set; }
        public decimal? silverMonthly { get; set; }
        public decimal? silverYearly { get; set; }
        public decimal? silverPerUserMonthly { get; set; }
        public decimal? goldPerUserMonthly { get; set; }
        public decimal? silverPerUserYearly { get; set; }
        public decimal? goldPerUserYearly { get; set; }
    }
}