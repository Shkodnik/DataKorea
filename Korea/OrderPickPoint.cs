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
    
    public partial class OrderPickPoint
    {
        public int OrderId { get; set; }
        public string PickPointId { get; set; }
        public string PickPointAddress { get; set; }
    
        public virtual Order Order { get; set; }
    }
}