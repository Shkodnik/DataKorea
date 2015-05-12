using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models
{
    [Serializable]
    public class SaveProduct : BaseEntity
    {
        [Display(Name = "Категория в кэше", Order = 100)]
        [Index]
        public virtual Guid IdCach { get; set; }

        [Display(Name = "Категория в программе", Order = 100)]
        [Index]
        public virtual Guid IdProgProd { get; set; }

        [Display(Name = "Категория на сайте", Order = 300)]
        [Index]
        public virtual int IdSiteProd { get; set; }

        public SaveProduct Edit(Product ProductItem, ProductTree TreeItem)
        {
            SaveProduct product = new SaveProduct()
            {
                Id = Guid.NewGuid(),
                IdCach = TreeItem.Id,
                IdProgProd = TreeItem.IdProdram,
                IdSiteProd = ProductItem.ProductId
            };
            return product;
        }

        public List<SaveProduct> ReadSave(string puth)
        {
            List<SaveProduct> SavePropertysValue = new List<SaveProduct>();
            BinaryFormatter formatter = new BinaryFormatter();
            string pathDataProduct = puth + "\\Product.dat";
            if (System.IO.File.Exists(pathDataProduct))
            {
                try
                {
                    using (FileStream fs = new FileStream(pathDataProduct, FileMode.OpenOrCreate))
                    {
                        SavePropertysValue = (List<SaveProduct>)formatter.Deserialize(fs);
                    }
                }
                catch
                {
                    System.IO.File.Delete(pathDataProduct);
                    SavePropertysValue = new List<SaveProduct>();
                }
            }
            return SavePropertysValue;
        }

        
    }
}