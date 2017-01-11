using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class GetContractorPaidRequestsViewModel
    {
        public int ServiceRequestInvoice { get; set; }
        public string Homeowner { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaidDate { get; set; }
        public string PayStatus { get; set; }
        public string PayPalIDNumber { get; set; }
    }
}