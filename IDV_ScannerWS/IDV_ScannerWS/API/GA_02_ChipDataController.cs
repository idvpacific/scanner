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
    public class GA_02_ChipDataController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public string Post(string BaseID, string Forenames, string Surname, string Nationality, string Sex, string Date_of_Birth, string Document_Number, string MRZ, string Air_Baud_Rate, string Chip_ID, string BAC, string SAC, string ExpireDate)
        {
            string Res = "";
            try
            {
                BaseID = BaseID.ToString().Trim();
                Forenames = Forenames.ToString().Trim();
                Surname = Surname.ToString().Trim();
                Nationality = Nationality.ToString().Trim();
                Sex = Sex.ToString().Trim();
                Date_of_Birth = Date_of_Birth.ToString().Trim();
                Document_Number = Document_Number.ToString().Trim();
                MRZ = MRZ.ToString().Trim();
                Air_Baud_Rate = Air_Baud_Rate.ToString().Trim();
                Chip_ID = Chip_ID.ToString().Trim();
                BAC = BAC.ToString().Trim();
                SAC = SAC.ToString().Trim();
                ExpireDate = ExpireDate.ToString().Trim();
                SQ.Execute_TSql("Insert Into US_GA_02_ChipData Values ('" + BaseID + "','" + Forenames + "','" + Surname + "','" + Nationality + "','" + Sex + "','" + Date_of_Birth + "','" + Document_Number + "','" + MRZ + "','" + Air_Baud_Rate + "','" + Chip_ID + "','" + BAC + "','" + SAC + "','" + ExpireDate + "')");
                Res = "OK";
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
