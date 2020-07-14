using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Accord;
using IDV_ScannerWS.Modules;
using Newtonsoft.Json;

namespace IDV_ScannerWS.API
{
    public class ResultItems
    {
        public string ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class DL_05_ResultController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        //--------------------------------------------------------------------------
        public string Post(string APPID)
        {
            var RSTITM = new List<ResultItems>();
            try
            {
                string DTSel = "2";
                try
                {
                    DataTable DTT = new DataTable();
                    DTT = SQ.Get_DTable_TSQL("Select OCR_Code From US_DL_06_OCRT Where (APPID = '" + APPID + "')");
                    DTSel = DTT.Rows[0][0].ToString().Trim();
                }
                catch (Exception)
                { }
                APPID = APPID.Trim();
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select Document_ID,Document_Key,Document_Value From US_DL_03_Documents Where (App_ID = '" + APPID + "') Order By Document_ID");
                if ((DTSel == "2") || (DTSel == "3"))
                {
                    foreach (DataRow RW in DT.Rows)
                    {
                        int AddACC = 0;
                        if ((RW[1].ToString().ToLower().IndexOf("given") >= 0) || (RW[1].ToString().ToLower().Replace(" ", "").Trim() == "givenname")) { AddACC = 1; }
                        if ((RW[1].ToString().ToLower().IndexOf("sur") >= 0) || (RW[1].ToString().ToLower().Replace(" ", "").Trim() == "surname")) { AddACC = 2; }
                        if ((RW[1].ToString().ToLower().IndexOf("birth") >= 0) || (RW[1].ToString().ToLower().Replace(" ", "").Trim() == "birthdate")) { AddACC = 3; }
                        if (RW[1].ToString().ToLower().Replace(" ", "").Trim() == "documentnumber") { AddACC = 4; }
                        if ((RW[1].ToString().ToLower().IndexOf("expiration") >= 0) || (RW[1].ToString().ToLower().Replace(" ", "").Trim() == "expirationdate")) { AddACC = 5; }
                        if (RW[1].ToString().ToLower().Replace(" ", "").Trim() == "address") { AddACC = 6; }
                        if (AddACC != 0)
                        {
                            ResultItems NRI = new ResultItems();
                            NRI.ID = AddACC.ToString().Trim();
                            switch (AddACC)
                            {
                                case 1: { NRI.Key = "Firstname"; break; }
                                case 2: { NRI.Key = "Surname"; break; }
                                case 3: { NRI.Key = "Date of Birth"; break; }
                                case 4: { NRI.Key = "Licence Number"; break; }
                                case 5: { NRI.Key = "Licence Expiry"; break; }
                                case 6: { NRI.Key = "Address"; break; }
                            }
                            NRI.Value = RW[2].ToString().Trim();
                            RSTITM.Add(NRI);
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception) { }
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(RSTITM);
        }
    }
}
