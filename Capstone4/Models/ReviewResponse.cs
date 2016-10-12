using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ReviewResponse
    {
        public int ID { get; set; }
        [DataType(DataType.MultilineText)]
        public string Response { get; set; }
        [Column(TypeName = "datetime2")]
        [Display(Name = "Response Date")]
        public DateTime? ResponseDate { get; set; }
        public int? ContractorID { get; set; }
        public virtual Contractor Contractor { get; set; }
    }
}