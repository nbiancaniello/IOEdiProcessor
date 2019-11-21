using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class PartV2
    {
        public string PartNumber { get; set; }
        public string PartDescription { get; set; }
        public string AxiomPartNum { get; set; }
        public string AxiomPartDescription { get; set; }
        public string PoNumber { get; set; }
        public string PoLineNumber { get; set; }
        public string CountryOrigin { get; set; }
        public string PartTotalDespatchedQty { get; set; }
        public string SchedulingAgreementNo { get; set; }
        public string ReleaseNumber { get; set; }
        public string DockCode { get; set; }

        public List<PalletV2> PalletV2 { get; set; }

        public PartV2()
        {
        }
    }
}
