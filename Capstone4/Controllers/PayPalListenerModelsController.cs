using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Capstone4.Models;

namespace Capstone4.Controllers
{
    public class PayPalListenerModelsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: PayPalListenerModels
        public ActionResult Index()
        {
            return View(db.PayPalListenerModels.ToList());
        }

        // GET: PayPalListenerModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayPalListenerModel payPalListenerModel = db.PayPalListenerModels.Find(id);
            if (payPalListenerModel == null)
            {
                return HttpNotFound();
            }
            return View(payPalListenerModel);
        }

        // GET: PayPalListenerModels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PayPalListenerModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,_PayPalCheckoutInfo")] PayPalListenerModel payPalListenerModel)
        {
            if (ModelState.IsValid)
            {
                db.PayPalListenerModels.Add(payPalListenerModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(payPalListenerModel);
        }

        // GET: PayPalListenerModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayPalListenerModel payPalListenerModel = db.PayPalListenerModels.Find(id);
            if (payPalListenerModel == null)
            {
                return HttpNotFound();
            }
            return View(payPalListenerModel);
        }

        // POST: PayPalListenerModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,_PayPalCheckoutInfo")] PayPalListenerModel payPalListenerModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payPalListenerModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payPalListenerModel);
        }

        // GET: PayPalListenerModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayPalListenerModel payPalListenerModel = db.PayPalListenerModels.Find(id);
            if (payPalListenerModel == null)
            {
                return HttpNotFound();
            }
            return View(payPalListenerModel);
        }

        // POST: PayPalListenerModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PayPalListenerModel payPalListenerModel = db.PayPalListenerModels.Find(id);
            db.PayPalListenerModels.Remove(payPalListenerModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public EmptyResult GetIPN(PayPalCheckoutInfo payPalCheckoutInfo)
        {
            //ngrok http -host - header = localhost 37234
            PayPalListenerModel model = new PayPalListenerModel();
            model._PayPalCheckoutInfo = payPalCheckoutInfo;
            byte[] parameters = Request.BinaryRead(Request.ContentLength);

            if (parameters != null && parameters.Length > 0)
            {
                model.GetStatus(parameters, model);
            }

            return new EmptyResult();
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
