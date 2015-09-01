using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupMemberDetails
    {
        public int id { get; set; }
        public int groupId { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string userEmail { get; set; }
        public bool isSubscribed { get; set; }
    }
}