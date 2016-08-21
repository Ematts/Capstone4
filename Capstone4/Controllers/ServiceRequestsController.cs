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
using System.IO;
using System.Net.Mail;
using PayPal.AdaptivePayments.Model;
using PayPal.AdaptivePayments;

namespace Capstone4.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceRequests
        public ActionResult Index()
        {
            var serviceRequests = db.ServiceRequests.Include(s => s.Address).Include(s => s.Contractor).Include(s => s.Homeowner);
            return View(serviceRequests.ToList());
        }

        // GET: ServiceRequests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            ServiceRequest serviceRequestPic = db.ServiceRequests.Include(i => i.ServiceRequestFilePaths).SingleOrDefault(i => i.ID == id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public ActionResult Create()
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return RedirectToAction("Must_be_logged_in", "ServiceRequests");
            }

            if (!this.User.IsInRole("Admin") && (!this.User.IsInRole("Homeowner")))
            {
                return RedirectToAction("Must_be_logged_in", "ServiceRequests");
            }
            return View();
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,AddressID,ContractorID,HomeownerID,PostedDate,Price,CompletionDeadline,Description,Service_Number,Expired")] ServiceRequest serviceRequest, Models.Address address, IEnumerable<HttpPostedFileBase> files)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();
            if (ModelState.IsValid)
            {

                foreach (var i in db.Homeowners)
                {
                    if (i.UserId == identity)
                    {

                        serviceRequest.HomeownerID = i.ID;

                    }
                }
                serviceRequest.PostedDate = DateTime.Now;
                if (serviceRequest.PostedDate > serviceRequest.CompletionDeadline)
                {
                    return RedirectToAction("Date_Issue", "ServiceRequests");
                }
                foreach (var file in files)
                {

                    if (file != null && file.ContentLength > 0)
                    {


                        var photo = new ServiceRequestFilePath() { FileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName) };
                        file.SaveAs(Path.Combine(Server.MapPath("~/images"), photo.FileName));
                        serviceRequest.ServiceRequestFilePaths.Add(photo);
                    }

                }
                db.Addresses.Add(address);
                db.SaveChanges();
                serviceRequest.AddressID = address.ID;
                db.ServiceRequests.Add(serviceRequest);
                db.SaveChanges();
                serviceRequest.Service_Number = serviceRequest.ID;
                db.SaveChanges();
                postServiceRequest(serviceRequest);
                return RedirectToAction("Index");
            }


            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,AddressID,ContractorID,HomeownerID,PostedDate,Price,CompletionDeadline,Description,Service_Number,Expired")] ServiceRequest serviceRequest, Models.Address address)
        {
            if (ModelState.IsValid)
            {
                serviceRequest.Service_Number = serviceRequest.ID;
                db.Entry(serviceRequest).State = EntityState.Modified;
                if (serviceRequest.AddressID == null)
                {
                    Models.Address newAdd = new Models.Address();
                    newAdd.Street = address.Street;
                    newAdd.City = address.City;
                    newAdd.State = address.State;
                    newAdd.Street = address.Street;
                    newAdd.Zip = address.Zip;
                    db.Addresses.Add(newAdd);
                    serviceRequest.AddressID = address.ID;
                    serviceRequest.Service_Number = serviceRequest.ID;
                    db.Entry(serviceRequest).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                foreach (var i in db.Addresses)
                {
                    if (i.ID == serviceRequest.AddressID)
                    {
                        i.Street = address.Street;
                        i.City = address.City;
                        i.State = address.State;
                        i.Zip = address.Zip;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (serviceRequest.Address != null)
            {
                db.Addresses.Remove(serviceRequest.Address);
            }
            db.ServiceRequests.Remove(serviceRequest);
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

        public ActionResult Date_Issue()
        {
            ViewBag.Message = "Completion deadline must be later than posted date";

            return View();
        }

        public ActionResult Must_be_logged_in()
        {
            ViewBag.Message = "You must log in as a registered homeowner to create a service request";

            return View();
        }

        public ActionResult AcceptView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            ServiceRequest serviceRequestPic = db.ServiceRequests.Include(i => i.ServiceRequestFilePaths).SingleOrDefault(i => i.ID == id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            return View(serviceRequest);
        }

        public ActionResult Accept(int id)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var contractors = db.Contractors.ToList();
            var serviceRequests = db.ServiceRequests.ToList();
            foreach (var con in contractors)
            {
                if (con.UserId == identity)
                {
                    ContractorAcceptance acceptance = new ContractorAcceptance();
                    foreach (var request in serviceRequests)
                    {
                        if(request.ID == id)
                        {
                            acceptance.ServiceRequest = request;
                        }
                    }
                    acceptance.ContractorID = con.ID;
                    acceptance.ServiceRequestID = id;
                    acceptance.AcceptanceDate = DateTime.Now;
                    db.ContractorAcceptances.Add(acceptance);
                    db.SaveChanges();
                    NotifyAcceptance(acceptance);

                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", "ContractorAcceptances");
        }

        public ActionResult ConfirmCompletionView(int? id)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequestPic = db.ServiceRequests.Include(i => i.ServiceRequestFilePaths).SingleOrDefault(i => i.ID == id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            if ((identity != serviceRequest.Contractor.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }
            return View(serviceRequest);
        }

        public ActionResult AddContractorPhotosView(int? ID)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if ((identity != serviceRequest.Contractor.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }
            return View(serviceRequest);

        }

        public ActionResult ConfirmCompletion(int? ID)
        {

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            serviceRequest.CompletionDate = DateTime.Now;
            serviceRequest.AmountDue = serviceRequest.Price * .9m;
            db.SaveChanges();
            Notify_Homeowner_of_Completion(serviceRequest);
            return RedirectToAction("Contractor_Thank_You", "ServiceRequests", new { id = serviceRequest.ID });

        }
        public ActionResult Contractor_Thank_You(int? ID)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if ((identity != serviceRequest.Contractor.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }


            return View(serviceRequest);

        }

        public ActionResult PaymentSuccess(int? ID)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if ((identity != serviceRequest.Homeowner.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            serviceRequest.ContractorPaid = true;
            db.SaveChanges();

            var myMessage = new SendGrid.SendGridMessage();
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            string url = "http://localhost:37234/ServiceRequests/AcceptView/" + serviceRequest.ID;
            myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Payment Confirmed!!";
            String message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + "Thank you for using Work Warriors!  You have completed payment for the following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + serviceRequest.Price + "<br>" + "<br>" + "Service Number: <br>"  + serviceRequest.Service_Number + "<br>" + "<br>" + "To review " + serviceRequest.Contractor.Username + "'s service, click on link below: <br><a href =" + url + "> Click Here </a>"; 
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);
            Notify_Contractor_of_Payment(serviceRequest);

            return View(serviceRequest);

        }

        public ActionResult PaymentFailure(int? ID)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if ((identity != serviceRequest.Homeowner.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }


            return View(serviceRequest);

        }

        public ActionResult PaymentView(int? ID)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if ((identity != serviceRequest.Homeowner.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }
            string payname = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\payname.txt");
            string paypass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\paypass.txt");
            string sig = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\sig.txt");
            string appid = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\appid.txt");
            ReceiverList receiverList = new ReceiverList();
            receiverList.receiver = new List<Receiver>();
            Receiver receiver = new Receiver(serviceRequest.Price);
            receiver.email = "workwarriors@gmail.com";
            receiver.primary = true;
            receiverList.receiver.Add(receiver);
            Receiver receiver2 = new Receiver(serviceRequest.AmountDue);
            receiver2.email = serviceRequest.Contractor.ApplicationUser.Email;
            receiver2.primary = false;
            receiverList.receiver.Add(receiver2);
            RequestEnvelope requestEnvelope = new RequestEnvelope("en_US");
            string actionType = "PAY";
            //string successUrl = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/CompletedBids/SuccessView/{0}";
            //string failureUrl = "http://" + System.Web.HttpContext.Current.Request.Url.Authority + "/CompletedBids/FailureView/{0}";
            string successUrl = "http://localhost:37234/ServiceRequests/PaymentSuccess/" + serviceRequest.ID;
            string failureUrl = "http://localhost:37234/ServiceRequests/PaymentFailure/" + serviceRequest.ID;
            string returnUrl = successUrl;
            string cancelUrl = failureUrl;
            string currencyCode = "USD";
            PayRequest payRequest = new PayRequest(requestEnvelope, actionType, cancelUrl, currencyCode, receiverList, returnUrl);
            payRequest.ipnNotificationUrl = "http://replaceIpnUrl.com";
            string memo = serviceRequest.Description + " Invoice = " + serviceRequest.Service_Number;
            payRequest.memo = memo;
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", "sandbox");
            sdkConfig.Add("account1.apiUsername", payname); //PayPal.Account.APIUserName
            sdkConfig.Add("account1.apiPassword", paypass); //PayPal.Account.APIPassword
            sdkConfig.Add("account1.apiSignature", sig); //.APISignature
            sdkConfig.Add("account1.applicationId", appid); //.ApplicatonId

            AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(sdkConfig);
            PayResponse payResponse = adaptivePaymentsService.Pay(payRequest);
            ViewData["paykey"] = payResponse.payKey;
            return View(serviceRequest);

        }
        public ActionResult AddContractorPhotos(int? ID, IEnumerable<HttpPostedFileBase> files)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);
            serviceRequest.CompletedServiceRequestFilePaths = new List<CompletedServiceRequestFilePath>();
            foreach (var file in files)
            {

                if (file != null && file.ContentLength > 0)
                {


                    var photo = new CompletedServiceRequestFilePath() { FileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName) };
                    file.SaveAs(Path.Combine(Server.MapPath("~/images"), photo.FileName));
                    serviceRequest.CompletedServiceRequestFilePaths.Add(photo);
                }

            }
            db.SaveChanges();
            return RedirectToAction("ConfirmCompletionView", "ServiceRequests", new { id = serviceRequest.ID }); 

        }

        public void postServiceRequest(ServiceRequest serviceRequest)
        {
            
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var contractors = db.Contractors.ToList();
            foreach (var i in db.Contractors)
            {

                var myMessage = new SendGrid.SendGridMessage();
                myMessage.AddTo(i.ApplicationUser.Email);
                myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
                myMessage.Subject = "New Service Request Posting!!";
                string url = "http://localhost:37234/ServiceRequests/AcceptView/" + serviceRequest.ID;
                string message = "Hello " + i.FirstName + "," + "<br>" + "<br>" + "A new service request has been posted by " + serviceRequest.Homeowner.Username + " for the following job Location: <br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + serviceRequest.Price + "<br>" + "<br>" + "Must be completed by: <br>" + serviceRequest.CompletionDeadline + "<br>" + "<br>" + "Date Posted: <br>" + serviceRequest.PostedDate + "<br>" + "<br>" + "To accept job, click on link below: <br><a href =" + url + "> Click Here </a>";
                myMessage.Html = message;
                var credentials = new NetworkCredential(name, pass);
                var transportWeb = new SendGrid.Web(credentials);
                transportWeb.DeliverAsync(myMessage);

            }
        }

        public void NotifyAcceptance(ContractorAcceptance contractorAcceptance)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(contractorAcceptance.ServiceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Service Request Acceptance!!";
            string url = "http://localhost:37234/ContractorAcceptances/NotifyHomeownerView/" + contractorAcceptance.ID;
            string message = "Hello " + contractorAcceptance.ServiceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + contractorAcceptance.Contractor.Username + " has offered to perform your following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + contractorAcceptance.ServiceRequest.Address.Street + "<br>" + contractorAcceptance.ServiceRequest.Address.City + "<br>" + contractorAcceptance.ServiceRequest.Address.State + "<br>" + contractorAcceptance.ServiceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + contractorAcceptance.ServiceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + contractorAcceptance.ServiceRequest.Price + "<br>" + "<br>" + "Must be completed by: <br>" + contractorAcceptance.ServiceRequest.CompletionDeadline + "<br>" + "<br>" + "Date Posted: <br>" + contractorAcceptance.ServiceRequest.PostedDate + "<br>" + "<br>" + "To confirm contractor, click on link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Homeowner_of_Completion(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Job Complete!!";
            string url = "http://localhost:37234/ServiceRequests/PaymentView/" + serviceRequest.ID;
            string message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + serviceRequest.Contractor.Username + " has confirmed completion your following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + serviceRequest.Price + "<br>" + "<br>" + "To complete payment, click on link below: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        public void Notify_Contractor_of_Payment(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "You've been paid!!";
            string url = "http://localhost:37234/ServiceRequests/PaymentView/" + serviceRequest.ID;
            string message = "Hello " + serviceRequest.Contractor.FirstName + "," + "<br>" + "<br>" + "$" + serviceRequest.AmountDue + " has been credited to your Paypal account for the following service:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Service Number: <br>" + serviceRequest.Service_Number;
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

    }
}
