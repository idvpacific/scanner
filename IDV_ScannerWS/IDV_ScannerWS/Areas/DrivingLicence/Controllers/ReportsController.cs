using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;
using System.Web.Security;
using System.Net;
using System.IO;

namespace IDV_ScannerWS.Areas.DrivingLicence.Controllers
{
    public class ReportsController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard", new { id = "" }); }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Details()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string AppID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From US_DL_01_Application_V Where (ApplicationID = '" + AppID + "')");
                int ErrGB = 0;
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        SQ.Execute_TSql("Update US_DL_01_Application Set [Is_Read] = '1' Where (ApplicationID = '" + AppID + "')");
                        ViewBag.App_ID = AppID;
                        ViewBag.Base = DT.Rows[0];
                        DataTable DTOCR = new DataTable();
                        DTOCR = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_03_Documents Where (App_ID = '" + AppID + "')");
                        ViewBag.OCR = DTOCR.Rows;
                        DataTable DTOCRIDV = new DataTable();
                        DTOCRIDV = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_03_Documents_OCR Where (App_ID = '" + AppID + "')");
                        ViewBag.IDVOCR = DTOCRIDV.Rows;
                        DataTable DTCON = new DataTable();
                        DTCON = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_04_DocumentsLast Where (App_ID = '" + AppID + "')");
                        ViewBag.Confirm = DTCON.Rows;
                        DataTable DTALT = new DataTable();
                        DTALT = SQ.Get_DTable_TSQL("Select Alert_Text From US_DL_02_Alerts Where (App_ID = '" + AppID + "')");
                        ViewBag.Alert = DTALT.Rows;
                        DataTable DTCNT = new DataTable();
                        DTCNT = SQ.Get_DTable_TSQL("Select Email,Phone From US_DL_05_EMPH Where (App_ID = '" + AppID + "')");
                        ViewBag.Contact = DTCNT.Rows;
                        ErrGB = 1;
                    }
                }
                if (ErrGB == 0) { return RedirectToAction("Index", "Dashboard"); }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Editor()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string AppID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From US_DL_01_Application_V Where (ApplicationID = '" + AppID + "')");
                int ErrGB = 0;
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        SQ.Execute_TSql("Update US_DL_01_Application Set [Is_Read] = '1' Where (ApplicationID = '" + AppID + "')");
                        ViewBag.App_ID = AppID;
                        ViewBag.Base = DT.Rows[0];
                        DataTable DTOCR = new DataTable();
                        DTOCR = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_03_Documents Where (App_ID = '" + AppID + "') Order By Document_ID");
                        ViewBag.OCR = DTOCR.Rows;
                        DataTable DTALT = new DataTable();
                        DTALT = SQ.Get_DTable_TSQL("Select Alert_Text From US_DL_02_Alerts Where (App_ID = '" + AppID + "') Order By Alert_Text");
                        ViewBag.Alert = DTALT.Rows;
                        DataTable DTCNT = new DataTable();
                        DTCNT = SQ.Get_DTable_TSQL("Select Email,Phone From US_DL_05_EMPH Where (App_ID = '" + AppID + "')");
                        ViewBag.Contact = DTCNT.Rows;
                        ErrGB = 1;
                    }
                }
                if (ErrGB == 0) { return RedirectToAction("Index", "Dashboard"); }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult EditBase(string AI,string EM, string PH, string ST)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                AI = AI.Trim();
                EM = EM.Trim();
                PH = PH.Trim();
                ST = ST.Trim();
                SQ.Execute_TSql("Update US_DL_05_EMPH Set [Email] = '" + EM + "',[Phone] = '" + PH + "' Where (App_ID = '" + AI + "')");
                string STT = "None";
                switch (ST)
                {
                    case "1": { STT = "Uploading"; break; }
                    case "2": { STT = "Pending"; break; }
                    case "3": { STT = "Unknown"; break; }
                    case "4": { STT = "Attention"; break; }
                    case "5": { STT = "Caution"; break; }
                    case "6": { STT = "OCR"; break; }
                    case "7": { STT = "Failed"; break; }
                    case "8": { STT = "Passed"; break; }
                }
                SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '" + ST + "',[Status_Text] = '" + STT + "' Where (ApplicationID = '" + AI + "')");
                ResVal = "2";
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult ClearOCR(string AI)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                AI = AI.Trim();
                SQ.Execute_TSql("Delete From US_DL_03_Documents Where (App_ID = '" + AI + "')");
                ResVal = "2";
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult ClearALT(string AI)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                AI = AI.Trim();
                SQ.Execute_TSql("Delete From US_DL_02_Alerts Where (App_ID = '" + AI + "')");
                ResVal = "2";
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult ADDOCR(string AI,string DI,string DK,string DV)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                AI = AI.Trim();
                DI = DI.Trim();
                DK = DK.Trim();
                DV = DV.Trim();
                SQ.Execute_TSql("Insert Into US_DL_03_Documents Values ('" + AI + "','" + DI + "','" + DK + "','" + DV + "')");
                ResVal = "2";
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult ADDALT(string AI, string DV)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                AI = AI.Trim();
                DV = DV.Trim();
                SQ.Execute_TSql("Insert Into US_DL_02_Alerts Values ('" + AI + "','" + DV + "')");
                ResVal = "2";
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public void SendCallBack(string APPID)
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
            catch (Exception)
            {}
        }
        //----------------------------------------------------------------------------------------------------
    }
}