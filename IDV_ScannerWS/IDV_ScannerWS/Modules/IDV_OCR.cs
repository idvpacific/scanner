using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Accord.Vision.Detection;
using Accord.Vision.Detection.Cascades;
using System.Drawing;
using System.Drawing.Imaging;
using RestSharp;
using System.Data;

namespace IDV_ScannerWS.Modules
{
    public class IDV_OCR
    {
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        SQL_Tranceiver Sq = new SQL_Tranceiver();
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public bool FaceDetector(Bitmap IMG)
        {
            try
            {
                HaarCascade cascade = new FaceHaarCascade();
                var detector = new HaarObjectDetector(cascade, 30);
                Rectangle[] objects = detector.ProcessFrame(IMG);
                if (objects.Length > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public Bitmap ImageRotation(Bitmap IMG)
        {
            try
            {
                Image IMGL = null;
                IMGL = new Bitmap(IMG);
                try { IMGL.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                return new Bitmap(IMGL);
            }
            catch (Exception)
            {
                return null;
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public string IDVOCR()
        {
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL("Select * From AD_IDVOCR");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        var client = new RestClient(DT.Rows[0][0].ToString().Trim());
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("apikey", DT.Rows[0][1].ToString().Trim());
                        request.AddFile("file", "/C:/Users/E-MasTech/Desktop/image_2020-06-12_18-17-30.png");
                        request.AddParameter("filetype", "PNG");
                        request.AddParameter("isOverlayRequired", DT.Rows[0][2].ToString().Trim());
                        request.AddParameter("detectOrientation", DT.Rows[0][3].ToString().Trim());
                        request.AddParameter("scale", DT.Rows[0][4].ToString().Trim());
                        request.AddParameter("isTable", DT.Rows[0][5].ToString().Trim());
                        request.AddParameter("OCREngine", DT.Rows[0][6].ToString().Trim());
                        IRestResponse response = client.Execute(request);
                        return response.Content.Trim();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public void GetData(string AppID)
        {

        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
    }
}