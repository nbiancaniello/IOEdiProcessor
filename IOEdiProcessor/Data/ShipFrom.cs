using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class ShipFrom
    {
        public string ShipFromID { get; set; }
        public string ShipFromName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string DunsNumber { get; set; }
        public string ShipFromSupplierID { get; set; }

        public ShipFrom()
        {
        }
    }
}
