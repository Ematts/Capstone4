using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ContractorAcceptance
    {
        public int ID { get; set; }
        public int? ContractorID { get; set; }
        [ForeignKey("ContractorID")]
        public virtual Contractor Contractor { get; set; }
        public int ServiceRequestID { get; set; }
        public virtual ServiceRequest ServiceRequest { get; set; }
        [Display(Name = "Acceptance Date")]
        public DateTime AcceptanceDate { get; set; }
        public string AcceptanceAmbigTime { get; set; }
    }
}