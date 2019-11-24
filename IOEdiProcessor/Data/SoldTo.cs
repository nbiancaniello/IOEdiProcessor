using IOEdiProcessor.Logic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class SoldTo
    {
        public string SoldToID { get; set; }
        public string SoldToName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        public SoldTo()
        {
        }

        //public SoldTo(DbDataReader dr)
        //{
        //    this.SoldToID = dr["BUYER_ID"].ToString();
        //    this.SoldToName = UtilLogic.ParseText(dr["BUYER_NAME"].ToString());
        //    this.Address1 = UtilLogic.ParseText(dr["BUYER_ADDRESS1"].ToString());
        //    this.Address2 = UtilLogic.ParseText(dr["BUYER_ADDRESS2"].ToString());
        //    this.City = UtilLogic.ParseText(dr["BUYER_CITY"].ToString());
        //    this.PostalCode = dr["BUYER_PCODE"].ToString();
        //    this.Country = UtilLogic.ParseText(dr["BUYER_COUNTRY"].ToString());
        //}
    }
}
