using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace IOEdiProcessor.Data
{
    public class ReqDates
    {
        public string ReqDelDate { get; set; }
        public ReqDates()
        {
        }

        //public ReqDates(DbDataReader dr)
        //{
        //    DateTime reqDelDate = new DateTime();
        //    this.ReqDelDate = (DateTime.TryParse(dr["REQ_DEL_DATE"].ToString(), out reqDelDate)) ? reqDelDate.ToString("yyyyMMdd") : null;
        //}
    }
}
