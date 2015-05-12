using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Domain
{
    public class PositionForImport : ListItem
    {
        [Display(Name = "Продукт", Order = 150)]
        public virtual ICollection<ProductForImport> Products { get; set; }


        public void DeletePosition(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                PositionForImport positionforimport = db.PositionForImports.Single(p => p.Id == id);
                db.PositionForImports.Remove(positionforimport);
                db.SaveChanges();
            }
        }

        public PositionForImport Edit(List<ProductForImport> item)
        {
            List<ProductForImport> old = this.Products.ToList();
            if (item.OrderBy(i => i.Id) != old.OrderBy(i => i.Id))
            {
                List<ProductForImport> create = item.Where(o => !old.Select(i => i.Id).Contains(o.Id)).ToList();
                List<ProductForImport> delete = old.Where(o => !item.Select(i => i.Id).Contains(o.Id)).ToList();

                foreach (ProductForImport cr in create)
                {
                    this.Products.Add(cr);
                }
                foreach (ProductForImport dl in delete)
                {
                    this.Products.Remove(dl);
                }
            }
            return this;
        }
	}
}