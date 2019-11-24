using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Measure
    {
        public string GrossWeightKG { get; set; }
        public string GrossWeightLB { get; set; }
        public string NetWeightKG { get; set; }
        public string NetWeightLB { get; set; }
        public string TotalNumberPallets { get; set; }
        public string TotalNumberPackages { get; set; }

        public Measure()
        {
        }

        //public Measure(DbDataReader dr)
        //{
        //    this.GrossWeightKG = (double.Parse(dr["GROSS_WEIGHT"].ToString()) * 0.45359).ToString("#####0");
        //    this.GrossWeightLB = ((double)dr["GROSS_WEIGHT"]).ToString("#####0");
        //    this.NetWeightKG = (double.Parse(dr["NET_WEIGHT"].ToString()) * 0.45359).ToString("#####0");
        //    this.NetWeightLB = ((double)dr["NET_WEIGHT"]).ToString("#####0");
        //    this.TotalNumberPallets = dr["PALLETS"].ToString();
        //    this.TotalNumberPackages = (dr["PALLETS"].ToString() == "0") ? dr["PACKAGES"].ToString() : "0";
        //}
    }
}
