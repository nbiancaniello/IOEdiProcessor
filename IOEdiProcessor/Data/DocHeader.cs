using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class DocHeader
    {
        public string DocumentType { get; set; }
        public string ASNType { get; set; }
        public string ShipmentNumber { get; set; }
        public string PackListNumber { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string CallOffNumber { get; set; }
        public string MaterialIssuer { get; set; }

        public DocHeader()
        {

        }
    }
}
