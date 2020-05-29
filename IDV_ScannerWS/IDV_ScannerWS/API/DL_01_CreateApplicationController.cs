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
    public class DL_01_CreateApplicationController : ApiController
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        public string Post(string CID, string DID, string UID, string IP, string UNM, string PAS)
        {
            string LastRes = "";
            try
            {
                CID = CID.ToString().Trim();
                DID = DID.ToString().Trim();
                UID = UID.ToString().Trim();
                IP = IP.ToString().Trim();
                UNM = UNM.ToString().Trim();
                PAS = PAS.ToString().Trim();
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
                            string RelatCode = (CID + DID + UID + PB.Make_Security_Code(10) + PB.Get_Date().Replace("/", "") + PB.Get_Time().Replace(":", "")).Replace(" ", "");
                            SQ.Execute_TSql("Insert Into US_DL_01_Application Values ('" + CID + "','" + DID + "','" + UID + "','" + IP + "','" + PB.Get_Date() + "','" + PB.Get_Time() + "','" + RelatCode + "','0','1','Wait For Upload','" + PB.Get_Date() + "','" + PB.Get_Time() + "')");
                            DataTable DT2 = new DataTable();
                            DT2 = SQ.Get_DTable_TSQL("Select App_ID From US_DL_01_Application Where (Company_ID = '" + CID + "') And (Dealer_ID = '" + DID + "') And (User_ID = '" + UID + "') And (RelateCode = '" + RelatCode + "')");
                            if (DT2.Rows != null)
                            {
                                if (DT2.Rows.Count == 1)
                                {
                                    string LSTR = "";
                                    int L_CID = 0;
                                    int L_DID = 0;
                                    int L_UID = 0;
                                    int L_APPID = int.Parse(DT2.Rows[0][0].ToString().Trim());
                                    try { L_CID = int.Parse(CID.ToString().Trim()); } catch (Exception) { }
                                    try { L_DID = int.Parse(DID.ToString().Trim()); } catch (Exception) { }
                                    try { L_UID = int.Parse(UID.ToString().Trim()); } catch (Exception) { }
                                    if (L_CID > 100) { L_CID = L_CID - 100; }
                                    if (L_DID > 100) { L_DID = L_DID - 100; }
                                    if (L_UID > 100) { L_UID = L_UID - 100; }
                                    LSTR = L_CID.ToString() + L_DID.ToString() + L_UID.ToString() + PB.Make_Security_Code(5) + L_APPID.ToString();
                                    LSTR = LSTR.Replace(" ", "").Trim();
                                    SQ.Execute_TSql("Update US_DL_01_Application Set [ApplicationID] = '" + LSTR + "' Where (App_ID = '" + DT2.Rows[0][0].ToString().Trim() + "') And (Company_ID = '" + CID + "') And (Dealer_ID = '" + DID + "') And (User_ID = '" + UID + "') And (RelateCode = '" + RelatCode + "')");
                                    LastRes = LSTR;
                                }
                                else
                                {
                                    LastRes = "ERR_006"; // Application not founded.
                                }
                            }
                            else
                            {
                                LastRes = "ERR_005"; // Applicarion not created.
                            }
                        }
                        else
                        {
                            LastRes = "ERR_004"; // License expired.
                        }
                    }
                    else
                    {
                        LastRes = "ERR_003"; // Company not founded.
                    }
                }
                else
                {
                    LastRes = "ERR_002"; // Sql no response.
                }
            }
            catch (Exception)
            {
                LastRes = "ERR_001"; // Function error.
            }
            return LastRes;
        }
    }
}
