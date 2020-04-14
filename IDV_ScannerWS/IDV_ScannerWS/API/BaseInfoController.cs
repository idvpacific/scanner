using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.API
{
    public class BaseInfoController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public string Post(string UserCode, string SDK_Code, string Document_ID, string FirstName, string LastName,string StatusCode,string DocNo)
        {
            string Res = "";
            try
            {
                UserCode = UserCode.ToString().Trim();
                SDK_Code = SDK_Code.ToString().Trim();
                Document_ID = Document_ID.ToString().Trim();
                FirstName = FirstName.ToString().Trim();
                LastName = LastName.ToString().Trim();
                StatusCode = StatusCode.ToString().Trim();
                DocNo = DocNo.ToString().Trim();
                string SDK_Name = "NA";
                string Document_Name = "NA";
                string Status_Name = "NA";
                long Ins_Code = SQ.Get_New_ID_Refrence("US_BaseInfo", "Code", "UserCode", UserCode);
                if (SDK_Code == "1") { SDK_Name = "Passport Validation [GA]"; }
                if (SDK_Code == "2") { SDK_Name = "Scanner Validation"; }
                if (Document_ID == "1") { Document_Name = "Passport"; }
                if (Document_ID == "2") { Document_Name = "Driving Licence"; }
                if (StatusCode == "1") { Status_Name = "Accepted"; }
                if (StatusCode == "2") { Status_Name = "Failed"; }
                if (StatusCode == "3") { Status_Name = "Reffered"; }
                if (StatusCode == "4") { Status_Name = "No Validation"; }
                SQ.Execute_TSql("Insert Into US_BaseInfo Values ('" + Ins_Code.ToString() + "','" + UserCode + "','" + SDK_Code + "','" + SDK_Name + "','" + Document_ID + "','" + Document_Name + "','" + FirstName + "','" + LastName + "','" + PB.Get_Date_Formated("dd/MM/yyyy") + "','" + PB.Get_Time() + "','0','" + StatusCode + "','" + Status_Name + "','" + DocNo + "')");
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select ID From US_BaseInfo Where (Code = '" + Ins_Code + "') And (UserCode = '" + UserCode + "')");
                Res = DT.Rows[0][0].ToString().Trim();
            }
            catch (Exception)
            {
                Res = "ERR";
            }
            return Res;
        }
        //--------------------------------------------------------------------------
    }
}
