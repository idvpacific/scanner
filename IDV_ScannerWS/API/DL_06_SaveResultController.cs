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
    public class DL_06_SaveResultController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public void Post(string APPID,string DocID,string Key, string Value)
        {
            try
            {
                APPID = APPID.Trim();
                DocID = DocID.Trim();
                Key = Key.Trim();
                Value = Value.Trim();
                SQ.Execute_TSql("Delete From US_DL_04_DocumentsLast Where (App_ID = '" + APPID + "') And (Document_ID = '" + DocID + "')");
                SQ.Execute_TSql("Insert Into US_DL_04_DocumentsLast Values ('" + APPID+ "','" + DocID + "','" + Key + "','" + Value + "')");
            }
            catch (Exception)
            { }
        }
    }
}
