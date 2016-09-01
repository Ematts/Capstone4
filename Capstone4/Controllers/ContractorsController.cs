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
    public class ContractorsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Contractors
        public ActionResult Index()
        {
            var contractors = db.Contractors.Include(c => c.Address).Include(c => c.ApplicationUser);
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

        public ActionResult ReviewsIndex(string searchString, string clientUsername)
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
                    model.OverallRating = ratings.Average();
                }
                else
                {
                    model.OverallRating = 0;
                }
                model.TotalRatings = contractor.ContractorReviews.Count();
                models.Add(model);

            }
            var UsernameLst = new List<string>();
            var UsernameQry = from d in db.Contractors
                              orderby d.Username
                              select d.Username;
            UsernameLst.AddRange(UsernameQry.Distinct());
            ViewBag.clientUsername = new SelectList(UsernameLst);
            if (!String.IsNullOrEmpty(searchString))
            {
                foreach (var model in models.ToList())
                {
                    if(!model.Username.StartsWith(searchString))
                    {
                        models.Remove(model);
                    }
                    
                }                       
            }
            if (!String.IsNullOrEmpty(clientUsername))
            {
                foreach (var model in models.ToList())
                {
                    if (model.Username != clientUsername)
                    {
                        models.Remove(model);
                    }

                }
            }
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
            foreach(var request in db.ServiceRequests)
            {
                if(request.ContractorID == id && request.ContractorReviewID != null)
                {
                    model.ServiceRequests.Add(request);
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
        public ActionResult Create([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName")] Contractor contractor, Address address)
        {
            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();
            if (identity == null)
            {
                return RedirectToAction("Must_be_logged_in", "Contractors");
            }

            if (ModelState.IsValid)
            {

                db.Addresses.Add(address);
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

            if (contractor == null)
            {
                return HttpNotFound();
            }


            return View(contractor);
        }

        // POST: Contractors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName,Street,Rating")] Contractor contractor, Address address)
        {
            if (ModelState.IsValid)
            {

                db.Entry(contractor).State = EntityState.Modified;
                if (contractor.AddressID == null)
                {
                    Address newAdd = new Address();
                    newAdd.Street = address.Street;
                    newAdd.City = address.City;
                    newAdd.State = address.State;
                    newAdd.Street = address.Street;
                    newAdd.Zip = address.Zip;
                    db.Addresses.Add(newAdd);
                    contractor.AddressID = address.ID;
                    db.Entry(contractor).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                foreach (var i in db.Addresses)
                {
                    if (i.ID == contractor.AddressID)
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
            if (contractor.AddressID != null)
            {
                db.Addresses.Remove(contractor.Address);
            }
            foreach(var request in db.ServiceRequests.ToList())
            {
                if (request.ContractorReview.ContractorID == id)
                {
                    request.ContractorReviewID = null;
                }
            }
            foreach(var review in db.ContractorReviews.ToList())
            {
                if(review.ContractorID == id)
                {
                    
                    db.ContractorReviews.Remove(review);
                    
                }
                
            }
            foreach(var acceptance in db.ContractorAcceptances)
            {
                if(acceptance.ContractorID == id)
                {
                    acceptance.ServiceRequest.ContractorID = null;
                    db.ContractorAcceptances.Remove(acceptance);
                }
            }
           
            db.Contractors.Remove(contractor);
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
    }
}
