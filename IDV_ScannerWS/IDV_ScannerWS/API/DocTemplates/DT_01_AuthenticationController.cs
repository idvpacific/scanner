﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.API.DocTemplates
{
    public class DT_01_AuthenticationController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        //--------------------------------------------------------------------------
        public string Post(string USN, string PSW)
        {
            string LastRes = "";
            try
            {
                USN = USN.ToString().Trim();
                PSW = PSW.ToString().Trim();
                DataTable DT1 = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select UID,Fullname From AD_UserList Where (Username = '" + USN + "') And (Password = '" + PSW + "') And (User_Active = '1')");
                if (DT1.Rows != null)
                {
                    if (DT1.Rows.Count == 1)
                    {
                        LastRes = "EMASOK_" + DT1.Rows[0][0].ToString().Trim() + "_" + DT1.Rows[0][1].ToString().Trim();
                    }
                    else
                    {
                        LastRes = "ER3"; // Company not founded.
                    }
                }
                else
                {
                    LastRes = "ER2"; // Sql no response.
                }
            }
            catch (Exception)
            {
                LastRes = "ER1"; // Function error.
            }
            return LastRes;
        }
        //--------------------------------------------------------------------------
    }
}
