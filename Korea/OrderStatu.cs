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
    
    public partial class OrderStatu
    {
        public OrderStatu()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public int OrderStatusID { get; set; }
        public string StatusName { get; set; }
        public byte CommandID { get; set; }
        public bool IsDefault { get; set; }
        public bool IsCanceled { get; set; }
        public string Color { get; set; }
        public int SortOrder { get; set; }
    
        public virtual ICollection<Order> Orders { get; set; }
    }
}
