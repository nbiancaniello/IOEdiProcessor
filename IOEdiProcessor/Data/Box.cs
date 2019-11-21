using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Box
    {
        public string BoxID { get; set; }
        public string BoxSerialNumber { get; set; }
        public string BoxLabelNumber { get; set; }
        public string ManufactureDate { get; set; }
        public string LotNumber { get; set; }
        public string QuantityPerBox { get; set; }

        public Box()
        {

        }
    }
}
