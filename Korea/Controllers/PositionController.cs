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
    public class PositionController : Controller
    {
        private KoreaContext db = new KoreaContext();

        // GET: /Position/
        public ActionResult Index()
        {
            return View(db.PositionForImports.OrderBy(p => p.Weight)
                                             .ThenBy(p => p.Title)
                                             .ToList());
        }

        //// GET: /Position/Create
        //public ActionResult Create()
        //{
        //    return View(new PositionForImport()
        //    {
        //         Weight = 0
        //    });
        //}

        // POST: /Position/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Weight")] PositionForImport positionforimport)
        {
            if (ModelState.IsValid)
            {
                string titleTrim = positionforimport.Title
                                                .Trim(' ', ' ');
                PositionForImport temp = db.PositionForImports.FirstOrDefault(p => p.Title == titleTrim);

                if (temp == null)
                {
                    positionforimport.Title = titleTrim;
                    positionforimport.Id = Guid.NewGuid();
                    db.PositionForImports.Add(positionforimport);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(positionforimport);
        }

        // GET: /Position/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PositionForImport positionforimport = db.PositionForImports.Find(id);
            if (positionforimport == null)
            {
                return HttpNotFound();
            }
            return View(positionforimport);
        }

        // POST: /Position/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Weight")] PositionForImport positionforimport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(positionforimport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(positionforimport);
        }

        // GET: /Position/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PositionForImport positionforimport = db.PositionForImports.Find(id);
            if (positionforimport == null)
            {
                return HttpNotFound();
            }
            return View(positionforimport);
        }

        // POST: /Position/Delete/5
        [HttpPost]
        public ActionResult DeletePost(Guid id)
        {
            try
            {
                new PositionForImport().DeletePosition(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    PositionForImport positionforimport = db.PositionForImports.Find(id);
        //    db.PositionForImports.Remove(positionforimport);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
