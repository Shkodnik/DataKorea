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
    
    public partial class Photo
    {
        public Photo()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public int PhotoId { get; set; }
        public int ObjId { get; set; }
        public string PhotoName { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public string Description { get; set; }
        public Nullable<int> PhotoSortOrder { get; set; }
        public bool Main { get; set; }
        public string OriginName { get; set; }
        public string Type { get; set; }
        public Nullable<int> ColorID { get; set; }
    
        public virtual Color Color { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
