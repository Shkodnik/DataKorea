namespace Korea
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Property
    {
        public Property()
        {
            this.PropertyValues = new HashSet<PropertyValue>();
        }
    
        public int PropertyID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> UseInFilter { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<bool> Expanded { get; set; }
        public Nullable<bool> IsModel { get; set; }
    
        public virtual ICollection<PropertyValue> PropertyValues { get; set; }

        public Property Create(string Name)
        {
            Property SiteProperty = new Property();
            using (koreaEntities1 db = new koreaEntities1())
            {                
                SiteProperty = db.Properties.FirstOrDefault(p => p.Name == Name);
                if (SiteProperty == null)
                {
                    SiteProperty = new Property
                    {
                        PropertyID = -1,
                        Name = Name,
                        UseInFilter = true,
                        SortOrder = 0,
                        Expanded = false,
                        IsModel = false
                    };
                    db.Properties.Add(SiteProperty);
                    db.SaveChanges();
                }
            }
            return SiteProperty;
        }

        public void Ñleaning()
        {
            using (koreaEntities1 db = new koreaEntities1())
            {
                List<Property> PropertiesList = db.Properties.ToList();
                List<PropertyValue> propertyValues = db.PropertyValues.ToList();
                List<ProductPropertyValue> productPropertyValuest = db.ProductPropertyValues.ToList();
                List<Product> products = db.Products.ToList();                
                List<ProductPropertyValue> ProductPropertyValueDelete = productPropertyValuest.Where(p => !products.Select(p2 => p2.ProductId)
                                                                                                                   .Contains(p.ProductID))
                                                                                              .ToList();
                List<PropertyValue> PropertyValueDelete = propertyValues.Where(p => !productPropertyValuest.Select(p2 => p2.PropertyValueID)
                                                                                                           .Contains(p.PropertyValueID))
                                                                        .ToList();

                foreach (ProductPropertyValue item in ProductPropertyValueDelete)
                {
                    db.ProductPropertyValues.Remove(item);
                }
                db.SaveChanges();

                foreach (PropertyValue item in PropertyValueDelete)
                {
                    db.PropertyValues.Remove(item);
                }
                db.SaveChanges();
            }
        }        
    }
}
