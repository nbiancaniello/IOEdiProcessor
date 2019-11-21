using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class PalletV2
    {
        public string PalletID { get; set; }
        public string PalletSerialNumber { get; set; }
        public string PalletLabelNumber { get; set; }
        public string PalletCode { get; set; }
        public string CustomerPalletCode { get; set; }
        public string PalletType { get; set; }
        public string PalletDescription { get; set; }
        public string CustomerPalletDescription { get; set; }
        public string LidCode { get; set; }
        public string CustomerLidCode { get; set; }
        public string LidType { get; set; }
        public string LidDescription { get; set; }
        public string CustomerLidDescription { get; set; }
        //public string LidQuantity { get; set; }
        public string BoxMaterialCode { get; set; }
        public string CustomerBoxCode { get; set; }
        public string BoxType { get; set; }
        public string BoxDescription { get; set; }
        public string CustomerBoxDescription { get; set; }
        public string NumberOfBoxes { get; set; }
        public string NumberOfBoxesUOM { get; set; }
        public string NumberOfParts { get; set; }
        public string NumberOfPartsUOM { get; set; }
        public string NumberOfPartsPerBox { get; set; }

        public List<Box> Boxes { get; set; }

        public PalletV2()
        {
            Boxes = new List<Box>();
        }
    }
}
