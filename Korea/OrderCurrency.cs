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
    
    public partial class OrderCurrency
    {
        public int OrderID { get; set; }
        public string CurrencyCode { get; set; }
        public int CurrencyNumCode { get; set; }
        public double CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
