//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneConnect
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserRole
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Nullable<int> GroupProductId { get; set; }
        public Nullable<int> RoleId { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedAt { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual ProductRole ProductRole { get; set; }
    }
}
