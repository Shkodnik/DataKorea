using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Korea.Models.Domain
{
    public class BrandForImport : ListItem
    {
        [Required]
        public virtual string Abbreviation { get; set; }

        [Required]
        [Index]
        public virtual int ExtId { get; set; }

        public void DeleteBrand(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                BrandForImport brend = db.BrandForImports.Find(id);
                //clear all nested cars
                ICollection<ModelForImport> models = db.ModelForImports.Where(c => c.BrandId == id).ToList();
                foreach (ModelForImport model in models)
                {
                    new ModelForImport().DeleteModel(model.Id);
                }
                //delete self and save
                db.BrandForImports.Remove(brend);
                db.SaveChanges();
            }
        }
    }
}