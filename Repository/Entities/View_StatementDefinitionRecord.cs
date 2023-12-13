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
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("NIS.View_StatementDefinition")]
    public partial class View_StatementDefinitionRecord
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long PublishedBy { get; set; }
        public long Owner { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.DateTime> PublishedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantCode { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public long UpdateBy { get; set; }
        public string PublishedByName { get; set; }
        public string OwnerName { get; set; }
    }
}