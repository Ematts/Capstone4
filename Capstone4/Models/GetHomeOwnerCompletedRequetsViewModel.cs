using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class GetHomeOwnerCompletedRequetsViewModel
    {
        [Display(Name = "Service Number")]
        public int Invoice { get; set; }
        public string Contractor { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }
        [Display(Name = "Amount Due")]
        public decimal? AmountPaid { get; set; }
        [Display(Name = "Payment ID")]
        public int? PayPalIDNumber { get; set; }
        public int? ID { get; set; }
        public int? ReviewID { get; set; }

    }
}