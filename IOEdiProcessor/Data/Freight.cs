using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class Freight
    {
        public string SCACCode { get; set; }
        public string CarrierName { get; set; }
        public string ForwarderDunsNumber { get; set; }
        public string TermsOfDelivery { get; set; }
        public string TrackingNumber { get; set; }
        public string ProNumber { get; set; }
        public string TransportationMethod { get; set; }
        public string EquipmentDescription { get; set; }


        public Freight()
        {
        }
        //public Freight(DbDataReader dr)
        //{
        //    this.SCACCode = dr["SCAC"].ToString();
        //    this.CarrierName = dr["SCAC_NAME"].ToString();
        //    this.ForwarderDunsNumber = "123456789"; //Hardcoded, ToDo: create a table and save this data.
        //    this.TermsOfDelivery = dr["TERMS_OF_DELIVERY"].ToString();
        //    this.TrackingNumber = dr["TRACKING_NUMBER"].ToString();
        //    this.ProNumber = dr["TRAILER_ID"].ToString();
        //    this.TransportationMethod = dr["TRANSPORT_METHOD"].ToString();
        //    this.EquipmentDescription = dr["EQUIPMENT_DESCRIPTION"].ToString();
        //}
    }
}
