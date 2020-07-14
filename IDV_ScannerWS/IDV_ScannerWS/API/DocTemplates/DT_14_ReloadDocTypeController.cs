using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web.Http;
using System.Web.Mvc;
using IDV_ScannerWS.Modules;
using Newtonsoft.Json;

namespace IDV_ScannerWS.API.DocTemplates
{
    public class DT_14_ReloadDocTypeController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        //--------------------------------------------------------------------------
        public string Post(string TP)
        {
            try
            {
                TP = TP.Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select DocType_ID,DocType_Name From Template_03_DocumentType Order By DocType_Name");
                return JsonConvert.SerializeObject(DT);
            }
            catch (Exception)
            { return ""; }
        }
    }
}
