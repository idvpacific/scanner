using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    public class DL_07_CallBackURLController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public void Post(string APPID)
        {
            try
            {
                APPID = APPID.Trim();
                DataTable DT1 = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select Company_ID,Dealer_ID,User_ID,App_ID,ApplicationID,Status_Code,Status_Text,User_IP,Ins_Date,Ins_Time,Update_Date,Update_Time From US_DL_01_Application Where (ApplicationID = '" + APPID + "')");
                if (DT1.Rows != null)
                {
                    if (DT1.Rows.Count == 1)
                    {
                        DataTable DT2 = new DataTable();
                        DT2 = SQ.Get_DTable_TSQL("Select ExpiryDate,CallBackURL From Reader_Company Where (ID = '" + DT1.Rows[0][0].ToString().Trim() + "') And (Is_Active = '1') And (Removed = '0')");
                        if (DT2.Rows != null)
                        {
                            if (DT2.Rows.Count == 1)
                            {
                                string EXPD = DT2.Rows[0][0].ToString().Trim();
                                if (EXPD.Trim() == "") { EXPD = PB.Get_Date(); }
                                if (PB.ExpiredDateCheck(EXPD, PB.Get_Date()) == true)
                                {
                                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(DT2.Rows[0][1].ToString().Trim());
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                    {
                                        string json = "{\"did\":\"" + DT1.Rows[0][1].ToString().Trim() + "\"," +
                                                      "\"uid\":\"" + DT1.Rows[0][2].ToString().Trim() + "\"," +
                                                      "\"appid\":\"" + DT1.Rows[0][4].ToString().Trim() + "\"," +
                                                      "\"UserIP\":\"" + DT1.Rows[0][7].ToString().Trim() + "\"," +
                                                      "\"CreateDate\":\"" + DT1.Rows[0][8].ToString().Trim() + "\"," +
                                                      "\"CreateTime\":\"" + DT1.Rows[0][9].ToString().Trim() + "\"," +
                                                      "\"UpdateDate\":\"" + DT1.Rows[0][10].ToString().Trim() + "\"," +
                                                      "\"UpdateTime\":\"" + DT1.Rows[0][11].ToString().Trim() + "\"," +
                                                      "\"StatusCode\":\"" + DT1.Rows[0][5].ToString().Trim() + "\"," +
                                                      "\"StatusText\":\"" + DT1.Rows[0][6].ToString().Trim() + "\"}";
                                        streamWriter.Write(json);
                                    }
                                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ) { }
        }
    }
}
