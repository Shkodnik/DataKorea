using Korea.Models;
using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Controllers
{
    [Authorize]
    public class GenerationController : Controller
    {
        //
        // GET: /Generation/
        public ActionResult Index()
        {
            using (KoreaContext db = new KoreaContext())
            {
                IQueryable<GenerationForImport> IQueryableGenerations = db.GenerationForImports.Include("Model");
                ICollection<GenerationForImport> generations = IQueryableGenerations.OrderBy(p => p.Weight)
                                                                       .ThenBy(p => p.Model)
                                                                       .ThenBy(p => p.Title)
                                                                       .ToList();
                return View(generations);
            }
        }


        

        //
        // GET: /Generation/Create
        public ActionResult Create()
        {
            FillModels();
            return View(new GenerationForImport()
            {
                Weight = 0
            });
        }

        //
        // POST: /Generation/Create
        [HttpPost]
        public ActionResult Create(GenerationForImport Generation)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    if (db.GenerationForImports.Where(g => g.Title == Generation.Title
                                                      && g.ModelId == Generation.ModelId)
                                               .Count() == 0)
                    {
                        Generation.Id = Guid.NewGuid();
                        db.GenerationForImports.Add(Generation);
                        db.SaveChanges();
                    }
                    else {
                        throw new Exception(); 
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                FillModels();
                return View(Generation);
            }
        }

        //
        // GET: /Generation/Edit/5
        public ActionResult Edit(Guid id)
        {
            FillModels();
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.GenerationForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }

        //
        // POST: /Generation/Edit/5
        [HttpPost]
        public ActionResult Edit(GenerationForImport generation)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    GenerationForImport Generation = db.GenerationForImports
                                                       .Single(g => g.Id == generation.Id);

                    if (Generation.Title != generation.Title
                        || Generation.ModelId != generation.ModelId) 
                    {
                        if (db.GenerationForImports.Where(g => g.Title == generation.Title
                                                      && g.ModelId == generation.ModelId)
                                               .Count() == 0)
                        {
                            Generation.Edit(generation);
                            db.SaveChanges();
                        }
                        else
                        {
                            throw new Exception();
                        }  
                    }
                    else
                    {
                        Generation.Weight = generation.Weight;
                        db.SaveChanges();
                    }
                                  
                }
                return RedirectToAction("Index");
            }
            catch
            {
                FillModels();
                return View(generation);
            }
        }

        //
        // GET: /Generation/Delete/5
        public ActionResult Delete(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.GenerationForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }

        //
        // POST: /Generation/Delete/5
        [HttpPost]
        public ActionResult DeletePost(Guid id)
        {
            try
            {
                new GenerationForImport().DeleteGeneration(id);                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        /// <summary>
        /// Подключение к вьюхам моделей
        /// </summary>
        private void FillModels()
        {
            using (KoreaContext db = new KoreaContext())
            {
                ViewData["ModelId"] = new SelectList(
                    db.ModelForImports
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.Title)
                    .ToList()
                    , "Id", "Title");
            }
        }
    }
}
