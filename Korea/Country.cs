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
    
    public partial class Country
    {
        public Country()
        {
            this.Brands = new HashSet<Brand>();
            this.Taxes = new HashSet<Tax>();
            this.Contacts = new HashSet<Contact>();
            this.PaymentMethods = new HashSet<PaymentMethod>();
            this.ShippingMethods = new HashSet<ShippingMethod>();
        }
    
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryISO2 { get; set; }
        public string CountryISO3 { get; set; }
    
        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<Tax> Taxes { get; set; }
        public virtual ICollection<Contact> Contacts { get; set; }
        public virtual Country Country1 { get; set; }
        public virtual Country Country2 { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
        public virtual ICollection<ShippingMethod> ShippingMethods { get; set; }
    }
}