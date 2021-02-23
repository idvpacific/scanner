using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using IDV_ScannerWS.Modules;
using System.Web.Security;
using System.IO;

namespace IDV_ScannerWS.Areas.DrivingLicence.Controllers
{
    public class CustomersController : Controller
    {
        //----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        Crypt EncDec = new Crypt();
        //----------------------------------------------------------------------------------------------------
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard", new { id = "" }); }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Company()
        {
            try
            {
                ViewBag.DT = null;
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,Name,Prime_Contact_First_Name + ' ' + Prime_Contact_Last_Name,Prime_Contact_Email,Prime_Contact_Position,Secondary_Contact_First_Name + ' ' + Secondary_Contact_Last_Name,Secondary_Contact_Email,Secondary_Contact_Position,Is_Active From Reader_Company Where (Removed = '0') Order By Name");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult Company_GridReload()
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,Name,Prime_Contact_First_Name + ' ' + Prime_Contact_Last_Name,Prime_Contact_Email,Prime_Contact_Position,Secondary_Contact_First_Name + ' ' + Secondary_Contact_Last_Name,Secondary_Contact_Email,Secondary_Contact_Position,Is_Active From Reader_Company Where (Removed = '0') Order By Name");
                foreach (DataRow Row in DT.Rows)
                {
                    ResSTR += "<tr>";
                    ResSTR += "<td style=\"text-align:left;cursor:pointer\" onclick=\"ShowBranch('" + Row[0].ToString().Trim() + "')\">" + Row[1].ToString().Trim() + "</td>";
                    ResSTR += "<td style=\"text-align:left;cursor:pointer\" onclick=\"ShowBranch('" + Row[0].ToString().Trim() + "')\">" + Row[2].ToString().Trim() + "</td>";
                    ResSTR += "<td style=\"text-align:left;cursor:pointer\" onclick=\"ShowBranch('" + Row[0].ToString().Trim() + "')\">" + Row[3].ToString().Trim() + "</td>";
                    ResSTR += "<td style=\"text-align:left;cursor:pointer\" onclick=\"ShowBranch('" + Row[0].ToString().Trim() + "')\">" + Row[4].ToString().Trim() + "</td>";
                    if (Row[8].ToString().Trim() == "0")
                    {
                        ResSTR += "<td style=\"text-align:center;cursor:pointer\" onclick=\"ShowBranch('" + Row[0].ToString().Trim() + "')\"><i class=\"far fa-circle\"></i></td>";
                    }
                    else
                    {
                        ResSTR += "<td style=\"text-align:center;cursor:pointer\" onclick=\"ShowBranch('" + Row[0].ToString().Trim() + "')\"><i class=\"far fa-check-circle\"></i></td>";
                    }
                    ResSTR += "<td style=\"text-align:center\">";
                    ResSTR += "<a href=\"javascript:void(0)\" onclick=\"DLCF('" + Row[0].ToString().Trim() + "','1')\" class=\"fa fa-sticky-note\" style=\"text-decoration:none;margin-right:5px\" title=\" Download Authentication Keys \"></a>";
                    ResSTR += "<a href=\"javascript:void(0)\" onclick=\"DLCF('" + Row[0].ToString().Trim() + "','2')\" class=\"fa fa-lock\" style=\"text-decoration:none;margin-right:5px\" title=\" Download Secure Config File \"></a>";
                    ResSTR += "<a href=\"javascript:void(0)\" onclick=\"ShowInfo('" + Row[0].ToString().Trim() + "')\" class=\"fa fa-info-circle\" style=\"text-decoration:none;margin-right:5px\" title=\" Show Company Information \"></a>";
                    ResSTR += "<a href=\"javascript:void(0)\" onclick=\"RowEdit('" + Row[0].ToString().Trim() + "')\" class=\"fa fa-pen\" style=\"text-decoration:none;margin-right:5px\" title=\" Edit Company Account \"></a>";
                    ResSTR += "<a href=\"javascript:void(0)\" onclick=\"RowDelete('" + Row[0].ToString().Trim() + "')\" class=\"fa fa-trash\" style=\"text-decoration:none\" title=\" Delete Company Account \"></a>";
                    ResSTR += "</td>";
                    ResSTR += "</tr>";
                }
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
        public JsonResult Company_A1(string NM, string ABN, string EXD, string ADD)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                NM = NM.ToString().Trim();
                ABN = ABN.ToString().Trim();
                EXD = EXD.ToString().Trim();
                ADD = ADD.ToString().Trim();
                long BC = SQ.Get_New_ID("Reader_Company", "ID");
                SQ.Execute_TSql("Insert Into Reader_Company Values ('" + BC.ToString() + "','" + NM + "','" + ABN + "','" + ADD + "','','','','','','','','','','','','','','','','','" + EXD + "','1','0','" + PB.Get_Date() + "','" + PB.Get_Time() + "')");
                ResSTR = BC.ToString();
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
        public JsonResult Company_A2(string BC, string PK, string PS, string CBU, string CPS, string ATU, string ATP)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                BC = BC.ToString().Trim();
                PK = PK.ToString().Trim();
                PS = PS.ToString().Trim();
                CBU = CBU.ToString().Trim();
                CPS = CPS.ToString().Trim();
                ATU = ATU.ToString().Trim();
                ATP = ATP.ToString().Trim();
                SQ.Execute_TSql("Update Reader_Company Set [API_PrivateKey] = '" + PK + "',[API_Password] = '" + PS + "',[CallBackURL] = '" + CBU + "',[Config_Password] = '" + CPS + "',[Authentication_User] = '" + ATU + "',[Authentication_Password] = '" + ATP + "' Where (ID = '" + BC + "')");
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
        public JsonResult Company_A3(string BC, string FN, string LN, string PS, string PN, string EA)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                BC = BC.ToString().Trim();
                FN = FN.ToString().Trim();
                LN = LN.ToString().Trim();
                PS = PS.ToString().Trim();
                PN = PN.ToString().Trim();
                EA = EA.ToString().Trim();
                SQ.Execute_TSql("Update Reader_Company Set [Prime_Contact_First_Name] = '" + FN + "',[Prime_Contact_Last_Name] = '" + LN + "',[Prime_Contact_Email] = '" + EA + "',[Prime_Contact_Phone] = '" + PN + "',[Prime_Contact_Position] = '" + PS + "' Where (ID = '" + BC + "')");
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
        public JsonResult Company_A4(string BC, string FN, string LN, string PS, string PN, string EA)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                BC = BC.ToString().Trim();
                FN = FN.ToString().Trim();
                LN = LN.ToString().Trim();
                PS = PS.ToString().Trim();
                PN = PN.ToString().Trim();
                EA = EA.ToString().Trim();
                SQ.Execute_TSql("Update Reader_Company Set [Secondary_Contact_First_Name] = '" + FN + "',[Secondary_Contact_Last_Name] = '" + LN + "',[Secondary_Contact_Email] = '" + EA + "',[Secondary_Contact_Phone] = '" + PN + "',[Secondary_Contact_Position] = '" + PS + "' Where (ID = '" + BC + "')");
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
        public JsonResult Company_ShowInfo(string CID)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                CID = CID.ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From Reader_Company Where (ID = '" + CID + "')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        ResVal = "2";
                        ResSTR += "Company Name" + " : " + DT.Rows[0][1].ToString().Trim() + "\r\n";
                        ResSTR += "Company ABN" + " : " + DT.Rows[0][2].ToString().Trim() + "\r\n";
                        ResSTR += "Expiry Date" + " : " + DT.Rows[0][20].ToString().Trim() + "\r\n";
                        ResSTR += "Company Address" + " : " + DT.Rows[0][3].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Prime Contact :" + "\r\n";
                        ResSTR += "First Name" + " : " + DT.Rows[0][4].ToString().Trim() + "\r\n";
                        ResSTR += "Last Name" + " : " + DT.Rows[0][5].ToString().Trim() + "\r\n";
                        ResSTR += "Position" + " : " + DT.Rows[0][8].ToString().Trim() + "\r\n";
                        ResSTR += "Phone" + " : " + DT.Rows[0][7].ToString().Trim() + "\r\n";
                        ResSTR += "Email" + " : " + DT.Rows[0][6].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Secondary Contact :" + "\r\n";
                        ResSTR += "First Name" + " : " + DT.Rows[0][9].ToString().Trim() + "\r\n";
                        ResSTR += "Last Name" + " : " + DT.Rows[0][10].ToString().Trim() + "\r\n";
                        ResSTR += "Position" + " : " + DT.Rows[0][13].ToString().Trim() + "\r\n";
                        ResSTR += "Phone" + " : " + DT.Rows[0][12].ToString().Trim() + "\r\n";
                        ResSTR += "Email" + " : " + DT.Rows[0][11].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Private Key :" + "\r\n";
                        ResSTR += DT.Rows[0][14].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Password Key :" + "\r\n";
                        ResSTR += DT.Rows[0][15].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Callback URL :" + "\r\n";
                        ResSTR += DT.Rows[0][16].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Windows App Password :" + "\r\n";
                        ResSTR += DT.Rows[0][17].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Authentication Username :" + "\r\n";
                        ResSTR += DT.Rows[0][18].ToString().Trim() + "\r\n" + "\r\n";
                        ResSTR += "* Authentication Password :" + "\r\n";
                        ResSTR += DT.Rows[0][19].ToString().Trim() + "\r\n" + "\r\n";
                    }
                    else { ResVal = "1"; ResSTR = "Company not founded to show information"; }
                }
                else { ResVal = "1"; ResSTR = "Company not founded to show information"; }

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
        public JsonResult Company_Remove(string CID)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                CID = CID.ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Name From Reader_Company Where (ID = '" + CID + "')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        string ComName = DT.Rows[0][0].ToString().Trim();
                        SQ.Execute_TSql("Update Reader_Company Set [Removed] = '1' Where (ID = '" + CID + "')");
                        ResVal = "2";
                        ResSTR = "Selected company [ " + ComName + " ] deleted successfully from list";
                    }
                    else { ResVal = "1"; ResSTR = "Company not founded to delete from list"; }
                }
                else { ResVal = "1"; ResSTR = "Company not founded to delete from list"; }
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
        public JsonResult GenerateRandomKey(string WOT)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                bool EXFW = false;
                DataTable DT = new DataTable();
                string WFS = "";
                if (WOT == "1") { WFS = "API_PrivateKey"; }
                if (WOT == "2") { WFS = "API_Password"; }
                if (WOT == "3") { WFS = "Config_Password"; }
                if (WOT == "4") { WFS = "Authentication_User"; }
                if (WOT == "5") { WFS = "Authentication_Password"; }
                while (EXFW == false)
                {
                    if (WOT == "1") { ResSTR = PB.Make_Security_Code(40); }
                    if (WOT == "2") { ResSTR = PB.Make_Security_Code(20); }
                    if (WOT == "3") { ResSTR = PB.Make_Security_Code(8); }
                    if (WOT == "4") { ResSTR = PB.Make_Security_Code(8); }
                    if (WOT == "5") { ResSTR = PB.Make_Security_Code(10); }
                    DT = new DataTable();
                    DT = SQ.Get_DTable_TSQL("Select ID From Reader_Company Where (" + WFS + " = '" + ResSTR + "')");
                    if (DT.Rows != null)
                    {
                        if (DT.Rows.Count == 0) { EXFW = true; }
                    }
                    else { EXFW = true; }
                }
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
        public ActionResult DwnldFL()
        {
            try
            {
                var DATAID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                string[] DFET = DATAID.Split('-');
                string ComID = DFET[0].ToString().Trim();
                string DataType = DFET[1].ToString().Trim();
                string CompanyID = "";
                string CompanyName = "";
                string LicExpDate = "";
                string PK = "";
                string PS = "";
                string WPS = "";
                string ATU = "";
                string ATP = "";
                string MainBody = "";
                string DownFileName = "";
                string API_Address = "";
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID,Name,API_PrivateKey,API_Password,Config_Password,Authentication_User,Authentication_Password,ExpiryDate From Reader_Company Where (ID = '" + ComID + "') And (Is_Active = '1') And (Removed = '0')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        CompanyID = DT.Rows[0][0].ToString().Trim();
                        CompanyName = DT.Rows[0][1].ToString().Trim();
                        LicExpDate = DT.Rows[0][7].ToString().Trim();
                        PK = DT.Rows[0][2].ToString().Trim();
                        PS = DT.Rows[0][3].ToString().Trim();
                        WPS = DT.Rows[0][4].ToString().Trim();
                        ATU = DT.Rows[0][5].ToString().Trim();
                        ATP = DT.Rows[0][6].ToString().Trim();
                        API_Address = Request.Url.GetLeftPart(UriPartial.Authority);
                        if (API_Address.Substring((API_Address.Length) - 1, 1) == "/")
                        {
                            API_Address = API_Address.Substring(0, API_Address.Length - 1).Trim();
                        }
                        if (DataType == "1")
                        {
                            MainBody = "";
                            MainBody += "---------------------------------------------------------" + "\r\n";
                            MainBody += "-                  IDV Pacific Pty Ltd                  -" + "\r\n";
                            MainBody += "---------------------------------------------------------" + "\r\n";
                            MainBody += "- Create Date : " + PB.Get_Date() + "\r\n";
                            MainBody += "- Create Time : " + PB.Get_Time() + "\r\n";
                            MainBody += "- Create For : " + CompanyName + " Company" + "\r\n";
                            if (LicExpDate.Trim() != "")
                            {
                                MainBody += "- License Expired : " + LicExpDate + "\r\n";
                            }
                            else
                            {
                                MainBody += "- License Expired : " + "Unlimited" + "\r\n";
                            }
                            MainBody += "---------------------------------------------------------" + "\r\n\r\n";
                            MainBody += "API Address : " + API_Address + "\r\n\r\n";
                            MainBody += "---------------------------------------------------------" + "\r\n\r\n";
                            MainBody += "API Private Key : " + PK + "\r\n";
                            MainBody += "API Password : " + PS + "\r\n\r\n";
                            MainBody += "---------------------------------------------------------" + "\r\n\r\n";
                            MainBody += "Windows Application Configuration Password : " + WPS + "\r\n\r\n";
                            MainBody += "---------------------------------------------------------" + "\r\n\r\n";
                            MainBody += "Authentication Username : " + ATU + "\r\n";
                            MainBody += "Authentication Password : " + ATP + "\r\n\r\n";
                            MainBody += "---------------------------------------------------------" + "\r\n";
                            MainBody += "-               All right reserved © 2020               -" + "\r\n";
                            MainBody += "---------------------------------------------------------";
                            DownFileName = CompanyName.Trim() + " - All Secure Keys.txt";
                        }
                        else
                        {
                            string EncCode1 = PB.Make_Security_Code(10);
                            System.Threading.Thread.Sleep(100);
                            string EncCode2 = PB.Make_Security_Code(15);
                            System.Threading.Thread.Sleep(100);
                            string EncCode3 = PB.Make_Security_CodeFake(1200);
                            System.Threading.Thread.Sleep(100);
                            string EncCode4 = PB.Make_Security_CodeFake(2300);
                            CompanyID = CompanyID.Replace("-", "_");
                            CompanyName = CompanyName.Replace("-", "_");
                            LicExpDate = LicExpDate.Replace("-", "_");
                            PK = PK.Replace("-", "^");
                            PS = PS.Replace("-", "^");
                            WPS = WPS.Replace("-", "^");
                            ATU = ATU.Replace("-", "^");
                            ATP = ATP.Replace("-", "^");
                            API_Address = API_Address.Replace("-", "^");
                            MainBody = "IDV" + "-" + CompanyID + "-" + CompanyName + "-" + LicExpDate + "-" + PK + "-" + API_Address + "-" + "EMAS" + "-" + PS + "-" + WPS + "-" + ATU + "-" + ATP + "-" + "PACIFIC";
                            MainBody = EncDec.Encrypt(MainBody, EncCode1);
                            MainBody = EncDec.Encrypt(MainBody, EncCode2);
                            MainBody = EncCode3 + EncCode1 + MainBody + EncCode2 + EncCode4;
                            string FKMB = MainBody.Trim();
                            MainBody = "";
                            for (int i = 0; i < FKMB.Length; i++)
                            {
                                MainBody += FKMB.Substring(i, 1);
                                if (((i % 80) == 0) && (i != 0)) { MainBody += "\r\n"; }
                            }
                            DownFileName = CompanyName.Trim() + " - Config.IDV";
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(404);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(404);
                }
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(MainBody);
                writer.Flush();
                stream.Position = 0;
                return File(stream, "text/csv", DownFileName);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(404);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult CompanyEditor()
        {
            try
            {
                var ComID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From Reader_Company Where (ID = '" + ComID + "') And (Removed = '0')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        ViewBag.ComID = DT.Rows[0][0].ToString().Trim();
                        ViewBag.ComName = DT.Rows[0][1].ToString().Trim();
                        ViewBag.DTR = DT.Rows[0];
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Company", "Customers", new { id = "" });
                    }
                }
                else
                {
                    return RedirectToAction("Company", "Customers", new { id = "" });
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Company", "Customers", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        [HttpPost]
        public JsonResult Company_A1_Edit(string BC, string NM, string ABN, string EXD, string ADD, string CKAC)
        {
            try
            {
                string ResSTR = ""; string ResVal = "0";
                BC = BC.ToString().Trim();
                NM = NM.ToString().Trim();
                ABN = ABN.ToString().Trim();
                EXD = EXD.ToString().Trim();
                ADD = ADD.ToString().Trim();
                CKAC = CKAC.ToString().Trim();
                string ISAC = "1";
                if (CKAC.ToString().Trim().ToLower() == "false") { ISAC = "0"; }
                SQ.Execute_TSql("Update Reader_Company Set [Name] = '" + NM + "',[ABN] = '" + ABN + "',[Address] = '" + ADD + "',[ExpiryDate] = '" + EXD + "',[Is_Active] = '" + ISAC + "' Where (ID = '" + BC + "')");
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
        //----------------------------------------------------------------------------------------------------
        public ActionResult Dealer()
        {
            try
            {
                ViewBag.DT = null;
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From US_DL_02_DealerList_V Order By Dealer_ID,Name");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Device()
        {
            try
            {
                ViewBag.DT = null;
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From US_DL_03_DeviceList_V Order By User_ID,Dealer_ID,Name");
                ViewBag.DT = DT.Rows;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ActionResult Logs()
        {
            try
            {
                ViewBag.DT = null;
                string TDDT = PB.Get_Date();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Company_ID_T,Branch_ID_T,Counter_ID_T,Device_Name,Ins_Date,Ins_Time,IP_Address,Desctiption From Reader_Log Where (Ins_Date = '" + TDDT + "') Order By Ins_Date,Ins_Time");
                ViewBag.DT = DT.Rows;
                ViewBag.DateN = TDDT;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "" });
            }
        }
        //----------------------------------------------------------------------------------------------------
        public JsonResult ReloadGrid(string CN, string DID, string UID, string IP, string SD)
        {
            try
            {
                string GridView_HTML = "";
                string LSTDate = "";
                CN = CN.Trim();
                DID = DID.Trim();
                UID = UID.Trim();
                IP = IP.Trim();
                SD = SD.Trim();
                if (CN != "") { CN = " And (Company_ID_T = '" + CN + "')"; }
                if (DID != "") { DID = " And (Branch_ID_T = '" + DID + "')"; }
                if (UID != "") { UID = " And (Counter_ID_T = '" + UID + "')"; }
                if (IP != "") { IP = " And (IP_Address = '" + IP + "')"; }
                if (SD != "")
                {
                    LSTDate = " And (Ins_Date = '" + SD + "')";
                }
                else
                {
                    LSTDate = " And (Ins_Date = '" + PB.Get_Date() + "')";
                }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Company_ID_T,Branch_ID_T,Counter_ID_T,Device_Name,Ins_Date,Ins_Time,IP_Address,Desctiption From Reader_Log Where (Device_Name <> '')" + CN + DID + UID + IP + LSTDate + " Order By Ins_Date,Ins_Time");
                foreach (DataRow Row in DT.Rows)
                {
                    GridView_HTML += "<tr>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[0].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[1].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[2].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[3].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[4].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[5].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:center;cursor:pointer\">" + Row[6].ToString().Trim() + "</td>";
                    GridView_HTML += "<td style=\"text-align:left;cursor:pointer\">" + Row[7].ToString().Trim() + "</td>";
                    GridView_HTML += "</tr>";
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = GridView_HTML.Trim(), Value = "2" } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //----------------------------------------------------------------------------------------------------
    }
}