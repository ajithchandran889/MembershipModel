using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class ProductSubscribeDetails
    {
        public string transactionID { get; set; }
        public double sAmountPaid { get; set; }
        public string payerEmail { get; set; }
        public string item { get; set; }
        public string tempItemIds { get; set; }
        public double amountPaid { get; set; }
    }
}