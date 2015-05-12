using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Domain
{
    public class ModelForImport : ListItem
    {
        public BrandForImport Brand { get; set; }


        [ForeignKey("Brand")]
        [Display(Name = "Марка", Order = 150)]
        public Guid BrandId { get; set; }

        

        [Required]
        [Index]
        public virtual int ExtId { get; set; }

        public void DeleteModel(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                ModelForImport model = db.ModelForImports.Find(id);
                IEnumerable<GenerationForImport> generations = db.GenerationForImports.Where(p => p.ModelId == model.Id);
                foreach (GenerationForImport generation in generations)
                {
                    new GenerationForImport().DeleteGeneration(generation.Id);
                }
                db.ModelForImports.Remove(model);
                db.SaveChanges();
            }
        }
	}
}