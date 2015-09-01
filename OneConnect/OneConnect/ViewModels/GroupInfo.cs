using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneConnect.ViewModels
{
    public class GroupInfo
    {
        public GroupDetails groupDetails { get; set; }

        public List<GroupProductDetails> groupProducts { get; set; }

        public List<GroupMemberDetails> groupMembers { get; set; }

        public List<GroupMemberRoleDetails> groupMemberRoles { get; set; }
    }
}