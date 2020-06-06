﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;
using System.Web.Security;

namespace IDV_ScannerWS.Areas.DrivingLicence.Controllers
{
    public class DashboardController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index()
        {
            try
            {
                ViewBag.C1 = "0"; ViewBag.C2 = "0";
                ViewBag.C3 = "0"; ViewBag.C4 = "0";
                ViewBag.C5 = "0"; ViewBag.C6 = "0";
                ViewBag.C7 = "0"; ViewBag.C8 = "0";
                ViewBag.C9 = "0"; ViewBag.C10 = "0";
                ViewBag.C11 = "0"; ViewBag.C12 = "0";
                ViewBag.C13 = "0"; ViewBag.C14 = "0";
                ViewBag.C15 = "0"; ViewBag.C16 = "0";
                ViewBag.C17 = "0"; ViewBag.C18 = "0";
                ViewBag.C19 = "0"; ViewBag.C20 = "0"; ViewBag.C21 = "0";
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '8')");
                ViewBag.C1 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '8') And (Is_Read = '0')");
                ViewBag.C2 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '7')");
                ViewBag.C3 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '7') And (Is_Read = '0')");
                ViewBag.C4 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '6')");
                ViewBag.C5 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '6') And (Is_Read = '0')");
                ViewBag.C6 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '5')");
                ViewBag.C7 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '5') And (Is_Read = '0')");
                ViewBag.C8 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '4')");
                ViewBag.C9 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '4') And (Is_Read = '0')");
                ViewBag.C10 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '3')");
                ViewBag.C11 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '3') And (Is_Read = '0')");
                ViewBag.C12 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '2')");
                ViewBag.C13 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '2') And (Is_Read = '0')");
                ViewBag.C14 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '1')");
                ViewBag.C15 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Status_Code = '1') And (Is_Read = '0')");
                ViewBag.C16 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application");
                ViewBag.C17 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(App_ID) From US_DL_01_Application Where (Is_Read = '0')");
                ViewBag.C18 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(Distinct(Company_ID)) From US_DL_01_Application");
                ViewBag.C19 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(Distinct(Dealer_ID)) From US_DL_01_Application");
                ViewBag.C20 = DT.Rows[0][0].ToString().Trim();
                DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Count(Distinct(User_ID)) From US_DL_01_Application");
                ViewBag.C21 = DT.Rows[0][0].ToString().Trim();
            }
            catch (Exception)
            {
                ViewBag.C1 = "0"; ViewBag.C2 = "0";
                ViewBag.C3 = "0"; ViewBag.C4 = "0";
                ViewBag.C5 = "0"; ViewBag.C6 = "0";
                ViewBag.C7 = "0"; ViewBag.C8 = "0";
                ViewBag.C9 = "0"; ViewBag.C10 = "0";
                ViewBag.C11 = "0"; ViewBag.C12 = "0";
                ViewBag.C13 = "0"; ViewBag.C14 = "0";
                ViewBag.C15 = "0"; ViewBag.C16 = "0";
                ViewBag.C17 = "0"; ViewBag.C18 = "0";
                ViewBag.C19 = "0"; ViewBag.C20 = "0"; ViewBag.C21 = "0";
            }
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Logout()
        {
            Session["Admin_Logined"] = "0";
            Session["Admin_UID"] = "0";
            Session["Admin_Username"] = "0";
            Session["Admin_FullName"] = "";
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login", new { area = "" });
        }
        //----------------------------------------------------------------------------------------------------
    }
}