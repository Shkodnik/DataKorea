//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Korea
{
    using System;
    using System.Collections.Generic;
    
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int ParentId { get; set; }
        public int EntityId { get; set; }
        public int Type { get; set; }
        public System.Guid CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public System.DateTime AddDate { get; set; }
        public bool Checked { get; set; }
        public string IP { get; set; }
    }
}