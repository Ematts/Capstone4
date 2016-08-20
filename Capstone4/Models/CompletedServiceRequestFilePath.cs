using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class CompletedServiceRequestFilePath
    {
        public int CompletedServiceRequestFilePathId { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public int ServiceRequestID { get; set; }
        public virtual ServiceRequest ServiceRequest { get; set; }
    }
}