using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Header
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string PartnerID { get; set; }
        public string Standard { get; set; }
        public string Version { get; set; }
        public string TransactionSet { get; set; }
        public string TestProd { get; set; }
        public string LeadingZero { get; set; }

        public Header()
        {
        }
    }
}
