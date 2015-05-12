using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Domain
{
    public class GenerationForImport : ListItem
    {

        public ModelForImport Model { get; set; }

        [ForeignKey("Model")]
        [Display(Name = "Модель", Order = 150)]
        public Guid ModelId { get; set; }

        public virtual ICollection<ProductForImport> Products { get; set; }

        public void DeleteGeneration(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                GenerationForImport generation = db.GenerationForImports.Single(p => p.Id == id);
                db.GenerationForImports.Remove(generation);
                db.SaveChanges();
            }
        }

        public GenerationForImport FporIndex(Dictionary<Guid, string> ModelDic)
        {
            using (KoreaContext db = new KoreaContext())
            {
                if (this.ModelId != null) 
                {
                    this.Title = ModelDic.Single(g => g.Key == this.ModelId).Value + " > " + this.Title;
                }
                return this;
            }
        }

        public GenerationForImport Edit(GenerationForImport item)
        {
            this.Title = item.Title;
            this.ModelId = item.ModelId;
            this.Weight = item.Weight;
            return this;
        }
    }
}