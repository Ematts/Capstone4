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
    public class HomeownersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Homeowners
        public ActionResult Index()
        {
            var homeowners = db.Homeowners.Include(h => h.Address).Include(h => h.ApplicationUser);
            foreach (var i in db.Homeowners.ToList())
            {
                if (i.Address == null)
                {
                    Address address = new Address();
                    i.Address = address;
                }
            }
            return View(homeowners.ToList());
        }

        // GET: Homeowners/Details/5
        public ActionResult Details(int? id)
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

        // GET: Homeowners/Create
        public ActionResult Create()
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return RedirectToAction("Must_be_logged_in", "Homeowners");
            }

            if (!this.User.IsInRole("Admin") && (!this.User.IsInRole("Homeowner")))
            {
                return RedirectToAction("Must_be_logged_in", "Homeowners");
            }
            foreach (var homeowner in db.Homeowners)
            {
                if (homeowner.UserId == identity)
                {
                    return RedirectToAction("Already_Exists", "Homeowners");
                }
            }

            return View();
        }

        // POST: Homeowners/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName")] Homeowner homeowner, Address address)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return RedirectToAction("Must_be_logged_in", "Homeowners");
            }

            if (ModelState.IsValid)
            {
                foreach (var i in db.Addresses.ToList())
                {
                    if (i.FullAddress == address.FullAddress)
                    {
                        homeowner.AddressID = i.ID;
                        homeowner.Address = i;
                        homeowner.Address.validated = address.validated;
                        homeowner.Address.vacant = address.vacant;
                        homeowner.UserId = identity;
                        db.Homeowners.Add(homeowner);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                db.Addresses.Add(address);
                homeowner.UserId = identity;
                homeowner.AddressID = address.ID;
                db.Homeowners.Add(homeowner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(homeowner);
        }

        // GET: Homeowners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Homeowner homeowner = db.Homeowners.Find(id);
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (homeowner == null)
            {
                return HttpNotFound();
            }

            if((homeowner.UserId != identity) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            return View(homeowner);
        }

        // POST: Homeowners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName,Street,NeedsManualValidation")] Homeowner homeowner, Address address)
        {
            var formInfo = address;
            address = db.Addresses.Where(x => x.ID == homeowner.AddressID).SingleOrDefault();
            var addressToCheck = address; 
            if (ModelState.IsValid)
            {

                db.Entry(homeowner).State = EntityState.Modified;
                if (homeowner.AddressID == null)
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
                        if(newAdd.FullAddress == i.FullAddress)
                        {
                            homeowner.AddressID = i.ID;
                            homeowner.Address = i;
                            homeowner.Address.validated = formInfo.validated;
                            homeowner.Address.vacant = formInfo.vacant;
                            if(homeowner.Address.validated == true)
                            {
                                homeowner.NeedsManualValidation = false;
                                homeowner.Inactive = false;

                            }
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }

                    db.Addresses.Add(newAdd);
                    homeowner.AddressID = newAdd.ID;
                    homeowner.Address = newAdd;
                    if (homeowner.Address.validated == true)
                    {
                        homeowner.NeedsManualValidation = false;
                        homeowner.Inactive = false;

                    }
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                List<Address> ids = new List<Address>();
                foreach (var i in db.Addresses.ToList())
                {
                    if (formInfo.FullAddress == i.FullAddress)
                    { 
                        homeowner.AddressID = i.ID;
                        homeowner.Address = i;
                        homeowner.Address.validated = formInfo.validated;
                        homeowner.Address.vacant = formInfo.vacant;
                        if (homeowner.Address.validated == true)
                        {
                            homeowner.NeedsManualValidation = false;
                            homeowner.Inactive = false;

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
                homeowner.AddressID = newAdd1.ID;
                homeowner.Address = newAdd1;
                if (homeowner.Address.validated == true)
                {
                    homeowner.NeedsManualValidation = false;
                    homeowner.Inactive = false;

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

            return View(homeowner);
        }

        // GET: Homeowners/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Homeowners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Homeowner homeowner = db.Homeowners.Find(id);
            foreach (var request in db.ServiceRequests.ToList())
            {
                if (request.HomeownerID == id && request.ContractorID != null)
                {

                    if(request.ContractorReviewID != null)
                    {
                        if(request.ContractorReview.ReviewResponseID != null)
                        {
                            db.ReviewResponses.Remove(request.ContractorReview.ReviewResponse);
                        }

                        db.ContractorReviews.Remove(request.ContractorReview);
                        db.SaveChanges();
                        UpdateRating(request.Contractor);
                    }
                }
            }
            foreach (var request in db.ServiceRequests.ToList())
            {

                if (request.HomeownerID == id && request.AddressID != null)
                {
                    List<Models.Address> ids = new List<Models.Address>();
                    Models.Address serviceAddressToCheck = db.Addresses.Where(x => x.ID == request.AddressID).SingleOrDefault();
                    db.ServiceRequests.Remove(request);
                    if (serviceAddressToCheck != null)
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
                            if (!ids.Contains(serviceAddressToCheck))
                            {
                                db.Addresses.Remove(serviceAddressToCheck);
                            }

                        }
                    }
                }
            }
            db.SaveChanges();
            Models.Address addressToCheck = db.Addresses.Where(x => x.ID == homeowner.AddressID).SingleOrDefault();
            foreach (var user in db.Users)
            {
                if (homeowner.UserId == user.Id)
                {
                    db.Homeowners.Remove(homeowner);
                    db.Users.Remove(user);
                }
            }
            db.SaveChanges();
            if (addressToCheck != null)
            {
                List<Models.Address> ids = new List<Models.Address>();
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
            ViewBag.Message = "You must log in as a registered user to create a homeowner profile";

            return View();
        }

        public ActionResult Manual_Validate_Thank_You(int? id)
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

        public ActionResult Already_Exists()
        {
            ViewBag.Message = "An account for this user already exists";

            return View();
        }
        [AllowAnonymous]

        public JsonResult doesUserNameExist(string Username)  
        {
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

