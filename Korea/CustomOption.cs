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
    
    public partial class CustomOption
    {
        public CustomOption()
        {
            this.Options = new HashSet<Option>();
        }
    
        public int CustomOptionsID { get; set; }
        public string Title { get; set; }
        public bool IsRequired { get; set; }
        public int InputType { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public int ProductID { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}