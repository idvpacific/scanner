using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;
using System.Web.Security;

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
    }
}