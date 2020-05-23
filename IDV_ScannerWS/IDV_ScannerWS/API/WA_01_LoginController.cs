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
    public class WA_01_LoginController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        Crypt CT = new Crypt();
        //--------------------------------------------------------------------------
        public string Post(string CID, string ATU, string ATP,string ConPass)
        {
            string Res = "";
            try
            {
                CID = CID.Trim();
                ATU = ATU.Trim();
                ATP = ATP.Trim();
                ConPass = ConPass.Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Config_Password,ExpiryDate From Reader_Company Where (ID = '" + CID + "') And (Authentication_User = '" + ATU + "') And (Authentication_Password = '" + ATP + "') And (Is_Active = '1') And (Removed = '0')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        bool ErrRes = false;
                        string EXPD = DT.Rows[0][1].ToString().Trim();
                        if (EXPD != "")
                        {
                            if (PB.ExpiredDateCheck(EXPD, PB.Get_Date()) == false)
                            {
                                Res = "ERR";
                                ErrRes = true;
                            }
                        }
                        if (ErrRes == false)
                        {
                            if(ConPass.ToUpper()==DT.Rows[0][0].ToString().Trim().ToUpper())
                            {
                                string OKRes = "EMASOK";
                                string RanKey = PB.Make_Security_Code(10);
                                Res = CT.Encrypt(OKRes, RanKey) + RanKey;
                            }
                            else
                            {
                                Res = "ERR";
                            }
                        }
                    }
                    else
                    {
                        Res = "ERR";
                    }
                }
                else
                {
                    Res = "ERR";
                }
            }
            catch (Exception)
            {
                Res = "ERR";
            }
            return Res;
        }

    }
}
