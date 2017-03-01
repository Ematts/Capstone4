using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace Capstone4.Models
{
    public class PayPalListenerModel
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public int ID { get; set; }
        public PayPalCheckoutInfo _PayPalCheckoutInfo { get; set; }

        public void GetStatus(byte[] parameters, PayPalListenerModel model)
        {

            //verify the transaction             
            string status = Verify(true, parameters);

            if (status == "VERIFIED")
            {

                //check that the payment_status is Completed                 
                if (_PayPalCheckoutInfo.payment_status.ToLower() == "completed")
                {

                    model._PayPalCheckoutInfo.TrxnDate = DateTime.UtcNow;
                    string memo = model._PayPalCheckoutInfo.memo;
                    string[] request = memo.Split(' ');
                    Array.Reverse(request);
                    int id = Convert.ToInt32(request[0]);
                    List<string> txnIds = new List<string>();
                    foreach (var i in db.PayPalListenerModels.ToList())
                    {
                        txnIds.Add(i._PayPalCheckoutInfo.txn_id);
                    }
                    if (!txnIds.Contains(model._PayPalCheckoutInfo.txn_id))
                    {
                        ServiceRequest serviceRequest = db.ServiceRequests.Find(id);
                        DateTime timeUtcPaid = DateTime.UtcNow;
                        TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(serviceRequest.Timezone);
                        model._PayPalCheckoutInfo.TrxnDate = TimeZoneInfo.ConvertTimeFromUtc(timeUtcPaid, Zone);
                        model._PayPalCheckoutInfo.Timezone = serviceRequest.Timezone;
                        serviceRequest.ContractorPaid = true;
                        serviceRequest.PayPalListenerModelID = model.ID;
                        serviceRequest.PayPalListenerModel = model;
                        model._PayPalCheckoutInfo.contractorEmail = serviceRequest.Contractor.ApplicationUser.Email;
                        db.PayPalListenerModels.Add(model);
                        var myMessage = new SendGrid.SendGridMessage();
                        string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
                        string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
                        string url = "http://localhost:37234/ServiceRequests/AddReview/" + serviceRequest.ID;
                        myMessage.AddTo(serviceRequest.Homeowner.ApplicationUser.Email);
                        myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
                        myMessage.Subject = "Payment Confirmed!!";
                        String message = "Hello " + serviceRequest.Homeowner.FirstName + "," + "<br>" + "<br>" + "Thank you for using Work Warriors!  You have completed payment for the following service request:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Bid price: <br>$" + serviceRequest.Price + "<br>" + "<br>" + "Service Number: <br>" + serviceRequest.Service_Number + "<br>" + "<br>" + "To review " + serviceRequest.Contractor.Username + "'s service, click on link below: <br><a href =" + url + "> Click Here </a>";
                        myMessage.Html = message;
                        var credentials = new NetworkCredential(name, pass);
                        var transportWeb = new SendGrid.Web(credentials);
                        transportWeb.DeliverAsync(myMessage);
                        Notify_Contractor_of_Payment(serviceRequest);
                        db.SaveChanges();
                    }



                    //check that txn_id has not been previously processed to prevent duplicates                      

                    //check that receiver_email is your Primary PayPal email                                          

                    //check that payment_amount/payment_currency are correct                       

                    //process payment/refund/etc                     

                }
                else if (status == "INVALID")
                {

                    //log for manual investigation             
                }
                else
                {
                    //log response/ipn data for manual investigation             
                }

            }

        }

        private string Verify(bool isSandbox, byte[] parameters)
        {

            string response = "";
            try
            {

                string url = isSandbox ?
                  "https://www.sandbox.paypal.com/cgi-bin/webscr" : "https://www.paypal.com/cgi-bin/webscr";

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                //must keep the original intact and pass back to PayPal with a _notify-validate command
                string data = Encoding.ASCII.GetString(parameters);
                data += "&cmd=_notify-validate";

                webRequest.ContentLength = data.Length;

                //Send the request to PayPal and get the response                 
                using (StreamWriter streamOut = new StreamWriter(webRequest.GetRequestStream(), System.Text.Encoding.ASCII))
                {
                    streamOut.Write(data);
                    streamOut.Close();
                }

                using (StreamReader streamIn = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    response = streamIn.ReadToEnd();
                    streamIn.Close();
                }

            }
            catch { }

            return response;

        }
        public void Notify_Contractor_of_Payment(ServiceRequest serviceRequest)
        {

            string name = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\name.txt");
            string pass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\password.txt");
            var myMessage = new SendGrid.SendGridMessage();
            myMessage.AddTo(serviceRequest.Contractor.ApplicationUser.Email);
            myMessage.From = new MailAddress("workwarriors@gmail.com", "Admin");
            myMessage.Subject = "You've been paid!!";
            string url = "http://localhost:37234/ServiceRequests/PaymentView/" + serviceRequest.ID;
            string message = "Hello " + serviceRequest.Contractor.FirstName + "," + "<br>" + "<br>" + "$" + serviceRequest.AmountDue + " has been credited to your Paypal account for the following service:" + "<br>" + "<br>" + "Job Location:" + "<br>" + "<br>" + serviceRequest.Address.Street + "<br>" + serviceRequest.Address.City + "<br>" + serviceRequest.Address.State + "<br>" + serviceRequest.Address.Zip + "<br>" + "<br>" + "Job Description: <br>" + serviceRequest.Description + "<br>" + "<br>" + "Service Number: <br>" + serviceRequest.Service_Number;
            myMessage.Html = message;
            var credentials = new NetworkCredential(name, pass);
            var transportWeb = new SendGrid.Web(credentials);
            transportWeb.DeliverAsync(myMessage);

        }
    }
}