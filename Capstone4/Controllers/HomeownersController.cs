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
     
            if (homeowner == null)
            {
                return HttpNotFound();
            }


            return View(homeowner);
        }

        // POST: Homeowners/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserId,AddressID,Username,FirstName,LastName,Street")] Homeowner homeowner, Address address)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(homeowner).State = EntityState.Modified;
                if(homeowner.AddressID == null)
                {
                    Address newAdd = new Address();
                    newAdd.Street = address.Street;
                    newAdd.City = address.City;
                    newAdd.State = address.State;
                    newAdd.Street = address.Street;
                    newAdd.Zip = address.Zip;
                    db.Addresses.Add(newAdd);
                    homeowner.AddressID = address.ID;
                    db.Entry(homeowner).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");

                }
                foreach(var i in  db.Addresses)
                {
                    if (i.ID == homeowner.AddressID)
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
            if (homeowner.Address != null)
            {
                db.Addresses.Remove(homeowner.Address);
            }
            db.Homeowners.Remove(homeowner);
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
            ViewBag.Message = "You must log in as a registered user to create a homeowner profile";

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
            var homeownerResult = db.Homeowners.Where(x => x.Username == Username);
            var contractorResult = db.Contractors.Where(x => x.Username == Username);
            if ((homeownerResult.Count() < 1) && (contractorResult.Count() < 1))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json("Username \"" + Username + "\" is already taken.", JsonRequestBehavior.AllowGet);
        }
    }
}
