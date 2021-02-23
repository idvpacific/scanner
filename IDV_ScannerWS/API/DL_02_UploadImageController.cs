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
    public class DL_02_UploadImageController : ApiController
    {
        public string Post(String AppID,string ImageCode)
        {
            string Res = "";
            try
            {
                AppID = AppID.Trim();
                ImageCode = ImageCode.Trim();
                string BaseFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID);
                var httpRequest = HttpContext.Current.Request;
                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);
                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        if (Directory.Exists(BaseFolder) == false) { Directory.CreateDirectory(BaseFolder); }
                        if (Directory.Exists(BaseFolder + "\\" + "Scanned") == false) { Directory.CreateDirectory(BaseFolder + "\\" + "Scanned"); }
                        if (Directory.Exists(BaseFolder + "\\" + "Result") == false) { Directory.CreateDirectory(BaseFolder + "\\" + "Result"); }
                        var filePath = HttpContext.Current.Server.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + ImageCode + ".jpg");
                        postedFile.SaveAs(filePath);
                    }
                }
                Res = "OK";
            }
            catch (Exception)
            {
                Res = "ERR_007" + " - " + AppID + " - " + ImageCode;
            }
            return Res;
        }
    }
}
