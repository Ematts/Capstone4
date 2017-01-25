using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class GetHomeownerReviewsViewModel
    {
        [Display(Name = "Service Number")]
        public int Invoice { get; set; }
        public string Contractor { get; set; }
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }
        public double? Rating { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }


    }
}