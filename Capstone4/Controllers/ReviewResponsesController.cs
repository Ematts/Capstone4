using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Capstone4.Models;

namespace Capstone4.Controllers
{
    public class ReviewResponsesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ReviewResponses
        public ActionResult Index()
        {
            var reviewResponses = db.ReviewResponses.Include(r => r.Contractor);
            return View(reviewResponses.ToList());
        }

        // GET: ReviewResponses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReviewResponse reviewResponse = db.ReviewResponses.Find(id);
            if (reviewResponse == null)
            {
                return HttpNotFound();
            }
            return View(reviewResponse);
        }

        // GET: ReviewResponses/Create
        public ActionResult Create()
        {
            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId");
            return View();
        }

        // POST: ReviewResponses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Review,ResponseDate,ContractorID")] ReviewResponse reviewResponse)
        {
            if (ModelState.IsValid)
            {
                db.ReviewResponses.Add(reviewResponse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId", reviewResponse.ContractorID);
            return View(reviewResponse);
        }

        // GET: ReviewResponses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReviewResponse reviewResponse = db.ReviewResponses.Find(id);
            if (reviewResponse == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId", reviewResponse.ContractorID);
            return View(reviewResponse);
        }

        // POST: ReviewResponses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Review,ResponseDate,ContractorID")] ReviewResponse reviewResponse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reviewResponse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId", reviewResponse.ContractorID);
            return View(reviewResponse);
        }

        // GET: ReviewResponses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReviewResponse reviewResponse = db.ReviewResponses.Find(id);
            if (reviewResponse == null)
            {
                return HttpNotFound();
            }
            return View(reviewResponse);
        }

        // POST: ReviewResponses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ReviewResponse reviewResponse = db.ReviewResponses.Find(id);
            db.ReviewResponses.Remove(reviewResponse);
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
