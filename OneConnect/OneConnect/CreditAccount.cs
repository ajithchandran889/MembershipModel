//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OneConnect
{
    using System;
    using System.Collections.Generic;
    
    public partial class CreditAccount
    {
        public int Id { get; set; }
        public Nullable<int> PcanId { get; set; }
        public string UserSubscribed { get; set; }
        public int FinancialTransactionId { get; set; }
        public int ProductId { get; set; }
        public string ProductSubscriptionModel { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedAt { get; set; }
        public string LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModiifedAt { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual Product Product { get; set; }
    }
}
