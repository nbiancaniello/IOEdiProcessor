using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Seller
    {
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string DunsNumber { get; set; }

        public Seller()
        {
        }
    }
}
