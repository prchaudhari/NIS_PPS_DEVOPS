//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace nIS
{
    using System;
    using System.Collections.Generic;
    
    public partial class NB_InvestmentTransaction
    {
        public long Id { get; set; }
        public long BatchId { get; set; }
        public string CustomerId { get; set; }
        public long InvestorId { get; set; }
        public long InvestmentId { get; set; }
        public long ProductId { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public string TransactionDesc { get; set; }
        public string WJXBFS1 { get; set; }
        public string WJXBFS2_Debit { get; set; }
        public string WJXBFS3_Credit { get; set; }
        public string WJXBFS4_Balance { get; set; }
        public string WJXBFS5_TransId { get; set; }
        public string TenantCode { get; set; }
    }
}
