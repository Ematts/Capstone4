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
using Newtonsoft.Json;

namespace Capstone4.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ServiceRequests
        public ActionResult Index()
        {
            var serviceRequests = db.ServiceRequests.Include(s => s.Address).Include(s => s.Contractor).Include(s => s.Homeowner);
            foreach (var i in db.ServiceRequests.ToList())
            {
                if (i.Address == null)
                {
                    Models.Address address = new Models.Address();
                    i.Address = address;
                }
            }
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

            if (!this.User.IsInRole("Homeowner"))
            {
                return RedirectToAction("Must_be_logged_in", "ServiceRequests");
            }

            ServiceRequest serviceRequest = new ServiceRequest();

            foreach (var i in db.Homeowners)
            {
                if (i.UserId == identity)
                {

                    serviceRequest.Homeowner = i;

                }
            }

            return View(serviceRequest);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,AddressID,ContractorID,HomeownerID,PostedDate,Price,CompletionDeadline,Description,Service_Number,Expired,Inactive")] ServiceRequest serviceRequest, Models.Address address, IEnumerable<HttpPostedFileBase> file)
        //{
        //    string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();
        //    List<Contractor> emailList;
        //    if (ModelState.IsValid)
        //    {

        //        foreach (var i in db.Homeowners)
        //        {
        //            if (i.UserId == identity)
        //            {

        //                serviceRequest.HomeownerID = i.ID;

        //            }
        //        }
        //        if (serviceRequest.CompletionDeadline < DateTime.Now)
        //        {
        //            return RedirectToAction("Date_Issue", "ServiceRequests");
        //        }
        //        foreach (var file1 in file)
        //        {

        //            if (file1 != null && file1.ContentLength > 0)
        //            {


        //                var photo = new ServiceRequestFilePath() { FileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file1.FileName) };
        //                file1.SaveAs(Path.Combine(Server.MapPath("~/images"), photo.FileName));
        //                serviceRequest.ServiceRequestFilePaths.Add(photo);
        //            }

        //        }
        //        foreach (var i in db.Addresses.ToList())
        //        {
        //            if (i.FullAddress == address.FullAddress)
        //            {
        //                serviceRequest.AddressID = i.ID;
        //                serviceRequest.Address = i;
        //                serviceRequest.Address.validated = address.validated;
        //                serviceRequest.Address.vacant = address.vacant;
        //                db.SaveChanges();
        //            }
        //        }
        //        if (serviceRequest.AddressID == null)
        //        {
        //            db.Addresses.Add(address);
        //            db.SaveChanges();
        //            serviceRequest.AddressID = address.ID;
        //        }

        //        db.ServiceRequests.Add(serviceRequest);
        //        db.SaveChanges();
        //        serviceRequest.Service_Number = serviceRequest.ID;
        //        emailList = GetDistance(serviceRequest);

        //        if (emailList.Count == 0)
        //        {
        //            List<Models.Address> ids = new List<Models.Address>();
        //            Models.Address addressToCheck = db.Addresses.Where(x => x.ID == serviceRequest.AddressID).SingleOrDefault();
        //            serviceRequest.Posted = false;
        //            serviceRequest.NeedsManualValidation = false;
        //            TempData["address"] = addressToCheck;
        //            db.SaveChanges();
        //            return RedirectToAction("noService", "ServiceRequests", new { address = TempData["address"] });
        //        }
        //        serviceRequest.Posted = true;
        //        serviceRequest.PostedDate = DateTime.Now;
        //        serviceRequest.NeedsManualValidation = false;
        //        db.SaveChanges();
        //        postServiceRequest(serviceRequest, emailList);
        //        return RedirectToAction("Index");
        //    }


        //    return View(serviceRequest);
        //}

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRequest()
        {

            var files = Enumerable.Range(0, Request.Files.Count).Select(i => Request.Files[i]);
            var form = Request.Form;
            string city = (form["Address.City"]);
            string state = (form["Address.State"]);
            string zip = (form["Address.Zip"]);
            string street = (form["Address.Street"]);
            string description = (form["Description"]);
            string priceString = (form["Price"]);
            string tzone = (form["Timezone"]);
            string Ambig = (form["AmbigTime"]);
            decimal price = Convert.ToDecimal(priceString);
            string dateString = (form["CompletionDeadline"]);
            string utcString = (form["UTCDate"]);
            DateTime completionDeadline = Convert.ToDateTime(dateString);
            DateTime? utc;
            try
            {
                utc = Convert.ToDateTime(utcString);
            }
            catch
            {
                utc = null;
            }
            bool vacant = form["Address.vacant"].Contains("true");
            bool validated = form["Address.validated"].Contains("true");
            bool inactive = form["Inactive"].Contains("true");
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            
            List<Contractor> emailList;
            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!this.User.IsInRole("Homeowner"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Models.Address address = new Models.Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
            ServiceRequest serviceRequest = new ServiceRequest() { Description = description, Price = price, CompletionDeadline = completionDeadline, Timezone = tzone, UTCDate = utc, AmbigTime = Ambig, Inactive = inactive };
            serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();
            foreach (var i in db.Homeowners)
            {
                if (i.UserId == identity)
                {

                    serviceRequest.HomeownerID = i.ID;

                }
            }

            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
            DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
            DateTime? deadline;
            

            if (serviceRequest.UTCDate == null)
            {
                deadline = serviceRequest.CompletionDeadline;
                
            }
            else
            {
                deadline = serviceRequest.UTCDate;
                Time = DateTime.UtcNow;
            }

            if (deadline < Time)
            {
                return Json(new { success = true, LateDate = true },
                JsonRequestBehavior.AllowGet); 
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

            foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        serviceRequest.AddressID = i.ID;
                        serviceRequest.Address = i;
                        serviceRequest.Address.validated = address.validated;
                        serviceRequest.Address.vacant = address.vacant;
                        db.SaveChanges();
                    }
                }

            if (serviceRequest.AddressID == null)
            {
                db.Addresses.Add(address);
                db.SaveChanges();
                serviceRequest.AddressID = address.ID;
                serviceRequest.Address = address;
            }

            db.ServiceRequests.Add(serviceRequest);
            db.SaveChanges();
            serviceRequest.Service_Number = serviceRequest.ID;
            emailList=GetDistance(serviceRequest);

            if (emailList.Count == 0)
            {
                List<Models.Address> ids = new List<Models.Address>();
                Models.Address addressToCheck = db.Addresses.Where(x => x.ID == serviceRequest.AddressID).SingleOrDefault();
                serviceRequest.Posted = false;
                serviceRequest.NeedsManualValidation = false;
                db.SaveChanges();
                return Json(new { success = true, noService = true, id = serviceRequest.ID },
                    JsonRequestBehavior.AllowGet);
            }
            serviceRequest.Posted = true;

            DateTime timeUtcPosted = DateTime.UtcNow;
            serviceRequest.PostedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPosted, Zone);
            DateTime myDt = DateTime.SpecifyKind(timeUtcPosted, DateTimeKind.Utc);
            bool dst = Zone.IsDaylightSavingTime(myDt);


            if(dst == true)
            {
                serviceRequest.PostedAmbigTime = "DST";
            }

            if (dst == false)
            {
                serviceRequest.PostedAmbigTime = "STD";
            }

            serviceRequest.NeedsManualValidation = false;
            db.SaveChanges();
            postServiceRequest(serviceRequest, emailList);
            return Json(new { success = true, id = serviceRequest.ID },
            JsonRequestBehavior.AllowGet);

        }

        // GET: ServiceRequests/Edit/5
        public ActionResult Edit(int? id, string description, decimal? price, DateTime? completionDeadline, string city, string state, string zip, string street)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if(serviceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if(serviceRequest.Posted == true)
            {
                return RedirectToAction("NoEdit", "ServiceRequests", new { id = serviceRequest.ID });
            }
            if(description != null)
            {
                serviceRequest.Description = description;
            }
            if (price != null)
            {
                serviceRequest.Price = price.Value;
            }
            if (completionDeadline != null)
            {
                serviceRequest.CompletionDeadline = completionDeadline.Value;
            }
            if (city != null)
            {
                serviceRequest.Address.City = city;
            }
            if (state != null)
            {
                serviceRequest.Address.State = state;
            }
            if (zip != null)
            {
                serviceRequest.Address.Zip = zip;
            }
            if (street != null)
            {
                serviceRequest.Address.Street = street;
            }
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,AddressID,ContractorID,HomeownerID,PostedDate,Price,CompletionDeadline,Description,Service_Number,Expired,ContractorReviewID,CompletionDate,AmountDue,ContractorPaid,Inactive,PayPalListenerModelID,ManualValidated,NeedsManualValidation")] ServiceRequest serviceRequest, Models.Address address, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                var formInfo = address;
                var addressToCheck = db.Addresses.Where(x => x.ID == serviceRequest.AddressID).SingleOrDefault();

                if (serviceRequest.CompletionDeadline < DateTime.Now)
                {
                    return RedirectToAction("Date_Issue", "ServiceRequests");
                }
                
                db.Entry(serviceRequest).State = EntityState.Modified;
                serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();

                foreach (var file in files)
                {

                    if (file != null && file.ContentLength > 0)
                    {

                        var photo = new ServiceRequestFilePath() { FileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName) };
                        file.SaveAs(Path.Combine(Server.MapPath("~/images"), photo.FileName));
                        serviceRequest.ServiceRequestFilePaths.Add(photo);
                    }

                }

                if (serviceRequest.AddressID == null)
                {
                    Models.Address newAdd = new Models.Address();
                    newAdd.Street = address.Street;
                    newAdd.City = address.City;
                    newAdd.State = address.State;
                    newAdd.Street = address.Street;
                    newAdd.Zip = address.Zip;
                    newAdd.validated = address.validated;
                    newAdd.vacant = address.vacant;
                    foreach (var i in db.Addresses.ToList())
                    {
                        if (newAdd.FullAddress == i.FullAddress)
                        {
                            serviceRequest.AddressID = i.ID;
                            serviceRequest.Address = i;
                            serviceRequest.Address.validated = formInfo.validated;
                            serviceRequest.Address.vacant = formInfo.vacant;
                            if (serviceRequest.Address.validated == true)
                            {
                                serviceRequest.NeedsManualValidation = false;
                                serviceRequest.Inactive = false;

                            }
                            db.SaveChanges();
                            if(serviceRequest.Posted != true && serviceRequest.NeedsManualValidation == false && serviceRequest.Inactive != true && serviceRequest.Address.validated == true)
                            {
                                List<Contractor> emailList;
                                if (serviceRequest == null)
                                {
                                    return HttpNotFound();
                                }
                                
                                emailList = GetDistance(serviceRequest);

                                if (emailList.Count == 0)
                                {
                                    serviceRequest.Posted = false;
                                    TempData["address"] = addressToCheck;
                                    db.SaveChanges();
                                    return RedirectToAction("noService", "ServiceRequests", new { address = TempData["address"] });
                                }
                                serviceRequest.Posted = true;
                                serviceRequest.PostedDate = DateTime.Now;
                                db.SaveChanges();
                                postServiceRequest(serviceRequest, emailList);
                                return RedirectToAction("Index");
                            }
                            return RedirectToAction("Index");
                        }
                    }
                    db.Addresses.Add(newAdd);
                    serviceRequest.AddressID = newAdd.ID;
                    serviceRequest.Address = newAdd;
                    if (serviceRequest.Address.validated == true)
                    {
                        serviceRequest.NeedsManualValidation = false;
                        serviceRequest.Inactive = false;

                    }
                    db.SaveChanges();
                    if (serviceRequest.Posted != true && serviceRequest.NeedsManualValidation == false && serviceRequest.Inactive != true && serviceRequest.Address.validated == true)
                    {
                        List<Contractor> emailList;
                        if (serviceRequest == null)
                        {
                            return HttpNotFound();
                        }

                        emailList = GetDistance(serviceRequest);

                        if (emailList.Count == 0)
                        {
                            serviceRequest.Posted = false;
                            TempData["address"] = addressToCheck;
                            db.SaveChanges();
                            return RedirectToAction("noService", "ServiceRequests", new { address = TempData["address"] });
                        }
                        serviceRequest.Posted = true;
                        serviceRequest.PostedDate = DateTime.Now;
                        db.SaveChanges();
                        postServiceRequest(serviceRequest, emailList);
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");
                }
                List<Models.Address> ids = new List<Models.Address>();
                foreach (var i in db.Addresses.ToList())
                {
                    if (formInfo.FullAddress == i.FullAddress)
                    {
                        serviceRequest.AddressID = i.ID;
                        serviceRequest.Address.validated = formInfo.validated;
                        serviceRequest.Address.vacant = formInfo.vacant;
                        if (serviceRequest.Address.validated == true)
                        {
                            serviceRequest.NeedsManualValidation = false;
                            serviceRequest.Inactive = false;

                        }
                        db.SaveChanges();

                        foreach (var x in db.Contractors.ToList())
                        {
                            ids.Add(x.Address);
                        }
                        foreach (var x in db.Homeowners.ToList())
                        {
                            ids.Add(x.Address);
                        }
                        foreach (var x in db.ServiceRequests.ToList())
                        {
                            ids.Add(x.Address);
                        }
                        if (!ids.Contains(addressToCheck))
                        {
                            db.Addresses.Remove(addressToCheck);
                        }
                        if (serviceRequest.Posted != true && serviceRequest.NeedsManualValidation == false && serviceRequest.Inactive != true && serviceRequest.Address.validated == true)
                        {
                            List<Contractor> emailList;
                            if (serviceRequest == null)
                            {
                                return HttpNotFound();
                            }
                            if (serviceRequest.CompletionDeadline < DateTime.Now)
                            {
                                return RedirectToAction("Date_Issue", "ServiceRequests");
                            }

                            emailList = GetDistance(serviceRequest);

                            if (emailList.Count == 0)
                            {
                                serviceRequest.Posted = false;
                                TempData["address"] = addressToCheck;
                                db.SaveChanges();
                                return RedirectToAction("noService", "ServiceRequests", new { address = TempData["address"] });
                            }
                            serviceRequest.Posted = true;
                            serviceRequest.PostedDate = DateTime.Now;
                            db.SaveChanges();
                            postServiceRequest(serviceRequest, emailList);
                            return RedirectToAction("Index");
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                Models.Address newAdd1 = new Models.Address();
                newAdd1.Street = formInfo.Street;
                newAdd1.City = formInfo.City;
                newAdd1.State = formInfo.State;
                newAdd1.Street = formInfo.Street;
                newAdd1.Zip = formInfo.Zip;
                newAdd1.validated = formInfo.validated;
                newAdd1.vacant = formInfo.vacant;
                db.Addresses.Add(newAdd1);
                serviceRequest.AddressID = newAdd1.ID;
                serviceRequest.Address = newAdd1;
                if (serviceRequest.Address.validated == true)
                {
                    serviceRequest.Inactive = false;
                    serviceRequest.NeedsManualValidation = false;

                }
                db.SaveChanges();
                foreach (var x in db.Contractors.ToList())
                {
                    ids.Add(x.Address);
                }
                foreach (var x in db.Homeowners.ToList())
                {
                    ids.Add(x.Address);
                }
                foreach (var x in db.ServiceRequests.ToList())
                {
                    ids.Add(x.Address);
                }
                if (!ids.Contains(addressToCheck))
                {
                    db.Addresses.Remove(addressToCheck);
                }
                db.SaveChanges();
                if (serviceRequest.Posted != true && serviceRequest.NeedsManualValidation == false && serviceRequest.Inactive != true && serviceRequest.Address.validated == true)
                {
                    List<Contractor> emailList;
                    if (serviceRequest == null)
                    {
                        return HttpNotFound();
                    }

                    emailList = GetDistance(serviceRequest);

                    if (emailList.Count == 0)
                    {
                        serviceRequest.Posted = false;
                        TempData["address"] = addressToCheck;
                        db.SaveChanges();
                        return RedirectToAction("noService", "ServiceRequests", new { address = TempData["address"] });
                    }
                    serviceRequest.Posted = true;
                    serviceRequest.PostedDate = DateTime.Now;
                    db.SaveChanges();
                    postServiceRequest(serviceRequest, emailList);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");

            }
            return View(serviceRequest);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRequest()
        {
            var files = Enumerable.Range(0, Request.Files.Count).Select(i => Request.Files[i]);
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            string city = (form["Address.City"]);
            string state = (form["Address.State"]);
            string zip = (form["Address.Zip"]);
            string street = (form["Address.Street"]);
            string description = (form["Description"]);
            string priceString = (form["Price"]);
            string tzone = (form["Timezone"]);
            string Ambig = (form["AmbigTime"]);
            decimal price = Convert.ToDecimal(priceString);
            string dateString = (form["CompletionDeadline"]);
            string utcString = (form["UTCDate"]);
            DateTime completionDeadline = Convert.ToDateTime(dateString);
            DateTime? utc;
            try
            {
                utc = Convert.ToDateTime(utcString);
            }
            catch
            {
                utc = null;
            }
            bool vacant = form["Address.vacant"].Contains("true");
            bool validated = form["Address.validated"].Contains("true");
            bool inactive = form["Inactive"].Contains("true");
            int fileList = 0;

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!this.User.IsInRole("Homeowner"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Models.Address address = new Models.Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            serviceRequest.Timezone = tzone;
            serviceRequest.AmbigTime = Ambig;
            serviceRequest.UTCDate = utc;
            serviceRequest.CompletionDeadline = completionDeadline;
            serviceRequest.Service_Number = serviceRequest.ID;
            serviceRequest.Price = price;
            serviceRequest.Description = description;


            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if (serviceRequest.Posted == true)
            {
                return Json(new { success = true, Already = true, id = serviceRequest.ID },
                JsonRequestBehavior.AllowGet);
            }
            var addressToCheck = serviceRequest.Address;
            serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();
            bool addressAssigned = false;

            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
            DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
            DateTime? deadline;


            if (serviceRequest.UTCDate == null)
            {
                deadline = serviceRequest.CompletionDeadline;
            }
            else
            {
                deadline = serviceRequest.UTCDate;
                Time = DateTime.UtcNow;
            }

            if (deadline < Time)
            {
                return Json(new { success = true, LateDate = true },
                JsonRequestBehavior.AllowGet);
            }

            foreach (var path in db.ServiceRequestFilePaths.ToList())
            {
                if (path.ServiceRequestID == serviceRequest.ID)
                    fileList++;
            }

            foreach (var file in files)
            {
                fileList++;
            }

            if (fileList > 4)
            {
                return Json(new { success = true, tooManyPics = true, id = serviceRequest.ID },
                JsonRequestBehavior.AllowGet);
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

            if (serviceRequest.AddressID == null)
            {

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        serviceRequest.AddressID = i.ID;
                        serviceRequest.Address = i;
                        db.SaveChanges();
                        addressAssigned = true;
                    }

                }

            }
            if (serviceRequest.AddressID == null)
            {
                db.Addresses.Add(address);
                serviceRequest.AddressID = address.ID;
                serviceRequest.Address = address;
                db.SaveChanges();
                addressAssigned = true;
            }
            if (serviceRequest.AddressID != null && addressAssigned == false)
            {

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        serviceRequest.AddressID = i.ID;
                        serviceRequest.Address = i;
                        serviceRequest.Address.validated = address.validated;
                        serviceRequest.Address.vacant = address.vacant;
                        db.SaveChanges();
                        addressAssigned = true;
                    }
                }

            }
            if (addressAssigned == false)
            {
                db.Addresses.Add(address);
                serviceRequest.AddressID = address.ID;
                serviceRequest.Address = address;
                db.SaveChanges();
            }
            if (addressToCheck != null)
            {
                List<Models.Address> ids = new List<Models.Address>();
                foreach (var x in db.Contractors.ToList())
                {
                    ids.Add(x.Address);
                }
                foreach (var x in db.Homeowners.ToList())
                {
                    ids.Add(x.Address);
                }
                foreach (var x in db.ServiceRequests.ToList())
                {
                    ids.Add(x.Address);
                }
                if (!ids.Contains(addressToCheck))
                {
                    db.Addresses.Remove(addressToCheck);
                }
                db.SaveChanges();
            }
            //serviceRequest.Description = description;
            serviceRequest.NeedsManualValidation = false;
            //serviceRequest.Price = price;
            //serviceRequest.CompletionDeadline = completionDeadline;
            serviceRequest.Address.vacant = vacant;
            serviceRequest.Inactive = inactive;
            serviceRequest.Address.validated = validated;
            //serviceRequest.Timezone = tzone;
            //serviceRequest.AmbigTime = Ambig;
            //serviceRequest.UTCDate = utc;

            db.SaveChanges();
            if (serviceRequest.Posted != true && serviceRequest.NeedsManualValidation == false && serviceRequest.Inactive != true && serviceRequest.Address.validated == true)
            {
                List<Contractor> emailList = GetDistance(serviceRequest);
                if (emailList.Count == 0)
                {
                    serviceRequest.Posted = false;
                    db.SaveChanges();
                    return Json(new { success = true, noService = true, id = serviceRequest.ID },
                         JsonRequestBehavior.AllowGet);
                }

                serviceRequest.Posted = true;
                DateTime timeUtcPosted = DateTime.UtcNow;
                serviceRequest.PostedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPosted, Zone);
                DateTime myDt = DateTime.SpecifyKind(timeUtcPosted, DateTimeKind.Utc);
                bool dst = Zone.IsDaylightSavingTime(myDt);


                if (dst == true)
                {
                    serviceRequest.PostedAmbigTime = "DST";
                }

                if (dst == false)
                {
                    serviceRequest.PostedAmbigTime = "STD";
                }

                db.SaveChanges();
                postServiceRequest(serviceRequest, emailList);
                return Json(new { success = true, id = serviceRequest.ID },
                 JsonRequestBehavior.AllowGet);
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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

            if(serviceRequest.ContractorReview != null)
            {
                if (serviceRequest.ContractorReview.ReviewResponse != null)
                {
                    db.ReviewResponses.Remove(serviceRequest.ContractorReview.ReviewResponse);
                }
                db.ContractorReviews.Remove(serviceRequest.ContractorReview);
                db.SaveChanges();
                UpdateRating(serviceRequest.Contractor);
            }

            if (serviceRequest.PayPalListenerModel != null)
            {
                db.PayPalListenerModels.Remove(serviceRequest.PayPalListenerModel);
                db.SaveChanges();
            }

            List<Models.Address> ids = new List<Models.Address>();
            Models.Address addressToCheck = db.Addresses.Where(x => x.ID == serviceRequest.AddressID).SingleOrDefault();
            db.ServiceRequests.Remove(serviceRequest);
            if (addressToCheck != null)
            {
                foreach (var i in db.Addresses.ToList())
                {
                    foreach (var x in db.Contractors.ToList())
                    {
                        ids.Add(x.Address);
                    }
                    foreach (var x in db.Homeowners.ToList())
                    {
                        ids.Add(x.Address);
                    }
                    foreach (var x in db.ServiceRequests.ToList())
                    {
                        ids.Add(x.Address);
                    }
                    if (!ids.Contains(addressToCheck))
                    {
                        db.Addresses.Remove(addressToCheck);
                    }
                    
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePic()
        {
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            string description = (form["Description"]);
            if(description == "")
            {
                description = serviceRequest.Description;
            }
            string city = (form["Address.City"]);
            if (city == "")
            {
                city = serviceRequest.Address.City;
            }
            string state = (form["Address.State"]);
            if (state == "")
            {
                state = serviceRequest.Address.State;
            }
            string zip = (form["Address.Zip"]);
            if (zip == "")
            {
                zip = serviceRequest.Address.Zip;
            }
            string street = (form["Address.Street"]);
            if (street == "")
            {
                street = serviceRequest.Address.Street;
            }
            string priceString = (form["Price"]);
            decimal? price;
            decimal priceParse;
            if (Decimal.TryParse(priceString, out priceParse))
                price = priceParse;
            else
                price = serviceRequest.Price;
            string dateString = (form["CompletionDeadline"]);;
            DateTime date;
            DateTime dateParse;
            if (DateTime.TryParse(dateString, out dateParse))
                date = dateParse;
            else
                date = serviceRequest.CompletionDeadline;
            string dateToPass = Convert.ToString(date);
            string filename = (form["picName"]);
            string[] nameSplit = filename.Split('/');
            Array.Reverse(nameSplit);
            string fileToDelete = nameSplit[0];
            foreach (var pic in serviceRequest.ServiceRequestFilePaths.ToList())
            {
                if (pic.FileName == fileToDelete)
                {
                    db.ServiceRequestFilePaths.Remove(pic);
                    db.SaveChanges();
                }
                    
            }
            
            return Json(new { Result = "OK", id = serviceRequest.ID, description = description, price = price, completionDeadline = dateToPass, city = city, state = state, zip = zip, street = street });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteContractorPic()
        {
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            string filename = (form["picName"]);
            string[] nameSplit = filename.Split('/');
            Array.Reverse(nameSplit);
            string fileToDelete = nameSplit[0];
            foreach (var pic in serviceRequest.CompletedServiceRequestFilePaths.ToList())
            {
                if (pic.FileName == fileToDelete)
                {
                    db.CompletedServiceRequestFilePaths.Remove(pic);
                    db.SaveChanges();
                }

            }

            return Json(new { Result = "OK", id = serviceRequest.ID});
        }
        public ActionResult Activate (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);

            if(serviceRequest.NeedsManualValidation == true)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if(serviceRequest.Posted == true)
            {
                return RedirectToAction("Duplicate_Post", "ServiceRequests");
            }

            ServiceRequest serviceRequestPic = db.ServiceRequests.Include(i => i.ServiceRequestFilePaths).SingleOrDefault(i => i.ID == id);
            List<Contractor> emailList;
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
            DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
            DateTime? deadline;


            if (serviceRequest.UTCDate == null)
            {
                deadline = serviceRequest.CompletionDeadline;

            }
            else
            {
                deadline = serviceRequest.UTCDate;
                Time = DateTime.UtcNow;
            }

            if (deadline < Time)
            {
                return RedirectToAction("Date_Issue", "ServiceRequests");
            }

            serviceRequest.Inactive = false;
            
            emailList = GetDistance(serviceRequest);

            if (emailList.Count == 0)
            {
                serviceRequest.Posted = false;
                db.SaveChanges();
                return RedirectToAction("noService", "ServiceRequests", new { id = serviceRequest.ID });
            }
            serviceRequest.Posted = true;

            DateTime timeUtcPosted = DateTime.UtcNow;
            serviceRequest.PostedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPosted, Zone);
            DateTime myDt = DateTime.SpecifyKind(timeUtcPosted, DateTimeKind.Utc);
            bool dst = Zone.IsDaylightSavingTime(myDt);


            if (dst == true)
            {
                serviceRequest.PostedAmbigTime = "DST";
            }

            if (dst == false)
            {
                serviceRequest.PostedAmbigTime = "STD";
            }

            db.SaveChanges();
            postServiceRequest(serviceRequest, emailList);
            return RedirectToAction("Details", "ServiceRequests", new { id = serviceRequest.ID });

        }

        public ActionResult ActivateView(int? id)
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
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if(serviceRequest.Homeowner.ApplicationUser.Id != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }
            return View(serviceRequest);
        }

        public ActionResult Duplicate_Post()
        {
            ViewBag.Message = "This service request has already been posted.";

            return View();
        }

        public ActionResult NoEdit(int? id)
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

        public ActionResult EditTime(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            EditTimeViewModel model = new EditTimeViewModel() { ID = serviceRequest.ID, CompletionDeadline = serviceRequest.CompletionDeadline, Email = serviceRequest.Homeowner.ApplicationUser.Email, Username = serviceRequest.Homeowner.Username, Street = serviceRequest.Address.Street, City = serviceRequest.Address.City, State = serviceRequest.Address.State, Zip = serviceRequest.Address.Zip, Description = serviceRequest.Description, Price = serviceRequest.Price, Service_Number = serviceRequest.Service_Number, ServiceRequestFilePaths = serviceRequest.ServiceRequestFilePaths };
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if(serviceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTimeModify (int? id, DateTime? CompletionDeadline, DateTime? UTCDate, string AmbigTime)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
            DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
            DateTime? deadline;

            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (serviceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if (UTCDate == null)
            {
                deadline = CompletionDeadline;
            }
            else
            {
                deadline = UTCDate;
                Time = DateTime.UtcNow;
            }

            if (deadline < Time)
            {
                return RedirectToAction("Date_Issue", "ServiceRequests");
            }

            serviceRequest.CompletionDeadline = CompletionDeadline.Value;
            serviceRequest.UTCDate = UTCDate;
            serviceRequest.AmbigTime = AmbigTime;

            db.SaveChanges();

            return RedirectToAction("ActivateView", "ServiceRequests", new { id = serviceRequest.ID });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditManual([Bind(Include = "ID,AddressID,ContractorID,HomeownerID,PostedDate,Price,CompletionDeadline,Description,Service_Number,Expired,ContractorReviewID,CompletionDate,AmountDue,ContractorPaid,Inactive,PayPalListenerModelID,ManualValidated,NeedsManualValidation")] ServiceRequest serviceRequest)
        //{
        //    List<Contractor> emailList;
        //    if (ModelState.IsValid)
        //    {

        //        serviceRequest.Address = db.Addresses.Where(x => x.ID == serviceRequest.AddressID).SingleOrDefault();
        //        serviceRequest.Homeowner = db.Homeowners.Where(x => x.ID == serviceRequest.HomeownerID).SingleOrDefault();
        //        serviceRequest.PostedDate = DateTime.Now;
        //        db.Entry(serviceRequest).State = EntityState.Modified;
        //        emailList = GetDistance(serviceRequest);
        //        if (serviceRequest.PostedDate > serviceRequest.CompletionDeadline)
        //        {
        //            return RedirectToAction("Date_Issue", "ServiceRequests");
        //        }
        //        if (emailList.Count == 0)
        //        {
        //            serviceRequest.Posted = false;
        //            //TempData["address"] = addressToCheck;
        //            db.SaveChanges();
        //            return RedirectToAction("noService", "ServiceRequests", new { id = serviceRequest.ID });
        //        }
        //        if (serviceRequest.Posted == true)
        //        {
        //            return RedirectToAction("Duplicate_Post");
        //        }
        //        serviceRequest.Posted = true;
        //        serviceRequest.PostedDate = DateTime.Now;
        //        postServiceRequest(serviceRequest, emailList);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");

        //    }
        //    return View(serviceRequest);
        //}

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

        public ActionResult noService(int id)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            var address = db.Addresses.Where(x => x.ID == serviceRequest.AddressID).SingleOrDefault();
            if (serviceRequest.Address == null)
            {
                return HttpNotFound();
            }
            
            return View(address);
        }

        public ActionResult Manual_Validate_Thank_You(int? id)
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

        public ActionResult Expired (int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (!this.User.IsInRole("Contractor"))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if(serviceRequest.ContractorID != null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if (serviceRequest.Expired != true)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(serviceRequest);
        }

        public ActionResult AcceptView(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            ServiceRequest serviceRequestPic = db.ServiceRequests.Include(i => i.ServiceRequestFilePaths).SingleOrDefault(i => i.ID == id);
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
            DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            Contractor contractor = db.Contractors.Where(x => x.UserId == identity).SingleOrDefault();

            if (!this.User.IsInRole("Contractor"))
            {
                return RedirectToAction("Must_be_logged_in_to_accept_request");
            }

            if (contractor.Inactive == true)
            {
                return RedirectToAction("Account_Inactive", "Contractors", new { id = contractor.ID });
            }

            if(serviceRequest.Expired == true)
            {
                return RedirectToAction("Expired", "ServiceRequests", new { id = serviceRequest.ID });
            }

            foreach (var acceptance in serviceRequest.ContractorAcceptances)
            {
                if(acceptance.Contractor.UserId == identity)
                {
                    return RedirectToAction("Already_Accepted", "ContractorAcceptances", new { id = acceptance.ID });
                }
            }

            if(serviceRequest.ContractorID != null)
            {
                return RedirectToAction("Contractor_Already_Accepted", new { id = serviceRequest.ID });
            }

            return View(serviceRequest);
        }

        public ActionResult Contractor_Already_Accepted(int? id)
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

            if (!this.User.IsInRole("Contractor"))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(serviceRequest);
        }

        public ActionResult Must_be_logged_in_to_accept_request()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Accept(int id)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var contractors = db.Contractors.ToList();
            var serviceRequests = db.ServiceRequests.ToList();
            ContractorAcceptance acceptance = new ContractorAcceptance();
            foreach (var con in contractors)
            {
                if (con.UserId == identity)
                {
                    if (con.Inactive == true)
                    {
                        return RedirectToAction("Account_Inactive", "Contractors", new { id = con.ID });
                    }

                    foreach (var request in serviceRequests)
                    {
                        if(request.ID == id)
                        {
                            acceptance.ServiceRequest = request;
                        }
                    }
                    acceptance.ContractorID = con.ID;
                    acceptance.ServiceRequestID = id;
                    DateTime timeUtc = DateTime.UtcNow;
                    TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(acceptance.ServiceRequest.Timezone);
                    DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
                    DateTime? deadline;
              
                    if(acceptance.ServiceRequest.UTCDate == null)
                    {
                        deadline = acceptance.ServiceRequest.CompletionDeadline;
                        
                    }
                    else
                    {
                        deadline = acceptance.ServiceRequest.UTCDate;
                        Time = DateTime.UtcNow;
                        
                    }
                    if (Time > deadline)
                    {
                        return RedirectToAction("Expired", "ServiceRequests", new { id = acceptance.ServiceRequest.ID });
                    }

                    DateTime timeUtcAccepted = DateTime.UtcNow;
                    acceptance.AcceptanceDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcAccepted, Zone);
                    DateTime myDt = DateTime.SpecifyKind(timeUtcAccepted, DateTimeKind.Utc);
                    bool dst = Zone.IsDaylightSavingTime(myDt);

                    if (dst == true)
                    {
                        acceptance.AcceptanceAmbigTime = "DST";
                    }

                    if (dst == false)
                    {
                        acceptance.AcceptanceAmbigTime = "STD";
                    }

                    db.ContractorAcceptances.Add(acceptance);
                    db.SaveChanges();
                    NotifyAcceptance(acceptance);

                }
            }
            db.SaveChanges();
            return RedirectToAction("Details", "ContractorAcceptances", new { id = acceptance.ID });
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
            if ((identity == serviceRequest.Contractor.UserId) && (serviceRequest.CompletionDate != null))
            {
                return RedirectToAction("Already_Confirmed_Completion", new { id = serviceRequest.ID });
            }
            return View(serviceRequest);
        }

        public ActionResult Already_Confirmed_Completion(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if (serviceRequest.Contractor.UserId != identity)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmCompletion(int? ID)
        {

            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            DateTime timeUtcCompleted = DateTime.UtcNow;
            TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
            //DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
            serviceRequest.CompletionDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcCompleted, Zone);
            DateTime myDt = DateTime.SpecifyKind(timeUtcCompleted, DateTimeKind.Utc);
            bool dst = Zone.IsDaylightSavingTime(myDt);

            if (dst == true)
            {
                serviceRequest.CompletionAmbigTime = "DST";
            }

            if (dst == false)
            {
                serviceRequest.CompletionAmbigTime = "STD";
            }
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

        //public ActionResult PaymentSuccess(int? ID)
        //{

        //    string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

        //    if (identity == null)
        //    {
        //        return RedirectToAction("Unauthorized_Access", "Home");
        //    }

        //    ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

        //    if (serviceRequest == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    if ((identity != serviceRequest.Homeowner.UserId) && (!this.User.IsInRole("Admin")))
        //    {
        //        return RedirectToAction("Unauthorized_Access", "Home");
        //    }

        //    if(serviceRequest.PayPalListenerModelID == null || serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.payment_status != "Completed")
        //    {
        //        return RedirectToAction("Unauthorized_Access", "Home");
        //    }

        //    //serviceRequest.ContractorPaid = true;
        //    db.SaveChanges();

        //    //var myMessage = new SendGrid.SendGridMessage();
        //    //string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
        //    //string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
        //    //string url = "http://localhost:37234/ServiceRequests/AddReview/" + serviceRequest.ID;
        //    //myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
        //    //myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
        //    //myMessage.Subject = "Payment Confirmed!!";
        //    //String message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + "Thank you for using Work Warriors!  You have completed payment for the following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + serviceRequest.Price + "<br>" + "<br>" + "Service Number: <br>"  + serviceRequest.Service_Number + "<br>" + "<br>" + "To review " + serviceRequest.Contractor.Username + "'s service, click on link below: <br><a href =" + url + "> Click Here </a>"; 
        //    //myMessage.Html = message;
        //    //var credentials = new NetworkCredential(name, pass);
        //    //var transportWeb = new SendGrid.Web(credentials);
        //    //transportWeb.DeliverAsync(myMessage);
        //    //Notify_Contractor_of_Payment(serviceRequest);

        //    return View(serviceRequest);

        //}

        public ActionResult PayPalSuccess(int? ID)
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

            if (serviceRequest.PaymentError != false)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if (serviceRequest.PayPalListenerModelID == null)
            {
                serviceRequest.PayPalListenerModel = new PayPalListenerModel();
                serviceRequest.PayPalListenerModel._PayPalCheckoutInfo = new PayPalCheckoutInfo();
                serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.payment_status = null;

            }

            //if (serviceRequest.PayPalListenerModelID == null || serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.payment_status != "Completed")
            //{
            //    return RedirectToAction("Unauthorized_Access", "Home");
            //}

            //serviceRequest.ContractorPaid = true;
            //db.SaveChanges();

            //var myMessage = new SendGrid.SendGridMessage();
            //string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            //string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            //string url = "http://localhost:37234/ServiceRequests/AddReview/" + serviceRequest.ID;
            //myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            //myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            //myMessage.Subject = "Payment Confirmed!!";
            //String message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + "Thank you for using Work Warriors!  You have completed payment for the following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + serviceRequest.Price + "<br>" + "<br>" + "Service Number: <br>"  + serviceRequest.Service_Number + "<br>" + "<br>" + "To review " + serviceRequest.Contractor.Username + "'s service, click on link below: <br><a href =" + url + "> Click Here </a>"; 
            //myMessage.Html = message;
            //var credentials = new NetworkCredential(name, pass);
            //var transportWeb = new SendGrid.Web(credentials);
            //transportWeb.DeliverAsync(myMessage);
            //Notify_Contractor_of_Payment(serviceRequest);

            return View(serviceRequest);

        }
        public ActionResult getPayPalInfo(int? ID)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if(serviceRequest.PayPalListenerModelID == null)
            {
                return Json(new { success = true, found = false },
                JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = true, found = true, status = serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.payment_status, tx_id = serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.txn_id, date = serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.TrxnDate.ToString(), tZone = serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.Timezone },
                JsonRequestBehavior.AllowGet);
            }

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
            //if((serviceRequest.PaymentAttempted != true) || (serviceRequest.PayPalListenerModelID != null && serviceRequest.PayPalListenerModel._PayPalCheckoutInfo.payment_status == "Completed"))
            //{
            //    return RedirectToAction("Unauthorized_Access", "Home");
            //}
            if(serviceRequest.PaymentError != true)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }


            return View(serviceRequest);

        }

        public ActionResult AddReview(int? ID)
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

            if(serviceRequest.ContractorReviewID != null)
            {
                return RedirectToAction("Already_Reviewed", new { id = serviceRequest.ID });
            }

            return View(serviceRequest);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReview([Bind(Include = "ID,Review,Rating")] ContractorReview contractorReview, int ID)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest.ContractorReviewID != null)
            {
                return RedirectToAction("Already_Reviewed", new { id = serviceRequest.ID });
            }

            if (ModelState.IsValid)
            {
                //contractorReview.ReviewDate = DateTime.Now;
                DateTime timeUtcPosted = DateTime.UtcNow;
                TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
                contractorReview.ReviewDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPosted, Zone);
                contractorReview.ContractorID = serviceRequest.ContractorID;
                db.ContractorReviews.Add(contractorReview);
                db.SaveChanges();
                serviceRequest.ContractorReviewID = contractorReview.ID;
                db.SaveChanges();
                foreach (var contractor in db.Contractors)
                {
                    if(contractor.ID == contractorReview.ContractorID)
                    {
                        UpdateRating(contractor);
                    }
                }
                db.SaveChanges();
                Notify_Contractor_of_Review(contractorReview, serviceRequest);
                return RedirectToAction("HomeownerReviewDetails", "Homeowners", new { id = serviceRequest.ID });
            }
            return View(serviceRequest);
        }

        public void UpdateRating(Contractor contractor)
        {
            List<double> ratings;
            ratings = (from x in db.ContractorReviews
                       where x.ContractorID == contractor.ID
                       select x.Rating).ToList();
            if (ratings.Count > 0)
            {
                contractor.Rating = ratings.Average();
            }
            else
            {
                contractor.Rating = null;
            }

        }
        
        public ActionResult AddResponseView(AddResponseViewModel model, int? ID)
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

            if (serviceRequest.ContractorReviewID == null)
            {
                return HttpNotFound();
            }

            if (serviceRequest.ContractorReview.ReviewResponseID != null)
            {
                return RedirectToAction("Already_Responded", new { id = serviceRequest.ID });
            }

            model.ContractorUsername = serviceRequest.Contractor.Username;
            model.HomeownerUsername = serviceRequest.Homeowner.Username;
            model.Rating = serviceRequest.ContractorReview.Rating;
            model.Review = serviceRequest.ContractorReview.Review;
            model.ReviewDate = serviceRequest.ContractorReview.ReviewDate;
            model.Service_Number = serviceRequest.Service_Number;
            ModelState.Clear();

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddResponse (AddResponseViewModel model, int ID)
        {
            ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

            if (serviceRequest.ContractorReview.ReviewResponseID != null)
            {
                return RedirectToAction("Already_Responded", new { id = serviceRequest.ID });
            }

            ReviewResponse reviewResponse = new ReviewResponse();

            if (ModelState.IsValid)
            {
                //reviewResponse.ResponseDate = DateTime.Now;

                DateTime timeUtcPosted = DateTime.UtcNow;
                TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
                reviewResponse.ResponseDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPosted, Zone);
                reviewResponse.ContractorID = serviceRequest.ContractorID;
                reviewResponse.Response = model.Response;
                db.ReviewResponses.Add(reviewResponse);
                serviceRequest.ContractorReview.ReviewResponseID = reviewResponse.ID;
                db.SaveChanges();
                return RedirectToAction("SeeContractorReviews", "Contractors", new { id = serviceRequest.ContractorID });
            }
            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unlock()
        {
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            serviceRequest.PaymentAttempted = true;
            db.SaveChanges();

            return Json(new { success = true },
                JsonRequestBehavior.AllowGet);
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

            if (serviceRequest.CompletionDate == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if ((identity != serviceRequest.Homeowner.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            if (serviceRequest.PaymentError == false)
            {
                return RedirectToAction("Already_Paid", new { id = serviceRequest.ID });
            }

            //if ((identity == serviceRequest.Homeowner.UserId) && (serviceRequest.PayPalListenerModelID != null))
            //{
            //    return RedirectToAction("Already_Paid", new { id = serviceRequest.ID });
            //}

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
            string successUrl = "http://localhost:37234/ServiceRequests/PayPalSuccess/" + serviceRequest.ID;
            string failureUrl = "http://5ecef778.ngrok.io/ServiceRequests/PaymentFailure/" + serviceRequest.ID;
            string returnUrl = successUrl;
            string cancelUrl = failureUrl;
            string currencyCode = "USD";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            PayRequest payRequest = new PayRequest(requestEnvelope, actionType, cancelUrl, currencyCode, receiverList, returnUrl);
            payRequest.ipnNotificationUrl = "http://replaceIpnUrl.com";
            string memo = "Invoice = " + serviceRequest.Service_Number;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult getDetails()
        {
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            string key = (form["paykey"]);
            string payname = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\payname.txt");
            string paypass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\paypass.txt");
            string sig = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\sig.txt");
            string appid = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\appid.txt");
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", "sandbox");
            sdkConfig.Add("account1.apiUsername", payname); //PayPal.Account.APIUserName
            sdkConfig.Add("account1.apiPassword", paypass); //PayPal.Account.APIPassword
            sdkConfig.Add("account1.apiSignature", sig); //.APISignature
            sdkConfig.Add("account1.applicationId", appid); //.ApplicatonId
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            AdaptivePaymentsService adaptivePaymentsService = new AdaptivePaymentsService(sdkConfig);
            RequestEnvelope requestEnvelope = new RequestEnvelope("en_US");
            PaymentDetailsRequest paymentDetailsRequest = new PaymentDetailsRequest(requestEnvelope);
            paymentDetailsRequest.payKey = key;
            PaymentDetailsResponse paymentDetailsResponse = adaptivePaymentsService.PaymentDetails(paymentDetailsRequest);
            try {
                if ((paymentDetailsResponse.paymentInfoList.paymentInfo[0].transactionStatus == "COMPLETED") || (paymentDetailsResponse.paymentInfoList.paymentInfo[0].transactionStatus == "PENDING") || (paymentDetailsResponse.paymentInfoList.paymentInfo[0].transactionStatus == "PROCESSING"))
                {
                    ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
                    serviceRequest.PaymentError = false;
                    db.SaveChanges();
                    return Json(new { success = "go", id = id },
                    JsonRequestBehavior.AllowGet);
                }

                else if (paymentDetailsResponse.paymentInfoList.paymentInfo[0].transactionStatus == null)
                {
                    return Json(new { success = "stay", id = id },
                    JsonRequestBehavior.AllowGet);
                }

                else
                {
                    ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
                    serviceRequest.PaymentError = true;
                    db.SaveChanges();
                    return Json(new { success = "fail", id = id },
                    JsonRequestBehavior.AllowGet);
                }
            }

            catch
            {
                ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
                serviceRequest.PaymentError = true;
                db.SaveChanges();
                return Json(new { success = "fail", id = id },
                JsonRequestBehavior.AllowGet);
            }

            
        }

        public ActionResult Already_Paid(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if (serviceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(serviceRequest);
        }

        public ActionResult Already_Reviewed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if (serviceRequest.Homeowner.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(serviceRequest);
        }

        public ActionResult Already_Responded(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);

            if (serviceRequest == null)
            {
                return HttpNotFound();
            }

            if (serviceRequest.Contractor.UserId != identity)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(serviceRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddContractorPhotos()
        {
            var files = Enumerable.Range(0, Request.Files.Count).Select(i => Request.Files[i]);
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            serviceRequest.CompletedServiceRequestFilePaths = new List<CompletedServiceRequestFilePath>();
            int fileList = 0;
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!this.User.IsInRole("Contractor"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            foreach (var path in db.CompletedServiceRequestFilePaths.ToList())
            {
                if (path.ServiceRequestID == serviceRequest.ID)
                    fileList++;
            }

            foreach (var file in files)
            {
                fileList++;
            }

            if (fileList > 4)
            {
                return Json(new { success = true, tooManyPics = true, id = serviceRequest.ID },
                JsonRequestBehavior.AllowGet);
            }

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
            return Json(new { success = true, id = serviceRequest.ID },
            JsonRequestBehavior.AllowGet);

        }

        public void postServiceRequest(ServiceRequest serviceRequest, List<Contractor> emailList)
        {
            
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            foreach (var i in emailList)
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
            string url2 = "http://localhost:37234/Contractors/SeeContractorReviews/" + contractorAcceptance.ContractorID;
            string message = "Hello " + contractorAcceptance.ServiceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + contractorAcceptance.Contractor.Username + " has offered to perform your following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + contractorAcceptance.ServiceRequest.Address.Street + "<br>" + contractorAcceptance.ServiceRequest.Address.City + "<br>" + contractorAcceptance.ServiceRequest.Address.State + "<br>" + contractorAcceptance.ServiceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + contractorAcceptance.ServiceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + contractorAcceptance.ServiceRequest.Price + "<br>" + "<br>" + "Must be completed by: <br>" + contractorAcceptance.ServiceRequest.CompletionDeadline + "<br>" + "<br>" + "Date Posted: <br>" + contractorAcceptance.ServiceRequest.PostedDate + "<br>" + "<br>" + "To confirm contractor, click on link below: <br><a href =" + url + "> Click Here </a>" + "<br>" + "<br>" + "See " + contractorAcceptance.Contractor.Username + "'s reviews by clicking on the link below: <br><a href =" + url2 + "> Click Here </a>";
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
        public void Notify_Contractor_of_Review(ContractorReview contractorReview, ServiceRequest serviceRequest)
        {
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(contractorReview.Contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "You've been reviewed!!";
            string url = "http://localhost:37234/ServiceRequests/AddResponseView/" + serviceRequest.ID;
            string message = "Hello " + contractorReview.Contractor.FirstName + "," + "<br>" + "<br>" + serviceRequest.Homeowner.Username + " has given given your service a " + contractorReview.Rating + " star rating." + "<br>" + "<br>" + "Additionally, the following review has been posted:" + "<br > " + " <br >" + contractorReview.Review + "<br >" + "<br>" + "To respond to this review, please click on the following link: <br><a href =" + url + "> Click Here </a>";
            myMessage.Html = message; 
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }

        [AllowAnonymous]
        public JsonResult DateCheckManual(DateTime? CompletionDeadline, int ID, DateTime? UTCDate)
       {
            if (CompletionDeadline.HasValue)
            {
                ServiceRequest serviceRequest = db.ServiceRequests.Find(ID);

                DateTime timeUtc = DateTime.UtcNow;
                TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
                DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
                var timeToCheck = CompletionDeadline.Value;
                DateTime deadline;

                try
                {
                    deadline = TimeZoneInfo.ConvertTime(timeToCheck, Zone, Zone);

                }
                catch
                {
                    return Json("You have entered an invalid time.", JsonRequestBehavior.AllowGet);
                }

                bool ambig = Zone.IsAmbiguousTime(deadline);

                if ((ambig == true) && (UTCDate == null))
                {
                    TimeSpan[] offsets;
                    DateTime utcDateStandard;
                    DateTime utcDateDST;
                    offsets = Zone.GetAmbiguousTimeOffsets(deadline);
                    utcDateStandard = DateTime.SpecifyKind(deadline - offsets[0], DateTimeKind.Utc);
                    utcDateDST = DateTime.SpecifyKind(deadline - offsets[1], DateTimeKind.Utc);
                    Response.AddHeader("Z-ID", "ambigError");
                    Response.AddHeader("Standard-ID", utcDateStandard.ToString());
                    Response.AddHeader("DST-ID", utcDateDST.ToString());
                    return Json("You have entered an ambiguous time.", JsonRequestBehavior.AllowGet);
                }
                if ((ambig == true) && (UTCDate != null))
                {
                    if (DateTime.UtcNow > UTCDate)
                    {
                        return Json("The completion deadline must be later than the current time now.", JsonRequestBehavior.AllowGet);
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                if (deadline < Time)
                {
                    return Json("The completion deadline must be later than the current time now.", JsonRequestBehavior.AllowGet);
                }

                if (deadline > Time)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
                return null;
            
        }

        [AllowAnonymous]
        public JsonResult DateCheck(DateTime? CompletionDeadline, string checkStreet, string checkCity, string checkState, DateTime? UTCDate)
        {
          Checker checker = new Checker() { CompletionDeadline = CompletionDeadline, Street = checkStreet, City = checkCity, State = checkState };

            if (checker.Street == "" || checker.City == "" || checker.State == "" )
            {
                return Json("Please specify a full address to we can validate your Completion Deadline", JsonRequestBehavior.AllowGet);
            }

            Timezone zone = new Timezone();
            checker = zone.checkDate(checker);

            if (checker.Invalid == true)
            {
                return Json("You have entered an invalid time.", JsonRequestBehavior.AllowGet);
            }

            if ((checker.Ambig == true) && (UTCDate == null))
            {
                DateTime dt;
                TimeZoneInfo Zone;
                TimeSpan[] offsets;
                DateTime utcDateStandard;
                DateTime utcDateDST;
                //DateTime ambigConvertStandard;

                if (checker.CompletionDeadline.HasValue)
                {
                    dt = checker.CompletionDeadline.Value;
                    Zone = TimeZoneInfo.FindSystemTimeZoneById(checker.Timezone);
                    offsets = Zone.GetAmbiguousTimeOffsets(dt);
                    utcDateStandard = DateTime.SpecifyKind(dt - offsets[0], DateTimeKind.Utc);
                    utcDateDST = DateTime.SpecifyKind(dt - offsets[1], DateTimeKind.Utc);
                    //ambigConvertStandard = TimeZoneInfo.ConvertTimeFromUtc(utcDateStandard, Zone);
                    Response.AddHeader("Z-ID", "ambigError");
                    Response.AddHeader("Standard-ID", utcDateStandard.ToString());
                    Response.AddHeader("DST-ID", utcDateDST.ToString());
                    return Json("You have entered an ambiguous time.", JsonRequestBehavior.AllowGet);
                }

                //Response.AddHeader("Y-ID", checker.Ambig.ToString());
                Response.AddHeader("Z-ID", "ambigError");
                return Json("You have entered an ambiguous time.", JsonRequestBehavior.AllowGet);
            }

            if (checker != null && checker.CompletionDeadline.HasValue && checker.Timezone != null)
            {
                TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(checker.Timezone);
                var dt = checker.CompletionDeadline.Value;
                DateTime Time = TimeZoneInfo.ConvertTime(dt, Zone, Zone);
                bool dst = Time.IsDaylightSavingTime();
            }

            if (checker.Found == false)
            {
                return Json("Your address could not be found to verify your timezone", JsonRequestBehavior.AllowGet);
            }

            if ((checker.Ambig == true) && (UTCDate != null))
            {
                if(DateTime.UtcNow > UTCDate)
                {
                    return Json("The completion deadline must be later than the current time now.", JsonRequestBehavior.AllowGet);
                }
                Response.AddHeader("X-ID", checker.Timezone);
                Response.AddHeader("Continue", "OK");
                return Json(true, JsonRequestBehavior.AllowGet);
            }


            if (checker.OK == false)
            {
                return Json("The completion deadline must be later than the current time now.", JsonRequestBehavior.AllowGet);
            }

            if (checker.OK == true)
            {
                Response.AddHeader("X-ID", checker.Timezone);
                Response.AddHeader("Continue", "OK");
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("The completion deadline must be later than the current time now.", JsonRequestBehavior.AllowGet);
        }

        public List<Contractor> GetDistance(ServiceRequest serviceRequest)
        {

            string jobLocation = serviceRequest.Address.FullAddress;
            List<Contractor> contractorsToMail = new List<Contractor>();
            string source = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\distance.txt");
            foreach (var contractor in db.Contractors.ToList())
            {
                if (contractor.Inactive == false)
                {
                    string url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + jobLocation + "&destinations=" + contractor.Address.FullAddress + "&key=" + source;
                    WebRequest request = WebRequest.Create(url);
                    request.Credentials = CredentialCache.DefaultCredentials;
                    WebResponse response = request.GetResponse();
                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Parent result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Parent>(responseFromServer);
                    reader.Close();
                    response.Close();
                    serviceRequest.Address.googleAddress = result.origin_addresses[0];
                    db.SaveChanges();
                    if (result.rows[0].elements[0].status == "OK")
                    {
                        if ((result.rows[0].elements[0].distance.value) * 0.000621371 <= contractor.travelDistance)
                        {
                            contractorsToMail.Add(contractor);
                        }
                    }
                }

            }
            return (contractorsToMail);
        }
    }
}



