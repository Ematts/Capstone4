using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class GetContractorActiveRequestsViewModel
    {
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
        public DateTime? PostedDate { get; set; }
        public DateTime CompletionDeadline { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int ID { get; set; }
    }
}