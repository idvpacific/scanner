using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Data;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace IDV_ScannerWS.Modules
{
    public class Res_Register_Respons
    {
        public int Status { get; set; }
        public string result { get; set; }
        public string clientid { get; set; }
        public string message { get; set; }
    }

    public class Res_ValidateLogin
    {
        public int Status { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public string userid { get; set; }
        public string passwordhash { get; set; }
        public string twoFactorEnabled { get; set; }
    }


    public class Res_UserInformation
    {
        public int StatusCode { get; set; }
        public string result { get; set; }
        public string message { get; set; }
        public string userid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }

    public class Res_ResetPass
    {
        public int Status { get; set; }
        public string result { get; set; }
        public string email { get; set; }
        public string message { get; set; }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class HTTPRequest_Header
    {
        SQL_Tranceiver Sq = new SQL_Tranceiver();
        public string Send_Http_Request(string Url, string Method, string Post_Data)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
                req.KeepAlive = false;
                req.ProtocolVersion = HttpVersion.Version11;
                req.Method = Method;
                req.ContentType = "application/x-www-form-urlencoded";
                byte[] postBytes = Encoding.ASCII.GetBytes(Post_Data);
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = postBytes.Length;
                Stream requestStream = req.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                Stream resStream = response.GetResponseStream();
                var sr = new StreamReader(response.GetResponseStream());
                string responseText = sr.ReadToEnd();
                return responseText.Trim();
            }
            catch (Exception e)
            {
                return (e.Message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Res_Register_Respons ISSS_Register(string FirstName, string LastName, string Email, string Password)
        {
            Res_Register_Respons RR = new Res_Register_Respons();
            RR.Status = 0;
            RR.clientid = "0";
            RR.message = "";
            RR.result = "";
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL("Select * From MD_ISSS_Config");
                if (DT.Rows.Count == 1)
                {
                    string PostData = "";
                    PostData += "action=AddClient&";
                    PostData += "identifier=" + DT.Rows[0][1].ToString().Trim() + "&";
                    PostData += "secret=" + DT.Rows[0][2].ToString().Trim() + "&";
                    PostData += "responsetype=json&";
                    PostData += "firstname=" + FirstName.Trim() + "&";
                    PostData += "lastname=" + LastName.Trim() + "&";
                    PostData += "email=" + Email.Trim() + "&";
                    PostData += "password2=" + Password.Trim() + "&";
                    PostData += "country=AU";
                    string JsonInput = Send_Http_Request(DT.Rows[0][0].ToString().Trim(), "POST", PostData);
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    RR = jsonSerializer.Deserialize<Res_Register_Respons>(JsonInput);
                    RR.Status = 0;
                    if (RR.clientid != null)
                    {
                        if (RR.clientid != "0")
                        {
                            if (RR.clientid != "")
                            {
                                if (RR.result.Trim().ToLower() == "success") { RR.Status = 1; }
                            }
                        }
                    }
                }
                else
                {
                    RR.Status = 0;
                    RR.clientid = "0";
                    RR.result = "Error";
                    RR.message = "- " + "Icore server not response ...\r\n";
                    RR.message += "- " + "Please contact your services administrator\r\n";
                }
            }
            catch (Exception)
            {
                RR.Status = 0;
                RR.clientid = "0";
                RR.message = "Icore server not available now please try again ...";
                RR.result = "Error";
            }
            return RR;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Res_ValidateLogin ISSS_LoginValidate(string Email, string Password)
        {
            Res_ValidateLogin RR = new Res_ValidateLogin();
            RR.Status = 0;
            RR.result = "";
            RR.message = "";
            RR.userid = "0";
            RR.passwordhash = "";
            RR.twoFactorEnabled = "";
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL("Select * From MD_ISSS_Config");
                if (DT.Rows.Count == 1)
                {
                    string PostData = "";
                    PostData += "action=ValidateLogin&";
                    PostData += "identifier=" + DT.Rows[0][1].ToString().Trim() + "&";
                    PostData += "secret=" + DT.Rows[0][2].ToString().Trim() + "&";
                    PostData += "responsetype=json&";
                    PostData += "email=" + Email.Trim() + "&";
                    PostData += "password2=" + Password.Trim();
                    string JsonInput = Send_Http_Request(DT.Rows[0][0].ToString().Trim(), "POST", PostData);
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    RR = jsonSerializer.Deserialize<Res_ValidateLogin>(JsonInput);
                    RR.Status = 0;
                    if (RR.userid != null)
                    {
                        if (RR.userid != "0")
                        {
                            if (RR.userid != "")
                            {
                                if (RR.result.Trim().ToLower() == "success") { RR.Status = 1; }
                            }
                        }
                    }
                }
                else
                {
                    RR.Status = 0;
                    RR.result = "Error";
                    RR.userid = "0";
                    RR.passwordhash = "";
                    RR.twoFactorEnabled = "";
                    RR.message = "- " + "Icore server not response ...\r\n";
                    RR.message += "- " + "Please contact your services administrator\r\n";
                }
            }
            catch (Exception)
            {
                RR.Status = 0;
                RR.result = "Error";
                RR.message = "Icore server not available now please try again ...";
                RR.userid = "0";
                RR.passwordhash = "";
                RR.twoFactorEnabled = "";
            }
            return RR;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Res_UserInformation ISSS_UserInformation(string UID)
        {
            Res_UserInformation RR = new Res_UserInformation();
            RR.StatusCode = 0;
            RR.result = "";
            RR.message = "";
            RR.userid = "";
            RR.firstname = "";
            RR.lastname = "";
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL("Select * From MD_ISSS_Config");
                if (DT.Rows.Count == 1)
                {
                    string PostData = "";
                    PostData += "action=GetClientsDetails&";
                    PostData += "identifier=" + DT.Rows[0][1].ToString().Trim() + "&";
                    PostData += "secret=" + DT.Rows[0][2].ToString().Trim() + "&";
                    PostData += "responsetype=json&";
                    PostData += "clientid=" + UID.Trim();
                    string JsonInput = Send_Http_Request(DT.Rows[0][0].ToString().Trim(), "POST", PostData);
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    RR = jsonSerializer.Deserialize<Res_UserInformation>(JsonInput);
                    RR.StatusCode = 0;
                    if (RR.userid != null)
                    {
                        if (RR.userid != "0")
                        {
                            if (RR.userid != "")
                            {
                                if (RR.result.Trim().ToLower() == "success") { RR.StatusCode = 1; }
                            }
                        }
                    }
                }
                else
                {
                    RR.StatusCode = 0;
                    RR.result = "Error";
                    RR.userid = "";
                    RR.firstname = "";
                    RR.lastname = "";
                    RR.message = "- " + "Icore server not response ...\r\n";
                    RR.message += "- " + "Please contact your services administrator\r\n";
                }
            }
            catch (Exception)
            {
                RR.StatusCode = 0;
                RR.result = "Error";
                RR.userid = "";
                RR.firstname = "";
                RR.lastname = "";
                RR.message = "- " + "Icore server not response ...\r\n";
                RR.message += "- " + "Please contact your services administrator\r\n";
            }
            return RR;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public Res_ResetPass ISSS_ResetPassword(string Email)
        {
            Res_ResetPass RR = new Res_ResetPass();
            RR.Status = 0;
            RR.result = "";
            RR.email = "";
            RR.message = "";
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL("Select * From MD_ISSS_Config");
                if (DT.Rows.Count == 1)
                {
                    string PostData = "";
                    PostData += "action=ResetPassword&";
                    PostData += "identifier=" + DT.Rows[0][1].ToString().Trim() + "&";
                    PostData += "secret=" + DT.Rows[0][2].ToString().Trim() + "&";
                    PostData += "responsetype=json&";
                    PostData += "email=" + Email.Trim();
                    string JsonInput = Send_Http_Request(DT.Rows[0][0].ToString().Trim(), "POST", PostData);
                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    RR = jsonSerializer.Deserialize<Res_ResetPass>(JsonInput);
                    RR.Status = 0;
                    if (RR.result != null)
                    {
                        if (RR.result != "0")
                        {
                            if (RR.result != "")
                            {
                                if (RR.result.Trim().ToLower() == "success") { RR.Status = 1; }
                            }
                        }
                    }
                }
                else
                {
                    RR.Status = 0;
                    RR.result = "Error";
                    RR.email = "";
                    RR.message = "- " + "Icore server not response ...\r\n";
                    RR.message += "- " + "Please contact your services administrator\r\n";
                }
            }
            catch (Exception)
            {
                RR.Status = 0;
                RR.result = "Error";
                RR.email = "";
                RR.message = "- " + "Icore server not response ...\r\n";
                RR.message += "- " + "Please contact your services administrator\r\n";
            }
            return RR;
        }


    }
}