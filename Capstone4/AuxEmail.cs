using Capstone4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Capstone4
{
    public class AuxEmail
    {

        public bool WarnContractor(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Completion deadline about to expire!";
            string url = "http://localhost:37234/ServiceRequests/ConfirmCompletionView/" + serviceRequest.ID;
            string message = "Hello " + serviceRequest.Contractor.Username + "," + "<br>" + "<br>" + "The deadline for completing the service request \"" + serviceRequest.Description + "\" from \"" + serviceRequest.Homeowner.Username + "\" will expire in one minute.  Please be sure to confirm completion by clicking on the link below: <br><a href =" + url + "> Click Here </a>"; 
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);
            return true;

        }

        public void WarnHomeownerExpired(ServiceRequest serviceRequest)
        {
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Your service request has expired!";
            string message = "Hello " + serviceRequest.Homeowner.Username + "," + "<br>" + "<br>" + "Your service request \"" + serviceRequest.Description + "\" has expired because the completion deadline has passed.";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);
        }

        public void WarnBothExpired(ServiceRequest serviceRequest)
        {
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Your service request has expired!";
            string message = "Hello " + serviceRequest.Homeowner.Username + "," + "<br>" + "<br>" + "Your service request \"" + serviceRequest.Description + "\" has expired because the completion deadline has passed." + "<br>" + "<br>" + "A notification message has been sent to " + serviceRequest.Contractor.Username + ".";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);
            if (serviceRequest.ContractorID != null)
            {
                NotifyContractorExpired(serviceRequest);
            }
        }


        public void NotifyContractorExpired(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "Completion deadline expired!";
            string message = "Hello " + serviceRequest.Contractor.Username + "," + "<br>" + "<br>" + "The deadline for completing the service request \"" + serviceRequest.Description + "\" from \"" + serviceRequest.Homeowner.Username + "\" has expired.";
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);
        }
    }
}

