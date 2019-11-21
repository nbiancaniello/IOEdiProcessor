using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class SoldTo
    {
        public string SoldToID { get; set; }
        public string SoldToName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public SoldTo()
        {
        }
    }
}
