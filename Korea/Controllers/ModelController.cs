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
    public class ModelController : Controller
    {
        /// <summary>
        /// Get list of Cars
        /// </summary>
        /// <param name="id">Parent Car id or null if root</param>
        public ActionResult Index()
        {
            using (KoreaContext db = new KoreaContext())
            {
                IQueryable<ModelForImport> IQueryableModels = db.ModelForImports.Include("Brand");
                ICollection<ModelForImport> models = IQueryableModels.OrderBy(p => p.Weight)
                                                                       .ThenBy(p => p.Title)
                                                                       .ToList(); ;
                return View(models);
            }
        }
        

        

       
        /// <summary>
        /// Create Model stub
        /// </summary>
        public ActionResult Create()
        {
            FillBrand();
            return View(new ModelForImport()
            {
                Weight = 0
            });
        }

        /// <summary>
        /// Fill internal brands selected list
        /// </summary>
        private void FillBrand()
        {
            using (KoreaContext db = new KoreaContext())
            {
                ViewData["BrandId"] = new SelectList(
                    db.BrandForImports
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.Title)
                    .ToList()
                    , "Id", "Title");
            }
        }


        /// <summary>
        /// Validate and save Car data
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(ModelForImport model)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    model.Id = Guid.NewGuid();
                    db.ModelForImports.Add(model);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                FillBrand();
                return View(model);
            }
        }

        /// <summary>
        /// Get Car for edit
        /// </summary>
        /// <param name="id">Car's Id</param>
        public ActionResult Edit(Guid id)
        {
            FillBrand();
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.ModelForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }

        /// <summary>
        /// Validate and update Car
        /// </summary>
        /// <param name="car">Car for update</param>
        [HttpPost]
        public ActionResult Edit(ModelForImport model)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    db.ModelForImports.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                FillBrand();
                return View(model);
            }
        }

        /// <summary>
        /// Make delete question
        /// </summary>
        /// <param name="id">Car's Id for delete</param>
        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.ModelForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }



        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            try
            {
                Guid id = new Guid(form["Id"]);

                new ModelForImport().DeleteModel(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}