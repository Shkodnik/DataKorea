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
    
    public partial class CustomerCertificate
    {
        public System.Guid CustomerID { get; set; }
        public int CertificateID { get; set; }
    
        public virtual Certificate Certificate { get; set; }
    }
}