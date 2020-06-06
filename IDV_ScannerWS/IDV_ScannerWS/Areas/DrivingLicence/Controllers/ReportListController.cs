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
    public class ReportListController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard", new { id = "" }); }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Passed()
        {
            try
            {
                string STC = "8";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Failed()
        {
            try
            {
                string STC = "7";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult OCR()
        {
            try
            {
                string STC = "6";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Caution()
        {
            try
            {
                string STC = "5";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Attention()
        {
            try
            {
                string STC = "4";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Unknown()
        {
            try
            {
                string STC = "3";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Pending()
        {
            try
            {
                string STC = "2";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Waiting()
        {
            try
            {
                string STC = "1";
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read From US_DL_01_Application_V Where (Company_Removed = '0') And (Status_Code = '" + STC + "')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult AllDocuments()
        {
            try
            {
                int UNRD = 0;
                string SqlSelUR = "";
                try { if (RouteData.Values["id"].ToString().Trim().ToUpper() == "UNREAD") { UNRD = 1; } } catch (Exception) { UNRD = 0; }
                if (UNRD == 1) { SqlSelUR = " And (Is_Read = '0')"; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ApplicationID,Company_Name,Dealer_ID,User_ID,Ins_Date,Update_Date,Is_Read,Status_Text From US_DL_01_Application_V Where (Company_Removed = '0')" + SqlSelUR + " Order By App_ID");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
    }
}