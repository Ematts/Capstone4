﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Capstone4.Models
{
    public class Contractor
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int? AddressID { get; set; }
        [ForeignKey("AddressID")]
        public virtual Address Address { get; set; }
        [StringLength(15, MinimumLength = 6)]
        [Required]
        [Display(Name = "Contractor Username")]
        [Remote("doesUserNameExist", "Contractors", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string Username { get; set; }
        [StringLength(20, MinimumLength = 1)]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(25, MinimumLength = 1)]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public double? Rating { get; set; }
        public virtual ICollection<ContractorReview> ContractorReviews { get; set; }
        [Required]
        [Display(Name = "Miles willing to travel:")]
        public double travelDistance { get; set; }
        public bool Inactive { get; set; }
        public bool? NeedsManualValidation { get; set; }

    }
}