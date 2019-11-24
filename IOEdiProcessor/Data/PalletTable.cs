using System;
using System.Collections.Generic;
using System.Text;

namespace IOEdiProcessor.Data
{
    class PalletTable
    {
        public string Pallet_Serial { get; set; }
        public string Pallet_Code { get; set; }
        public string Pallet_Description { get; set; }
        public string Lid_Code { get; set; }
        public string Lid_Description { get; set; }
        public string Box_Label { get; set; }
        public string Box_Code { get; set; }
        public string Box_Description { get; set; }
        public string Box_UOM { get; set; }
        public string Quantity { get; set; }
        public string Quantity_UOM { get; set; }
        public string Manufacture_Date { get; set; }
        public string Lot_No { get; set; }
        public string Skid_Serial_No { get; set; }
        public string fk_Cust_Part_No { get; set; }
        public string Qty_Per_Box { get; set; }
        public string Item_Number { get; set; }
        public string Item_Description { get; set; }
        public string Manifest_No { get; set; }
        public string Storage_Location { get; set; }
        public string Sa_Num { get; set; }
    }
}
