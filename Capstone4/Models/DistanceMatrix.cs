﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone4.Models
{
    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Row
    {
        public Element[] elements { get; set; }
    }

    public class Parent
    {
        public string[] destination_addresses { get; set; }
        public string[] origin_addresses { get; set; }
        public Row[] rows { get; set; }
        public string status { get; set; }
        public Element[] elements { get; set; }
        public Distance distance { get; set; }
    }
    public class Pair
    {
        public string contractorAddress { get; set; }
        public double distance { get; set; }

    }
    
}