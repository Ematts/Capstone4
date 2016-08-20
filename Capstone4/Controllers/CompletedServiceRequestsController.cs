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
    public class CompletedServiceRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CompletedServiceRequests
        public ActionResult Index()
        {
            var completedServiceRequests = db.CompletedServiceRequests.Include(c => c.ServiceRequest);
            return View(completedServiceRequests.ToList());
        }

        // GET: CompletedServiceRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompletedServiceRequest completedServiceRequest = db.CompletedServiceRequests.Find(id);
            if (completedServiceRequest == null)
            {
                return HttpNotFound();
            }
            return View(completedServiceRequest);
        }

        // GET: CompletedServiceRequests/Create
        public ActionResult Create()
        {
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description");
            return View();
        }

        // POST: CompletedServiceRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ServiceRequestID,CompletionDate,AmountDue,ContractorPaid")] CompletedServiceRequest completedServiceRequest)
        {
            if (ModelState.IsValid)
            {
                db.CompletedServiceRequests.Add(completedServiceRequest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description", completedServiceRequest.ServiceRequestID);
            return View(completedServiceRequest);
        }

        // GET: CompletedServiceRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompletedServiceRequest completedServiceRequest = db.CompletedServiceRequests.Find(id);
            if (completedServiceRequest == null)
            {
                return HttpNotFound();
            }
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description", completedServiceRequest.ServiceRequestID);
            return View(completedServiceRequest);
        }

        // POST: CompletedServiceRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ServiceRequestID,CompletionDate,AmountDue,ContractorPaid")] CompletedServiceRequest completedServiceRequest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(completedServiceRequest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description", completedServiceRequest.ServiceRequestID);
            return View(completedServiceRequest);
        }

        // GET: CompletedServiceRequests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompletedServiceRequest completedServiceRequest = db.CompletedServiceRequests.Find(id);
            if (completedServiceRequest == null)
            {
                return HttpNotFound();
            }
            return View(completedServiceRequest);
        }

        // POST: CompletedServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompletedServiceRequest completedServiceRequest = db.CompletedServiceRequests.Find(id);
            db.CompletedServiceRequests.Remove(completedServiceRequest);
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
