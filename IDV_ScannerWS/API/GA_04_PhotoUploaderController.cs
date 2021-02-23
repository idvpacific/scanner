using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using IDV_ScannerWS.Modules;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;



namespace IDV_ScannerWS.API
{
    public class GA_04_PhotoUploaderController : ApiController
    {
        public string Post(String BaseID)
        {
            string Res = "";
            try
            {
                string BaseFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Gemalto/" + BaseID);
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        if (Directory.Exists(BaseFolder) == false) { Directory.CreateDirectory(BaseFolder); }
                        var filePath = HttpContext.Current.Server.MapPath("~/Photos/Gemalto/" + BaseID + "/" + postedFile.FileName);
                        postedFile.SaveAs(filePath);
                    }
                }
                Res = "OK";
            }
            catch (Exception)
            {
                Res = "ERR";
            }
            return Res;
        }
    }

}
//--------------------------------------------------------------------------
