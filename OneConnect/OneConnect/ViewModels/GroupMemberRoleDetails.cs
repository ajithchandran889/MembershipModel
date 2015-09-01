using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupMemberRoleDetails
    {
        public int id { get; set; }
        public int groupMemberId { get; set; }
        public int groupProductId { get; set; }
        public int roleId { get; set; }
        public bool isSubscribed { get; set; }
    }
}