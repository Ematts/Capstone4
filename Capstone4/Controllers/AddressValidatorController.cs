using Capstone4.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PayPal.AdaptiveAccounts;
using PayPal.AdaptiveAccounts.Model;
using SharpShip.UPS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Capstone4.Controllers
{
    public class AddressValidatorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        [HttpGet]
        public ActionResult getAddValStatus(string street, string city, string state, string zip)
        {
            Address newAdd = new Address() { Street = street, City = city, State = state, Zip = zip };

            foreach(var i in db.Addresses.ToList())
            {
                if((newAdd.FullAddress == i.FullAddress) && (i.validated == true))
                {
                    return Json(new { success = true, validated = true, vacant = i.vacant },
               JsonRequestBehavior.AllowGet);
                }
            }

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            string source = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\easypost.txt");
            EasyPost.ClientManager.SetCurrent(source);
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


            string source1 = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\ups1.txt");
            string source2 = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\ups2.txt");
            string source3 = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\ups3.txt");
            StreetLevelAddressValidator validator = new StreetLevelAddressValidator(source1, source2, source3);
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
        [ValidateAntiForgeryToken]
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
            string tzone = (form["Timezone"]);
            string Ambig = (form["AmbigTime"]);
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

            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!this.User.IsInRole("Homeowner"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Address address = new Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
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

            foreach (var i in db.Addresses.ToList())
            {
                if (i.FullAddress == address.FullAddress)
                {
                    serviceRequest.AddressID = i.ID;
                    serviceRequest.Address = i;
                    serviceRequest.Address.validated = address.validated;
                    serviceRequest.Address.vacant = address.vacant;
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
            serviceRequest.Posted = false;
            serviceRequest.NeedsManualValidation = true;
            
            //DateTime timeUtcPosted = DateTime.UtcNow;
            //serviceRequest.PostedDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPosted, Zone);
            //DateTime myDt = DateTime.SpecifyKind(timeUtcPosted, DateTimeKind.Utc);
            //bool dst = Zone.IsDaylightSavingTime(myDt);

            //if (dst == true)
            //{
            //    serviceRequest.PostedAmbigTime = "DST";
            //}

            //if (dst == false)
            //{
            //    serviceRequest.PostedAmbigTime = "STD";
            //}

            db.ServiceRequests.Add(serviceRequest);
            db.SaveChanges();
            serviceRequest.Service_Number = serviceRequest.ID;
            db.SaveChanges();

            return Json(new { success = true, id = serviceRequest.ID },
                 JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManualValidationEdit()
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

            Address address = new Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };

            ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
            if (serviceRequest == null)
            {
                return HttpNotFound();
            }
            serviceRequest.Timezone = tzone;
            serviceRequest.AmbigTime = Ambig;
            serviceRequest.UTCDate = utc;
            serviceRequest.CompletionDeadline = completionDeadline;
            serviceRequest.Service_Number = serviceRequest.ID;
            serviceRequest.Price = price;
            serviceRequest.Description = description;
            var addressToCheck = serviceRequest.Address;
            serviceRequest.ServiceRequestFilePaths = new List<ServiceRequestFilePath>();
            bool addressAssigned = false;

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
                
                foreach(var i in db.Addresses.ToList())
                {
                    if(i.FullAddress == address.FullAddress)
                    {
                        serviceRequest.AddressID = i.ID;
                        db.SaveChanges();
                        addressAssigned = true;
                    }

                }

            }

            if (serviceRequest.AddressID == null)
            {
                db.Addresses.Add(address);
                serviceRequest.AddressID = address.ID;
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
                        serviceRequest.Address.validated = address.validated;
                        serviceRequest.Address.vacant = address.vacant;
                        db.SaveChanges();
                        addressAssigned = true;
                    }
                }

                if (addressAssigned == false)
                {
                    db.Addresses.Add(address);
                    serviceRequest.AddressID = address.ID;
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

            }

            //serviceRequest.Description = description;
            serviceRequest.Posted = false;
            serviceRequest.PostedDate = null;
            serviceRequest.NeedsManualValidation = true;
            //serviceRequest.Price = price;
            //serviceRequest.CompletionDeadline = completionDeadline;
            serviceRequest.Address.vacant = vacant;
            serviceRequest.Inactive = inactive;
            serviceRequest.Address.validated = validated;
            db.SaveChanges();

            return Json(new { success = true, id = serviceRequest.ID },
                 JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ManualValidationHomeownerCreate()
        {

            var form = Request.Form;
            string username = (form["Screen_name"]);
            string firstname = (form["FirstName"]);
            string lastname = (form["LastName"]);
            string email = (form["Email"]);
            string password = (form["Password"]);
            string city = (form["City"]);
            string state = (form["State"]);
            string zip = (form["Zip"]);
            string street = (form["Street"]);
            bool vacant = form["vacant"].Contains("true");
            bool validated = form["validated"].Contains("true");
            bool inactive = form["Inactive"].Contains("true");
            var user = new ApplicationUser() { Email = email, UserName = email };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var role = db.Roles.Find("0");
                UserManager.AddToRole(user.Id, role.Name);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                Address address = new Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
                Homeowner homeowner = new Homeowner() { Username = username, FirstName = firstname, LastName = lastname, UserId = user.Id, Inactive = inactive, NeedsManualValidation = true };
                db.Homeowners.Add(homeowner);

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        homeowner.AddressID = i.ID;
                        homeowner.Address = i;
                        homeowner.Address.validated = address.validated;
                        homeowner.Address.vacant = address.vacant;
                    }
                }

                if (homeowner.AddressID == null)
                {
                    homeowner.AddressID = address.ID;
                    db.Addresses.Add(address);
                }

                db.SaveChanges();

                return Json(new { success = true, id = homeowner.ID },
                     JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManualValidationHomeownerEdit()
        {
            
            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            string username = (form["Username"]);
            string firstname = (form["FirstName"]);
            string lastname = (form["LastName"]);
            string city = (form["Address.City"]);
            string state = (form["Address.State"]);
            string zip = (form["Address.Zip"]);
            string street = (form["Address.Street"]);
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

            Homeowner homeowner = db.Homeowners.Find(id);
            homeowner.Username = username;
            homeowner.FirstName = firstname;
            homeowner.LastName = lastname;
            var addressToCheck = homeowner.Address;
            bool addressAssigned = false;

            if (homeowner == null)
            {
                return HttpNotFound();
            }

            if (homeowner.AddressID == null)
            {

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        homeowner.AddressID = i.ID;
                        db.SaveChanges();
                        addressAssigned = true;
                    }

                }

            }

            if (homeowner.AddressID == null)
            {
                db.Addresses.Add(address);
                homeowner.AddressID = address.ID;
                db.SaveChanges();
                addressAssigned = true;
            }



            if (homeowner.AddressID != null && addressAssigned == false)
            {

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        homeowner.AddressID = i.ID;
                        homeowner.Address.validated = address.validated;
                        homeowner.Address.vacant = address.vacant;
                        db.SaveChanges();
                        addressAssigned = true;
                    }
                }

                if (addressAssigned == false)
                {
                    db.Addresses.Add(address);
                    homeowner.AddressID = address.ID;
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

            }
            homeowner.NeedsManualValidation = true;
            homeowner.Address.vacant = vacant;
            homeowner.Inactive = inactive;
            homeowner.Address.validated = validated;
            db.SaveChanges();

            return Json(new { success = true, id = homeowner.ID },
                 JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ManualValidationContractorCreate()
        {

            var form = Request.Form;
            string username = (form["Screen_name"]);
            string firstname = (form["FirstName"]);
            string lastname = (form["LastName"]);
            string email = (form["Email"]);
            string password = (form["Password"]);
            string city = (form["City"]);
            string state = (form["State"]);
            string zip = (form["Zip"]);
            string street = (form["Street"]);
            string travelDistanceString = (form["travelDistance"]);
            double travelDistance = Convert.ToDouble(travelDistanceString);
            bool vacant = form["vacant"].Contains("true");
            bool validated = form["validated"].Contains("true");
            bool inactive = form["Inactive"].Contains("true");
            var user = new ApplicationUser() { Email = email, UserName = email };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var role = db.Roles.Find("1");
                UserManager.AddToRole(user.Id, role.Name);
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                Address address = new Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
                Contractor contractor = new Contractor() { Username = username, FirstName = firstname, LastName = lastname, UserId = user.Id, travelDistance = travelDistance, Inactive = inactive, NeedsManualValidation = true };
                db.Contractors.Add(contractor);

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        contractor.AddressID = i.ID;
                        contractor.Address = i;
                        contractor.Address.validated = address.validated;
                        contractor.Address.vacant = address.vacant;
                    }
                }

                if (contractor.AddressID == null)
                {
                    contractor.AddressID = address.ID;
                    db.Addresses.Add(address);
                }

                db.SaveChanges();

                return Json(new { success = true, id = contractor.ID },
                     JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ManualValidationContractorEdit()
        {

            var form = Request.Form;
            int id = Convert.ToInt16(form["ID"]);
            string username = (form["Username"]);
            string firstname = (form["FirstName"]);
            string lastname = (form["LastName"]);
            string city = (form["Address.City"]);
            string state = (form["Address.State"]);
            string zip = (form["Address.Zip"]);
            string street = (form["Address.Street"]);
            string travelDistanceString = (form["travelDistance"]);
            double travelDistance = Convert.ToDouble(travelDistanceString);
            bool vacant = form["Address.vacant"].Contains("true");
            bool validated = form["Address.validated"].Contains("true");
            bool inactive = form["Inactive"].Contains("true");

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!this.User.IsInRole("Admin") && (!this.User.IsInRole("Contractor")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Address address = new Address() { Street = street, City = city, State = state, Zip = zip, vacant = vacant, validated = validated };
            Contractor contractor = db.Contractors.Find(id);
            contractor.Username = username;
            contractor.FirstName = firstname;
            contractor.LastName = lastname;
            contractor.travelDistance = travelDistance;
            var addressToCheck = contractor.Address;
            bool addressAssigned = false;

            if (contractor == null)
            {
                return HttpNotFound();
            }

            if (contractor.AddressID == null)
            {

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        contractor.AddressID = i.ID;
                        db.SaveChanges();
                        addressAssigned = true;
                    }

                }

            }

            if (contractor.AddressID == null)
            {
                db.Addresses.Add(address);
                contractor.AddressID = address.ID;
                db.SaveChanges();
                addressAssigned = true;
            }



            if (contractor.AddressID != null && addressAssigned == false)
            {

                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        contractor.AddressID = i.ID;
                        contractor.Address.validated = address.validated;
                        contractor.Address.vacant = address.vacant;
                        db.SaveChanges();
                        addressAssigned = true;
                    }
                }

                if (addressAssigned == false)
                {
                    db.Addresses.Add(address);
                    contractor.AddressID = address.ID;
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

            }
            contractor.NeedsManualValidation = true;
            contractor.Address.vacant = vacant;
            contractor.Inactive = inactive;
            contractor.Address.validated = validated;
            db.SaveChanges();

            return Json(new { success = true, id = contractor.ID },
                 JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult VerifyPaypal(string firstName, string lastName, string email)
        {
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
            GetVerifiedStatusRequest request = new GetVerifiedStatusRequest();
            AccountIdentifierType accountIdentifierType = new AccountIdentifierType();
            RequestEnvelope requestEnvelope = new RequestEnvelope();
            requestEnvelope.errorLanguage = "en_US";
            accountIdentifierType.emailAddress = email;
            request.accountIdentifier = accountIdentifierType;
            request.requestEnvelope = requestEnvelope;
            request.matchCriteria = "NAME";
            request.firstName = firstName;
            request.lastName = lastName;
            AdaptiveAccountsService aas = new AdaptiveAccountsService(sdkConfig);
            GetVerifiedStatusResponse response = aas.GetVerifiedStatus(request);
            string status = response.accountStatus;
            
            if(status == "VERIFIED")
            {
                return Json(new { verified = true },
                JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json(new { verified = false },
                JsonRequestBehavior.AllowGet);
            }

            
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
