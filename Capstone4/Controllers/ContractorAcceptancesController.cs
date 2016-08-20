using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Capstone4.Models;
using Microsoft.AspNet.Identity;

namespace Capstone4.Controllers
{
    public class ContractorAcceptancesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContractorAcceptances
        public ActionResult Index()
        {
            var contractorAcceptances = db.ContractorAcceptances.Include(c => c.Contractor).Include(c => c.ServiceRequest);
            return View(contractorAcceptances.ToList());
        }

        // GET: ContractorAcceptances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }
            return View(contractorAcceptance);
        }

        // GET: ContractorAcceptances/Create
        public ActionResult Create()
        {
            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId");
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description");
            return View();
        }

        // POST: ContractorAcceptances/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ContractorID,ServiceRequestID,AcceptanceDate")] ContractorAcceptance contractorAcceptance)
        {
            if (ModelState.IsValid)
            {
                db.ContractorAcceptances.Add(contractorAcceptance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId", contractorAcceptance.ContractorID);
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description", contractorAcceptance.ServiceRequestID);
            return View(contractorAcceptance);
        }

        // GET: ContractorAcceptances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId", contractorAcceptance.ContractorID);
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description", contractorAcceptance.ServiceRequestID);
            return View(contractorAcceptance);
        }

        // POST: ContractorAcceptances/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ContractorID,ServiceRequestID,AcceptanceDate")] ContractorAcceptance contractorAcceptance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractorAcceptance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContractorID = new SelectList(db.Contractors, "ID", "UserId", contractorAcceptance.ContractorID);
            ViewBag.ServiceRequestID = new SelectList(db.ServiceRequests, "ID", "Description", contractorAcceptance.ServiceRequestID);
            return View(contractorAcceptance);
        }

        // GET: ContractorAcceptances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }
            return View(contractorAcceptance);
        }

        // POST: ContractorAcceptances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            var serviceRequests = db.ServiceRequests.ToList();
            var acceptances = db.ContractorAcceptances.ToList();
            //if (serviceRequest.Address != null)
            //{
            //    db.Addresses.Remove(serviceRequest.Address);
            //}
            foreach (var i in acceptances)
            {
                if (i.ID == id)
                {
                    i.ServiceRequest.ContractorID = null;
                }
            }
            db.ContractorAcceptances.Remove(contractorAcceptance);
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

        public ActionResult NotifyHomeownerView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }
            return View(contractorAcceptance);
        }

        public ActionResult ConfirmContractor(int id)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var contractors = db.Contractors.ToList();
            var homeowners = db.Homeowners.ToList();
            var serviceRequests = db.ServiceRequests.ToList();
            var acceptances = db.ContractorAcceptances.ToList();
            foreach (var h in homeowners)
            {
                if (h.UserId == identity)
                {
                    foreach (var a in acceptances)
                    {
                        if (a.ID == id)
                        {
                            a.ServiceRequest.Contractor = a.Contractor;
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", "ServiceRequests");
        }
    }
}


//public ActionResult Accept(int id)
//{

//    string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
//    var contractors = db.Contractors.ToList();
//    var serviceRequests = db.ServiceRequests.ToList();
//    foreach (var con in contractors)
//    {
//        if (con.UserId == identity)
//        {
//            ContractorAcceptance acceptance = new ContractorAcceptance();
//            foreach (var request in serviceRequests)
//            {
//                if (request.ID == id)
//                {
//                    acceptance.ServiceRequest = request;
//                }
//            }
//            acceptance.ContractorID = con.ID;
//            acceptance.ServiceRequestID = id;
//            acceptance.AcceptanceDate = DateTime.Now;
//            db.ContractorAcceptances.Add(acceptance);
//            db.SaveChanges();
//            NotifyAcceptance(acceptance);

//        }
//    }
//    db.SaveChanges();
//    return RedirectToAction("Index", "ContractorAcceptances");
//}