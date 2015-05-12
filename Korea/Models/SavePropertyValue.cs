using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Formatters.Binary;
using UnidecodeSharpFork;

namespace Korea.Models
{
    [Serializable]
    public class SavePropertyValue : BaseEntity
    {
        public int IdCach { get; set; }
        public Guid ProgPropertyValueId { get; set; }
        public int SitePropertyValueId { get; set; }

        public List<SavePropertyValue> ReadSave(string Name, string puth)
        {
            List<SavePropertyValue> SavePropertysValue = new List<SavePropertyValue>();
            BinaryFormatter formatter = new BinaryFormatter();
            string pathPropertyValue = puth + "\\Save" + Name.Unidecode() + ".dat";
                try
                {
                    using (FileStream fs = new FileStream(pathPropertyValue, FileMode.OpenOrCreate))
                    {
                        SavePropertysValue = (List<SavePropertyValue>)formatter.Deserialize(fs);
                    }
                }
                catch
                {
                    System.IO.File.Delete(pathPropertyValue);
                    SavePropertysValue = new List<SavePropertyValue>();
                }
            return SavePropertysValue;
        }

        public void Saving(List<SavePropertyValue> Save, string Name, string puth)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string pathPropertyValue = puth + "\\Save" + Name.Unidecode() + ".dat";
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