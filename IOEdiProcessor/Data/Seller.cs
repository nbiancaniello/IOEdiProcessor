using IOEdiProcessor.Logic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Seller
    {
        public string CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string DunsNumber { get; set; }

        public Seller()
        {
        }
        //public Seller(DbDataReader dr)
        //{
        //    this.CompanyID = dr["COMPANY_ID"].ToString();
        //    this.CompanyName = dr["COMPANY_NAME"].ToString();
        //    this.Address1 = UtilLogic.ParseText(dr["SELLER_ADDRESS1"].ToString());
        //    this.Address2 = UtilLogic.ParseText(dr["SELLER_ADDRESS2"].ToString());
        //    this.City = UtilLogic.ParseText(dr["SELLER_CITY"].ToString());
        //    this.Province = UtilLogic.ParseText(dr["SELLER_CITY"].ToString());
        //    this.PostalCode = dr["SELLER_PCODE"].ToString();
        //}
    }
}
