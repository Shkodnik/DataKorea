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
    using Korea.Models.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public partial class ProductPropertyValue
    {
        public int ProductID { get; set; }
        public int PropertyValueID { get; set; }
        public Nullable<int> SortOrder { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual PropertyValue PropertyValue { get; set; }

        public List<ProductPropertyValue> ProductsProperty(string Name,
                                                           List<SavePropertyValue> SavePropertysValue,
                                                           List<ProductForImport> ProductsProgram,
                                                           List<SaveProduct> saveProd)
        {
            List<ProductPropertyValue> NewProductsPropertyV = new List<ProductPropertyValue>();
            
            foreach (ProductForImport Product in ProductsProgram)
            {
                List<int> properties = new List<int>();
                switch (Name)
                {
                    case "���������":
                        properties = SavePropertysValue.Where(s => Product.Generations
                                                                          .Select(p => p.Id)
                                                                          .Contains(s.ProgPropertyValueId))
                                                       .Select(s => s.SitePropertyValueId)
                                                       .ToList();
                        break;
                    case "�������":
                        properties = SavePropertysValue.Where(s => Product.Positions
                                                                          .Select(p => p.Id)
                                                                          .Contains(s.ProgPropertyValueId))
                                                       .Select(s => s.SitePropertyValueId)
                                                       .ToList();

                        break;
                }
                
                int i = 1;
                foreach (int item in properties)
                {
                    ProductPropertyValue NewPPVTemp = new ProductPropertyValue();
                    NewPPVTemp.SortOrder = i;
                    NewPPVTemp.ProductID = saveProd.FirstOrDefault(s => s.IdProgProd == Product.Id)
                                                   .IdSiteProd;
                    NewPPVTemp.PropertyValueID = item;
                    NewProductsPropertyV.Add(NewPPVTemp);
                    i += 10;
                }
            }
            
            return NewProductsPropertyV;
        }

        public void CreateAndDelete(List<ProductPropertyValue> NewProductsProperty, int PropertyID)
        {
            using (koreaEntities1 db = new koreaEntities1())
            {
                List<PropertyValue> PropertysValueSiteCut = db.PropertyValues.Where(p => p.PropertyID == PropertyID)
                                                             .ToList();
                List<ProductPropertyValue> SiteProductPropertyV = db.ProductPropertyValues.ToList();
                List<ProductPropertyValue> ProductPropertyDelete = SiteProductPropertyV.Where(s => NewProductsProperty.Where(n => n.ProductID == s.ProductID
                                                                                                                             && n.PropertyValueID == s.PropertyValueID)
                                                                                                                      .Count() == 0)
                                                                                       .Where(s => PropertysValueSiteCut.Select(p => p.PropertyValueID)
                                                                                                                         .Contains(s.PropertyValueID))
                                                                                       .ToList();
                List<ProductPropertyValue> ProductPropertyCreate = NewProductsProperty.Where(n => SiteProductPropertyV.Where(s => s.ProductID == n.ProductID
                                                                                                                             && s.PropertyValueID == n.PropertyValueID)
                                                                                                                      .Count() == 0)
                                                                                      .ToList();
                foreach (ProductPropertyValue item in ProductPropertyDelete)
                {
                    db.ProductPropertyValues.Remove(item);
                }
                db.SaveChanges();
                foreach (ProductPropertyValue item in ProductPropertyCreate)
                {
                    db.ProductPropertyValues.Add(item);
                }
                db.SaveChanges();
            }
        }
    }
}