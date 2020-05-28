using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using IDV_ScannerWS.Modules;
using Newtonsoft.Json;

namespace IDV_ScannerWS.API
{
    public class ResultItems
    {
        public string ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class DL_05_ResultController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        //--------------------------------------------------------------------------
        public string Post(string APPID)
        {
            var RSTITM = new List<ResultItems>();
            try
            {
                APPID = APPID.Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_03_Documents Where (App_ID = '" + APPID + "') Order By Document_ID");
                foreach(DataRow RW in DT.Rows)
                {
                    ResultItems NRI = new ResultItems();
                    NRI.ID = RW[0].ToString().Trim();
                    NRI.Key = RW[1].ToString().Trim();
                    NRI.Value = RW[2].ToString().Trim();
                    RSTITM.Add(NRI);
                }
            }
            catch (Exception) { }
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(RSTITM);
        }
    }
}
