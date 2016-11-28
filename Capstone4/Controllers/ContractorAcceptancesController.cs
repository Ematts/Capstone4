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
using System.Net.Mail;

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
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }

            if(contractorAcceptance.ServiceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(contractorAcceptance);
        }

        public ActionResult Already_Accepted(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }

            if (contractorAcceptance.Contractor.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(contractorAcceptance);
        }

        public ActionResult Already_Confirmed_Contractor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }
            if (contractorAcceptance.ServiceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(contractorAcceptance);
        }

        public ActionResult Already_Confirmed_This_Contractor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorAcceptance contractorAcceptance = db.ContractorAcceptances.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (contractorAcceptance == null)
            {
                return HttpNotFound();
            }

            if (contractorAcceptance.ServiceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(contractorAcceptance);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                            if (a.ServiceRequest.ContractorID != null && a.ServiceRequest.ContractorID == a.ContractorID)
                            {
                                return RedirectToAction("Already_Confirmed_This_Contractor", new { id = a.ID });
                            }

                            if (a.ServiceRequest.ContractorID != null)
                            {
                                return RedirectToAction("Already_Confirmed_Contractor", new { id = a.ID });
                            }
                            a.ServiceRequest.Contractor = a.Contractor;
                            SendContractor(a);
                        }
                    }
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", "ServiceRequests");
        }

        public void SendContractor(ContractorAcceptance contractorAcceptance)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(contractorAcceptance.ServiceRequest.Contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Homeowner Confirmed Your Service!!";
            string url = "http://localhost:37234/ServiceRequests/ConfirmCompletionView/" + contractorAcceptance.ServiceRequest.ID;
            string url2 = "http://localhost:37234/Maps/Calculate/" + contractorAcceptance.ServiceRequest.ID;
            string message = "Hello " + contractorAcceptance.ServiceRequest.Contractor.FirstName + "," + "<br>" + "<br>" + contractorAcceptance.ServiceRequest.Homeowner.Username + " has confirmed your service for the following request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + contractorAcceptance.ServiceRequest.Address.Street + "<br>" + contractorAcceptance.ServiceRequest.Address.City + "<br>" + contractorAcceptance.ServiceRequest.Address.State + "<br>" + contractorAcceptance.ServiceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + contractorAcceptance.ServiceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + contractorAcceptance.ServiceRequest.Price + "<br>" + "<br>" + "Must be completed by: <br>" + contractorAcceptance.ServiceRequest.CompletionDeadline + "<br>" + "<br>" + "Date Posted: <br>" + contractorAcceptance.ServiceRequest.PostedDate + "<br>" + "<br>" + "When the job is complete, please confirm completion by clicking on the link below: <br><a href =" + url + "> Click Here </a>" + "<br>" + "<br>" + "Get directions by clicking on the link below: <br><a href =" + url2 + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);



        }
    }
}


