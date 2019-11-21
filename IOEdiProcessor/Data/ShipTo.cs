using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class ShipTo
    {
        public string ShipToID { get; set; }
        public string ShipToName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ShipToDockCode { get; set; }
        public string ShipToDropZone { get; set; }
        public string UltimateDestination { get; set; }
        public string CustomerShiptToID { get; set; }
        public string DunsNumber { get; set; }

        public ShipTo()
        {
        }
    }
}
