using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ContractorReviewsIndexViewModel
    {
        [Display(Name = "Contractor")]
        public string Username { get; set; }
        [Display(Name = "Overall Rating")]
        public double Rating { get; set; }
        [Display(Name = "Total Ratings")]
        public int? TotalRatings { get; set; }
        public int ID { get; set; }
    }
}