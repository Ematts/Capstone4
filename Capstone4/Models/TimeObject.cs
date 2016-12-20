using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class TimeObject
    {
        public int dstOffset { get; set; }
        public int rawOffset { get; set; }
        public string status { get; set; }
        public string timeZoneId { get; set; }
        public string timeZoneName { get; set; }
    }
}