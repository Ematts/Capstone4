﻿using System;
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
    public class ContractorReviewsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ContractorReviews
        public ActionResult Index()
        {
            return View(db.ContractorReviews.ToList());
        }

        // GET: ContractorReviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorReview contractorReview = db.ContractorReviews.Find(id);
            if (contractorReview == null)
            {
                return HttpNotFound();
            }
            return View(contractorReview);
        }

        // GET: ContractorReviews/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContractorReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Review,Rating,ReviewDate")] ContractorReview contractorReview)
        {
            if (ModelState.IsValid)
            {
                db.ContractorReviews.Add(contractorReview);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contractorReview);
        }

        // GET: ContractorReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorReview contractorReview = db.ContractorReviews.Find(id);
            if (contractorReview == null)
            {
                return HttpNotFound();
            }
            return View(contractorReview);
        }

        // POST: ContractorReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Review,Rating,ReviewDate,ContractorID,ReviewResponseID")] ContractorReview contractorReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contractorReview).State = EntityState.Modified;   
                db.SaveChanges();
                foreach (var contractor in db.Contractors.ToList())
                {
                    if (contractor.ID == contractorReview.ContractorID)
                    {
                       
                        UpdateRating(contractor);
                        
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(contractorReview);
        }

        // GET: ContractorReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContractorReview contractorReview = db.ContractorReviews.Find(id);
            if (contractorReview == null)
            {
                return HttpNotFound();
            }
            return View(contractorReview);
        }

        // POST: ContractorReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, Contractor contractor)
        {
            ContractorReview contractorReview = db.ContractorReviews.Find(id);
            contractor = contractorReview.Contractor;

            foreach (var request in db.ServiceRequests.ToList())
            {
                if (request.ContractorReviewID == id)
                {
                    request.ContractorReviewID = null;
                    
                }
            }

            if(contractorReview.ReviewResponseID != null)
            {
                db.ReviewResponses.Remove(contractorReview.ReviewResponse);
            }

            db.ContractorReviews.Remove(contractorReview);
            db.SaveChanges();
            UpdateRating(contractor);
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

        public ActionResult AddResponse(int? ID)
        {

            string identity = System.Web.HttpContext.Current.User.Identity.GetUserId();

            if (identity == null)
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }

            ContractorReview review = db.ContractorReviews.Find(ID);

            if (review == null)
            {
                return HttpNotFound();
            }

            if ((identity != review.Contractor.UserId) && (!this.User.IsInRole("Admin")))
            {
                return RedirectToAction("Unauthorized_Access", "Home");
            }


            return View(review);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddResponse([Bind(Include = "ID,ResponseDate,Response")] ReviewResponse reviewResponse, int ID)
        {
            ContractorReview review = db.ContractorReviews.Find(ID);
            if (ModelState.IsValid)
            {
                reviewResponse.ResponseDate = DateTime.Now;
                reviewResponse.ContractorID = review.ContractorID;
                db.ReviewResponses.Add(reviewResponse);
                db.SaveChanges();
                review.ReviewResponseID = reviewResponse.ID;
                db.SaveChanges();
                return RedirectToAction("Index", "ReviewResponses");
            }
            return View(review);
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
