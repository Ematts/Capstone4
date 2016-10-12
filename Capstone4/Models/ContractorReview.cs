using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ContractorReview
    {
        public int ID { get; set; }
        [DataType(DataType.MultilineText)]
        public string Review { get; set; }
        [Required]
        [Range(0.1, 5)]
        public double Rating { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Review Date")]
        public DateTime? ReviewDate { get; set; }
        public int? ContractorID { get; set; }
        public virtual Contractor Contractor { get; set; }
        public int? ReviewResponseID { get; set; }
        public virtual ReviewResponse ReviewResponse { get; set; }
    }
}