using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Library.Models;

namespace Library.Controllers
{
    public class BiographiesController : Controller
    {
        private Learning_1Entities db = new Learning_1Entities();

        // GET: Biographies
        public ActionResult Index()
        {
            var biographies = db.Biographies.Include(b => b.Author);
            return View(biographies.ToList());
        }

        // GET: Biographies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Biography biography = db.Biographies.Find(id);
            if (biography == null)
            {
                return HttpNotFound();
            }
            return View(biography);
        }

        // GET: Biographies/Create
        public ActionResult Create()
        {
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorId", "AuthorName");
            return View();
        }

        // POST: Biographies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BiographyID,AuthorID,BiographyText")] Biography biography)
        {
            if (ModelState.IsValid)
            {
                db.Biographies.Add(biography);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorId", "AuthorName", biography.AuthorID);
            return View(biography);
        }

        // GET: Biographies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Biography biography = db.Biographies.Find(id);
            if (biography == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorId", "AuthorName", biography.AuthorID);
            return View(biography);
        }

        // POST: Biographies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BiographyID,AuthorID,BiographyText")] Biography biography)
        {
            if (ModelState.IsValid)
            {
                db.Entry(biography).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorID = new SelectList(db.Authors, "AuthorId", "AuthorName", biography.AuthorID);
            return View(biography);
        }

        // GET: Biographies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Biography biography = db.Biographies.Find(id);
            if (biography == null)
            {
                return HttpNotFound();
            }
            return View(biography);
        }

        // POST: Biographies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Biography biography = db.Biographies.Find(id);
            db.Biographies.Remove(biography);
            db.SaveChanges();
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
    }
}
