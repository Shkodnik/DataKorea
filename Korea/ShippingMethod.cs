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
    
    public partial class ShippingMethod
    {
        public ShippingMethod()
        {
            this.Orders = new HashSet<Order>();
            this.ShippingParams = new HashSet<ShippingParam>();
            this.Cities = new HashSet<City>();
            this.Countries = new HashSet<Country>();
            this.PaymentMethods = new HashSet<PaymentMethod>();
        }
    
        public int ShippingMethodID { get; set; }
        public int ShippingType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int SortOrder { get; set; }
        public string IconFileName { get; set; }
    
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ShippingParam> ShippingParams { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Country> Countries { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}