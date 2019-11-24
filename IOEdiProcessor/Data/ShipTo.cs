using IOEdiProcessor.Logic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class ShipTo
    {
        public string ShipToID { get; set; }
        public string ShipToName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string ShipToDockCode { get; set; }
        public string ShipToDropZone { get; set; }
        public string UltimateDestination { get; set; }
        public string CustomerShiptToID { get; set; }
        public string DunsNumber { get; set; }

        public ShipTo()
        {
        }
        //public ShipTo(DbDataReader dr, string custID)
        //{
        //    this.ShipToID = dr["SHIP_TO_ID"].ToString();
        //    this.ShipToName = dr["SHIP_TO_NAME"].ToString();
        //    this.Address1 = UtilLogic.ParseText(dr["SHIP_TO_ADDRESS1"].ToString());
        //    this.Address2 = UtilLogic.ParseText(dr["SHIP_TO_ADDRESS2"].ToString());
        //    this.Address3 = UtilLogic.ParseText(dr["SHIP_TO_ADDRESS3"].ToString());
        //    this.City = UtilLogic.ParseText(dr["SHIP_TO_CITY"].ToString());
        //    this.Province = UtilLogic.ParseText(dr["SHIP_TO_PROV"].ToString());
        //    this.PostalCode = dr["SHIP_TO_PCODE"].ToString();
        //    this.Country = UtilLogic.ParseText(dr["SHIP_TO_COUNTRY"].ToString());
        //    this.ShipToDockCode = (custID == "VWT" | custID == "DELP" | custID == "DELP_CN") ? dr["SHIP_TO_DOCK_CODE"].ToString() : "";
        //    this.ShipToDropZone = (custID == "VWT" | custID == "DELP" | custID == "DELP_CN") ? dr["SHIP_TO_DROP_ZONE"].ToString() : "";
        //    this.UltimateDestination = dr["SHIP_TO_UDESTINATION"].ToString();
        //    this.CustomerShiptToID = dr["CUST_SHIP_TO_ID"].ToString();
        //}
    }
}
