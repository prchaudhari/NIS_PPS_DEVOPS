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
    
    public partial class RenderEngineRecord
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public Nullable<int> PriorityLevel { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int NumberOfThread { get; set; }
        public bool InUse { get; set; }
        public string NetworkLocation { get; set; }
        public string Domain { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
