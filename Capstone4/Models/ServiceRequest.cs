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
        public int? ContractorReviewID { get; set; }
        public virtual ContractorReview ContractorReview { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Posted Date")]
        public DateTime? PostedDate { get; set; }
        public DateTime? UTCDate { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Completion Deadline")]
        [System.Web.Mvc.Remote("DateCheck", "ServiceRequests", HttpMethod = "POST", AdditionalFields = "checkStreet,checkCity,checkState,UTCDate", ErrorMessage = "The completion deadline must be later than the current time.")]
        public DateTime CompletionDeadline { get; set; }
        [StringLength(100, MinimumLength = 6)]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Service Number")]
        public int Service_Number { get; set; }
        public bool Expired { get; set; }
        public bool? Posted { get; set; }
        public bool? NeedsManualValidation { get; set; }
        public bool? WarningSent { get; set; }
        public bool? PaymentError { get; set; }
        public bool? PaymentAttempted { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }
        [Display(Name = "Amount to pay contractor")]
        public decimal AmountDue { get; set; }
        [Display(Name = "Contractor paid")]
        public bool ContractorPaid { get; set; }
        public int? PayPalListenerModelID { get; set; }
        public string AmbigTime { get; set; }
        public string CompletionAmbigTime { get; set; }
        public virtual PayPalListenerModel PayPalListenerModel { get; set; }
        public bool Inactive { get; set; }
        public string Timezone { get; set; }
        public virtual ICollection<ServiceRequestFilePath> ServiceRequestFilePaths { get; set; }
        public virtual ICollection<CompletedServiceRequestFilePath> CompletedServiceRequestFilePaths { get; set; }
        public virtual ICollection<ContractorAcceptance> ContractorAcceptances { get; set; }
    }
}