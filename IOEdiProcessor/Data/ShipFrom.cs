using IOEdiProcessor.Logic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class ShipFrom
    {
        public string ShipFromID { get; set; }
        public string ShipFromName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string DunsNumber { get; set; }
        public string ShipFromSupplierID { get; set; }

        public ShipFrom()
        {
        }

        //public ShipFrom(DbDataReader dr)
        //{
        //    this.ShipFromID = dr["SHIP_FROM_ID"].ToString();
        //    this.ShipFromName = dr["SHIP_FROM_NAME"].ToString();
        //    this.Address1 = UtilLogic.ParseText(dr["SHIP_FROM_ADDRESS1"].ToString());
        //    this.Address2 = UtilLogic.ParseText(dr["SHIP_FROM_ADDRESS2"].ToString());
        //    this.Address3 = UtilLogic.ParseText(dr["SHIP_FROM_ADDRESS3"].ToString());
        //    this.PostalCode = dr["SHIP_FROM_PCODE"].ToString();
        //    this.City = UtilLogic.ParseText(dr["SHIP_FROM_CITY"].ToString());
        //    this.Province = UtilLogic.ParseText(dr["SHIP_FROM_PROV"].ToString());
        //    this.Country = UtilLogic.ParseText(dr["SHIP_FROM_COUNTRY"].ToString());
        //    this.DunsNumber = "259898559"; //Hardcoded, ToDo: create a table and save this data.
        //    this.ShipFromSupplierID = dr["SHIP_FROM_SUPPLIER_ID"].ToString();
        //}
    }
}
