using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Capstone4.Models
{
    public class GetHomeownerActiveRequestsViewModel
    {
        [Display(Name = "Service Number")]
        public int Invoice { get; set; }
        public string Contractor { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public DateTime? PostedDate { get; set; }
        public DateTime? CompletionDeadline { get; set; }
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }
        public List<SelectListItem> ContractorAcceptances { get; set; }
        public int AcceptId { get; set; }
        public int? ServiceID { get; set; }
    }
    
}