using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class ManualValidateViewModel
    {
        [Display(Name = "Username")]
        public string Screen_name { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool vacant { get; set; }
        public bool validated { get; set; }
        public bool Inactive { get; set; }
        public string Type { get; set; }
        public ICollection<Homeowner> Homeowners { get; set; }
        public ICollection<Contractor> Contractors { get; set; }
        public ICollection<ServiceRequest> ServiceRequests { get; set; }
    }
}