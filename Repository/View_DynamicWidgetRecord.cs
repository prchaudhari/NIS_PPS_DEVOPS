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
    
    public partial class View_DynamicWidgetRecord
    {
        public long Id { get; set; }
        public string WidgetName { get; set; }
        public string WidgetType { get; set; }
        public long PageTypeId { get; set; }
        public long EntityId { get; set; }
        public string Title { get; set; }
        public string ThemeType { get; set; }
        public string ThemeCSS { get; set; }
        public string WidgetSettings { get; set; }
        public string WidgetFilterSettings { get; set; }
        public string Status { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public long LastUpdatedBy { get; set; }
        public Nullable<long> PublishedBy { get; set; }
        public Nullable<System.DateTime> PublishedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string TenantCode { get; set; }
        public string PublishedByName { get; set; }
        public string CreatedByName { get; set; }
        public string PageTypeName { get; set; }
        public string EntityName { get; set; }
        public Nullable<long> CloneOfWidgetId { get; set; }
        public string Version { get; set; }
    }
}
