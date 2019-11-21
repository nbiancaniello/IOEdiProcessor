using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Pallet
    {
        public string PalletID { get; set; }
        public string PalletSerialNumber { get; set; }
        public string PalletLabelNumber { get; set; }
        public string PalletCode { get; set; }
        public string PalletDescription { get; set; }
        public string LidCode { get; set; }
        public string LidDescription { get; set; }
        public string LidQuantity { get; set; }
        public string BoxMaterialCode { get; set; }
        public string BoxDescription { get; set; }
        public string NumberOfBoxes { get; set; }
        public string NumberOfBoxesUOM { get; set; }
        public string NumberOfParts { get; set; }
        public string NumberOfPartsUOM { get; set; }
        public string NumberOfPartsPerBox { get; set; }
        public string BuyerPartNumber { get; set; }
        public string PartDescription { get; set; }
        public string PartTotalDespatchedQty { get; set; }
        public string ManifestNumber { get; set; }
        public string CountryOrigin { get; set; }
        public string StorageLocation { get; set; }
        public string SchedulingAgreementNo { get; set; }

        public List<Box> Boxes { get; set; }

        public Pallet()
        {
            Boxes = new List<Box>();
        }
    }
}
