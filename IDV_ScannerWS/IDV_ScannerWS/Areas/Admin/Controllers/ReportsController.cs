using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.Areas.Admin.Controllers
{
    public class ReportsController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public string Get_Date()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
                string Txt = DateTime.Now.ToString("dd/MM/yyyy");
                return Txt.ToString().Trim();
            }
            catch
            {
                return "";
            }
        }

        public bool ExpiredDateCheck(string BaseDate, string MaxDate)
        {
            bool Res = false;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
            DateTime BDate = DateTime.Parse(BaseDate);
            DateTime MDate = DateTime.Parse(MaxDate);
            if (BDate.Date >= MDate.Date)
            {
                Res = true;
            }
            return Res;
        }

        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard"); }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Details()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string Doc_ID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select SDK_Code,Document_ID From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                if (DT.Rows.Count != 1) { return RedirectToAction("Index", "Dashboard"); }
                if ((DT.Rows[0][0].ToString().Trim() == "1") && (DT.Rows[0][1].ToString().Trim() == "1")) { return RedirectToAction("GAP", "Reports", new { id = Doc_ID }); }
                if ((DT.Rows[0][0].ToString().Trim() == "1") && (DT.Rows[0][1].ToString().Trim() == "2")) { return RedirectToAction("GAD", "Reports", new { id = Doc_ID }); }
                if ((DT.Rows[0][0].ToString().Trim() == "2") && (DT.Rows[0][1].ToString().Trim() == "1")) { return RedirectToAction("ACP", "Reports", new { id = Doc_ID }); }
                if ((DT.Rows[0][0].ToString().Trim() == "2") && (DT.Rows[0][1].ToString().Trim() == "2")) { return RedirectToAction("ACD", "Reports", new { id = Doc_ID }); }
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult GAP()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string Doc_ID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select SDK_Code,Document_ID From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                if (DT.Rows.Count != 1) { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][0].ToString().Trim() != "1") { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][1].ToString().Trim() != "1") { return RedirectToAction("Index", "Dashboard"); }
                ViewBag.Doc_ID = Doc_ID;
                DataTable DT1 = new DataTable();
                DataTable DT2 = new DataTable();
                DataTable DTNT = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select UserCode,SDK_Name,Document_Name,FirstName + ' ' + LastName,Ins_Date,Ins_Time,Status_Code,Status_Text From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                ViewBag.C1 = DT1.Rows[0][1].ToString().Trim();
                ViewBag.C2 = DT1.Rows[0][2].ToString().Trim();
                ViewBag.C3 = DT1.Rows[0][3].ToString().Trim();
                ViewBag.C4 = DT1.Rows[0][4].ToString().Trim();
                ViewBag.C5 = DT1.Rows[0][5].ToString().Trim();
                ViewBag.C6 = DT1.Rows[0][6].ToString().Trim();
                ViewBag.C7 = DT1.Rows[0][7].ToString().Trim();
                DT2 = SQ.Get_DTable_TSQL("Select Company_Name,Branch_Name,User_ID,User_Name,Address From CO_Counter_V_Full Where (User_ID = '" + DT1.Rows[0][0].ToString().Trim() + "')");
                ViewBag.C8 = DT2.Rows[0][0].ToString().Trim();
                ViewBag.C9 = DT2.Rows[0][1].ToString().Trim();
                string CCode = "000000" + DT2.Rows[0][2].ToString().Trim();
                ViewBag.C10 = CCode.Substring(CCode.Length - 6, 6);
                ViewBag.C11 = DT2.Rows[0][3].ToString().Trim();
                ViewBag.C12 = DT2.Rows[0][4].ToString().Trim();
                DTNT = SQ.Get_DTable_TSQL("Select * From US_Message Where (ID = '" + Doc_ID + "') Order By Ins_Date,Ins_Time");
                ViewBag.Notes = DTNT.Rows;
                SQ.Execute_TSql("Update US_BaseInfo Set [Is_Read] = '1' Where (ID = '" + Doc_ID + "')");
                DataTable DD1 = new DataTable();
                DataTable DD2 = new DataTable();
                DataTable DD3 = new DataTable();
                DD1 = SQ.Get_DTable_TSQL("Select * From US_GA_01_ImageData Where (BaseID = '" + Doc_ID + "')");
                DD2 = SQ.Get_DTable_TSQL("Select * From US_GA_02_ChipData Where (BaseID = '" + Doc_ID + "')");
                DD3 = SQ.Get_DTable_TSQL("Select * From US_GA_03_Validate Where (BaseID = '" + Doc_ID + "') Order By Item_Code");
                string EXPRes = "";
                string EXPResDate = "";
                string EXPRes_Color = "black";
                try { if (DD2 != null) { if (DD2.Rows.Count != 0) { EXPResDate = DD2.Rows[0][12].ToString().Trim(); } } } catch (Exception) { }
                try { if (EXPResDate == "") { if (DD1 != null) { if (DD1.Rows.Count != 0) { EXPResDate = DD1.Rows[0][9].ToString().Trim(); } } } } catch (Exception) { }
                if(EXPResDate.Trim()!="")
                {
                    if(PB.ExpiredDateCheck(EXPResDate,PB.Get_Date())==true)
                    {
                        EXPRes = "Expire Date : " + EXPResDate; EXPRes_Color = "#118024";
                    }
                    else
                    {
                        EXPRes = "Expired on : " + EXPResDate; EXPRes_Color = "#c76c0a";
                    }
                }
                else
                {
                    EXPRes = "No passport expiration date available"; EXPRes_Color = "#ce1a1a";
                }
                ViewBag.EXPRes = "( " + EXPRes.Trim() + " )";
                ViewBag.EXPRes_Color = EXPRes_Color.Trim();
                ViewBag.DDTAB = DD3.Rows;
                ViewBag.MRZD1 = "";
                ViewBag.MRZD2 = "";
                ViewBag.MRZD3 = "";
                ViewBag.MRZD4 = "";
                ViewBag.MRZD5 = "";
                ViewBag.MRZD6 = "";
                ViewBag.MRZD7 = "";
                ViewBag.MRZD8 = "";
                ViewBag.CPD1 = "";
                ViewBag.CPD2 = "";
                ViewBag.CPD3 = "";
                ViewBag.CPD4 = "";
                ViewBag.CPD5 = "";
                ViewBag.CPD6 = "";
                ViewBag.CPD7 = "";
                ViewBag.CPD8 = "";
                try { ViewBag.MRZD1 = DD1.Rows[0][2].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD2 = DD1.Rows[0][3].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD3 = DD1.Rows[0][4].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD4 = DD1.Rows[0][5].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD5 = DD1.Rows[0][6].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD6 = DD1.Rows[0][7].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD7 = DD1.Rows[0][8].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.MRZD8 = DD1.Rows[0][9].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD1 = DD2.Rows[0][1].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD2 = DD2.Rows[0][2].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD3 = DD2.Rows[0][3].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD4 = DD2.Rows[0][4].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD5 = DD2.Rows[0][5].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD6 = DD2.Rows[0][6].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD7 = DD2.Rows[0][7].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.CPD8 = DD2.Rows[0][12].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.D1 = DD2.Rows[0][8].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.D2 = DD2.Rows[0][9].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.D3 = DD2.Rows[0][10].ToString().Trim(); } catch (Exception) { }
                try { ViewBag.D4 = DD2.Rows[0][11].ToString().Trim(); } catch (Exception) { }
                if (ViewBag.MRZD1 == "") { ViewBag.MRZD1 = "NA"; }
                if (ViewBag.MRZD2 == "") { ViewBag.MRZD2 = "NA"; }
                if (ViewBag.MRZD3 == "") { ViewBag.MRZD3 = "NA"; }
                if (ViewBag.MRZD4 == "") { ViewBag.MRZD4 = "NA"; }
                if (ViewBag.MRZD5 == "") { ViewBag.MRZD5 = "NA"; }
                if (ViewBag.MRZD6 == "") { ViewBag.MRZD6 = "NA"; }
                if (ViewBag.MRZD7 == "") { ViewBag.MRZD7 = "NA"; }
                if (ViewBag.MRZD8 == "") { ViewBag.MRZD8 = "NA"; }
                if (ViewBag.CPD1 == "") { ViewBag.CPD1 = "NA"; }
                if (ViewBag.CPD2 == "") { ViewBag.CPD2 = "NA"; }
                if (ViewBag.CPD3 == "") { ViewBag.CPD3 = "NA"; }
                if (ViewBag.CPD4 == "") { ViewBag.CPD4 = "NA"; }
                if (ViewBag.CPD5 == "") { ViewBag.CPD5 = "NA"; }
                if (ViewBag.CPD6 == "") { ViewBag.CPD6 = "NA"; }
                if (ViewBag.CPD7 == "") { ViewBag.CPD7 = "NA"; }
                if (ViewBag.CPD8 == "") { ViewBag.CPD8 = "NA"; }
                ViewBag.DG1_R = "btn-danger"; ViewBag.DG1_V = "btn-outline-dark";
                ViewBag.DG2_R = "btn-danger"; ViewBag.DG2_V = "btn-outline-dark";
                ViewBag.DG3_R = "btn-danger"; ViewBag.DG3_V = "btn-outline-dark";
                ViewBag.DG4_R = "btn-danger"; ViewBag.DG4_V = "btn-outline-dark";
                ViewBag.DG5_R = "btn-danger"; ViewBag.DG5_V = "btn-outline-dark";
                ViewBag.DG6_R = "btn-danger"; ViewBag.DG6_V = "btn-outline-dark";
                ViewBag.DG7_R = "btn-danger"; ViewBag.DG7_V = "btn-outline-dark";
                ViewBag.DG8_R = "btn-danger"; ViewBag.DG8_V = "btn-outline-dark";
                ViewBag.DG9_R = "btn-danger"; ViewBag.DG9_V = "btn-outline-dark";
                ViewBag.DG10_R = "btn-danger"; ViewBag.DG10_V = "btn-outline-dark";
                ViewBag.DG11_R = "btn-danger"; ViewBag.DG11_V = "btn-outline-dark";
                ViewBag.DG12_R = "btn-danger"; ViewBag.DG12_V = "btn-outline-dark";
                ViewBag.DG13_R = "btn-danger"; ViewBag.DG13_V = "btn-outline-dark";
                ViewBag.DG14_R = "btn-danger"; ViewBag.DG14_V = "btn-outline-dark";
                ViewBag.DG15_R = "btn-danger"; ViewBag.DG15_V = "btn-outline-dark";
                ViewBag.DG16_R = "btn-danger"; ViewBag.DG16_V = "btn-outline-dark";
                ViewBag.Chip_Val1 = "fa-circle text-dark";
                ViewBag.Chip_Val2 = "fa-circle text-dark";
                ViewBag.Chip_Val3 = "fa-circle text-dark";
                ViewBag.Chip_Val4 = "fa-circle text-dark";
                ViewBag.Chip_Val5 = "fa-circle text-dark";
                ViewBag.Chip_Val6 = "fa-circle text-dark";
                foreach (DataRow RW in DD3.Rows)
                {
                    switch (RW[1].ToString().Trim().ToUpper())
                    {
                        case "CD_SCDG1_VALIDATE":
                            {
                                ViewBag.DG1_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG1_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG2_VALIDATE":
                            {
                                ViewBag.DG2_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG2_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG3_VALIDATE":
                            {
                                ViewBag.DG3_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG3_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG4_VALIDATE":
                            {
                                ViewBag.DG4_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG4_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG5_VALIDATE":
                            {
                                ViewBag.DG5_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG5_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG6_VALIDATE":
                            {
                                ViewBag.DG6_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG6_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG7_VALIDATE":
                            {
                                ViewBag.DG7_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG7_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG8_VALIDATE":
                            {
                                ViewBag.DG8_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG8_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG9_VALIDATE":
                            {
                                ViewBag.DG9_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG9_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG10_VALIDATE":
                            {
                                ViewBag.DG10_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG10_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG11_VALIDATE":
                            {
                                ViewBag.DG11_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG11_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG12_VALIDATE":
                            {
                                ViewBag.DG12_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG12_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG13_VALIDATE":
                            {
                                ViewBag.DG13_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG13_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG14_VALIDATE":
                            {
                                ViewBag.DG14_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG14_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG15_VALIDATE":
                            {
                                ViewBag.DG15_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG15_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCDG16_VALIDATE":
                            {
                                ViewBag.DG16_R = "btn-success";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.DG16_V = "btn-success"; }
                                break;
                            }
                        case "CD_SCBAC_STATUS":
                            {
                                if (RW[2].ToString().Trim().ToUpper() == "TS_SUCCESS") { ViewBag.Chip_Val1 = "fa-check-circle text-success"; } else { ViewBag.Chip_Val1 = "fa-times-circle text-danger"; }
                                break;
                            }
                        case "CD_SAC_STATUS":
                            {
                                if (RW[2].ToString().Trim().ToUpper() == "TS_SUCCESS") { ViewBag.Chip_Val2 = "fa-check-circle text-success"; } else { ViewBag.Chip_Val2 = "fa-times-circle text-danger"; }
                                break;
                            }
                        case "CD_ACTIVE_AUTHENTICATION":
                            {
                                if (RW[2].ToString().Trim().ToUpper() == "TS_SUCCESS") { ViewBag.Chip_Val3 = "fa-check-circle text-success"; } else { ViewBag.Chip_Val3 = "fa-times-circle text-danger"; }
                                break;
                            }
                        case "CD_SCSIGNATURE_VALIDATE":
                            {
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.Chip_Val4 = "fa-check-circle text-success"; } else { ViewBag.Chip_Val4 = "fa-times-circle text-danger"; }
                                break;
                            }
                        case "CD_VALIDATE_DOC_SIGNER_CERT":
                            {
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.Chip_Val5 = "fa-check-circle text-success"; } else { ViewBag.Chip_Val5 = "fa-times-circle text-danger"; }
                                break;
                            }
                        case "CD_SCSIGNEDATTRS_VALIDATE":
                            {
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.Chip_Val6 = "fa-check-circle text-success"; } else { ViewBag.Chip_Val6 = "fa-times-circle text-danger"; }
                                break;
                            }
                    }
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult GAD()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string Doc_ID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select SDK_Code,Document_ID From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                if (DT.Rows.Count != 1) { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][0].ToString().Trim() != "1") { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][1].ToString().Trim() != "2") { return RedirectToAction("Index", "Dashboard"); }
                ViewBag.Doc_ID = Doc_ID;
                DataTable DT1 = new DataTable();
                DataTable DT2 = new DataTable();
                DataTable DTNT = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select UserCode,SDK_Name,Document_Name,FirstName + ' ' + LastName,Ins_Date,Ins_Time,Status_Code,Status_Text From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                ViewBag.C1 = DT1.Rows[0][1].ToString().Trim();
                ViewBag.C2 = DT1.Rows[0][2].ToString().Trim();
                ViewBag.C3 = DT1.Rows[0][3].ToString().Trim();
                ViewBag.C4 = DT1.Rows[0][4].ToString().Trim();
                ViewBag.C5 = DT1.Rows[0][5].ToString().Trim();
                ViewBag.C6 = DT1.Rows[0][6].ToString().Trim();
                ViewBag.C7 = DT1.Rows[0][7].ToString().Trim();
                DT2 = SQ.Get_DTable_TSQL("Select Company_Name,Branch_Name,User_ID,User_Name,Address From CO_Counter_V_Full Where (User_ID = '" + DT1.Rows[0][0].ToString().Trim() + "')");
                ViewBag.C8 = DT2.Rows[0][0].ToString().Trim();
                ViewBag.C9 = DT2.Rows[0][1].ToString().Trim();
                string CCode = "000000" + DT2.Rows[0][2].ToString().Trim();
                ViewBag.C10 = CCode.Substring(CCode.Length - 6, 6);
                ViewBag.C11 = DT2.Rows[0][3].ToString().Trim();
                ViewBag.C12 = DT2.Rows[0][4].ToString().Trim();
                DTNT = SQ.Get_DTable_TSQL("Select * From US_Message Where (ID = '" + Doc_ID + "') Order By Ins_Date,Ins_Time");
                ViewBag.Notes = DTNT.Rows;
                SQ.Execute_TSql("Update US_BaseInfo Set [Is_Read] = '1' Where (ID = '" + Doc_ID + "')");



                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult ACP()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string Doc_ID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select SDK_Code,Document_ID From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                if (DT.Rows.Count != 1) { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][0].ToString().Trim() != "2") { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][1].ToString().Trim() != "1") { return RedirectToAction("Index", "Dashboard"); }
                ViewBag.Doc_ID = Doc_ID;
                DataTable DT1 = new DataTable();
                DataTable DT2 = new DataTable();
                DataTable DTNT = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select UserCode,SDK_Name,Document_Name,FirstName + ' ' + LastName,Ins_Date,Ins_Time,Status_Code,Status_Text From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                ViewBag.C1 = DT1.Rows[0][1].ToString().Trim();
                ViewBag.C2 = DT1.Rows[0][2].ToString().Trim();
                ViewBag.C3 = DT1.Rows[0][3].ToString().Trim();
                ViewBag.C4 = DT1.Rows[0][4].ToString().Trim();
                ViewBag.C5 = DT1.Rows[0][5].ToString().Trim();
                ViewBag.C6 = DT1.Rows[0][6].ToString().Trim();
                ViewBag.C7 = DT1.Rows[0][7].ToString().Trim();
                DT2 = SQ.Get_DTable_TSQL("Select Company_Name,Branch_Name,User_ID,User_Name,Address From CO_Counter_V_Full Where (User_ID = '" + DT1.Rows[0][0].ToString().Trim() + "')");
                ViewBag.C8 = DT2.Rows[0][0].ToString().Trim();
                ViewBag.C9 = DT2.Rows[0][1].ToString().Trim();
                string CCode = "000000" + DT2.Rows[0][2].ToString().Trim();
                ViewBag.C10 = CCode.Substring(CCode.Length - 6, 6);
                ViewBag.C11 = DT2.Rows[0][3].ToString().Trim();
                ViewBag.C12 = DT2.Rows[0][4].ToString().Trim();
                DTNT = SQ.Get_DTable_TSQL("Select * From US_Message Where (ID = '" + Doc_ID + "') Order By Ins_Date,Ins_Time");
                ViewBag.Notes = DTNT.Rows;
                SQ.Execute_TSql("Update US_BaseInfo Set [Is_Read] = '1' Where (ID = '" + Doc_ID + "')");



                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult ACD()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string Doc_ID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select SDK_Code,Document_ID From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                if (DT.Rows.Count != 1) { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][0].ToString().Trim() != "2") { return RedirectToAction("Index", "Dashboard"); }
                if (DT.Rows[0][1].ToString().Trim() != "2") { return RedirectToAction("Index", "Dashboard"); }
                ViewBag.Doc_ID = Doc_ID;
                DataTable DT1 = new DataTable();
                DataTable DT2 = new DataTable();
                DataTable DTNT = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select UserCode,SDK_Name,Document_Name,FirstName + ' ' + LastName,Ins_Date,Ins_Time,Status_Code,Status_Text From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                ViewBag.C1 = DT1.Rows[0][1].ToString().Trim();
                ViewBag.C2 = DT1.Rows[0][2].ToString().Trim();
                ViewBag.C3 = DT1.Rows[0][3].ToString().Trim();
                ViewBag.C4 = DT1.Rows[0][4].ToString().Trim();
                ViewBag.C5 = DT1.Rows[0][5].ToString().Trim();
                ViewBag.C6 = DT1.Rows[0][6].ToString().Trim();
                ViewBag.C7 = DT1.Rows[0][7].ToString().Trim();
                DT2 = SQ.Get_DTable_TSQL("Select Company_Name,Branch_Name,User_ID,User_Name,Address From CO_Counter_V_Full Where (User_ID = '" + DT1.Rows[0][0].ToString().Trim() + "')");
                ViewBag.C8 = DT2.Rows[0][0].ToString().Trim();
                ViewBag.C9 = DT2.Rows[0][1].ToString().Trim();
                string CCode = "000000" + DT2.Rows[0][2].ToString().Trim();
                ViewBag.C10 = CCode.Substring(CCode.Length - 6, 6);
                ViewBag.C11 = DT2.Rows[0][3].ToString().Trim();
                ViewBag.C12 = DT2.Rows[0][4].ToString().Trim();
                DTNT = SQ.Get_DTable_TSQL("Select * From US_Message Where (ID = '" + Doc_ID + "') Order By Ins_Date,Ins_Time");
                ViewBag.Notes = DTNT.Rows;
                SQ.Execute_TSql("Update US_BaseInfo Set [Is_Read] = '1' Where (ID = '" + Doc_ID + "')");



                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult AddNewComment(string DID, string Message)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                DID = DID.Trim();
                Message = Message.Trim();
                if ((DID == "") || (Message == "")) { ResVal = "1"; ResSTR += "An error occurred while checking the note text"; }
                if (ResVal == "0")
                {
                    SQ.Execute_TSql("Insert Into US_Message Values ('" + DID + "','" + Session["Admin_UID"].ToString().Trim() + "','" + Session["Admin_FullName"].ToString().Trim() + "','" + PB.Get_Date_Formated("dd/MM/yyyy") + "','" + PB.Get_Time() + "','" + Message + "')");
                    ResSTR = "New note text successfully attached";
                    ResVal = "2";
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "The server encountered an error while executing your request" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public JsonResult ReloadNotes(string DID)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                DID = DID.Trim();
                if (DID == "") { ResVal = "1"; ResSTR += "An error occurred while checking the note text"; }
                if (ResVal == "0")
                {
                    DataTable DT = new DataTable();
                    DT = SQ.Get_DTable_TSQL("Select * From US_Message Where (ID = '" + DID + "') Order By Ins_Date,Ins_Time");
                    foreach (DataRow RW in DT.Rows)
                    {
                        ResSTR += "<h4><i class=\"fa fa-user\" style=\"margin-right:10px;margin-top:20px\"></i>" + RW[2].ToString().Trim() + " <span style=\"font-size:12px\">[ " + RW[3].ToString().Trim() + " - " + RW[4].ToString().Trim() + " ] </span></h4>";
                        ResSTR += "<h6 style=\"margin-left:10px;text-align:justify;text-justify:auto;border-left:1px dashed black;padding-left:10px\">" + RW[5].ToString().Trim() + "</h6>";
                    }
                    ResVal = "2";
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "The server encountered an error while reloading document notes" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
    }
}