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
    
    public partial class ScheduleLogDetailRecord
    {
        public long Id { get; set; }
        public long ScheduleLogId { get; set; }
        public long ScheduleId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> NumberOfRetry { get; set; }
        public string Status { get; set; }
        public string LogMessage { get; set; }
        public System.DateTime CreationDate { get; set; }
        public string TenantCode { get; set; }
        public string StatementFilePath { get; set; }
    }
}