using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class ShipDates
    {
        public string ShipDate { get; set; }
        public string ShipTime { get; set; }

        public ShipDates()
        {
        }

        //public ShipDates(DbDataReader dr)
        //{
        //    this.ShipDate = DateTime.Parse(dr["PLN_SHIP_DATE"].ToString()).ToString("yyyyMMdd");
        //    this.ShipTime = DateTime.Parse(dr["SHIP_TIME"].ToString()).ToString("HHmm");
        //}
    }
}
