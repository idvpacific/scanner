using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IDV_ScannerWS.Modules;

namespace IDV_ScannerWS.API
{
    public class DL_03_PublishController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        AssureID AID = new AssureID();
        IDV_OCR IDVOCR = new IDV_OCR();
        //--------------------------------------------------------------------------
        public string PostAsync(string CID, string DID, string UID, string APPID, string UNM, string PAS)
        {
            string LastRes = "";
            try
            {



                string OCRType_Code = "2";
                // Code 1 : IDV OCR
                // Code 2 : Accuant OCR
                // Code 3 : Accuant Authentication



                CID = CID.ToString().Trim();
                DID = DID.ToString().Trim();
                UID = UID.ToString().Trim();
                APPID = APPID.ToString().Trim();
                DataTable DT1 = new DataTable();
                DT1 = SQ.Get_DTable_TSQL("Select ExpiryDate From Reader_Company Where (ID = '" + CID + "') And (Authentication_User = '" + UNM + "') And (Authentication_Password = '" + PAS + "') And (Is_Active = '1') And (Removed = '0')");
                if (DT1.Rows != null)
                {
                    if (DT1.Rows.Count == 1)
                    {
                        string EXPD = DT1.Rows[0][0].ToString().Trim();
                        if (EXPD.Trim() == "") { EXPD = PB.Get_Date(); }
                        if (PB.ExpiredDateCheck(EXPD, PB.Get_Date()) == true)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '2',[Status_Text] = 'Pending',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (Company_ID = '" + CID + "') And (Dealer_ID = '" + DID + "') And (User_ID = '" + UID + "') And (ApplicationID = '" + APPID + "')");
                            SQ.Execute_TSql("Delete From US_DL_06_OCRT Where (APPID = '" + APPID + "')");
                            if (OCRType_Code == "1")
                            {
                                SQ.Execute_TSql("Insert Into US_DL_06_OCRT Values ('" + APPID + "','" + OCRType_Code + "','IDV OCR')");
                            }
                            else
                            {
                                if (OCRType_Code == "2")
                                {
                                    SQ.Execute_TSql("Insert Into US_DL_06_OCRT Values ('" + APPID + "','" + OCRType_Code + "','Acuant OCR')");
                                }
                                else
                                {
                                    SQ.Execute_TSql("Insert Into US_DL_06_OCRT Values ('" + APPID + "','" + OCRType_Code + "','Acuant Authenticate')");
                                }
                            }
                            Task.Run(() =>
                            {
                                if (OCRType_Code == "1")
                                {
                                    IDVOCR.GetData(APPID);
                                }
                                else
                                {
                                    if (OCRType_Code == "2")
                                    {
                                        AID.GetData(APPID, false);
                                    }
                                    else
                                    {
                                        AID.GetData(APPID, true);
                                    }
                                }
                            });
                            LastRes = "OK";
                        }
                        else
                        {
                            LastRes = "ERR_011"; // License expired.
                        }
                    }
                    else
                    {
                        LastRes = "ERR_010"; // Company not founded.
                    }
                }
                else
                {
                    LastRes = "ERR_009"; // Sql no response.
                }
            }
            catch (Exception)
            {
                LastRes = "ERR_008"; // Function error.
            }
            return LastRes;
        }


    }
}
