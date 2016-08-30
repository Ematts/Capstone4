using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class SeeContractorReviewViewModel
    {

        [Display(Name = "Contractor")]
        public string ContractorUsername { get; set; }
        [Display(Name = "Overall Rating")]
        public double OverallRating { get; set; }
        [Display(Name = "Total Ratings")]
        public int? TotalRatings { get; set; }
        [DataType(DataType.MultilineText)]
        public string Review { get; set; }
        [Range(0, 5)]
        public double? Rating { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }
        [Display(Name = "Reviewed by")]
        public string HomeownerUsername { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; }

    }
}