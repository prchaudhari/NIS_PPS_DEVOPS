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
    
    public partial class ScheduleLogArchiveRecord
    {
        public long Id { get; set; }
        public long ScheduleId { get; set; }
        public string ScheduleName { get; set; }
        public int NumberOfRetry { get; set; }
        public string LogFilePath { get; set; }
        public System.DateTime LogCreationDate { get; set; }
        public string Status { get; set; }
        public System.DateTime ArchivalDate { get; set; }
        public string TenantCode { get; set; }
    }
}
