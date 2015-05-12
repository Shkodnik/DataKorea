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
    public class BrandController : Controller
    {
        /// <summary>
        /// Get list of Brands
        /// </summary>
        /// <param name="id">Parent Brand id or null if root</param>
        public ActionResult Index()
        {
            using (KoreaContext db = new KoreaContext())
            {
                ICollection<BrandForImport> brands = db.BrandForImports.OrderBy(p => p.Weight)
                                                                       .ThenBy(p => p.Title)
                                                                       .ToList();
                return View(brands);
            }
        }

        /// <summary>
        /// Create Brand stub
        /// </summary>
        public ActionResult Create()
        {
            return View(new BrandForImport() { 
                Weight = 0
            });
        }

        /// <summary>
        /// Validate and save Brand data
        /// </summary>
        /// <param name="brand"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(BrandForImport brand)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    if (db.BrandForImports.Where(b => b.Abbreviation == brand.Abbreviation
                                                 || b.Title == brand.Title)
                                          .Count() != 0)
                    {
                        return View();
                    }
                    brand.Id = Guid.NewGuid();
                    db.BrandForImports.Add(brand);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Get Brand for edit
        /// </summary>
        /// <param name="id">Brand's Id</param>
        public ActionResult Edit(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.BrandForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }

        /// <summary>
        /// Validate and update Brand
        /// </summary>
        /// <param name="brand">Brand for update</param>
        [HttpPost]
        public ActionResult Edit(BrandForImport brand)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    db.BrandForImports.Attach(brand);
                    db.Entry(brand).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Make delete question
        /// </summary>
        /// <param name="id">Brand's Id for delete</param>
        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.BrandForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }



        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            try
            {
                Guid id = new Guid(form["Id"]);
                new BrandForImport().DeleteBrand(id);

                

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}