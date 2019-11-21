using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Contact
    {
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string DunsNumber { get; set; }

        public Contact()
        {
        }
    }
}
