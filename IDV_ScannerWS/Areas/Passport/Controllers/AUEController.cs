using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.Areas.Passport.Controllers
{
    public class AUEController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard"); }
        //----------------------------------------------------------------------------------------------------
        public ActionResult GA_NewPassport()
        {
            DataTable DT = new DataTable();
            DT = SQ.Get_DTable_TSQL("Select * From CO_Counter_V_Full Where (User_IsActive = '1') Order By Company_Name,Branch_Name,User_Name");
            ViewBag.DT = DT.Rows;
            return View();
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult GA_NewPassportSave(string Txt_FirstName, string Txt_LastName, string Txt_DocumentIDF, string CounterID, string StatusID, string Txt_AirBaudrate, string Txt_ChipID, string Txt_BAC, string Txt_SAC, string Txt_MRZ_FirstName, string Txt_MRZ_LastName, string Txt_MRZ_Nationality, string Txt_MRZ_Gender, string Txt_MRZ_Birthdate, string Txt_MRZ_DocumentNo, string Txt_MRZ_Data, string Txt_CHIP_FirstName, string Txt_CHIP_LastName, string Txt_CHIP_Nationality, string Txt_CHIP_Gender, string Txt_CHIP_Birthdate, string Txt_CHIP_DocumentNo, string Txt_CHIP_Data, string RData, string VData, string Validation, string MRZ_ExpireD, string Chip_ExpireD)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                if (Txt_FirstName.Trim() == "") { ResVal = "1"; ResSTR += "The first name of the holder of the passport is required\r\n"; }
                if (Txt_LastName.Trim() == "") { ResVal = "1"; ResSTR += "The Last name of the holder of the passport is required\r\n"; }
                if (Txt_DocumentIDF.Trim() == "") { ResVal = "1"; ResSTR += "The Document ID of the passport is required\r\n"; }
                if (CounterID.Trim() == "") { ResVal = "1"; ResSTR += "The passport registration company identification code is required on the system\r\n"; }
                if (StatusID.Trim() == "") { StatusID = "1"; }
                if (ResVal == "0")
                {
                    // Ready Data :
                    try { Txt_FirstName = Txt_FirstName.Trim(); } catch (Exception) { Txt_FirstName = ""; }
                    try { Txt_LastName = Txt_LastName.Trim(); } catch (Exception) { Txt_LastName = ""; }
                    try { Txt_DocumentIDF = Txt_DocumentIDF.Trim(); } catch (Exception) { Txt_DocumentIDF = ""; }
                    try { CounterID = CounterID.Trim(); } catch (Exception) { CounterID = ""; }
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
                    try { Txt_MRZ_Data = Txt_MRZ_Data.Trim().Replace("$","<"); } catch (Exception) { Txt_MRZ_Data = ""; }
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
                    // Base Info :
                    string InsID = "0";
                    string SDK_Name = "Passport Validation [GA]";
                    string Document_Name = "Passport";
                    string Status_Name = "NA";
                    long Ins_Code = SQ.Get_New_ID_Refrence("US_BaseInfo", "Code", "UserCode", CounterID);
                    if (StatusID == "1") { Status_Name = "Accepted"; }
                    if (StatusID == "2") { Status_Name = "Failed"; }
                    if (StatusID == "3") { Status_Name = "Reffered"; }
                    if (StatusID == "4") { Status_Name = "No Validation"; }
                    SQ.Execute_TSql("Insert Into US_BaseInfo Values ('" + Ins_Code.ToString() + "','" + CounterID + "','1','" + SDK_Name + "','1','" + Document_Name + "','" + Txt_FirstName + "','" + Txt_LastName + "','" + PB.Get_Date_Formated("dd/MM/yyyy") + "','" + PB.Get_Time() + "','0','" + StatusID + "','" + Status_Name + "','" + Txt_DocumentIDF + "')");
                    DataTable DT1 = new DataTable();
                    DT1 = SQ.Get_DTable_TSQL("Select ID From US_BaseInfo Where (Code = '" + Ins_Code + "') And (UserCode = '" + CounterID + "')");
                    InsID = DT1.Rows[0][0].ToString().Trim();
                    // MRZ Data :
                    SQ.Execute_TSql("Insert Into US_GA_01_ImageData Values ('" + InsID + "','Passport','" + Txt_MRZ_FirstName + "','" + Txt_MRZ_LastName + "','" + Txt_MRZ_Nationality + "','" + Txt_MRZ_Gender + "','" + Txt_MRZ_Birthdate + "','" + Txt_MRZ_DocumentNo + "','" + Txt_MRZ_Data + "','" + MRZ_ExpireD + "')");
                    // Chip Data
                    SQ.Execute_TSql("Insert Into US_GA_02_ChipData Values ('" + InsID + "','" + Txt_CHIP_FirstName + "','" + Txt_CHIP_LastName + "','" + Txt_CHIP_Nationality + "','" + Txt_CHIP_Gender + "','" + Txt_CHIP_Birthdate + "','" + Txt_CHIP_DocumentNo + "','" + Txt_CHIP_Data + "','" + Txt_AirBaudrate + "','" + Txt_ChipID + "','" + Txt_BAC + "','" + Txt_SAC + "','" + Chip_ExpireD + "')");
                    // Chip Validation Data :
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
                                SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCDG" + (i + 1).ToString() + "_VALIDATE','RFID_VC_NO_VALID')");
                            }
                            else
                            {
                                SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCDG" + (i + 1).ToString() + "_VALIDATE','RFID_VC_VALID')");
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
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCBAC_STATUS','TS_SUCCESS')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCBAC_STATUS','TS_NOT_PERFORMED')"); }
                                    break;
                                }
                            case 1:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SAC_STATUS','TS_SUCCESS')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SAC_STATUS','TS_NOT_PERFORMED')"); }
                                    break;
                                }
                            case 2:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_ACTIVE_AUTHENTICATION','TS_SUCCESS')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_ACTIVE_AUTHENTICATION','TS_NOT_PERFORMED')"); }
                                    break;
                                }
                            case 3:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCSIGNATURE_VALIDATE','RFID_VC_VALID')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCSIGNATURE_VALIDATE','RFID_VC_NO_VALID')"); }
                                    break;
                                }
                            case 4:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_VALIDATE_DOC_SIGNER_CERT','RFID_VC_VALID')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_VALIDATE_DOC_SIGNER_CERT','RFID_VC_NO_CSC_LOADED')"); }
                                    break;
                                }
                            case 5:
                                {
                                    if (ChSel == "2") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCSIGNEDATTRS_VALIDATE','RFID_VC_VALID')"); }
                                    if (ChSel == "3") { SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + InsID + "','CD_SCSIGNEDATTRS_VALIDATE','RFID_VC_NO_VALID')"); }
                                    break;
                                }
                        }
                    }
                    // Feed Back;
                    ResSTR = InsID;
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
        [HttpPost]
        public JsonResult GA_NewPassportSaveImage(string DID, string FileCode, HttpPostedFileBase IMGF)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                DID = DID.Trim();
                FileCode = FileCode.Trim();
                if (IMGF == null) { ResVal = "1"; ResSTR = ""; }
                if (ResVal == "0")
                {
                    string FileNameLst = "";
                    switch (FileCode)
                    {
                        case "1": { FileNameLst = "FC"; break; }
                        case "2": { FileNameLst = "RF"; break; }
                        case "3": { FileNameLst = "NM"; break; }
                        case "4": { FileNameLst = "UV"; break; }
                        case "5": { FileNameLst = "IR"; break; }
                    }
                    var path = Server.MapPath("~/Photos/Gemalto/" + DID);
                    if (!System.IO.Directory.Exists(path)) { System.IO.Directory.CreateDirectory(path); }
                    IMGF.SaveAs(path + "/" + FileNameLst + ".jpeg");
                    ResSTR = "";
                    ResVal = "2";
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = ResSTR.Trim(), Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "Base data added successfully but for image uploading the server encountered an error while executing your request" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult GA_NewDrivinglicense()
        {
            return RedirectToAction("Index", "Dashboard");
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult AC_NewPassport()
        {
            return RedirectToAction("Index", "Dashboard");
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult AC_NewDrivinglicense()
        {
            return RedirectToAction("Index", "Dashboard");
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult EditSelector()
        {
            try
            {
                if (RouteData.Values["id"] == null) { return RedirectToAction("Index", "Dashboard"); }
                string Doc_ID = RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select SDK_Code,Document_ID From US_BaseInfo Where (ID = '" + Doc_ID + "')");
                if (DT.Rows.Count != 1) { return RedirectToAction("Index", "Dashboard"); }
                if ((DT.Rows[0][0].ToString().Trim() == "1") && (DT.Rows[0][1].ToString().Trim() == "1")) { return RedirectToAction("GAP", "Editor", new { id = Doc_ID }); }
                if ((DT.Rows[0][0].ToString().Trim() == "1") && (DT.Rows[0][1].ToString().Trim() == "2")) { return RedirectToAction("GAD", "Editor", new { id = Doc_ID }); }
                if ((DT.Rows[0][0].ToString().Trim() == "2") && (DT.Rows[0][1].ToString().Trim() == "1")) { return RedirectToAction("ACP", "Editor", new { id = Doc_ID }); }
                if ((DT.Rows[0][0].ToString().Trim() == "2") && (DT.Rows[0][1].ToString().Trim() == "2")) { return RedirectToAction("ACD", "Editor", new { id = Doc_ID }); }
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