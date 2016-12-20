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
            var requests = db.ServiceRequests.ToList();

            foreach (var i in requests)
            {

                //DateTime timeUtc = DateTime.UtcNow;
                //TimeZoneInfo Zone = TimeZoneInfo.FindSystemTimeZoneById(i.Timezone);
                //DateTime Time = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, Zone);
                DateTime WarnTime = i.CompletionDeadline.AddMinutes(-1);

                if ((i.ContractorID != null) && (i.Expired != true) && (i.CompletionDate == null) && (i.WarningSent != true) && (WarnTime < DateTime.Now))
                {
                    
                    AuxEmail conWarn = new AuxEmail();
                    i.WarningSent = conWarn.WarnContractor(i);
                    db.SaveChanges();
                    
                }

                if ((i.CompletionDeadline < DateTime.Now) && (i.Expired == false))
                {

                    i.Expired = true;
                    db.SaveChanges();

                    if (i.ContractorID == null)
                    {
                        AuxEmail homeownerWarnExpired = new AuxEmail();
                        homeownerWarnExpired.WarnHomeownerExpired(i);
                    }

                    if (i.ContractorID != null && i.CompletionDate == null)
                    {
                        AuxEmail bothWarnExpired = new AuxEmail();
                        bothWarnExpired.WarnBothExpired(i);
                    }


                }
            }
        }
    }
}



