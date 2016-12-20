using Capstone4.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Capstone4
{
    public class Checker
    {
        public string Timezone { get; set; }
        public bool OK { get; set; }
        public bool Found { get; set; }
        public bool Invalid { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public DateTime? CompletionDeadline { get; set; }
        //[ForeignKey("AddressID")]
        //public virtual Address Address { get; set; }
    }
}