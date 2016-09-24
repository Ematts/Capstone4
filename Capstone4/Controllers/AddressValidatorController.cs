using Capstone4.Models;
using Microsoft.AspNet.Identity;
using SharpShip.UPS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Capstone4.Controllers
{
    public class AddressValidatorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult getAddValStatus(string street, string city, string state, string zip)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            EasyPost.ClientManager.SetCurrent("wGW1bI8SYpamubvkDKNkFw");
            EasyPost.Address address = new EasyPost.Address()
            {
                company = "",
                street1 = street,
                street2 = "",
                city = city,
                state = state,
                country = "US",
                zip = zip,
                verify = new List<string>() { "delivery" }
            };
            if (address == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            address.Create();

            if (address.verifications.delivery.success)
            {

                return Json(new { success = true, street = street, validated = address.verifications.delivery.success },
               JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = true, street = street, validated = address.verifications.delivery.success, errors = address.verifications.delivery.errors },
               JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult RunStreetLevelValidation(string street, string city, string state, string zip)
        {


            StreetLevelAddressValidator validator = new StreetLevelAddressValidator("7D15274E4E2A07DE", "Honeybump20!", "pennywise79");
            SharpShip.Entities.Address myAddress = new SharpShip.Entities.Address()
            {
                AddressLine1 = street,
                City = city,
                StateProvince = state,
                CountryCode = "US",
                PostalCode = zip
            };

            List<SharpShip.Entities.Address> results = new List<SharpShip.Entities.Address>();
            var isvalid = validator.Validate(myAddress, ref results);


            if (isvalid == true && results.Count > 0)
            {
                return Json(new { success = true, street = street, validated = isvalid, results = results },
               JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { success = true, street = street, validated = false },
               JsonRequestBehavior.AllowGet);
            }
        }

        //public ActionResult sendContractorMail()
        //{

        //    db = new ApplicationDbContext();
        //    var myMessage = new SendGrid.SendGridMessage();
        //    string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
        //    string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");

        //    List<String> recipients = new List<String> { };

        //    foreach (var i in db.Contractors)
        //    {

        //        recipients.Add(i.email);

        //    };

        //    myMessage.AddTo(recipients);
        //    myMessage.From = new MailAddress("monsymonster@msn.com", "Joe Johnson");
        //    myMessage.Subject = "New Service Request Posting!!";
        //    myMessage.Text = "Service request:";
        //    var credentials = new NetworkCredential(name, pass);
        //    var transportWeb = new SendGrid.Web(credentials);
        //    transportWeb.DeliverAsync(myMessage);

        //    return RedirectToAction("About", "Home");

        //}

        [HttpPost]
        public ActionResult ManualValidation()
        {
            var files = Enumerable.Range(0, Request.Files.Count).Select(i => Request.Files[i]);            
            var form = Request.Form;
            string city = (form["Address.City"]);
            string state = (form["Address.State"]);
            string zip = (form["Address.Zip"]);
            string street = (form["Address.Street"]);
            string description = (form["Description"]);
            string priceString = (form["Price"]);
            decimal price = Convert.ToDecimal(priceString);
            string dateString = (form["CompletionDeadline"]);
            DateTime completionDeadline = Convert.ToDateTime(dateString);
            bool vacant = form["Address.vacant"].Contains("true");
            bool validated = form["Address.validated"].Contains("true");
            bool inactive = form["Inactive"].Contains("true");

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!this.User.IsInRole("Admin") && (!this.User.IsInRole("Homeowner")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Address address = new Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
            ServiceRequest serviceRequest = new ServiceRequest() { Description = description, Price = price, CompletionDeadline = completionDeadline, Inactive = inactive };
            serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();

            foreach (var i in db.Homeowners)
            {
                if (i.UserId == identity)
                {

                    serviceRequest.HomeownerID = i.ID;

                }
            }

            foreach (var i in db.Addresses.ToList())
            {
                if (i.FullAddress == address.FullAddress)
                {
                    serviceRequest.AddressID = i.ID;
                }
            }

            if (serviceRequest.AddressID == null)
            {
                serviceRequest.AddressID = address.ID;
                db.Addresses.Add(address);
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

            serviceRequest.PostedDate = DateTime.Now;
            db.ServiceRequests.Add(serviceRequest);
            db.SaveChanges();
            serviceRequest.Service_Number = serviceRequest.ID;
            db.SaveChanges();

            return Json(new { success = true, id = serviceRequest.ID },
                 JsonRequestBehavior.AllowGet);
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
