using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ServiceRequest
    {
        public int ID { get; set; }
        public int? AddressID { get; set; }
        [ForeignKey("AddressID")]
        public virtual Address Address { get; set; }
        public int? ContractorID { get; set; }
        [ForeignKey("ContractorID")]
        public virtual Contractor Contractor { get; set; }
        public int HomeownerID { get; set; }
        [ForeignKey("HomeownerID")]
        public virtual Homeowner Homeowner { get; set; }
        //public int CompletedRequestID { get; set; }
        //[ForeignKey("CompletedRequestID")]
        //public virtual CompletedRequest CompletedRequest { get; set; }
        //public int ReviewID { get; set; }
        //[ForeignKey("ReviewID")]
        //public virtual Review Review { get; set; }
        //public virtual ICollection<ServiceRequestPhoto> ServiceRequestPhotos { get; set; }
        //public virtual ICollection<ContractorPhoto> ContractorPhotos { get; set; }
        [Display(Name = "Posted Date")]
        public DateTime PostedDate { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Completion Deadline")]
        public DateTime CompletionDeadline { get; set; }
        [StringLength(100, MinimumLength = 6)]
        [Required]
        public string Description { get; set; }
        [Display(Name = "Service Number")]
        public int Service_Number { get; set; }
        public bool Expired { get; set; }
    }
}