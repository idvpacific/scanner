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
    public class GA_01_MRZ_DataController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public string Post(string BaseID, string Document_Type, string Forenames, string Surname, string Nationality, string Sex, string Date_of_Birth, string Document_Number, string MRZ, string ExpireDate)
        {
            string Res = "";
            try
            {
                BaseID = BaseID.ToString().Trim();
                Document_Type = Document_Type.ToString().Trim();
                Forenames = Forenames.ToString().Trim();
                Surname = Surname.ToString().Trim();
                Nationality = Nationality.ToString().Trim();
                Sex = Sex.ToString().Trim();
                Date_of_Birth = Date_of_Birth.ToString().Trim();
                Document_Number = Document_Number.ToString().Trim();
                MRZ = MRZ.ToString().Trim();
                ExpireDate = ExpireDate.ToString().Trim();
                SQ.Execute_TSql("Insert Into US_GA_01_ImageData Values ('" + BaseID + "','" + Document_Type + "','" + Forenames + "','" + Surname + "','" + Nationality + "','" + Sex + "','" + Date_of_Birth + "','" + Document_Number + "','" + MRZ + "','" + ExpireDate + "')");
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
