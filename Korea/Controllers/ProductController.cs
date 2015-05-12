using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Korea.Models.Domain;
using Korea.Models;

namespace Korea.Controllers
{
    //[Authorize]
    public class ProductController : Controller
    {
        private KoreaContext db = new KoreaContext();
        private ProductForImport CerateProductGL = new ProductForImport();

        // GET: /Product/
        public ActionResult Index()
        {
            Dictionary<Guid, string> ModelDic = db.ModelForImports.ToDictionary(m => m.Id, m=> m.Title);
            ICollection<ProductForImport> products = db.ProductForImports.Include("Category")
                                                                        .Include("Supplier")
                                                                        .Include("Generations")
                                                                        .ToList();
            //.Products.Include(p => p.Car).Include(p => p.Category).Include(p => p.Supplier);
            ICollection<ProductForImport> product = products
                    .OrderBy(c => c.Сode)
                    .Take(100)
                    .Select(p => p.FporIndex(ModelDic))
                    //
                    ////.Where(c => c.GenerationId != null)
                    //
                    .ToList();
            //ProductForImport temp = product.Where(p => p.Id)
            return View(product);

        }

        public ActionResult Scroll(Guid id)
        {
            return View();
        }

        public ActionResult PIndex(ProductForImport Product)
        {
            using (KoreaContext db = new KoreaContext())
            {
                
                return PartialView("PIndex", Product.Generations.ToList());
            }
        }

        public ActionResult PIndexPosition(ProductForImport Product)
        {
            using (KoreaContext db = new KoreaContext())
            {
                IEnumerable<PositionForImport> Position = db.ProductForImports.FirstOrDefault(p => p.Id == Product.Id).Positions;
                return PartialView("PIndexPosition", Position);
            }
        }

        public ActionResult ErrorList()
        {
            var model = TempData["list"] as List<string>;
            return View(model);
        }

        public ActionResult addGeneration(Guid productId, Guid GenerationId, string Views)
        {
            db.ProductForImports
                .Single(p => p.Id == productId)
                .Generations
                .Add(
                    db.GenerationForImports
                    .Single(m => m.Id == GenerationId)
                    );
            db.SaveChanges();
            return RedirectToAction(Views, new { id = productId });
        }


        public ActionResult DeleteGeneration(Guid productId, Guid generationId, string Views)
        {
            using (KoreaContext db = new KoreaContext())
            {
                db.ProductForImports
                    .Single(p => p.Id == productId)
                    .Generations
                    .Remove(
                        db.GenerationForImports
                        .Single(m => m.Id == generationId)
                        );
                db.SaveChanges();
            }
            //return RedirectToAction("Edit", new { id = productId });
            return RedirectToAction(Views, new { id = productId });
        }



        //public GenerationForImport GetGeneration(int? FromYearP, int? ToYearP)
        //{
        //    GenerationForImport TempGeneration = db.GenerationForImports.FirstOrDefault(g => g.ToYear == ToYearP
        //                                                                    && g.FromYear == FromYearP);
        //    if (TempGeneration == null)
        //    {
        //        TempGeneration = new GenerationForImport()
        //        {
        //            Id = Guid.NewGuid(),
        //            FromYear = FromYearP,
        //            ToYear = ToYearP
        //        };
        //        TempGeneration.Name = TempGeneration.ToName();
        //        db.GenerationForImports.Add(TempGeneration);
        //        db.SaveChanges();
        //    }
        //    if (TempGeneration.Id == Guid.Empty)
        //        throw new InvalidOperationException("Id is empty");
        //    return TempGeneration;
        //}


        // GET: /Product/Create
        public ActionResult Create()
        {
            FillLists();
            CerateProductGL = new ProductForImport();
            return View(new ProductForImport()
            );
        }

        //private void FillLists2()
        //{
        //    using (KoreaContext db = new KoreaContext())
        //    {
        //        ViewData["CategoryId"] = new SelectList(
        //            db.CategoryForImports
        //            .OrderBy(b => b.Weight)
        //            .ThenBy(b => b.Title)
        //            .ToList()
        //            , "Id", "Title");
        //    }
        //}


