using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Edifact
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DocHeader DocHeader { get; set; }
        public ShipTo ShipTo { get; set; }
        public ShipFrom ShipFrom { get; set; }
        public SoldTo SoldTo { get; set; }
        public Seller Seller { get; set; }
        public Freight Freight { get; set; }
        public ShipDates ShipDates { get; set; }
        public ReqDates ReqDates { get; set; }
        public Measure Measure { get; set; }
        public List<Pallet> Pallets { get; set; }
        public List<PartV2> PartsV2 { get; set; }

        public Edifact()
        {

        }
    }
}
