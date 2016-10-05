using Capstone4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone4.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(PayPalCheckoutInfo payPalCheckoutInfo)
        {
            //ngrok http -host - header = localhost 37234
            PayPalListenerModel model = new PayPalListenerModel();
            model._PayPalCheckoutInfo = payPalCheckoutInfo;
            byte[] parameters = Request.BinaryRead(Request.ContentLength);

            if (parameters != null && parameters.Length > 0)
            {
                model.GetStatus(parameters, model);
                return new EmptyResult();
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Unauthorized_Access()
        {
            ViewBag.Message = "You are not authorized to view this page.";

            return View();
        }
    }
}