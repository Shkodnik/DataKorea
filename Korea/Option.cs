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
    
    public partial class Option
    {
        public int OptionID { get; set; }
        public int CustomOptionsID { get; set; }
        public string Title { get; set; }
        public double PriceBC { get; set; }
        public int PriceType { get; set; }
        public Nullable<int> SortOrder { get; set; }
    
        public virtual CustomOption CustomOption { get; set; }
    }
}
