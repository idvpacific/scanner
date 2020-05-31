using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using IDV_ScannerWS.API.Local_Result;
using IDV_ScannerWS.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDV_ScannerWS.API.DrivingLicense
{
    public class DrivingLicRaw
    {
        public string did { get; set; }
        public string uid { get; set; }
        public string appid { get; set; }
    }

    public class retrieveDocumentInfoController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public HttpResponseMessage Get()
        {
            int Res_Code = 0;
            string Res_E = "";
            var res = Request.CreateResponse(HttpStatusCode.OK);
            var StrBldr = new StringBuilder();
            try
            {
                string H_API_Key = "";
                string H_API_Pass = "";
                var H_Request = Request;
                var H_Headers = H_Request.Headers;
                if (H_Headers.Contains("X-Auth-API-Key")) { H_API_Key = H_Headers.GetValues("X-Auth-API-Key").First(); }
                if (H_Headers.Contains("X-Auth-API-Pass")) { H_API_Pass = H_Headers.GetValues("X-Auth-API-Pass").First(); }
                if (Res_Code == 0) { if (H_API_Key.Trim() == "") { Res_Code = 1; Res_E = "Header API-Key It is mandatory [Err002]"; } }
                if (Res_Code == 0) { if (H_API_Pass.Trim() == "") { Res_Code = 1; Res_E = "Header API-Pass It is mandatory [Err003]"; } }
                if (Res_Code == 0)
                {
                    DataTable DT = new DataTable();
                    DT = SQ.Get_DTable_TSQL("Select ID From Reader_Company Where (API_PrivateKey = '" + H_API_Key + "') And (API_Password = '" + H_API_Pass + "') And (Is_Active = '1') And (Removed = '0')");
                    if (DT.Rows != null)
                    {
                        if (DT.Rows.Count == 1)
                        {

                            StrBldr.AppendLine("Dear User ...");
                            StrBldr.AppendLine("To use the API, you are required to submit [ POST ] information with the following fields");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("<-- Header Required -------------------------------------------------->");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("Content-Type : application/json");
                            StrBldr.AppendLine("X-Auth-API-Key : Your Account API Private Key");
                            StrBldr.AppendLine("X-Auth-API-Pass : Your Account API Password");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("<--------------------------------------------------------------------->");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("<-- Body Required [ Raw Data , JSON Type ] ---------------------------->");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("did : Compnay dealer registering ID [Required]");
                            StrBldr.AppendLine("uid : Comapany user ID [Required]");
                            StrBldr.AppendLine("appid : Scanned document ID to retrieve the information [Required]");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("<--------------------------------------------------------------------->");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("Thanks");
                            StrBldr.AppendLine("IDV Pacific Pty Ltd");
                        }
                        else
                        {
                            Res_Code = 1; Res_E = "Authentication failed, Please check your structure [Err005]";
                        }
                    }
                    else
                    {
                        Res_Code = 1; Res_E = "Authentication failed, Please check your structure [Err004]";
                    }
                }
            }
            catch (Exception)
            {
                Res_Code = 1; Res_E = "IDV server have trouble checking in your request, Please check your header and body structure [Err001]";
            }
            if (Res_Code == 0)
            {
                res.Content = new StringContent(StrBldr.ToString(), Encoding.UTF8, "text/plain");
            }
            else
            {
                res.Content = new StringContent(Res_E.ToString(), Encoding.UTF8, "text/plain");
            }
            return res;
        }
        //--------------------------------------------------------------------------
        public async Task<JsonResult<Res_Structure_Public>> PostAsync()
        {
            int Res_Code = 0;
            string Res_E = "";
            var jsonObject = new JObject();
            var jsonObject_Sec = new JObject();
            var jsonObject_Trd = new JObject();
            string H_raw = "";
            try
            {
                string H_API_Key = "";
                string H_API_Pass = "";

                DrivingLicRaw DLR = new DrivingLicRaw();
                var H_Request = Request;
                var H_Headers = H_Request.Headers;
                if (H_Headers.Contains("X-Auth-API-Key")) { H_API_Key = H_Headers.GetValues("X-Auth-API-Key").First(); }
                if (H_Headers.Contains("X-Auth-API-Pass")) { H_API_Pass = H_Headers.GetValues("X-Auth-API-Pass").First(); }
                if (Res_Code == 0) { if (H_API_Key.Trim() == "") { Res_Code = 1; Res_E = "Header API-Key It is mandatory [Err002]"; } }
                if (Res_Code == 0) { if (H_API_Pass.Trim() == "") { Res_Code = 1; Res_E = "Header API-Pass It is mandatory [Err003]"; } }
                if (Res_Code == 0)
                {
                    DataTable DT = new DataTable();
                    DT = SQ.Get_DTable_TSQL("Select ID From Reader_Company Where (API_PrivateKey = '" + H_API_Key + "') And (API_Password = '" + H_API_Pass + "') And (Is_Active = '1') And (Removed = '0')");
                    if (DT.Rows != null)
                    {
                        if (DT.Rows.Count == 1)
                        {
                            try
                            {
                                H_raw = await RawContentReader.Read(H_Request);
                                H_raw = H_raw.Trim();
                                if (H_raw != "")
                                {
                                    DLR = new DrivingLicRaw();
                                    DLR = Newtonsoft.Json.JsonConvert.DeserializeObject<DrivingLicRaw>(H_raw);
                                    if (Res_Code == 0) { if (DLR.did == null) { Res_Code = 1; Res_E = "The DealerID parameter is missing, please provide sufficient information for your request [Err008]"; } }
                                    if (Res_Code == 0) { if (DLR.uid == null) { Res_Code = 1; Res_E = "The UserID parameter is missing, please provide sufficient information for your request [Err009]"; } }
                                    if (Res_Code == 0) { if (DLR.appid == null) { Res_Code = 1; Res_E = "The ApplicationID parameter is missing, please provide sufficient information for your request [Err010]"; } }
                                    if (Res_Code == 0)
                                    {
                                        DataTable DT_App = new DataTable();
                                        DT_App = SQ.Get_DTable_TSQL("Select Dealer_ID,User_ID,User_IP,Ins_Date,Ins_Time,Status_Code,Status_Text,Update_Date,Update_Time From US_DL_01_Application Where (ApplicationID = '" + DLR.appid.ToString().Trim() + "') And (Company_ID = '" + DT.Rows[0][0].ToString().Trim() + "') And (Dealer_ID = '" + DLR.did.ToString().Trim() + "') And (User_ID = '" + DLR.uid.ToString().Trim() + "')");
                                        if (DT_App.Rows != null)
                                        {
                                            if (DT_App.Rows.Count == 1)
                                            {
                                                // Base Information :
                                                jsonObject_Sec = new JObject();
                                                jsonObject_Sec.Add("DealerID", DT_App.Rows[0][0].ToString().Trim());
                                                jsonObject_Sec.Add("UserID", DT_App.Rows[0][1].ToString().Trim());
                                                jsonObject_Sec.Add("UserIP", DT_App.Rows[0][2].ToString().Trim());
                                                jsonObject_Sec.Add("Publish_Date", DT_App.Rows[0][3].ToString().Trim());
                                                jsonObject_Sec.Add("Publish_Time", DT_App.Rows[0][4].ToString().Trim());
                                                jsonObject_Sec.Add("Status_Code", DT_App.Rows[0][5].ToString().Trim());
                                                jsonObject_Sec.Add("Status_Description", DT_App.Rows[0][6].ToString().Trim());
                                                jsonObject_Sec.Add("LastUpdate_Date", DT_App.Rows[0][7].ToString().Trim());
                                                jsonObject_Sec.Add("LastUpdate_Time", DT_App.Rows[0][8].ToString().Trim());
                                                jsonObject.Add("Application_Info", jsonObject_Sec);
                                                // Document OCR :
                                                DataTable DT_OCR = new DataTable();
                                                DT_OCR = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_03_Documents Where (App_ID = '" + DLR.appid + "') Order By Document_ID");
                                                if (DT_OCR.Rows != null)
                                                {
                                                    jsonObject_Sec = new JObject();
                                                    foreach (DataRow RW in DT_OCR.Rows)
                                                    {
                                                        try
                                                        {
                                                            jsonObject_Trd = new JObject();
                                                            jsonObject_Trd.Add("Key", RW[1].ToString().Trim());
                                                            jsonObject_Trd.Add("Value", RW[2].ToString().Trim());
                                                            jsonObject_Sec.Add("D" + RW[0].ToString().Trim(), jsonObject_Trd);
                                                        }
                                                        catch (Exception) {}
                                                    }
                                                    jsonObject.Add("OCR", jsonObject_Sec);
                                                }
                                                // Document Confirmed :
                                                DataTable DT_Con = new DataTable();
                                                DT_Con = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_04_DocumentsLast Where (App_ID = '" + DLR.appid + "') Order By Document_ID");
                                                if (DT_Con.Rows != null)
                                                {
                                                    jsonObject_Sec = new JObject();
                                                    foreach (DataRow RW in DT_Con.Rows)
                                                    {
                                                        try
                                                        {
                                                            jsonObject_Trd = new JObject();
                                                            jsonObject_Trd.Add("Key", RW[1].ToString().Trim());
                                                            jsonObject_Trd.Add("Value", RW[2].ToString().Trim());
                                                            jsonObject_Sec.Add("D" + RW[0].ToString().Trim(), jsonObject_Trd);
                                                        }
                                                        catch (Exception) { }
                                                    }
                                                    jsonObject.Add("Confirmed", jsonObject_Sec);
                                                }
                                                // Document Alerts :
                                                DataTable DT_Alert = new DataTable();
                                                DT_Alert = SQ.Get_DTable_TSQL("Select Alert_Text From US_DL_02_Alerts Where (App_ID = '" + DLR.appid + "')");
                                                if (DT_Alert.Rows != null)
                                                {
                                                    jsonObject_Sec = new JObject();
                                                    int AlertNo = 0;
                                                    foreach (DataRow RW in DT_Alert.Rows)
                                                    {
                                                        AlertNo++;
                                                        jsonObject_Sec.Add("D" + AlertNo.ToString(), RW[0].ToString().Trim());
                                                    }
                                                    jsonObject.Add("Alerts", jsonObject_Sec);
                                                }
                                                // Document Contact :
                                                DataTable DT_EMPH = new DataTable();
                                                DT_EMPH = SQ.Get_DTable_TSQL("Select Email,Phone From US_DL_05_EMPH Where (App_ID = '" + DLR.appid + "')");
                                                if (DT_EMPH.Rows != null)
                                                {
                                                    if (DT_EMPH.Rows.Count == 1)
                                                    {
                                                        jsonObject_Sec = new JObject();
                                                        jsonObject_Sec.Add("Email", DT_EMPH.Rows[0][0].ToString().Trim());
                                                        jsonObject_Sec.Add("Phone", DT_EMPH.Rows[0][1].ToString().Trim());
                                                        jsonObject.Add("ContactDetails", jsonObject_Sec);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Res_Code = 1;
                                                Res_E = "No information was found appropriate to your request, please check your request and try again [Err012]";
                                            }
                                        }
                                        else
                                        {
                                            Res_Code = 1;
                                            Res_E = "No information was found appropriate to your request, please check your request and try again [Err011]";
                                        }
                                    }
                                }
                                else
                                {
                                    Res_Code = 1;
                                    Res_E = "Invalid request, please check your request and try again [Err007]";
                                }
                            }
                            catch (Exception)
                            {
                                Res_Code = 1;
                                Res_E = "Invalid request, please check your request and try again [Err006]";

                            }
                        }
                        else
                        {
                            Res_Code = 1; Res_E = "Authentication failed, Please check your structure [Err005]";
                        }
                    }
                    else
                    {
                        Res_Code = 1; Res_E = "Authentication failed, Please check your structure [Err004]";
                    }
                }
            }
            catch (Exception)
            {
                Res_Code = 1; Res_E = "IDV server have trouble checking in your request, Please check your header and body structure [Err001]";
            }
            try
            {
                if (Res_Code == 0) { return Json(new Res_Structure_Public() { d = jsonObject, e = null }); } else { return Json(new Res_Structure_Public() { d = null, e = Res_E.Trim() }); }
            }
            catch (Exception)
            {
                return Json(new Res_Structure_Public() { d = null, e = "Retrieve Document Returned Error ..." });
            }
        }
        //--------------------------------------------------------------------------
    }
}
