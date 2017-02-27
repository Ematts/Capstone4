using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class EditTimeViewModel
    {
        [Required]
        [Display(Name = "Completion Deadline")]
        [System.Web.Mvc.Remote("DateCheckManual", "ServiceRequests", HttpMethod = "POST", AdditionalFields = "ID,UTCDate", ErrorMessage = "The completion deadline must be later than the current time.")]
        public DateTime CompletionDeadline { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        public int ID { get; set; }
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
        [Required]
        public decimal Price { get; set; }
        [StringLength(100, MinimumLength = 6)]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Service Number")]
        public int Service_Number { get; set; }
        public ICollection<ServiceRequestFilePath> ServiceRequestFilePaths { get; set; }

    }
}