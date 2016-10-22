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
    public class AdminsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admins
        public ActionResult Index()
        {
            var admins = db.Admins.Include(a => a.ApplicationUser);
            return View(admins.ToList());
        }

        public ActionResult ManualValidate(ManualValidateViewModel model)
        {
            
            if (!this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            model.Homeowners = new List<Homeowner>();
            model.Contractors = new List<Contractor>();
            model.ServiceRequests = new List<ServiceRequest>();

            foreach(var i in db.Homeowners.ToList())
            {
                if(i.NeedsManualValidation == true)
                {
                    model.Homeowners.Add(i);
                }
            }

            foreach (var i in db.Contractors.ToList())
            {
                if (i.NeedsManualValidation == true)
                {
                    model.Contractors.Add(i);
                }
            }

            foreach (var i in db.ServiceRequests.ToList())
            {
                if (i.NeedsManualValidation == true)
                {
                    model.ServiceRequests.Add(i);
                }
            }

            return View(model);
        }

        // GET: Admins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // GET: Admins/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserId,Username,FirstName,LastName")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Admins.Add(admin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           
            return View(admin);
        }

        // GET: Admins/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            
            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,Username,FirstName,LastName")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(admin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(admin);
        }

        // GET: Admins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admin admin = db.Admins.Find(id);
            if (admin == null)
            {
                return HttpNotFound();
            }
            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Admin admin = db.Admins.Find(id);
            db.Admins.Remove(admin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ValidateAddress(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            return View(serviceRequest);
        }

        public ActionResult ManualValidateHomeowner(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homeowner homeowner = db.Homeowners.Find(id);
            if (homeowner == null)
            {
                return HttpNotFound();
            }
            return View(homeowner);
        }

        public ActionResult ManualValidateContractor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            if (contractor == null)
            {
                return HttpNotFound();
            }
            return View(contractor);
        }

        public ActionResult ValidateHomeowner(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homeowner homeowner = db.Homeowners.Find(id);
            if (homeowner == null)
            {
                return HttpNotFound();
            }

            homeowner.Address.validated = true;
            homeowner.Address.ManualValidated = true;
            homeowner.NeedsManualValidation = false;
            homeowner.Inactive = false;
            Notify_Homeowner_of_Account_Activation(homeowner);
            db.SaveChanges();
            return RedirectToAction("ManualValidate");
        }

        public ActionResult ValidateContractor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            if (contractor == null)
            {
                return HttpNotFound();
            }
            contractor.Inactive = false;
            contractor.Address.validated = true;
            contractor.Address.ManualValidated = true;
            contractor.NeedsManualValidation = false;
            Notify_Contractor_of_Account_Activation(contractor);
            db.SaveChanges();
            return RedirectToAction("ManualValidate");
        }
        public ActionResult Validate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            
            serviceRequest.Address.validated = true;
            serviceRequest.Address.ManualValidated = true;
            serviceRequest.NeedsManualValidation = false;
            Notify_Homeowner_of_Validation(serviceRequest);
            db.SaveChanges();
            return RedirectToAction("ManualValidate");
        }

        public ActionResult Invalidate(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            serviceRequest.Inactive = true;
            serviceRequest.Address.ManualValidated = false;
            serviceRequest.NeedsManualValidation = false;
            serviceRequest.Posted = false;
            Notify_Homeowner_of_Invalidation(serviceRequest);
            db.SaveChanges();
            return RedirectToAction("ManualValidate");
        }

        public ActionResult InvalidateHomeowner(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homeowner homeowner = db.Homeowners.Find(id);
            if (homeowner == null)
            {
                return HttpNotFound();
            }
            homeowner.Inactive = true;
            homeowner.Address.ManualValidated = false;
            homeowner.NeedsManualValidation = false;
            Notify_Homeowner_of_Account_Invalidation(homeowner);
            db.SaveChanges();
            return RedirectToAction("ManualValidate");
        }

        public ActionResult InvalidateContractor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            if (contractor == null)
            {
                return HttpNotFound();
            }
            contractor.Inactive = true;
            contractor.Address.ManualValidated = false;
            contractor.NeedsManualValidation = false;
            Notify_Contractor_of_Account_Invalidation(contractor);
            db.SaveChanges();
            return RedirectToAction("ManualValidate");
        }


        public void Notify_Homeowner_of_Validation(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Address validated!!";
            string url = "http://localhost:37234/ServiceRequests/ActivateView/" + serviceRequest.ID;
            string message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + "Your address for this service request has been validated.  To activate this request, please click on the link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Homeowner_of_Account_Activation(Homeowner homeowner)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Address validated!!";
            string url = "http://localhost:37234/Homeowners/Details/" + homeowner.ID;
            string message = "Hello " + homeowner.FirstName + "," + "<br>" + "<br>" + "Your address has been validated and your account activated.  To see account details, please click on the link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Contractor_of_Account_Activation(Contractor contractor)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Address validated!!";
            string url = "http://localhost:37234/Contractors/Details/" + contractor.ID;
            string message = "Hello " + contractor.FirstName + "," + "<br>" + "<br>" + "Your address has been validated and your account activated.  To see account details, please click on the link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Homeowner_of_Account_Invalidation(Homeowner homeowner)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Address validated!!";
            string url = "http://localhost:37234/Homeowners/Details/" + homeowner.ID;
            string message = "Hello " + homeowner.FirstName + "," + "<br>" + "<br>" + "Your address could not be validated.  To see account details, please click on the link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Contractor_of_Account_Invalidation(Contractor contractor)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Address validated!!";
            string url = "http://localhost:37234/Contractors/Details/" + contractor.ID;
            string message = "Hello " + contractor.FirstName + "," + "<br>" + "<br>" + "Your address could not be validated.  To see account details, please click on the link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Homeowner_of_Invalidation(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Address invalid";
            string url = "http://localhost:37234/ServiceRequests/ActivateView/" + serviceRequest.ID;
            string message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + "The address of " + serviceRequest.Address.FullAddress + " could not be validated.";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [AllowAnonymous]

        public JsonResult doesUserNameExist(string Username)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var homeownerResult = db.Homeowners.Where(x => x.Username == Username && x.UserId != identity);
            var contractorResult = db.Contractors.Where(x => x.Username == Username && x.UserId != identity);
            var adminResult = db.Admins.Where(x => x.Username == Username && x.UserId != identity);
            if ((homeownerResult.Count() < 1) && (contractorResult.Count() < 1) && (adminResult.Count() < 1))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json("Username \"" + Username + "\" is already taken.", JsonRequestBehavior.AllowGet);
        }
    }
}
