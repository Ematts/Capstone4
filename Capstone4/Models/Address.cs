using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class Address
    {
        public int ID { get; set; }
        [StringLength(40, MinimumLength = 6)]
        [Required]
        public string Street { get; set; }
        [StringLength(40, MinimumLength = 1)]
        [Required]
        public string City { get; set; }
        [StringLength(2, MinimumLength = 2)]
        [Required]
        public string State { get; set; }
        [StringLength(5, MinimumLength = 5)]
        [Required]
        public string Zip { get; set; }
        [Display(Name = "Full Address")]
        public string FullAddress
        {
            get { return Street + ", " + City + ", " + State + " " + Zip; }
        }
        public string googleAddress { get; set; }
        public bool vacant { get; set; }
        public bool validated { get; set; }
    }
}

