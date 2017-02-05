using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class GetContractorActiveRequestsViewModel
    {
        [Display(Name = "Service Number")]
        public int Invoice { get; set; }
        public string Homeowner { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        [Display(Name = "Full Address")]
        public string FullAddress
        {
            get { return Street + ", " + City + ", " + State + " " + Zip; }
        }
        public string Description { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Posted Date")]
        public DateTime? PostedDate { get; set; }
        [Display(Name = "Completion Deadline")]
        public DateTime CompletionDeadline { get; set; }
        [Display(Name = "Completion Date")]
        public DateTime? CompletionDate { get; set; }
        public int ID { get; set; }
        public string Expired { get; set; }
        [Display(Name = "Time Remaining")]
        public string TimeLeft { get; set; }
    }
}