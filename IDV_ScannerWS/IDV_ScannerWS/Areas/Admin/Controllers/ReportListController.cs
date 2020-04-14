using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.Areas.Admin.Controllers
{
    public class ReportListController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard"); }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Accepted()
        {
            int UNRD = 0;
            string SqlSelUR = "";
            try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
            if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
            try
            {
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,UserCode,Document_Name,DocNo,FirstName,LastName,Ins_Date,Ins_Time,Is_Read From US_BaseInfo Where (Status_Code = '1')" + SqlSelUR + " Order By ID");
                ViewBag.DT = DT.Rows;
            }
            catch (Exception) { ViewBag.DT = null; }
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Failed()
        {
            int UNRD = 0;
            string SqlSelUR = "";
            try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
            if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
            try
            {
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,UserCode,Document_Name,DocNo,FirstName,LastName,Ins_Date,Ins_Time,Is_Read From US_BaseInfo Where (Status_Code = '2')" + SqlSelUR + " Order By ID");
                ViewBag.DT = DT.Rows;
            }
            catch (Exception) { ViewBag.DT = null; }
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Reffered()
        {
            int UNRD = 0;
            string SqlSelUR = "";
            try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
            if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
            try
            {
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,UserCode,Document_Name,DocNo,FirstName,LastName,Ins_Date,Ins_Time,Is_Read From US_BaseInfo Where (Status_Code = '3')" + SqlSelUR + " Order By ID");
                ViewBag.DT = DT.Rows;
            }
            catch (Exception) { ViewBag.DT = null; }
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult NoValidation()
        {
            int UNRD = 0;
            string SqlSelUR = "";
            try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
            if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
            try
            {
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,UserCode,Document_Name,DocNo,FirstName,LastName,Ins_Date,Ins_Time,Is_Read From US_BaseInfo Where (Status_Code = '4')" + SqlSelUR + " Order By ID");
                ViewBag.DT = DT.Rows;
            }
            catch (Exception) { ViewBag.DT = null; }
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult AllDocuments()
        {
            int UNRD = 0;
            string SqlSelUR = "";
            try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
            if (UNRD == 1) { SqlSelUR = " Where (Is_Read = '0')"; }
            try
            {
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,UserCode,Document_Name,DocNo,FirstName,LastName,Status_Text,Ins_Date,Ins_Time,Is_Read,Status_Code From US_BaseInfo" + SqlSelUR + " Order By ID");
                ViewBag.DT = DT.Rows;
            }
            catch (Exception) { ViewBag.DT = null; }
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        public JsonResult DeleteRow(string DID)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                DID = DID.Trim();
                if (DID == "") { ResVal = "1"; ResSTR += "An error occurred while deleting the data"; }
                if (ResVal == "0")
                {
                    DataTable DT = new DataTable();
                    DT = SQ.Get_DTable_TSQL("Select SDK_Code From US_BaseInfo Where (ID = '" + DID + "')");
                    if (DT.Rows.Count > 0)
                    {
                        int SDKK = 1;
                        if (DT.Rows[0][0].ToString().Trim() != "1") { SDKK = 2; }
                        SQ.Execute_TSql("Delete From US_BaseInfo Where (ID = '" + DID + "')");
                        SQ.Execute_TSql("Delete From US_Message Where (ID = '" + DID + "')");
                        SQ.Execute_TSql("Delete From US_GA_01_ImageData Where (BaseID = '" + DID + "')");
                        SQ.Execute_TSql("Delete From US_GA_02_ChipData Where (BaseID = '" + DID + "')");
                        SQ.Execute_TSql("Delete From US_GA_03_Validate Where (BaseID = '" + DID + "')");
                        string BaseFolder = "";
                        if (SDKK == 1) { BaseFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Gemalto/" + DID); } else { BaseFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + DID); }
                        if (System.IO.Directory.Exists(BaseFolder) == true) { System.IO.Directory.Delete(BaseFolder, true); }
                        ResVal = "2";
                    }
                    else
                    {
                        ResSTR += "An error occurred while deleting the data. No information found with this ID";
                        ResVal = "1";
                    }
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "The server encountered an error while deleting document data" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------
    }
}