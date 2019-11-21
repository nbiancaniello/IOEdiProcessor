using System;

namespace IOEdiProcessor.Data
{
    public class Shipping_Containers
    {
        public int rowID { get; set; }
        public string Container_ID { get; set; }
        public string Container_Type { get; set; }
        public string Label_Type { get; set; }
        public string Label_Sub_Type { get; set; }
        public DateTime Date_Printed { get; set; }
        public int fk_SHIP_PLANID { get; set; }
        public int ver_Ship_PlanID { get; set; }
        public int fk_Receiving_ID { get; set; }
        public int fk_Production_ID { get; set; }
        public int fk_Transfer_ID { get; set; }
        public string fk_CUST_ID { get; set; }
        public int Qty { get; set; }
        public double Current_Qty { get; set; }
        public string Qty_UOM { get; set; }
        public string Axiom_Part_No { get; set; }
        public string fk_CUST_PART_NO { get; set; }
        public string PLANT_ID { get; set; }
        public int fk_MBOL_ID { get; set; }
        public string fk_Skid_ID { get; set; }
        public string Skid_Serial_No { get; set; }
        public string Mixed_Load_No { get; set; }
        public string Rec_Pack_List_No { get; set; }
        public string Class { get; set; }
        public int POM { get; set; }
        public int Current_Loc { get; set; }
        public string Shipped_On { get; set; }
        public string Received_On { get; set; }
        public int Ret_Cont { get; set; }
        public int Exp_Cont { get; set; }
        public int Wood_Pallet { get; set; }
        public int Plastic_Pallet { get; set; }
        public int Lid { get; set; }
        public string Lot_No { get; set; }
        public string Original_Lot_No { get; set; }
        public int fk_JOB_ID { get; set; }
        public int fk_USERID { get; set; }
        public string Gross_Weight { get; set; }
        public string Net_Weight { get; set; }
        public int Receiving_Posted { get; set; }
        public int Production_Posted { get; set; }
        public int Production_Posted_Nav { get; set; }
        public int Shipping_Posted { get; set; }
        public int Transfer_Posted { get; set; }
        public string Scanned_By { get; set; }
        public DateTime Scan_Time { get; set; }
        public int fk_AM_ID { get; set; }
        public string Temp_Use { get; set; }
        public int QA_Checker { get; set; }
        public int fk_QA_ID { get; set; }
        public int FIFO_Exempt { get; set; }
        public string Area { get; set; }
        public string Row { get; set; }
        public string Repack_User { get; set; }
        public string Production_Date { get; set; }
        public int Unscanned { get; set; }
        public string Tesla_Licence_Plate { get; set; }
        public int ACC_Rec_ID { get; set; }
        public int fk_Hold_ID { get; set; }
        public string Scrap_Disposition_No { get; set; }
    }
}
