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
    public class AddLogController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public void Post(string C, string D, string U, string IP, string I, string T)
        {
            try
            {
                C = C.Trim();
                D = D.Trim();
                U = U.Trim();
                IP = IP.Trim();
                I = I.Trim();
                T = T.Trim();
                T = T.Replace(",", ".");
                IP = IP.Replace(",", ".");
                int CI = 0;
                int DI = 0;
                int UI = 0;
                string DeviceName = "";
                if (I == "1") { DeviceName = "AT9000-AT10KI PAS"; }
                if (I == "2") { DeviceName = "CR5400 DL"; }
                try { CI = int.Parse(C); } catch (Exception) { }
                try { DI = int.Parse(D); } catch (Exception) { }
                try { UI = int.Parse(U); } catch (Exception) { }
                SQ.Execute_TSql("Insert Into Reader_Log Values ('" + CI + "','" + C  + "','" + DI + "','" + D + "','" + UI + "','" + U + "','" + I + "','" + DeviceName + "','" + IP + "','" + T + "','" + PB.Get_Date() + "','" + PB.Get_Time() + "')"); ;
            }
            catch (Exception)
            { }
        }
    }
}
