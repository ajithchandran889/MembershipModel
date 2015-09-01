using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupMemberRoleDetailsSave
    {
        public int groupId { get; set; }

        public List<GroupMemberRoleDetails> groupMemberRoleDetails { get; set; }
    }
}