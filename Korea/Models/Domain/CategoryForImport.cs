using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Korea.Models.Domain
{

    public class CategoryForImport: ListItem
    {
        [ForeignKey("Parent")]
        [Display(Name = "Родительская категория", Order = 90)]
        public virtual Guid? CategoryId { get; set; }
        
        public virtual CategoryForImport Parent { get; set; }
        public virtual List<CategoryForImport> Children { get; set; }

        [MaxLength(16)]
        [Index]
        [Display(Name = "Идентификатор", Order = 120)]
        public virtual string OuterKey { get; set; }

        [Required]
        [Index]
        public virtual int ExtId { get; set; }

        public void DeleteCategory(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                //load entity
                CategoryForImport category = db.CategoryForImports.Find(id);
                //load children's id
                List<Guid> childrenIds = db.CategoryForImports.Where(c => c.CategoryId == id).Select(c => c.Id).ToList();

                Guid NoCategoryId = db.CategoryForImports.FirstOrDefault(c => c.Title == "Без категории").Id;

                if (NoCategoryId == null)
                {
                    CategoryForImport NoCategory = new CategoryForImport()
                    {
                        Id = Guid.NewGuid(),
                        Title = "Без категории",
                        CategoryId = null,
                        Children = new List<CategoryForImport>(),
                        ExtId = -1,
                        OuterKey = "",
                        Weight = 0,
                        Parent = new CategoryForImport()
                    };
                    db.CategoryForImports.Add(NoCategory);
                    db.SaveChanges();
                }

                //iterate children
                foreach (Guid childId in childrenIds)
                {
                    IEnumerable<ProductForImport> producsts = db.ProductForImports.Where(p => p.Id == childId).ToList();
                    foreach (ProductForImport producst in producsts)
                    {
                        producst.CategoryId = NoCategoryId;
                    }


                    new CategoryForImport().DeleteCategory(id);
                }

                //delete self and save
                db.CategoryForImports.Remove(db.CategoryForImports.Find(id));
                db.SaveChanges();
            }
        }

        public CategoryForImport EditJoin(CategoryForImport category)
        {
            this.Title = category.Title;
            return this;
        }

        public List<CategoryForImport> EditCategorys(List<CategoryForImport> ImportCategorys, List<CategoryForImport> CategorysProgram, KoreaContext db)
        {
                
                List<CategoryForImport> categorysEdit = CategorysProgram.Join(ImportCategorys,
                                                                             c => c.Title,
                                                                             i => i.Title,
                                                                             (i, c) => i.EditJoin(c))
                                                                        .ToList();
                db.SaveChanges();
                return categorysEdit;
        }

        public void CreateCategorys(List<CategoryForImport> ImportCategorys, List<CategoryForImport> CategorysProgram)
        {
            using (KoreaContext db = new KoreaContext())
            {
                List<CategoryForImport> categorysCreate = ImportCategorys.Where(i => CategorysProgram.FirstOrDefault(c => c.Title == i.Title) == null)
                                                                             .ToList();
                foreach (CategoryForImport categoryCreate in categorysCreate)
                {
                    db.CategoryForImports.Add(categoryCreate);
                    
                }
                db.SaveChanges();
                
            }
        }

        public void DeleteCategorys(List<CategoryForImport> ImportCategorys, List<CategoryForImport> CategorysProgram)
        {
            using (KoreaContext db = new KoreaContext())
            {
                List<CategoryForImport> categorysDelete = CategorysProgram.Where(c => ImportCategorys.FirstOrDefault(i => i.Title == c.Title) == null)
                                                                  .ToList();
                CategoryForImport NotCategory = db.CategoryForImports.FirstOrDefault(c => c.Title == "Без категории");
                categorysDelete.Remove(NotCategory);
                foreach (CategoryForImport categoryDelete in categorysDelete)
                {
                    PereDel(categoryDelete.Id, db, NotCategory.Id);
                }
            }
        }

        private void PereDel(Guid id, KoreaContext db, Guid idNotCategory)
        {
            //load entity
            CategoryForImport category = db.CategoryForImports.Find(id);
            //load children's id
            List<Guid> childrenIds = db.CategoryForImports.Where(c => c.CategoryId == id).Select(c => c.Id).ToList();

            //iterate children
            foreach (Guid childId in childrenIds)
            {
                IEnumerable<ProductForImport> producsts = db.ProductForImports.Where(p => p.Id == childId).ToList();
                foreach (ProductForImport producst in producsts)
                {
                    producst.CategoryId = idNotCategory;
                }


                PereDel(childId, db, idNotCategory);
            }

            //delete self and save
            db.CategoryForImports.Remove(category);
            db.SaveChanges();
        }
                
    }
}