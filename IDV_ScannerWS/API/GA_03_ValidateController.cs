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
    public class GA_03_ValidateController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public string Post(string BaseID, string Item_Code, string Validate)
        {
            string Res = "";
            try
            {
                BaseID = BaseID.ToString().Trim();
                Item_Code = Item_Code.ToString().Trim();
                Validate = Validate.ToString().Trim();
                SQ.Execute_TSql("Insert Into US_GA_03_Validate Values ('" + BaseID + "','" + Item_Code + "','" + Validate + "')");
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
