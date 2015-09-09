using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;

namespace OneConnect.ViewModels
{
    public class GroupInfo
    {
        public GroupDetails groupDetails { get; set; }

        public AccountInfo accInfo { get; set; }

        public IEnumerable<UserDetails> users { get; set; }

        public List<GroupProductDetails> groupProducts { get; set; }

        public List<GroupMemberDetails> groupMembers { get; set; }

        public List<GroupMemberRoleDetails> groupMemberRoles { get; set; }
    }
}