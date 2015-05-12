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
using System.Linq;
    
    public partial class Product
    {
        public Product()
        {
            this.CustomOptions = new HashSet<CustomOption>();
            this.Offers = new HashSet<Offer>();
            this.ExportFeedSelectedProducts = new HashSet<ExportFeedSelectedProduct>();
            this.OrderItems = new HashSet<OrderItem>();
            this.ProductCategories = new HashSet<ProductCategory>();
            this.ProductFiles = new HashSet<ProductFile>();
            this.ProductVideos = new HashSet<ProductVideo>();
            this.ProductPropertyValues = new HashSet<ProductPropertyValue>();
            this.Ratios = new HashSet<Ratio>();
            this.RecentlyViewsDatas = new HashSet<RecentlyViewsData>();
            this.Coupons = new HashSet<Coupon>();
            this.Taxes = new HashSet<Tax>();
        }
    
        public int ProductId { get; set; }
        public string ArtNo { get; set; }
        public string Name { get; set; }
        public double Ratio { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> Weight { get; set; }
        public string Size { get; set; }
        public string BriefDescription { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public System.DateTime DateAdded { get; set; }
        public System.DateTime DateModified { get; set; }
        public Nullable<bool> Recomended { get; set; }
        public Nullable<bool> New { get; set; }
        public Nullable<bool> Bestseller { get; set; }
        public Nullable<bool> OnSale { get; set; }
        public Nullable<int> BrandID { get; set; }
        public Nullable<bool> AllowPreOrder { get; set; }
        public Nullable<int> SortBestseller { get; set; }
        public Nullable<int> SortNew { get; set; }
        public Nullable<int> SortDiscount { get; set; }
        public string UrlPath { get; set; }
        public Nullable<bool> CategoryEnabled { get; set; }
        public string SalesNote { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> ShippingPrice { get; set; }
        public Nullable<double> MinAmount { get; set; }
        public Nullable<double> MaxAmount { get; set; }
        public double Multiplicity { get; set; }
        public bool HasMultiOffer { get; set; }
        public string Instructions { get; set; }
        public Nullable<bool> SpecialCat { get; set; }
    
        public virtual Brand Brand { get; set; }
        public virtual ICollection<CustomOption> CustomOptions { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<ExportFeedSelectedProduct> ExportFeedSelectedProducts { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ICollection<ProductFile> ProductFiles { get; set; }
        public virtual ICollection<ProductVideo> ProductVideos { get; set; }
        public virtual ICollection<ProductPropertyValue> ProductPropertyValues { get; set; }
        public virtual ICollection<Ratio> Ratios { get; set; }
        public virtual ICollection<RecentlyViewsData> RecentlyViewsDatas { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<Tax> Taxes { get; set; }


        public Product ArtNoTrim(Char CharTrim)
        {
            this.ArtNo = this.ArtNo.Trim(CharTrim);
            return this;
        }

        public Product Edit(Product item)
        {
            this.AllowPreOrder = item.AllowPreOrder;
            this.ArtNo = this.ArtNo;
            this.Bestseller = item.Bestseller;
            this.BriefDescription = item.BriefDescription;
            this.CategoryEnabled = item.CategoryEnabled;
            this.BriefDescription = item.BriefDescription;
            this.Name = item.Name;
            return this;
        }

        public Product FillBrand(int? BrandId)
        {
            this.BrandID = BrandId;
            return this;
        }

        public void ConnectBrand(Dictionary<Guid, Guid> DicGuid, 
                                 List<SaveBrand> saveBrands, 
                                 List<SaveProduct> saveProd)
        {
            Product product = new Product ();
            //������!!
            Dictionary<int, Guid> step1 = DicGuid.Join(saveProd,                                                            
                                                            d => d.Key,
                                                            s => s.IdProgProd,
                                                            (d, s) => new {s.IdSiteProd, d.Value})
                                                      .ToDictionary(n => n.IdSiteProd, n => n.Value);
            Dictionary<int, Guid> stepTest = step1.Where(s => s.Key == 87505).ToDictionary(s => s.Key,s => s.Value);
            List<SaveBrand> saveBrandsTest = saveBrands.Where(s => s.IdProgProd == new Guid("d516982b-d411-4109-81d4-026800aa61fe")).ToList();
            List<int> temp = step1.Join(saveBrands,
                                                            s1 => s1.Value,
                                                            s2 => s2.IdProgProd,
                                                            (s1, s2) => s1.Key)
                                                            .ToList();
            List<int> temp2 = temp.Distinct().ToList();
            List<int> temp3 = temp.Where(t1 => temp.Where(t2 => t2==t1).Count() > 1).ToList();
            Dictionary<int, int> step2 = step1.Join(saveBrands,
                                                            s1 => s1.Value,
                                                            s2 => s2.IdProgProd,
                                                            (s1, s2) => new {s1.Key, s2.IdSiteProd})
                                                      .ToDictionary(n => n.Key, n => n.IdSiteProd);
            using (koreaEntities1 db = new koreaEntities1())
            {
                List<Product> Products = db.Products.ToList();
                List<Product> ProductsBrand = Products.Join(step2,
                                                            p => p.ProductId,
                                                            s => s.Key,
                                                            (p, s) => p.FillBrand(s.Value))
                                                      .ToList();
                db.SaveChanges();
                List<Product> ProductsNull = Products.Where(p => !step2.Select(s => s.Key)
                                                                       .Contains(p.ProductId))
                                                     .Select(p => p.FillBrand(null))
                                                     .ToList();
                db.SaveChanges();
            }
        }
    }
}
