using System;
using System.Collections.Generic;
using System.Text;

namespace Reporting
{
    class Report
    {
        public DateTime executedDate { get; set; }
        public float totalDistance { get; set; }
        public int totalBikes { get; set; }
        public int totalRides { get; set; }
        public string resultsHTML { get; set; }
        public string resultsDebug { get; set; }
    }
}
