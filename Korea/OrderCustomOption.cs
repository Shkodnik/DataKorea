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
    
    public partial class OrderCustomOption
    {
        public int OrderCustomOptionsId { get; set; }
        public int CustomOptionId { get; set; }
        public int OptionId { get; set; }
        public string CustomOptionTitle { get; set; }
        public string OptionTitle { get; set; }
        public decimal OptionPriceBC { get; set; }
        public int OptionPriceType { get; set; }
        public int OrderedCartID { get; set; }
    
        public virtual OrderItem OrderItem { get; set; }
    }
}