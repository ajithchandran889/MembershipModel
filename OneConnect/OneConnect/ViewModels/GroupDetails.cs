using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupDetails
    {

        public int groupId { get; set; }
        public string groupName { get; set; }
        public string description { get; set; }
        public bool isActive{ get; set; }

    }
}