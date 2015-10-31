using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SafetyZoneAPI.Models
{
    public class CrimeDataViewModel
    {
        public double Lat { get; set; }
        public double Long { get; set; }
        public string LGAName { get; set; }
        public int CrimeRatingIndex { get; set; }
    }
}