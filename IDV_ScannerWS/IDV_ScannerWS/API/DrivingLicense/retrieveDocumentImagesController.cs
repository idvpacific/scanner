using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using IDV_ScannerWS.API.Local_Result;
using IDV_ScannerWS.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDV_ScannerWS.API.DrivingLicense
{
    public class DrivingLicImageRaw
    {
        public string did { get; set; }
        public string uid { get; set; }
        public string appid { get; set; }
        public string imageid { get; set; }
    }
    public class retrieveDocumentImagesController : ApiController
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
                            StrBldr.AppendLine("To use the API, you are required to submit [ POST ] application images with the following fields");
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
                            StrBldr.AppendLine("appid : Scanned document ID to retrieve the images [Required]");
                            StrBldr.AppendLine("imageid : ID of relevant image to be retrieved  [Required]");
                            StrBldr.AppendLine();
                            StrBldr.AppendLine();
                            StrBldr.AppendLine("---------------------------------------------------------");
                            StrBldr.AppendLine("|  imageid  |  Description                               |");
                            StrBldr.AppendLine("---------------------------------------------------------");
                            StrBldr.AppendLine("|     1     |  Extracted face from the document          |");
                            StrBldr.AppendLine("|     2     |  Visible image, front of the ID            |");
                            StrBldr.AppendLine("|     3     |  Visible image, back of the ID             |");
                            StrBldr.AppendLine("|     4     |  Infrared (IR) image, front of the ID      |");
                            StrBldr.AppendLine("|     5     |  Infrared (IR) image, back of the ID       |");
                            StrBldr.AppendLine("|     6     |  Ultra Violet Image (UV), front of the ID  |");
                            StrBldr.AppendLine("|     7     |  Ultra Violet (UV), back of the ID         |");
                            StrBldr.AppendLine("|     8     |  Signature on the ID (if presented)        |");
                            StrBldr.AppendLine("---------------------------------------------------------");
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
        public async Task<HttpResponseMessage> PostAsync()
        {
            HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.OK);
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

                DrivingLicImageRaw DLR = new DrivingLicImageRaw();
                var H_Request = Request;
                var H_Headers = H_Request.Headers;
                if (H_Headers.Contains("X-Auth-API-Key")) { H_API_Key = H_Headers.GetValues("X-Auth-API-Key").First(); }
                if (H_Headers.Contains("X-Auth-API-Pass")) { H_API_Pass = H_Headers.GetValues("X-Auth-API-Pass").First(); }
                if (Res_Code == 0) { if (H_API_Key.Trim() == "") { Res_Code = 1; Res_E = "Header API-Key It is required [Err002]"; } }
                if (Res_Code == 0) { if (H_API_Pass.Trim() == "") { Res_Code = 1; Res_E = "Header API-Pass It is required [Err003]"; } }
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
                                    DLR = new DrivingLicImageRaw();
                                    DLR = Newtonsoft.Json.JsonConvert.DeserializeObject<DrivingLicImageRaw>(H_raw);
                                    if (Res_Code == 0) { if (DLR.did == null) { Res_Code = 1; Res_E = "The DealerID is missing, please provide sufficient information for your request [Err008]"; } }
                                    if (Res_Code == 0) { if (DLR.uid == null) { Res_Code = 1; Res_E = "The UserID is missing, please provide sufficient information for your request [Err009]"; } }
                                    if (Res_Code == 0) { if (DLR.appid == null) { Res_Code = 1; Res_E = "The ApplicationID is missing, please provide sufficient information for your request [Err010]"; } }
                                    if (Res_Code == 0) { if (DLR.imageid == null) { Res_Code = 1; Res_E = "The ImageID is missing, please provide sufficient information for your request [Err011]"; } }
                                    if (Res_Code == 0)
                                    {
                                        DataTable DT_App = new DataTable();
                                        DT_App = SQ.Get_DTable_TSQL("Select User_ID From US_DL_01_Application Where (ApplicationID = '" + DLR.appid.ToString().Trim() + "') And (Company_ID = '" + DT.Rows[0][0].ToString().Trim() + "') And (Dealer_ID = '" + DLR.did.ToString().Trim() + "') And (User_ID = '" + DLR.uid.ToString().Trim() + "')");
                                        if (DT_App.Rows != null)
                                        {
                                            if (DT_App.Rows.Count == 1)
                                            {
                                                var ImgPath = HttpContext.Current.Server.MapPath("~/Photos/Acuant/" + DLR.appid + "/Result/" + DLR.imageid + ".jpg");
                                                if(File.Exists(ImgPath)==true)
                                                {
                                                    res = new HttpResponseMessage(HttpStatusCode.OK);
                                                    var stream = new FileStream(ImgPath, FileMode.Open, FileAccess.Read);
                                                    res.Content = new StreamContent(stream);

                                                    res.Content.Headers.ContentDisposition =
                                                        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                                                        {
                                                            FileName = DLR.appid.ToString().Trim() + "-" + DLR.imageid.ToString().Trim() + ".jpg" 
                                                        };
                                                    res.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                                                }
                                                else
                                                {
                                                    Res_Code = 1;
                                                    Res_E = "No image was found related to your request, please check your request details and try again later [Err014]";
                                                }
                                            }
                                            else
                                            {
                                                Res_Code = 1;
                                                Res_E = "No image was found related to your request, please check your request details and try again later [Err013]";
                                            }
                                        }
                                        else
                                        {
                                            Res_Code = 1;
                                            Res_E = "No image was found related to your request, please check your request details and try again later [Err012]";
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
                            Res_Code = 1; Res_E = "Authentication failed, Please check your credentials [Err005]";
                        }
                    }
                    else
                    {
                        Res_Code = 1; Res_E = "Authentication failed, Please check your credentials [Err004]";
                    }
                }
            }
            catch (Exception)
            {
                Res_Code = 1; Res_E = "We ran into an issue, please try again later or contact support [Err001]";
            }
            if (Res_Code == 1)
            {
                res = new HttpResponseMessage(HttpStatusCode.OK);
                res.Content = new StringContent(Res_E.ToString(), Encoding.UTF8, "text/plain");
            }
            return res;
        }
        //--------------------------------------------------------------------------
    }
}
