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
using UnidecodeSharpFork;

namespace Korea.Models
{
    [Serializable]
    public class SaveBrand : BaseEntity
    {
        [Display(Name = "Брэнд в программе", Order = 100)]
        [Index]
        public virtual Guid IdProgProd { get; set; }

        [Display(Name = "Брэнд на сайте", Order = 300)]
        [Index]
        public virtual int IdSiteProd { get; set; }

        [Display(Name = "Брэнд в кэше", Order = 300)]
        [Index]
        public virtual int IdCach { get; set; }

        [Display(Name = "Брэнд в кэше", Order = 300)]
        [Index]
        public virtual string Value { get; set; }


        public List<SaveBrand> ReadSave(string puth)
        {
            List<SaveBrand> SaveBrands = new List<SaveBrand>();
            BinaryFormatter formatter = new BinaryFormatter();
            string pathPropertyValue = puth + "\\SaveBrand.dat";
            try
            {
                using (FileStream fs = new FileStream(pathPropertyValue, FileMode.OpenOrCreate))
                {
                    SaveBrands = (List<SaveBrand>)formatter.Deserialize(fs);
                }
            }
            catch
            {
                System.IO.File.Delete(pathPropertyValue);
                SaveBrands = new List<SaveBrand>();
            }
            return SaveBrands;
        }

        public void Saving(List<SaveBrand> Save, string puth)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string pathPropertyValue = puth + "\\SaveBrand.dat";
            try
            {
                // получаем поток, куда будем записывать сериализованный объект
                using (FileStream fs = new FileStream(pathPropertyValue, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, Save);
                }
            }
            catch
            {
                System.IO.File.Delete(pathPropertyValue);
            }
        }

    }
}