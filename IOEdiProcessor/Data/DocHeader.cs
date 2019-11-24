using System;
using System.Collections.Generic;
using System.Data.Common;
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

        //public DocHeader(DbDataReader dr, string docType, string PurposeCode, string LeadingZero)
        //{
        //    this.DocumentType = docType;
        //    this.ASNType = (PurposeCode == "00") ? "ORIGINAL" : "CANCELLATION";
        //    this.ShipmentNumber = dr["MBOL_NUMBER"].ToString();
        //    this.PackListNumber = (dr["PACK_LIST_NUMBER"].ToString() != null) ? dr["PACK_LIST_NUMBER"].ToString() : "000000000000000";
        //    this.Date = DateTime.Now.ToString("yyyyMMdd");
        //    this.Time = DateTime.Now.ToString("HHmm");
        //    this.CallOffNumber = dr["CALL_OFF_NUM"].ToString();
        //    this.MaterialIssuer = dr["MAT_ISSUER"].ToString();

        //    // VWT doesn't want leading zeroes.
        //    if (LeadingZero == "0")
        //    {
        //        this.PackListNumber = Int32.Parse(this.PackListNumber).ToString("###############");
        //    }
        //}
    }
}
