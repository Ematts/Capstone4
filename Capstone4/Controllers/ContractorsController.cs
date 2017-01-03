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
using PayPal.AdaptiveAccounts.Model;

namespace Capstone4.Controllers
{
    public class ContractorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contractors
        public ActionResult Index()
        {
            var contractors = db.Contractors.Include(c => c.Address).Include(c => c.ApplicationUser);
            foreach (var i in db.Contractors.ToList())
            {
                if (i.Address == null)
                {
                    Address address = new Address();
                    i.Address = address;
                }
            }
            return View(contractors.ToList());
        }

        // GET: Contractors/Details/5
        public ActionResult Details(int? id)
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

        public ActionResult ReviewsIndex(string autocomplete)
        {
            var contractors = db.Contractors.ToList();
            List<ContractorReviewsIndexViewModel> models = new List<ContractorReviewsIndexViewModel>();
            foreach(var contractor in contractors)
            {
                ContractorReviewsIndexViewModel model = new ContractorReviewsIndexViewModel { Username = contractor.Username, ID = contractor.ID };
                if (contractor.ContractorReviews.Count > 0)
                {
                    List<double> ratings;
                    ratings = (from x in contractor.ContractorReviews
                               select x.Rating).ToList();
                    model.Rating = ratings.Average();
                }
                else
                {
                    model.Rating = 0;
                }
                model.TotalRatings = contractor.ContractorReviews.Count();
                models.Add(model);

            }

            if (!String.IsNullOrEmpty(autocomplete))
            {
                List<int> displayList = GetList(autocomplete);

                foreach (var model in models.ToList())
                {

                    if(!displayList.Contains(model.ID))
                    {
                        models.Remove(model);
                    }

                }
            }

            //var UsernameLst = new List<string>();
            //var UsernameQry = from d in db.Contractors
            //                  orderby d.Username
            //                  select d.Username;
            //UsernameLst.AddRange(UsernameQry.Distinct());
            //ViewBag.clientUsername = new SelectList(UsernameLst);
            //if (!String.IsNullOrEmpty(searchString))
            //{
            //    foreach (var model in models.ToList())
            //    {
            //        if(!model.Username.StartsWith(searchString))
            //        {
            //            models.Remove(model);
            //        }

            //    }                       
            //}
            //if (!String.IsNullOrEmpty(clientUsername))
            //{
            //    foreach (var model in models.ToList())
            //    {
            //        if (model.Username != clientUsername)
            //        {
            //            models.Remove(model);
            //        }

            //    }
            //}
            return View(models);
        }
        public ActionResult SeeContractorReviews(SeeContractorReviewViewModel model, int? id)
        {
            Contractor contractor = db.Contractors.Find(id);
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (contractor == null)
            {
                return HttpNotFound();
            }
            if (contractor.ContractorReviews.Count > 0)
            {
                List<double> ratings;
                ratings = (from x in contractor.ContractorReviews
                           select x.Rating).ToList();
                model.OverallRating = ratings.Average();
            }
            else
            {
                model.OverallRating = 0;
            }
            model.ContractorUsername = contractor.Username;
            model.TotalRatings = contractor.ContractorReviews.Count();
            model.ServiceRequests = new List<ServiceRequest>();
            foreach (var request in db.ServiceRequests)
            {
                if(request.ContractorID == id && request.ContractorReviewID != null)
                {
                    
                    model.ServiceRequests.Add(request);
                    
                    if(request.ContractorReview.ReviewResponseID != null)
                    {
                        model.Response = request.ContractorReview.ReviewResponse.Response;
                    }
                    else
                    {
                        request.ContractorReview.ReviewResponse = new ReviewResponse() { ResponseDate = null, Response = null };
                    }
                    
                }
            }

            
            return View(model);
        }

