using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class CompletedServiceRequest
    {
        public int ID { get; set; }
        public int ServiceRequestID { get; set; }
        public virtual ServiceRequest ServiceRequest { get; set; }
        [Display(Name = "Completion Date")]
        public DateTime CompletionDate { get; set; }
        [Display(Name = "Amount to pay contractor")]
        public decimal AmountDue { get; set; }
        [Display(Name = "Contractor paid")]
        public bool ContractorPaid { get; set; }
        //public virtual ICollection<CompletedServiceRequestFilePath> CompletedServiceRequestFilePaths { get; set; }
    }
}