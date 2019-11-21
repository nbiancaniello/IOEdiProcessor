using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Freight
    {
        public string SCACCode { get; set; }
        public string CarrierName { get; set; }
        public string ForwarderDunsNumber { get; set; }
        public string TermsOfDelivery { get; set; }
        public string TrackingNumber { get; set; }
        public string ProNumber { get; set; }
        public string TransportationMethod { get; set; }
        public string EquipmentDescription { get; set; }


        public Freight()
        {
        }
    }
}