        // GET: Contractors/Create
        public ActionResult Create()
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return RedirectToAction("Must_be_logged_in", "Contractors");
            }
            if (!this.User.IsInRole("Admin") && (!this.User.IsInRole("Contractor")))
            {
                return RedirectToAction("Must_be_logged_in", "Contractors");
            }

            foreach (var contractor in db.Contractors)
            {
                if (contractor.UserId == identity)
                {
                    return RedirectToAction("Already_Exists", "Contractors");
                }
            }


            return View();
        }

        // POST: Contractors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName,travelDistance")] Contractor contractor, Address address)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return RedirectToAction("Must_be_logged_in", "Contractors");
            }

            if (ModelState.IsValid)
            {

                db.Addresses.Add(address);
                AddressesController c = new AddressesController();
                c.getGoogleAddress(address);
                contractor.UserId = identity;
                contractor.AddressID = address.ID;
                db.Contractors.Add(contractor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contractor);
        }

        // GET: Contractors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contractor contractor = db.Contractors.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (contractor == null)
            {
                return HttpNotFound();
            }


            if ((contractor.UserId != identity) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }


            return View(contractor);
        }

        // POST: Contractors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName,Street,Rating,travelDistance,NeedsManualValidation")] Contractor contractor, Address address)
        {
            var formInfo = address;
            address = db.Addresses.Where(x => x.ID == contractor.AddressID).SingleOrDefault();
            var addressToCheck = address;

            if (ModelState.IsValid)
            {

                db.Entry(contractor).State = EntityState.Modified;
                if (contractor.AddressID == null)
                {
                    Address newAdd = new Address();
                    newAdd.Street = formInfo.Street;
                    newAdd.City = formInfo.City;
                    newAdd.State = formInfo.State;
                    newAdd.Street = formInfo.Street;
                    newAdd.Zip = formInfo.Zip;
                    newAdd.validated = address.validated;
                    newAdd.vacant = address.vacant;
                    foreach (var i in db.Addresses.ToList())
                    {
                        if (newAdd.FullAddress == i.FullAddress)
                        {
                            contractor.AddressID = i.ID;
                            contractor.Address = i;
                            contractor.Address.validated = formInfo.validated;
                            contractor.Address.vacant = formInfo.vacant;
                            if (contractor.Address.validated == true)
                            {
                                contractor.NeedsManualValidation = false;
                                contractor.Inactive = false;

                            }
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }

                    db.Addresses.Add(newAdd);
                    contractor.AddressID = newAdd.ID;
                    contractor.Address = newAdd;
                    if (contractor.Address.validated == true)
                    {
                        contractor.NeedsManualValidation = false;
                        contractor.Inactive = false;

                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }

                List<Address> ids = new List<Address>();
                foreach (var i in db.Addresses.ToList())
                {
                    if (formInfo.FullAddress == i.FullAddress)
                    {
                        contractor.AddressID = i.ID;
                        contractor.Address = i;
                        contractor.Address.validated = formInfo.validated;
                        contractor.Address.vacant = formInfo.vacant;
                        if (contractor.Address.validated == true)
                        {
                            contractor.NeedsManualValidation = false;
                            contractor.Inactive = false;

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
                        return RedirectToAction("Index");
                    }
                }

                Address newAdd1 = new Address();
                newAdd1.Street = formInfo.Street;
                newAdd1.City = formInfo.City;
                newAdd1.State = formInfo.State;
                newAdd1.Street = formInfo.Street;
                newAdd1.Zip = formInfo.Zip;
                newAdd1.validated = formInfo.validated;
                newAdd1.vacant = formInfo.vacant;
                db.Addresses.Add(newAdd1);
                contractor.AddressID = newAdd1.ID;
                contractor.Address = newAdd1;
                if (contractor.Address.validated == true)
                {
                    contractor.NeedsManualValidation = false;
                    contractor.Inactive = false;

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
                return RedirectToAction("Index");

            }

            return View(contractor);
        }

        // GET: Contractors/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Contractors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contractor contractor = db.Contractors.Find(id);

            foreach(var request in db.ServiceRequests.ToList())
            {
                if (request.ContractorReviewID != null)
                {
                    if (request.ContractorReview.ContractorID == id)
                    {
                        request.ContractorReviewID = null;
                    }
                }
            }
            foreach(var review in db.ContractorReviews.ToList())
            {
                if (review.ContractorID != null)
                {
                    if(review.ContractorID == id)
                    {
                        if(review.ReviewResponse != null)
                        {
                            db.ReviewResponses.Remove(review.ReviewResponse);
                        }
                        db.ContractorReviews.Remove(review);

                    }
                }
                
            }

            foreach (var acceptance in db.ContractorAcceptances.ToList())
            {
                if (acceptance.ContractorID != null)
                {
                    if (acceptance.ContractorID == id)
                    {
                        db.ContractorAcceptances.Remove(acceptance);
                    }
                }
            }

            foreach (var request in db.ServiceRequests.ToList())
            {
                if (request.ContractorID != null)
                {
                    if (request.ContractorID == id)
                    {
                        request.ContractorID = null;
                    }
                }
            }


            List<Models.Address> ids = new List<Models.Address>();
            Models.Address addressToCheck = db.Addresses.Where(x => x.ID == contractor.AddressID).SingleOrDefault();
           
            foreach (var user in db.Users)
            {
                if (contractor.UserId == user.Id)
                {
                    db.Contractors.Remove(contractor);
                    db.Users.Remove(user);
                }
            }
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

        public ActionResult VerifyPaypal(string firstName, string lastName, string accountID)
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
            GetVerifiedStatusRequest request = new GetVerifiedStatusRequest();
            AccountIdentifierType accountIdentifierType = new AccountIdentifierType();
           

            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Must_be_logged_in()
        {
            ViewBag.Message = "You must log in as a registered contractor to create a contractor profile";

            return View();
        }

        public ActionResult Already_Exists()
        {
            ViewBag.Message = "An account for this user already exists";

            return View();
        }


        public ActionResult Manual_Validate_Thank_You(int? id)
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

        public ActionResult SeeReviews(int? id)
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

        public List<int> GetList(String autocomplete)
        {

            List<int> contractorsToDisplay = new List<int>();
            string source = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\distance.txt");
            foreach (var contractor in db.Contractors.ToList())
            {

                string url = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + autocomplete + "&destinations=" + contractor.Address.FullAddress + "&key=" + source;
                WebRequest request = WebRequest.Create(url);
                request.Credentials = CredentialCache.DefaultCredentials;
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Parent result = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Parent>(responseFromServer);
                reader.Close();
                response.Close();
                db.SaveChanges();
                if (result.rows[0].elements[0].status == "OK")
                {
                    if ((result.rows[0].elements[0].distance.value) * 0.000621371 <= contractor.travelDistance)
                    {
                        contractorsToDisplay.Add(contractor.ID);
                    }

                }
            }
            return (contractorsToDisplay);
        }
    }
}
