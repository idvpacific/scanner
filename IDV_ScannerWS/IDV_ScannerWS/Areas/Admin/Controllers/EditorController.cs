using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.Areas.Admin.Controllers
{
    public class EditorController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard"); }
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
                DataTable DT_Base = new DataTable();
                DataTable DT_MRZ = new DataTable();
                DataTable DT_Chip = new DataTable();
                DataTable DT_Validate = new DataTable();
                DT_Base = SQ.Get_DTable_TSQL("Select * From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                DT_MRZ = SQ.Get_DTable_TSQL("Select * From US_GA_01_ImageData Where (BaseID = '" + Doc_ID + "')");
                DT_Chip = SQ.Get_DTable_TSQL("Select * From US_GA_02_ChipData Where (BaseID = '" + Doc_ID + "')");
                DT_Validate = SQ.Get_DTable_TSQL("Select * From US_GA_03_Validate Where (BaseID = '" + Doc_ID + "')");
                // Base ViewBag Data :
                ViewBag.D1 = ""; ViewBag.D2 = ""; ViewBag.D3 = "";
                ViewBag.D4_1 = ""; ViewBag.D4_2 = ""; ViewBag.D4_3 = ""; ViewBag.D4_4 = "";
                ViewBag.D5 = ""; ViewBag.D6 = ""; ViewBag.D7 = ""; ViewBag.D8 = "";
                ViewBag.D9_1 = ""; ViewBag.D9_2 = ""; ViewBag.D9_3 = ""; ViewBag.D9_4 = ""; ViewBag.D9_5 = ""; ViewBag.D9_6 = ""; ViewBag.D9_7 = ""; ViewBag.D9_8 = ""; ViewBag.D9_9 = ""; ViewBag.D9_10 = ""; ViewBag.D9_11 = ""; ViewBag.D9_12 = ""; ViewBag.D9_13 = ""; ViewBag.D9_14 = ""; ViewBag.D9_15 = ""; ViewBag.D9_16 = "";
                ViewBag.D10_1 = ""; ViewBag.D10_2 = ""; ViewBag.D10_3 = ""; ViewBag.D10_4 = ""; ViewBag.D10_5 = ""; ViewBag.D10_6 = ""; ViewBag.D10_7 = ""; ViewBag.D10_8 = ""; ViewBag.D10_9 = ""; ViewBag.D10_10 = ""; ViewBag.D10_11 = ""; ViewBag.D10_12 = ""; ViewBag.D10_13 = ""; ViewBag.D10_14 = ""; ViewBag.D10_15 = ""; ViewBag.D10_16 = "";
                ViewBag.D11_1 = ""; ViewBag.D11_2 = ""; ViewBag.D11_3 = "";
                ViewBag.D12_1 = ""; ViewBag.D12_2 = ""; ViewBag.D12_3 = "";
                ViewBag.D13_1 = ""; ViewBag.D13_2 = ""; ViewBag.D13_3 = "";
                ViewBag.D14_1 = ""; ViewBag.D14_2 = ""; ViewBag.D14_3 = "";
                ViewBag.D15_1 = ""; ViewBag.D15_2 = ""; ViewBag.D15_3 = "";
                ViewBag.D16_1 = ""; ViewBag.D16_2 = ""; ViewBag.D16_3 = "";
                ViewBag.D17 = ""; ViewBag.D18 = ""; ViewBag.D19 = ""; ViewBag.D20 = ""; ViewBag.D21 = ""; ViewBag.D22 = ""; ViewBag.D23 = ""; ViewBag.D24 = "";
                ViewBag.D25 = ""; ViewBag.D26 = ""; ViewBag.D27 = ""; ViewBag.D28 = ""; ViewBag.D29 = ""; ViewBag.D30 = ""; ViewBag.D31 = ""; ViewBag.D32 = "";
                // Create Page From DataBase :  
                ViewBag.D1 = DT_Base.Rows[0][7].ToString().Trim();
                ViewBag.D2 = DT_Base.Rows[0][8].ToString().Trim();
                ViewBag.D3 = DT_Base.Rows[0][14].ToString().Trim();
                if (DT_Base.Rows[0][12].ToString().Trim() == "1") { ViewBag.D4_1 = "selected"; }
                if (DT_Base.Rows[0][12].ToString().Trim() == "2") { ViewBag.D4_2 = "selected"; }
                if (DT_Base.Rows[0][12].ToString().Trim() == "3") { ViewBag.D4_3 = "selected"; }
                if (DT_Base.Rows[0][12].ToString().Trim() == "4") { ViewBag.D4_4 = "selected"; }
                //--------
                if (DT_MRZ.Rows.Count != 0)
                {
                    ViewBag.D17 = DT_MRZ.Rows[0][2].ToString().Trim();
                    ViewBag.D18 = DT_MRZ.Rows[0][3].ToString().Trim();
                    ViewBag.D19 = DT_MRZ.Rows[0][4].ToString().Trim();
                    ViewBag.D20 = DT_MRZ.Rows[0][5].ToString().Trim();
                    ViewBag.D21 = DT_MRZ.Rows[0][6].ToString().Trim();
                    ViewBag.D22 = DT_MRZ.Rows[0][7].ToString().Trim();
                    ViewBag.D24 = DT_MRZ.Rows[0][8].ToString().Trim();
                    ViewBag.D23 = DT_MRZ.Rows[0][9].ToString().Trim();
                }
                if (DT_Chip.Rows.Count != 0)
                {
                    ViewBag.D25 = DT_Chip.Rows[0][1].ToString().Trim();
                    ViewBag.D26 = DT_Chip.Rows[0][2].ToString().Trim();
                    ViewBag.D27 = DT_Chip.Rows[0][3].ToString().Trim();
                    ViewBag.D28 = DT_Chip.Rows[0][4].ToString().Trim();
                    ViewBag.D29 = DT_Chip.Rows[0][5].ToString().Trim();
                    ViewBag.D30 = DT_Chip.Rows[0][6].ToString().Trim();
                    ViewBag.D32 = DT_Chip.Rows[0][7].ToString().Trim();
                    ViewBag.D5 = DT_Chip.Rows[0][8].ToString().Trim();
                    ViewBag.D6 = DT_Chip.Rows[0][9].ToString().Trim();
                    ViewBag.D7 = DT_Chip.Rows[0][10].ToString().Trim();
                    ViewBag.D8 = DT_Chip.Rows[0][11].ToString().Trim();
                    ViewBag.D31 = DT_Chip.Rows[0][12].ToString().Trim();
                }
                foreach (DataRow RW in DT_Validate.Rows)
                {
                    switch (RW[1].ToString().Trim().ToUpper())
                    {
                        case "CD_SCDG1_VALIDATE":
                            {
                                ViewBag.D9_1 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_1 = "checked"; }
                                break;
                            }
                        case "CD_SCDG2_VALIDATE":
                            {
                                ViewBag.D9_2 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_2 = "checked"; }
                                break;
                            }
                        case "CD_SCDG3_VALIDATE":
                            {
                                ViewBag.D9_3 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_3 = "checked"; }
                                break;
                            }
                        case "CD_SCDG4_VALIDATE":
                            {
                                ViewBag.D9_4 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_4 = "checked"; }
                                break;
                            }
                        case "CD_SCDG5_VALIDATE":
                            {
                                ViewBag.D9_5 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_5 = "checked"; }
                                break;
                            }
                        case "CD_SCDG6_VALIDATE":
                            {
                                ViewBag.D9_6 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_6 = "checked"; }
                                break;
                            }
                        case "CD_SCDG7_VALIDATE":
                            {
                                ViewBag.D9_7 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_7 = "checked"; }
                                break;
                            }
                        case "CD_SCDG8_VALIDATE":
                            {
                                ViewBag.D9_8 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_8 = "checked"; }
                                break;
                            }
                        case "CD_SCDG9_VALIDATE":
                            {
                                ViewBag.D9_9 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_9 = "checked"; }
                                break;
                            }
                        case "CD_SCDG10_VALIDATE":
                            {
                                ViewBag.D9_10 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_10 = "checked"; }
                                break;
                            }
                        case "CD_SCDG11_VALIDATE":
                            {
                                ViewBag.D9_11 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_11 = "checked"; }
                                break;
                            }
                        case "CD_SCDG12_VALIDATE":
                            {
                                ViewBag.D9_12 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_12 = "checked"; }
                                break;
                            }
                        case "CD_SCDG13_VALIDATE":
                            {
                                ViewBag.D9_13 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_13 = "checked"; }
                                break;
                            }
                        case "CD_SCDG14_VALIDATE":
                            {
                                ViewBag.D9_14 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_14 = "checked"; }
                                break;
                            }
                        case "CD_SCDG15_VALIDATE":
                            {
                                ViewBag.D9_15 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_15 = "checked"; }
                                break;
                            }
                        case "CD_SCDG16_VALIDATE":
                            {
                                ViewBag.D9_16 = "checked";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D10_16 = "checked"; }
                                break;
                            }




                        case "CD_SCBAC_STATUS":
                            {
                                ViewBag.D11_1 = ""; ViewBag.D11_2 = ""; ViewBag.D11_3 = "";
                                if (RW[2].ToString().Trim().ToUpper() == "TS_SUCCESS") { ViewBag.D11_2 = "checked"; } else { ViewBag.D11_3 = "checked"; }
                                break;
                            }
                        case "CD_SAC_STATUS":
                            {
                                ViewBag.D12_1 = ""; ViewBag.D12_2 = ""; ViewBag.D12_3 = "";
                                if (RW[2].ToString().Trim().ToUpper() == "TS_SUCCESS") { ViewBag.D12_2 = "checked"; } else { ViewBag.D12_3 = "checked"; }
                                break;
                            }
                        case "CD_ACTIVE_AUTHENTICATION":
                            {
                                ViewBag.D13_1 = ""; ViewBag.D13_2 = ""; ViewBag.D13_3 = "";
                                if (RW[2].ToString().Trim().ToUpper() == "TS_SUCCESS") { ViewBag.D13_2 = "checked"; } else { ViewBag.D13_3 = "checked"; }
                                break;
                            }
                        case "CD_SCSIGNATURE_VALIDATE":
                            {
                                ViewBag.D14_1 = ""; ViewBag.D14_2 = ""; ViewBag.D14_3 = "";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D14_2 = "checked"; } else { ViewBag.D14_3 = "checked"; }
                                break;
                            }
                        case "CD_VALIDATE_DOC_SIGNER_CERT":
                            {
                                ViewBag.D15_1 = ""; ViewBag.D15_2 = ""; ViewBag.D15_3 = "";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D15_2 = "checked"; } else { ViewBag.D15_3 = "checked"; }
                                break;
                            }
                        case "CD_SCSIGNEDATTRS_VALIDATE":
                            {
                                ViewBag.D16_1 = ""; ViewBag.D16_2 = ""; ViewBag.D16_3 = "";
                                if (RW[2].ToString().Trim().ToUpper() == "RFID_VC_VALID") { ViewBag.D16_2 = "checked"; } else { ViewBag.D16_3 = "checked"; }
                                break;
                            }
                    }
                }
                // ViewBag Data Error Fixer :
                if ((ViewBag.D4_1 == "") && (ViewBag.D4_2 == "") && (ViewBag.D4_3 == "") && (ViewBag.D4_4 == "")) { ViewBag.D4_1 = "selected"; }
                if ((ViewBag.D11_1 == "") && (ViewBag.D11_2 == "") && (ViewBag.D11_3 == "")) { ViewBag.D11_1 = "checked"; }
                if ((ViewBag.D12_1 == "") && (ViewBag.D12_2 == "") && (ViewBag.D12_3 == "")) { ViewBag.D12_1 = "checked"; }
                if ((ViewBag.D13_1 == "") && (ViewBag.D13_2 == "") && (ViewBag.D13_3 == "")) { ViewBag.D13_1 = "checked"; }
                if ((ViewBag.D14_1 == "") && (ViewBag.D14_2 == "") && (ViewBag.D14_3 == "")) { ViewBag.D14_1 = "checked"; }
                if ((ViewBag.D15_1 == "") && (ViewBag.D15_2 == "") && (ViewBag.D15_3 == "")) { ViewBag.D15_1 = "checked"; }
                if ((ViewBag.D16_1 == "") && (ViewBag.D16_2 == "") && (ViewBag.D16_3 == "")) { ViewBag.D16_1 = "checked"; }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult GA_PassportSaveChanges(string IDVal,string Txt_FirstName, string Txt_LastName, string Txt_DocumentIDF, string StatusID, string Txt_AirBaudrate, string Txt_ChipID, string Txt_BAC, string Txt_SAC, string Txt_MRZ_FirstName, string Txt_MRZ_LastName, string Txt_MRZ_Nationality, string Txt_MRZ_Gender, string Txt_MRZ_Birthdate, string Txt_MRZ_DocumentNo, string Txt_MRZ_Data, string Txt_CHIP_FirstName, string Txt_CHIP_LastName, string Txt_CHIP_Nationality, string Txt_CHIP_Gender, string Txt_CHIP_Birthdate, string Txt_CHIP_DocumentNo, string Txt_CHIP_Data, string RData, string VData, string Validation, string MRZ_ExpireD, string Chip_ExpireD)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                if (Txt_FirstName.Trim() == "") { ResVal = "1"; ResSTR += "The first name of the holder of the passport is required\r\n"; }
                if (Txt_LastName.Trim() == "") { ResVal = "1"; ResSTR += "The Last name of the holder of the passport is required\r\n"; }
                if (Txt_DocumentIDF.Trim() == "") { ResVal = "1"; ResSTR += "The Document ID of the passport is required\r\n"; }
                if (StatusID.Trim() == "") { StatusID = "1"; }
                DataTable DTTST = new DataTable();
                DTTST = SQ.Get_DTable_TSQL("Select ID From US_BaseInfo Where (ID = '" + IDVal + "')");
                if (DTTST.Rows.Count == 0) { ResVal = "1"; ResSTR += "The passport information is not available in the database\r\n"; }
                if (ResVal == "0")
                {
                    try { Txt_FirstName = Txt_FirstName.Trim(); } catch (Exception) { Txt_FirstName = ""; }
                    try { Txt_LastName = Txt_LastName.Trim(); } catch (Exception) { Txt_LastName = ""; }
                    try { Txt_DocumentIDF = Txt_DocumentIDF.Trim(); } catch (Exception) { Txt_DocumentIDF = ""; }
                    try { StatusID = StatusID.Trim(); } catch (Exception) { StatusID = ""; }
                    try { Txt_AirBaudrate = Txt_AirBaudrate.Trim(); } catch (Exception) { Txt_AirBaudrate = ""; }
                    try { Txt_ChipID = Txt_ChipID.Trim(); } catch (Exception) { Txt_ChipID = ""; }
                    try { Txt_BAC = Txt_BAC.Trim(); } catch (Exception) { Txt_BAC = ""; }
                    try { Txt_SAC = Txt_SAC.Trim(); } catch (Exception) { Txt_SAC = ""; }
                    try { Txt_MRZ_FirstName = Txt_MRZ_FirstName.Trim(); } catch (Exception) { Txt_MRZ_FirstName = ""; }
                    try { Txt_MRZ_LastName = Txt_MRZ_LastName.Trim(); } catch (Exception) { Txt_MRZ_LastName = ""; }
                    try { Txt_MRZ_Nationality = Txt_MRZ_Nationality.Trim(); } catch (Exception) { Txt_MRZ_Nationality = ""; }
                    try { Txt_MRZ_Gender = Txt_MRZ_Gender.Trim(); } catch (Exception) { Txt_MRZ_Gender = ""; }
                    try { Txt_MRZ_Birthdate = Txt_MRZ_Birthdate.Trim(); } catch (Exception) { Txt_MRZ_Birthdate = ""; }
                    try { Txt_MRZ_DocumentNo = Txt_MRZ_DocumentNo.Trim(); } catch (Exception) { Txt_MRZ_DocumentNo = ""; }
                    try { Txt_MRZ_Data = Txt_MRZ_Data.Trim().Replace("$", "<"); } catch (Exception) { Txt_MRZ_Data = ""; }
                    try { Txt_CHIP_FirstName = Txt_CHIP_FirstName.Trim(); } catch (Exception) { Txt_CHIP_FirstName = ""; }
                    try { Txt_CHIP_LastName = Txt_CHIP_LastName.Trim(); } catch (Exception) { Txt_CHIP_LastName = ""; }
                    try { Txt_CHIP_Nationality = Txt_CHIP_Nationality.Trim(); } catch (Exception) { Txt_CHIP_Nationality = ""; }
                    try { Txt_CHIP_Gender = Txt_CHIP_Gender.Trim(); } catch (Exception) { Txt_CHIP_Gender = ""; }
                    try { Txt_CHIP_Birthdate = Txt_CHIP_Birthdate.Trim(); } catch (Exception) { Txt_CHIP_Birthdate = ""; }
                    try { Txt_CHIP_DocumentNo = Txt_CHIP_DocumentNo.Trim(); } catch (Exception) { Txt_CHIP_DocumentNo = ""; }
                    try { Txt_CHIP_Data = Txt_CHIP_Data.Trim().Replace("$", "<"); } catch (Exception) { Txt_CHIP_Data = ""; }
                    try { RData = RData.Trim(); } catch (Exception) { RData = ""; }
                    try { VData = VData.Trim(); } catch (Exception) { VData = ""; }
                    try { Validation = Validation.Trim(); } catch (Exception) { Validation = ""; }
                    try { MRZ_ExpireD = MRZ_ExpireD.Trim(); } catch (Exception) { MRZ_ExpireD = ""; }
                    try { Chip_ExpireD = Chip_ExpireD.Trim(); } catch (Exception) { Chip_ExpireD = ""; }
                    string Status_Name = "NA";
                    if (StatusID == "1") { Status_Name = "Accepted"; }
                    if (StatusID == "2") { Status_Name = "Failed"; }
                    if (StatusID == "3") { Status_Name = "Reffered"; }
                    if (StatusID == "4") { Status_Name = "No Validation"; }
                    SQ.Execute_TSql("Update US_BaseInfo Set [FirstName] = '" + Txt_FirstName + "',[LastName] = '" + Txt_LastName + "',[Status_Code] = '" + StatusID + "',[Status_Text] = '" + Status_Name + "',[DocNo] = '" + Txt_DocumentIDF + "' Where (ID = '" + IDVal + "')");
                    SQ.Execute_TSql("Delete From US_GA_01_ImageData Where (BaseID = '" + IDVal + "')");
                    SQ.Execute_TSql("Delete From US_GA_02_ChipData Where (BaseID = '" + IDVal + "')");
                    SQ.Execute_TSql("Delete From US_GA_03_Validate Where (BaseID = '" + IDVal + "')");
                    SQ.Execute_TSql("Insert Into US_GA_01_ImageData Values ('" + IDVal + "','Passport','" + Txt_MRZ_FirstName + "','" + Txt_MRZ_LastName + "','" + Txt_MRZ_Nationality + "','" + Txt_MRZ_Gender + "','" + Txt_MRZ_Birthdate + "','" + Txt_MRZ_DocumentNo + "','" + Txt_MRZ_Data + "','" + MRZ_ExpireD + "')");
                    SQ.Execute_TSql("Insert Into US_GA_02_ChipData Values ('" + IDVal + "','" + Txt_CHIP_FirstName + "','" + Txt_CHIP_LastName + "','" + Txt_CHIP_Nationality + "','" + Txt_CHIP_Gender + "','" + Txt_CHIP_Birthdate + "','" + Txt_CHIP_DocumentNo + "','" + Txt_CHIP_Data + "','" + Txt_AirBaudrate + "','" + Txt_ChipID + "','" + Txt_BAC + "','" + Txt_SAC + "','" + Chip_ExpireD + "')");
                    string ChSel = "";
                    string ChSelV = "";
                    for (int i = 0; i < RData.Length; i++)
                    {
                        ChSel = "0";
                        ChSelV = "0";
                        try
                        {
                            ChSel = RData.Substring(i, 1);
                            ChSelV = VData.Substring(i, 1);
                        }
                        catch (Exception)
                        {
                            ChSel = "0";
                            ChSelV = "0";
                        }
                        if (ChSel == "1")
                        {
                            if (ChSelV == "0")
                            {
                                SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCDG" + (i + 1).ToString() + "_VALIDATE','RFID_VC_NO_VALID')");
                            }
                            else
                            {
                                SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCDG" + (i + 1).ToString() + "_VALIDATE','RFID_VC_VALID')");
                            }
                        }
                    }
                    for (int i = 0; i < Validation.Length; i++)
                    {
                        ChSel = "0";
                        try { ChSel = Validation.Substring(i, 1); } catch (Exception) { ChSel = "0"; }
                        switch (i)
                        {
                            case 0:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCBAC_STATUS','TS_SUCCESS')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCBAC_STATUS','TS_NOT_PERFORMED')"); }
                                    break;
                                }
                            case 1:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SAC_STATUS','TS_SUCCESS')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SAC_STATUS','TS_NOT_PERFORMED')"); }
                                    break;
                                }
                            case 2:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_ACTIVE_AUTHENTICATION','TS_SUCCESS')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_ACTIVE_AUTHENTICATION','TS_NOT_PERFORMED')"); }
                                    break;
                                }
                            case 3:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCSIGNATURE_VALIDATE','RFID_VC_VALID')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCSIGNATURE_VALIDATE','RFID_VC_NO_VALID')"); }
                                    break;
                                }
                            case 4:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_VALIDATE_DOC_SIGNER_CERT','RFID_VC_VALID')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_VALIDATE_DOC_SIGNER_CERT','RFID_VC_NO_CSC_LOADED')"); }
                                    break;
                                }
                            case 5:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCSIGNEDATTRS_VALIDATE','RFID_VC_VALID')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + IDVal + "','CD_SCSIGNEDATTRS_VALIDATE','RFID_VC_NO_VALID')"); }
                                    break;
                                }
                        }
                    }
                    ResSTR = "";
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


                return RedirectToAction("Index", "Dashboard");
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


                return RedirectToAction("Index", "Dashboard");
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


                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
    }
}