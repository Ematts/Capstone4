using PayPal.AdaptiveAccounts;
using PayPal.AdaptiveAccounts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Capstone4
{
    public class PayPalVerify
    {
        public string VerifyPaypal(string firstName, string lastName, string email)
        {
            string payname = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\payname.txt");
            string paypass = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\paypass.txt");
            string sig = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\sig.txt");
            string appid = System.IO.File.ReadAllText(@"C:\Users\erick\Desktop\Credentials\appid.txt");
            Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
            sdkConfig.Add("mode", "sandbox");
            sdkConfig.Add("account1.apiUsername", payname); //PayPal.Account.APIUserName
            sdkConfig.Add("account1.apiPassword", paypass); //PayPal.Account.APIPassword
            sdkConfig.Add("account1.apiSignature", sig); //.APISignature
            sdkConfig.Add("account1.applicationId", appid); //.ApplicatonId
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GetVerifiedStatusRequest request = new GetVerifiedStatusRequest();
            AccountIdentifierType accountIdentifierType = new AccountIdentifierType();
            RequestEnvelope requestEnvelope = new RequestEnvelope();
            requestEnvelope.errorLanguage = "en_US";
            accountIdentifierType.emailAddress = email;
            request.accountIdentifier = accountIdentifierType;
            request.requestEnvelope = requestEnvelope;
            request.matchCriteria = "NAME";
            request.firstName = firstName;
            request.lastName = lastName;
            AdaptiveAccountsService aas = new AdaptiveAccountsService(sdkConfig);
            GetVerifiedStatusResponse response = aas.GetVerifiedStatus(request);
            string status = response.accountStatus;

            return status;
        }
    }
}