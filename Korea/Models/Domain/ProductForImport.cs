using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Models.Domain
{
    public class ProductForImport : BaseEntity
    {
        [MaxLength(256)]
        [Index]
        [Required]
        [Display(Name = "Название", Order = 100)]
        public virtual string Name { get; set; }

        [MaxLength(16)]
        [Index]
        [Required]
        [Display(Name = "Код", Order = 105)]
        public virtual string Сode { get; set; }


        public virtual CategoryForImport Category { get; set; }

        [ForeignKey("Category")]
        [Display(Name = "Категория", Order = 110)]
        public Guid CategoryId { get; set; }

        public virtual ICollection<GenerationForImport> Generations { get; set; }

        [Display(Name = "Описание", Order = 130)]
        public virtual string Description { get; set; }


        public SupplierForImport Supplier { get; set; }


        [ForeignKey("Supplier")]
        [Display(Name = "Поставщик", Order = 140)]
        public Guid? SupplierId { get; set; }

        [Display(Name = "Количество", Order = 145)]
        public virtual int Tale { get; set; }

        [Display(Name = "Позиция", Order = 150)]
        public virtual ICollection<PositionForImport> Positions { get; set; }

        [Display(Name = "Цена", Order = 155)]
        public virtual double Cost { get; set; }

        [Required]
        [Index]
        public virtual int ExtId { get; set; }

        public void DeleteProduct(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                ProductForImport product = db.ProductForImports.Find(id);
                //List<GenerationForImport> modelsProduct = product.Generations.ToList();
                //foreach (GenerationForImport link in modelsProduct)
                //{
                //    product.Generations.Remove(link);
                //}
                List<PositionForImport> positionProduct = product.Positions.ToList();
                foreach (PositionForImport link in positionProduct)
                {
                    new PositionForImport().DeletePosition(link.Id);
                }
                db.ProductForImports.Remove(product);
                db.SaveChanges();
            }
        }

        public ProductForImport SetCostTale(double cost, int tale)
        {
            this.Cost = cost;
            this.Tale = tale;

            return this;
        }
        public ProductForImport SetAll(ProductForImport Product)
        {
            this.CategoryId = Product.CategoryId;
            this.Name = Product.Name;
            this.SupplierId = Product.SupplierId;
            //List<GenerationForImport> GenerationCreate = this.Generations.Where(m1 => !Product.Generations
            //                                                                              .Select(m2 => m2.Id)
            //                                                                              .Contains(m1.Id))
            //                                                         .ToList();
            //List<GenerationForImport> GenerationDelete = Product.Generations.Where(m1 => !this.Generations
            //                                                                              .Select(m2 => m2.Id)
            //                                                                              .Contains(m1.Id))
            //                                                .ToList();
            //foreach (GenerationForImport Generation in GenerationCreate)
            //{
            //    this.Generations.Add(Generation);
            //}
            //foreach (GenerationForImport Generation in GenerationDelete)
            //{
            //    this.Generations.Remove(Generation);
            //}

            return this;
        }
        public ProductForImport FporIndex(Dictionary<Guid, string> ModelDic)
        {
            foreach (GenerationForImport Generation in this.Generations)
            {
                if (Generation.ModelId != null)
                {
                    string model = ModelDic.Single(g => g.Key == Generation.ModelId).Value;
                    if (Generation.Title.IndexOf(model) == -1)
                    {
                        Generation.Title = model + " > " + Generation.Title;
                    }
                }
                
            }
            return this;
        }

        public List<ProductForImport> ListPropertys(string Name)
        {
            List<ProductForImport> ProductsProgram = new List<ProductForImport>();

            using (KoreaContext db2 = new KoreaContext())
            {
                //temp = saveProd.Select(p => p.IdProgProd)
                //                          .ToList();

                switch (Name)
                {
                    case "Поколение":
                        ProductsProgram = db2.ProductForImports.Include("Generations")
                                                       .Where(p => p.Generations.Count() != 0)
                            //.Where(p => temp.Contains(p.Id))
                                                       .ToList();
                        break;
                    case "Позиция":
                        ProductsProgram = db2.ProductForImports.Include("Positions")
                                                       .Where(p => p.Positions.Count() != 0)
                            //.Where(p => temp.Contains(p.Id))
                                                       .ToList();
                        break;
                }

            }

            return ProductsProgram;
        }

        public Dictionary<Guid, Guid> DicBrand()
        {
            Dictionary<Guid, Guid> Dic = new Dictionary<Guid, Guid>();
            using (KoreaContext db2 = new KoreaContext())
            {
                Dic = db2.ProductForImports.Include("Supplier")
                                           .Where(p => p.SupplierId != null)
                    //.Where(p => temp.Contains(p.Id))
                                           .ToDictionary(p => p.Id, p => p.SupplierId.GetValueOrDefault());
            }
            return Dic;
        }

        
        //public ProductForImport OrderByGeneration()
        //{
        //    List<GenerationForImport> Generations = this.Generations.ToList();
        //    this.Generations = Generations.OrderBy(g => g.Weight).ToList();
        //    return this;
        //}
    }
}