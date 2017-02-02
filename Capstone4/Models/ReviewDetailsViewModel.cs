using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ReviewDetailsViewModel
    {
        [Display(Name = "Service Number")]
        public int Invoice { get; set; }
        public string Contractor { get; set; }
        public double Rating { get; set; }
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }
        [DataType(DataType.MultilineText)]
        public string Review { get; set; }
        [Display(Name = "Contractor Response")]
        [DataType(DataType.MultilineText)]
        public string Response { get; set; }

    }
}