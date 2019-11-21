using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Measure
    {
        public string GrossWeightKG { get; set; }
        public string GrossWeightLB { get; set; }
        public string NetWeightKG { get; set; }
        public string NetWeightLB { get; set; }
        public string TotalNumberPallets { get; set; }
        public string TotalNumberPackages { get; set; }

        public Measure()
        {
        }
    }
}
