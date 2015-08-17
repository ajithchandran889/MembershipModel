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
    
    public partial class UsersAditionalInfo
    {
        public int Id { get; set; }
        public string CustomUserId { get; set; }
        public string AspNetUserId { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string ContactInfo { get; set; }
        public Nullable<bool> IsOwner { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModifiedAt { get; set; }
        public string IpAddress { get; set; }
        public Nullable<System.DateTime> ForgotPasswordRequestAt { get; set; }
        public string ForgotPasswordToken { get; set; }
        public Nullable<System.DateTime> ChangeEmailReuestAt { get; set; }
        public string ChangeEmailToken { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string passwordRecoveryToken { get; set; }
        public string emailResetToken { get; set; }
        public string newEmailRequested { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
    }
}
