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
    public class SupplierController : Controller
    {
        /// <summary>
        /// Get list of Suppliers
        /// </summary>
        /// <param name="id">Parent Supplier id or null if root</param>
        public ActionResult Index()
        {
            using (KoreaContext db = new KoreaContext())
            {
                ICollection<SupplierForImport> suppliers = db.SupplierForImports.OrderBy(p => p.Weight)
                                                                               .ThenBy(p => p.Title)
                                                                               .ToList();
                return View(suppliers);
            }
        }

        /// <summary>
        /// Entire Supplier store cache
        /// </summary>
        private List<SupplierForImport> suppliers = null;


        /// <summary>
        /// Fill entire Supplier cache
        /// </summary>
        private void FillSupplierCache()
        {
            using (KoreaContext db = new KoreaContext())
            {
                //store categories to cache
                suppliers = db.SupplierForImports
                    .OrderBy(b => b.Weight)
                    .ThenBy(b => b.Title)
                    .ToList();

            }
        }

        /// <summary>
        /// Create Supplier stub
        /// </summary>
        public ActionResult Create()
        {
            return View(new SupplierForImport()
                    {
                        Weight = 0
                    });
        }

        /// <summary>
        /// Validate and save Supplier data
        /// </summary>
        /// <param name="supplier"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(SupplierForImport supplier)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    supplier.Id = Guid.NewGuid();
                    db.SupplierForImports.Add(supplier);
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
        /// Get Supplier for edit
        /// </summary>
        /// <param name="id">Supplier's Id</param>
        public ActionResult Edit(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.SupplierForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }

        /// <summary>
        /// Validate and update Supplier
        /// </summary>
        /// <param name="supplier">Supplier for update</param>
        [HttpPost]
        public ActionResult Edit(SupplierForImport supplier)
        {
            try
            {
                using (KoreaContext db = new KoreaContext())
                {
                    db.SupplierForImports.Attach(supplier);
                    db.Entry(supplier).State = EntityState.Modified;
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
        /// <param name="id">Supplier's Id for delete</param>
        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            using (KoreaContext db = new KoreaContext())
            {
                return View(
                    db.SupplierForImports.FirstOrDefault(b => b.Id == id)
                    );
            }
        }



        [HttpPost]
        public ActionResult Delete(FormCollection form)
        {
            try
            {
                Guid id = new Guid(form["Id"]);

                using (KoreaContext db = new KoreaContext())
                {


                    //load entity
                    SupplierForImport supplier = db.SupplierForImports.Find(id);


                    //delete self and save
                    db.SupplierForImports.Remove(db.SupplierForImports.Find(id));
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}