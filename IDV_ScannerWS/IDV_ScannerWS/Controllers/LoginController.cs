using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using IDV_ScannerWS.Modules;


namespace IDV_ScannerWS.Controllers
{
    public class LoginController : Controller
    {
        //---------------------------------------------------------------------------------\\
        SQL_Tranceiver Sq = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //---------------------------------------------------------------------------------\\
        public ActionResult Index()
        {
            Session["Admin_Logined"] = "0";
            Session["Admin_UID"] = "0";
            Session["Admin_Username"] = "0";
            Session["Admin_FullName"] = "";
            Session["Admin_Editable"] = "0";
            FormsAuthentication.SignOut();
            return View();
        }
        //---------------------------------------------------------------------------------\\
        [HttpPost]
        public JsonResult UserLogin(string Email, string Password)
        {
            try
            {
                Session["Admin_Logined"] = "0";
                Session["Admin_UID"] = "0";
                Session["Admin_Username"] = "0";
                Session["Admin_FullName"] = "";
                Session["Admin_Editable"] = "0";
                FormsAuthentication.SignOut();
                string ResVal = "0";
                Email = Email.ToString().Trim().ToLower();
                Password = Password.ToString().Trim();
                if ((Email == "") || (Password == "")) { ResVal = "1"; }
                if (ResVal == "0")
                {
                    DataTable DT = new DataTable();
                    DT = Sq.Get_DTable_TSQL("Select UID,Fullname,User_Edit From AD_UserList Where (Username = '" + Email.ToLower() + "') And (Password = '" + Password.ToLower() + "') And (User_Active = '1')");
                    if(DT.Rows.Count==1)
                    {
                        Session["Admin_Logined"] = "1";
                        Session["Admin_UID"] = DT.Rows[0][0].ToString().Trim();
                        Session["Admin_Username"] = Email;
                        Session["Admin_FullName"] = DT.Rows[0][1].ToString().Trim();
                        Session["Admin_Editable"] = DT.Rows[0][2].ToString().Trim();
                        FormsAuthentication.SetAuthCookie("ICADLG", false);
                        ResVal = "2";
                    }
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = "", Value = ResVal } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Text = "", Value = "1" } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //---------------------------------------------------------------------------------\\
    }
}