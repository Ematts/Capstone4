using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net.Mail;
using System.Net;
using Capstone4.Models;

namespace Capstone4
{
    public class Quartz : IJob
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Execute(IJobExecutionContext context)
        {
            db = new ApplicationDbContext();
            var requests = db.ServiceRequests.ToList();
            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            foreach (var i in requests)
            {

                if ((i.CompletionDeadline < DateTime.Now) && (i.Expired == false))
                {
                    var myMessage = new SendGrid.SendGridMessage();
                    myMessage.AddTo(i.Homeowner.ApplicationUser.Email);
                    myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
                    myMessage.Subject = "Your service request has expired!";
                    string message = "Hello " + i.Homeowner.Username + "," + "<br>" + "<br>" + "Your service request \"" + i.Description + "\" has expired because the completion deadline has passed.";
                    myMessage.Html = message;
                    var credentials = new NetworkCredential(name, pass);
                    var transportWeb = new SendGrid.Web(credentials);
                    transportWeb.DeliverAsync(myMessage);
                    if (i.ContractorID != null)
                    {
                        AuxEmail contractorWarn = new AuxEmail();
                        contractorWarn.WarnContractor(i);
                    }
                    i.Expired = true;
                    db.SaveChanges();
                }
            }
        }
    }
}


 