        private void FillLists()
        {
            using (KoreaContext db = new KoreaContext())
            {
                ViewData["CategoryId"] = new SelectList(
                    db.CategoryForImports
                    .Where(c => c.CategoryId != null)
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.Title)
                    .ToList()
                    , "Id", "Title");
                ViewData["SupplierId"] = new SelectList(
                    db.SupplierForImports
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.Title)
                    .ToList()
                    , "Id", "Title");
                ViewData["PositionId"] = new SelectList(
                    db.PositionForImports
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.Title)
                    .ToList()
                    , "Id", "Title");
                ViewData["GenerationsId"] = new SelectList(
                    db.GenerationForImports
                    .Include("Model")
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.ModelId)
                    .Select(m => new {Id = m.Id, Title = m.Model.Title + " > " + m.Title })
                    .ToList()
                    , "Id", "Title");
            }
        }

        // POST: /Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(ProductForImport product)
        //[Bind(Include = "Id,Name,Сode,CategoryId,CarId,Description,SupplierId,Position")]
        {

            try
            {
                //    List<GenerationForImport> Generations = db.GenerationForImports.ToList();
                //    GenerationForImport TempGeneration = Generations.FirstOrDefault(g => g.ToYear == product.Generation.ToYear
                //                                                                    && g.FromYear == product.Generation.FromYear);
                //    if (TempGeneration != null)
                //    {
                //        product.GenerationId = TempGeneration.Id;
                //        product.Generation = TempGeneration;
                //    }
                //    else
                //    {
                //        TempGeneration = new GenerationForImport()
                //        {
                //            Id = Guid.NewGuid(),
                //            FromYear = product.Generation.FromYear,
                //            ToYear = product.Generation.ToYear
                //        };
                //        TempGeneration.Name = TempGeneration.ToName();
                //        db.GenerationForImports.Add(TempGeneration);
                //        product.GenerationId = TempGeneration.Id;
                //    }
                
                product.Id = Guid.NewGuid();
                product.ExtId = -1;
                

                db.ProductForImports.Add(product);
                db.SaveChanges();

                return RedirectToAction("CreateStep2", new { id = product.Id });
            }
            catch
            {
                FillLists();
                return View(product);
            }
        }


        public ActionResult CreateStep2(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductForImport product = db.ProductForImports
                                         .Include("Generations").FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            FillLists();
            return View(product);
        }

        public ActionResult addPosition(Guid productId, string PositionString, string Views)
        {
            string PositionStringTrim = PositionString.Trim(' ',' ');
            ProductForImport product = db.ProductForImports
                                         .Single(p => p.Id == productId);
            PositionForImport PositionJoin = db.PositionForImports.FirstOrDefault(p => p.Title == PositionStringTrim);
            if (PositionJoin == null)
            {
                PositionForImport Position = new PositionForImport()
                {
                    Id = Guid.NewGuid(),
                    Title = PositionStringTrim,
                };
                Position.Products = new List<ProductForImport>();
                Position.Products.Add(product);
                db.PositionForImports.Add(Position);
            }
            else
            {
                TempData["ValidationPosition"] = "Такая позищия уже существует";
            }

            //db.ProductForImports
            //    .Single(p => p.Id == productId)
            //    .Positions
            //    .Add(
            //        db.PositionForImports
            //        .Single(m => m.Id == Position)
            //        );
            db.SaveChanges();
            return RedirectToAction(Views, new { id = productId });
        }


        public ActionResult DeletePosition(Guid productId, Guid Position, string Views)
        {
            using (KoreaContext db = new KoreaContext())
            {
                //db.ProductForImports
                //    .Single(p => p.Id == productId)
                //    .Positions
                //    .Remove(
                //            db.PositionForImports
                //            .Single(m => m.Id == Position)
                //            );
                db.PositionForImports.Remove(
                                             db.PositionForImports
                                             .Single(m => m.Id == Position)
                                            );
                db.SaveChanges();
            }
            return RedirectToAction(Views, new { id = productId });
        }


        // GET: /Product/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductForImport product = db.ProductForImports.Find(id);
                                         //.Include("Generations")
                                         //.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            FillLists();
            return View(product);
        }

        // POST: /Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Сode,CategoryId,Generation,Description,SupplierId,Position")] ProductForImport product)
        {
            if (ModelState.IsValid)
            {

                //db.ProductForImports.Attach(product);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.CategoryForImports.OrderBy(c => c.Weight), "Id", "OuterKey", product.CategoryId);
            ViewBag.SupplierId = new SelectList(db.SupplierForImports.OrderBy(s => s.Weight), "Id", "Title", product.SupplierId);
            return View(product);
        }

        // GET: /Product/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductForImport product = db.ProductForImports.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            new ProductForImport().DeleteProduct(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Search(string СodeN, string NameN, string CategoryN, string GenerationN, string PositionN)
        {
            IQueryable<ProductForImport> products = db.ProductForImports
                                                               .Include("Category")
                                                               .Include("Supplier")
                                                               .Include("Generations");
            //.Products.Include(p => p.Car).Include(p => p.Category).Include(p => p.Supplier);
            ICollection<ProductForImport> product = products.ToList();
            if (СodeN != "")
            {
                СodeN = СodeN.Trim(' ', ' ');
                product = product.Where(p => p.Сode.ToLower()
                             .Contains(СodeN.ToLower()))
                             .ToList();
            }
            if (NameN != "")
            {
                NameN = NameN.Trim(' ', ' ');
                product = product.Where(p => p.Name.ToLower()
                                 .Contains(NameN.ToLower()))
                                 .ToList();
            }
            if (CategoryN != "")
            {
                CategoryN = CategoryN.Trim(' ', ' ');
                product = product.Where(p => p.Category.Title.ToLower()
                                 .Contains(CategoryN.ToLower()))
                                 .ToList();
            }
            if (GenerationN != "")
            {
                GenerationN = GenerationN.Trim(' ', ' ');
                product = product.Where(p => p.Generations
                                              .Where(m => m.Title.ToLower()
                                                                 .Contains(GenerationN.ToLower()))
                                              .Count() != 0)
                                 .ToList();
            }
            if (PositionN != "")
            {
                PositionN = PositionN.Trim(' ', ' ');
                product = product.Where(p => p.Positions
                                              .Where(p2 => p2.Title.ToLower()
                                                                   .Contains(PositionN.ToLower()))
                                              .Count() != 0)
                                 .ToList();
            }

            Dictionary<Guid, string> ModelDic = db.ModelForImports.ToDictionary(m => m.Id, m => m.Title);
            product = product
                    .OrderBy(c => c.Сode)
                    .Select(p => p.FporIndex(ModelDic))
                    .ToList();
            return View("Index", product);
        }
    }
}
