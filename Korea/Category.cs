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
    using Korea.Models;
    using System;
    using System.Collections.Generic;
    
    public partial class Category
    {
        public Category()
        {
            this.ExportFeedSelectedCategories = new HashSet<ExportFeedSelectedCategory>();
            this.ProductCategories = new HashSet<ProductCategory>();
            this.Coupons = new HashSet<Coupon>();
            this.Taxes = new HashSet<Tax>();
        }
    
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentCategory { get; set; }
        public string Description { get; set; }
        public Nullable<int> Products_Count { get; set; }
        public int SortOrder { get; set; }
        public bool Enabled { get; set; }
        public int Total_Products_Count { get; set; }
        public string DisplayStyle { get; set; }
        public bool DisplayChildProducts { get; set; }
        public string UrlPath { get; set; }
        public bool HirecalEnabled { get; set; }
        public bool DisplayBrandsInMenu { get; set; }
        public bool DisplaySubCategoriesInMenu { get; set; }
        public Nullable<int> CatLevel { get; set; }
        public string BriefDescription { get; set; }
    
        public virtual ICollection<ExportFeedSelectedCategory> ExportFeedSelectedCategories { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<Tax> Taxes { get; set; }

        public Category Edit(Category NewCategory)
        {
            this.CatLevel = NewCategory.CatLevel;
            this.BriefDescription = NewCategory.BriefDescription;
            this.Name = NewCategory.Name;
            this.UrlPath = NewCategory.UrlPath;
            this.Description = NewCategory.Description;
            return this;
        }
    }
}
