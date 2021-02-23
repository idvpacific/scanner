using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IDV_ScannerWS.Modules;


namespace IDV_ScannerWS.API
{
    public class DL_08_EMPHController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public void Post(string APPID,string E,string P)
        {
            try
            {
                APPID = APPID.Trim();
                E = E.Trim();
                P = P.Trim();
                SQ.Execute_TSql("Delete From US_DL_05_EMPH Where (App_ID = '" + APPID + "')");
                SQ.Execute_TSql("Insert Into US_DL_05_EMPH Values ('" + APPID + "','" + E + "','" + P + "','" + PB.Get_Date() + "','" + PB.Get_Time() + "')");
            }
            catch (Exception)
            {}
        }
    }
}
