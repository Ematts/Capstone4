using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone4.Models
{
    public class AddResponseViewModel
    {
        public int ID { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(1000, MinimumLength = 2)]
        public string Response { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }
        [Display(Name = "Service Number")]
        public int? Service_Number { get; set; }
        [Display(Name = "Reviewed by")]
        public string HomeownerUsername { get; set; }
        public double Rating { get; set; }
        public string ContractorUsername { get; set; }
        [DataType(DataType.MultilineText)]
        [StringLength(1000, MinimumLength = 1)]
        public string Review { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }

    }
}