using Korea.Models;
using Korea.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Korea.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private KoreaContext db = new KoreaContext();

        public ActionResult Index()
        {
            //IQueryable<ProductForImport> products = db.ProductForImports.Include("Category").Include("Supplier");
            ////.Products.Include(p => p.Car).Include(p => p.Category).Include(p => p.Supplier);
            //ICollection<ProductForImport> product = products
            //        .OrderBy(c => c.Сode)
            //        .ToList();

            return RedirectToAction("Index", "Product");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}