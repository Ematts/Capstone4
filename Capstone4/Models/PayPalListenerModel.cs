using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                        serviceRequest.ContractorPaid = true;
                        serviceRequest.PayPalListenerModelID = model.ID;
                        serviceRequest.PayPalListenerModel = model;
                        db.PayPalListenerModels.Add(model);
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
    }
}