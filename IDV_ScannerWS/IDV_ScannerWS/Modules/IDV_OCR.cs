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
using Spire.Barcode;
using QRCodeDecoderLibrary;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Accord.Imaging.Filters;
using Accord;
using System.Drawing.Drawing2D;
using Accord.Statistics.Models.Markov.Topology;

namespace IDV_ScannerWS.Modules
{
    public class Word
    {
        public string WordText { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
    }

    public class Line
    {
        public string LineText { get; set; }
        public IList<Word> Words { get; set; }
        public double MaxHeight { get; set; }
        public double MinTop { get; set; }
    }

    public class TextOverlay
    {
        public IList<Line> Lines { get; set; }
        public bool HasOverlay { get; set; }
        public string Message { get; set; }
    }

    public class ParsedResult
    {
        public TextOverlay TextOverlay { get; set; }
        public string TextOrientation { get; set; }
        public int FileParseExitCode { get; set; }
        public string ParsedText { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDetails { get; set; }
    }

    public class OCR_Result
    {
        public IList<ParsedResult> ParsedResults { get; set; }
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string ProcessingTimeInMilliseconds { get; set; }
        public string SearchablePDFURL { get; set; }
    }

    public class TemplateList
    {
        public string TemplateID { get; set; }
        public double FrontImage_Key_Similarity { get; set; }
        public double FrontImage_ColorPicker_Similarity { get; set; }
        public double FrontImage_X_Coeficient { get; set; }
        public double FrontImage_Y_Coeficient { get; set; }
        public double FrontImage_X_TopLeft_Refrence { get; set; }
        public double FrontImage_Y_TopLeft_Refrence { get; set; }
        public double BackImage_Key_Similarity { get; set; }
        public double BackImage_ColorPicker_Similarity { get; set; }
        public double BackImage_X_Coeficient { get; set; }
        public double BackImage_Y_Coeficient { get; set; }
        public double BackImage_X_TopLeft_Refrence { get; set; }
        public double BackImage_Y_TopLeft_Refrence { get; set; }
        public double OCR_Line_Rank { get; set; }
        public double TemplateAverageRank { get; set; }
        public int TemplateAverageRank_Step2 { get; set; }
    }

    public class IDV_OCR
    {
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        SimilarityFunction SF = new SimilarityFunction();
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        int Rank_KeySimilarity_FI = 10;
        int Rank_ColorSimilarity_FI = 5;
        int Rank_KeySimilarity_BI = 8;
        int Rank_ColorSimilarity_BI = 4;
        int Rank_OCRFielad = 3;
        int Rank_TemplateAcceptedMinmumRank = 70;
        int NormalOcrLines_threshold = 20;
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        // Barcode 128 :
        public string Barcode_128_Read(Bitmap BTM)
        {
            try
            {
                string[] datas = BarcodeScanner.Scan(BTM);
                return datas[0].ToString().Trim();
            }
            catch (Exception)
            {
                return "";
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        // QR Code :
        public string Barcode_QR_Read(Bitmap QRCodeInputImage)
        {
            try
            {
                QRDecoder QRCodeDecoder;
                QRCodeDecoder = new QRDecoder();
#if DEBUG
                QRCodeTrace.Format("****");
                QRCodeTrace.Format("Decode image: {0} ", "EMAS");
                QRCodeTrace.Format("Image width: {0}, Height: {1}", QRCodeInputImage.Width, QRCodeInputImage.Height);
#endif
                byte[][] DataByteArray = QRCodeDecoder.ImageDecoder(QRCodeInputImage);
                string ECIA = QRCodeDecoder.ECIAssignValue >= 0 ? QRCodeDecoder.ECIAssignValue.ToString() : null;
                return QRCodeResult(DataByteArray);
            }
            catch (Exception)
            {
                return "";
            }
        }
        private string QRCodeResult(byte[][] DataByteArray)
        {
            try
            {
                if (DataByteArray == null) return string.Empty;
                if (DataByteArray.Length == 1) return ForDisplay(QRDecoder.ByteArrayToStr(DataByteArray[0]));
                StringBuilder Str = new StringBuilder();
                for (int Index = 0; Index < DataByteArray.Length; Index++)
                {
                    if (Index != 0) Str.Append("\r\n");
                    Str.AppendFormat("QR Code {0}\r\n", Index + 1);
                    Str.Append(ForDisplay(QRDecoder.ByteArrayToStr(DataByteArray[Index])));
                }
                return Str.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
        private string ForDisplay(string Result)
        {
            try
            {
                int Index;
                for (Index = 0; Index < Result.Length && (Result[Index] >= ' ' && Result[Index] <= '~' || Result[Index] >= 160); Index++) ;
                if (Index == Result.Length) return Result;
                StringBuilder Display = new StringBuilder(Result.Substring(0, Index));
                for (; Index < Result.Length; Index++)
                {
                    char OneChar = Result[Index];
                    if (OneChar >= ' ' && OneChar <= '~' || OneChar >= 160)
                    {
                        Display.Append(OneChar);
                        continue;
                    }
                    if (OneChar == '\r')
                    {
                        Display.Append("\r\n");
                        if (Index + 1 < Result.Length && Result[Index + 1] == '\n') Index++;
                        continue;
                    }
                    if (OneChar == '\n')
                    {
                        Display.Append("\r\n");
                        continue;
                    }
                    Display.Append('¿');
                }
                return Display.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public bool FaceDetector(Image IMGL)
        {
            try
            {
                Bitmap IMG = (Bitmap)IMGL;
                HaarCascade cascade = new FaceHaarCascade();
                for (int i = 5; i < 11; i++)
                {
                    var detector = new HaarObjectDetector(cascade, IMGL.Width / i, ObjectDetectorSearchMode.Single);
                    Rectangle[] objects = detector.ProcessFrame(IMG);
                    if (objects.Length == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Rectangle FaceDetector_FB(Image IMGL)
        {
            try
            {
                Bitmap IMG = (Bitmap)IMGL;
                HaarCascade cascade = new FaceHaarCascade();
                for (int i = 5; i < 11; i++)
                {
                    var detector = new HaarObjectDetector(cascade, IMGL.Width / i, ObjectDetectorSearchMode.Single);
                    Rectangle[] objects = detector.ProcessFrame(IMG);
                    if (objects.Length == 1)
                    {
                        return objects[0];
                    }
                }
                return Rectangle.Empty;
            }
            catch (Exception)
            {
                return Rectangle.Empty;
            }
        }
        public static Bitmap CropImage(Image source, Rectangle crop)
        {
            int LX = crop.X;
            int LY = crop.Y;
            int LW = crop.Width;
            int LH = crop.Height;
            if ((LX - (LW / 3)) >= 0) { crop.X = crop.X - (LW / 3); crop.Width = crop.Width + (LW / 3); }
            if ((LX + LW + (LW / 3)) <= source.Width) { crop.Width = crop.Width + (LW / 3); } else { crop.Width = source.Width - crop.X; }
            if ((LY - (LH / 3)) >= 0) { crop.Y = crop.Y - (LH / 3); crop.Height = crop.Height + (LH / 3); }
            if ((LY + LH + (LH / 3)) <= source.Height) { crop.Height = crop.Height + (LH / 3); }
            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        public Bitmap ImageRotation(Bitmap IMG, string Degree)
        {
            try
            {
                Image IMGL = null;
                IMGL = new Bitmap(IMG);
                switch (Degree.Trim())
                {
                    case "0":
                        {
                            try { IMGL.RotateFlip(RotateFlipType.RotateNoneFlipNone); } catch (Exception) { }
                            break;
                        }
                    case "90":
                        {
                            try { IMGL.RotateFlip(RotateFlipType.Rotate90FlipXY); } catch (Exception) { }
                            break;
                        }
                    case "180":
                        {
                            try { IMGL.RotateFlip(RotateFlipType.RotateNoneFlipXY); } catch (Exception) { }
                            break;
                        }
                    case "270":
                        {
                            try { IMGL.RotateFlip(RotateFlipType.Rotate270FlipXY); } catch (Exception) { }
                            break;
                        }
                    case "360":
                        {
                            try { IMGL.RotateFlip(RotateFlipType.RotateNoneFlipNone); } catch (Exception) { }
                            break;
                        }
                }

                return new Bitmap(IMGL);
            }
            catch (Exception)
            {
                return null;
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
        private string IDVOCR(Bitmap IMG, string FileType)
        {
            try
            {
                FileType = FileType.Trim();
                if (FileType == "") { FileType = "JPG"; }
                if (IMG == null) { return ""; }
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From AD_IDVOCR");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        var client = new RestClient(DT.Rows[0][0].ToString().Trim());
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("apikey", DT.Rows[0][1].ToString().Trim());
                        ImageConverter converter = new ImageConverter();
                        byte[] PIMG = (byte[])converter.ConvertTo(IMG, typeof(byte[]));
                        request.AddFile("file", PIMG, "IDV.JPG");
                        request.AddParameter("filetype", FileType);
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
            try
            {
                AppID = AppID.Trim();
                string TemplateSearchCode = "0";
                try
                {
                    DataTable DTRank = new DataTable();
                    DTRank = SQ.Get_DTable_TSQL("Select Top 1 * From AD_IDVOCR_Rank");
                    Rank_KeySimilarity_FI = int.Parse(DTRank.Rows[0][0].ToString().Trim());
                    Rank_ColorSimilarity_FI = int.Parse(DTRank.Rows[0][1].ToString().Trim());
                    Rank_KeySimilarity_BI = int.Parse(DTRank.Rows[0][2].ToString().Trim());
                    Rank_ColorSimilarity_BI = int.Parse(DTRank.Rows[0][3].ToString().Trim());
                    Rank_OCRFielad = int.Parse(DTRank.Rows[0][4].ToString().Trim());
                    Rank_TemplateAcceptedMinmumRank = int.Parse(DTRank.Rows[0][5].ToString().Trim());
                    NormalOcrLines_threshold = int.Parse(DTRank.Rows[0][6].ToString().Trim());
                }
                catch (Exception)
                {
                    Rank_KeySimilarity_FI = 10;
                    Rank_ColorSimilarity_FI = 5;
                    Rank_KeySimilarity_BI = 8;
                    Rank_ColorSimilarity_BI = 4;
                    Rank_OCRFielad = 3;
                    NormalOcrLines_threshold = 20;
                }
                DataTable DT_TemplateCode = new DataTable();
                DT_TemplateCode = SQ.Get_DTable_TSQL("Select Template_DrivingLicence_Code From AD_IDVOCR");
                if (DT_TemplateCode.Rows != null)
                {
                    if (DT_TemplateCode.Rows.Count == 1)
                    {
                        TemplateSearchCode = DT_TemplateCode.Rows[0][0].ToString().Trim();
                        DataTable DT_App = new DataTable();
                        DT_App = SQ.Get_DTable_TSQL("Select App_ID From US_DL_01_Application Where (ApplicationID = '" + AppID + "')");
                        if (DT_App.Rows != null)
                        {
                            if (DT_App.Rows.Count == 1)
                            {
                                // Images Variables :
                                bool IMG_Nor_Tag_Front = false; bool IMG_Nor_Tag_Back = false;
                                bool IMG_IR_Tag_Front = false; bool IMG_IR_Tag_Back = false;
                                bool IMG_UV_Tag_Front = false; bool IMG_UV_Tag_Back = false;
                                Image Img_Front_Nor = null; Image Img_Back_Nor = null;
                                Image Img_Front_IR = null; Image Img_Back_IR = null;
                                Image Img_Front_UV = null; Image Img_Back_UV = null;
                                // Prepare Images :
                                string FileNameIMG = "";
                                FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/2.jpg");
                                if (File.Exists(FileNameIMG) == true) { IMG_Nor_Tag_Front = true; Img_Front_Nor = new Bitmap(FileNameIMG); }
                                FileNameIMG = "";
                                FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/3.jpg");
                                if (File.Exists(FileNameIMG) == true) { IMG_Nor_Tag_Back = true; Img_Back_Nor = new Bitmap(FileNameIMG); }
                                FileNameIMG = "";
                                FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/4.jpg");
                                if (File.Exists(FileNameIMG) == true) { IMG_IR_Tag_Front = true; Img_Front_IR = new Bitmap(FileNameIMG); }
                                FileNameIMG = "";
                                FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/5.jpg");
                                if (File.Exists(FileNameIMG) == true) { IMG_IR_Tag_Back = true; Img_Back_IR = new Bitmap(FileNameIMG); }
                                FileNameIMG = "";
                                FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/6.jpg");
                                if (File.Exists(FileNameIMG) == true) { IMG_UV_Tag_Front = true; Img_Front_UV = new Bitmap(FileNameIMG); }
                                FileNameIMG = "";
                                FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/7.jpg");
                                if (File.Exists(FileNameIMG) == true) { IMG_UV_Tag_Back = true; Img_Back_UV = new Bitmap(FileNameIMG); }
                                // Test Application have Front or Back Image :
                                if ((IMG_Nor_Tag_Front == true) || (IMG_Nor_Tag_Back == true))
                                {
                                    bool IMG_Replacement = false;
                                    bool ContinueProcedure = false;
                                    bool FaceDetected = false;
                                    // Face Detection :
                                    if (IMG_Nor_Tag_Front == true)
                                    {
                                        if (IMG_Nor_Tag_Back == true)
                                        {
                                            if (FaceDetector(Img_Front_Nor) == true)
                                            {
                                                IMG_Replacement = false;
                                                ContinueProcedure = true;
                                                FaceDetected = true;
                                            }
                                            else
                                            {
                                                if (FaceDetector(Img_Back_Nor) == true)
                                                {
                                                    IMG_Replacement = true;
                                                    ContinueProcedure = true;
                                                    FaceDetected = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (FaceDetector(Img_Front_Nor) == true) { IMG_Replacement = false; ContinueProcedure = true; FaceDetected = true; }
                                        }
                                    }
                                    else
                                    {
                                        if (FaceDetector(Img_Back_Nor) == true) { IMG_Replacement = true; FaceDetected = true; }
                                        ContinueProcedure = true;
                                    }
                                    if (ContinueProcedure == true)
                                    {
                                        // Images Replacement :
                                        if (IMG_Replacement == true)
                                        {
                                            IMG_Nor_Tag_Front = false; IMG_Nor_Tag_Back = false;
                                            IMG_IR_Tag_Front = false; IMG_IR_Tag_Back = false;
                                            IMG_UV_Tag_Front = false; IMG_UV_Tag_Back = false;
                                            Img_Front_Nor = null; Img_Back_Nor = null;
                                            Img_Front_IR = null; Img_Back_IR = null;
                                            Img_Front_UV = null; Img_Back_UV = null;
                                            FileNameIMG = "";
                                            FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/3.jpg");
                                            if (File.Exists(FileNameIMG) == true) { IMG_Nor_Tag_Front = true; Img_Front_Nor = new Bitmap(FileNameIMG); }
                                            FileNameIMG = "";
                                            FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/2.jpg");
                                            if (File.Exists(FileNameIMG) == true) { IMG_Nor_Tag_Back = true; Img_Back_Nor = new Bitmap(FileNameIMG); }
                                            FileNameIMG = "";
                                            FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/5.jpg");
                                            if (File.Exists(FileNameIMG) == true) { IMG_IR_Tag_Front = true; Img_Front_IR = new Bitmap(FileNameIMG); }
                                            FileNameIMG = "";
                                            FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/4.jpg");
                                            if (File.Exists(FileNameIMG) == true) { IMG_IR_Tag_Back = true; Img_Back_IR = new Bitmap(FileNameIMG); }
                                            FileNameIMG = "";
                                            FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/7.jpg");
                                            if (File.Exists(FileNameIMG) == true) { IMG_UV_Tag_Front = true; Img_Front_UV = new Bitmap(FileNameIMG); }
                                            FileNameIMG = "";
                                            FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/6.jpg");
                                            if (File.Exists(FileNameIMG) == true) { IMG_UV_Tag_Back = true; Img_Back_UV = new Bitmap(FileNameIMG); }
                                        }
                                        // Select OCR Image :
                                        bool Selected_IMG_Front = true;
                                        Image Selected_IMG = null;
                                        if (IMG_Nor_Tag_Front == true)
                                        {
                                            if (IMG_Nor_Tag_Back == true)
                                            {
                                                if (FaceDetected == true)
                                                {
                                                    Selected_IMG_Front = true;
                                                    Selected_IMG = Img_Front_Nor;
                                                }
                                                else
                                                {
                                                    Selected_IMG_Front = false;
                                                    Selected_IMG = Img_Back_Nor;
                                                }
                                            }
                                            else
                                            {
                                                Selected_IMG_Front = true;
                                                Selected_IMG = Img_Front_Nor;
                                            }
                                        }
                                        else
                                        {
                                            Selected_IMG_Front = false;
                                            Selected_IMG = Img_Back_Nor;
                                        }
                                        string OCRResultLocal = IDVOCR((Bitmap)Selected_IMG, "JPG");
                                        OCR_Result OCRRes = JsonConvert.DeserializeObject<OCR_Result>(OCRResultLocal);
                                        OCR_Result OCRRes_Back = new OCR_Result();
                                        OCR_Result OCR_Last_Front = new OCR_Result();
                                        OCR_Result OCR_Last_Back = new OCR_Result();
                                        if (OCRRes.OCRExitCode == 1)
                                        {
                                            if (OCRRes.IsErroredOnProcessing == false)
                                            {
                                                if (OCRRes.ParsedResults[0].ErrorMessage.Trim() == "")
                                                {
                                                    bool TemplateFounded = false;
                                                    TemplateList LST_Template = new TemplateList();
                                                    if (Selected_IMG_Front == true)
                                                    {
                                                        //=============================================================================================================================================================
                                                        //======================================================================== Front Image ========================================================================
                                                        //=============================================================================================================================================================
                                                        string OCR_TextResult = "";
                                                        string OCR_TextOrientation = "0";
                                                        string OCR_TextResult_Back = "";
                                                        string OCR_TextOrientation_Back = "0";
                                                        OCR_TextResult = OCRRes.ParsedResults[0].ParsedText.Trim();
                                                        OCR_TextResult = OCR_TextResult.Replace("\r", " ");
                                                        OCR_TextResult = OCR_TextResult.Replace("\n", " ");
                                                        OCR_TextResult = OCR_TextResult.Replace("\t", " ");
                                                        OCR_TextResult = OCR_TextResult.Replace("  ", " ").Replace("  ", " "); ;
                                                        OCR_TextOrientation = OCRRes.ParsedResults[0].TextOrientation.Trim();
                                                        OCR_Last_Front = OCRRes;
                                                        // Searching for Template Start as Front Image :
                                                        DataTable DT_FIE_All = new DataTable();
                                                        DataTable DT_FIEK = new DataTable();
                                                        DT_FIE_All = SQ.Get_DTable_TSQL("Select DID,BackImage,OnlyBackImageBarcode,ONOCR,LNOCR,MNOCR,FrontImage From Template_06_BasicConfiguration Where (DTID = '" + TemplateSearchCode + "') And (FrontImage = '1') Order By DID");
                                                        if (DT_FIE_All.Rows != null)
                                                        {
                                                            if (DT_FIE_All.Rows.Count > 0)
                                                            {
                                                                double[,] FIE_TemplateCode = new double[DT_FIE_All.Rows.Count, 21];
                                                                int FIE_TemplateCode_Row = 0;
                                                                foreach (DataRow RW1 in DT_FIE_All.Rows)
                                                                {
                                                                    FIE_TemplateCode_Row++;
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 0] = int.Parse(RW1[0].ToString().Trim()); //Document ID ------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 1] = 0; // Front Image Key Similarity Average ----------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 2] = 0; // Front Image Color Picker Similarity Average -------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 3] = 0; // Back Image Key Similarity Average -----------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 4] = 0; // Back Image Color Picker Similarity Average --------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 5] = 0; // OCR Line Number Average Rank ----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 6] = 0; // X Coefficient Front Image -------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 7] = 0; // Y Coefficient Front Image -------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 8] = 0; // Scan X Top-Left Front Image -----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 9] = 0; // Scan Y Top-Left Front Image -----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 10] = 0; // X Coefficient Back Image -------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 11] = 0; // Y Coefficient Back Image -------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 12] = 0; // Base X Top-Left Back Image -----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 13] = 0; // Base Y Top-Left Back Image -----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 14] = 0; // Founded Front Image ------------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 15] = 0; // Founded Front Image Color Picker -----------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 16] = 0; // Founded Back Image -------------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 17] = 0; // Founded Back Image Color Picker ------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 18] = 0; // Calculate Front Image ----------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 19] = 0; // Calculate Back Image -----------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 20] = 0; // Template Last Rank -------------------------------> OK
                                                                    // Search For Key Front Image :
                                                                    DT_FIEK = new DataTable();
                                                                    DT_FIEK = SQ.Get_DTable_TSQL("Select KeyCode,Similarity,X1,Y1,Y2,KeyIndex From Template_08_FrontImage_Elements Where (DID = '" + RW1[0].ToString().Trim() + "') And (KeyActive = '1') And (KeyCode <> '') Order By X1,Y1");
                                                                    if (DT_FIEK.Rows != null)
                                                                    {
                                                                        if (DT_FIEK.Rows.Count > 0)
                                                                        {
                                                                            int[,] FIE_TemplateKey = new int[DT_FIEK.Rows.Count, 2];
                                                                            int FIE_TemplateKey_Row = 0;
                                                                            foreach (DataRow RW2 in DT_FIEK.Rows)
                                                                            {
                                                                                FIE_TemplateKey_Row++;
                                                                                FIE_TemplateKey[FIE_TemplateKey_Row - 1, 0] = SF.GetWordsSimilarityInString(RW2[0].ToString().Trim(), OCR_TextResult);
                                                                                FIE_TemplateKey[FIE_TemplateKey_Row - 1, 1] = int.Parse(RW2[1].ToString().Trim());
                                                                            }
                                                                            double LastRate_Cal = 0;
                                                                            double LastRate_Master = 0;
                                                                            for (int i = 0; i < FIE_TemplateKey_Row; i++)
                                                                            {
                                                                                LastRate_Cal += FIE_TemplateKey[i, 0];
                                                                                LastRate_Master += FIE_TemplateKey[i, 1];
                                                                            }
                                                                            LastRate_Cal /= FIE_TemplateKey_Row;
                                                                            LastRate_Master /= FIE_TemplateKey_Row;
                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 1] = (int)LastRate_Cal;
                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 14] = 1;
                                                                        }
                                                                    }
                                                                    if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 14] == 1)
                                                                    {
                                                                        // Get Front Image Scale :
                                                                        double FI_X_Coe = 0;
                                                                        double FI_Y_Coe = 0;
                                                                        bool ProContinue = false;
                                                                        if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 1] > 0)
                                                                        {
                                                                            int M_X1 = 0; int M_Y1 = 0; int M_X2 = 0; int M_Y2 = 0;
                                                                            int S_X1 = 0; int S_Y1 = 0; int S_X2 = 0; int S_Y2 = 0;
                                                                            M_X1 = int.Parse(DT_FIEK.Rows[0][2].ToString().Trim());
                                                                            M_Y1 = (int)((int.Parse(DT_FIEK.Rows[0][3].ToString().Trim()) + int.Parse(DT_FIEK.Rows[0][4].ToString().Trim())) / 2);
                                                                            M_X2 = int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][2].ToString().Trim());
                                                                            M_Y2 = (int)((int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][3].ToString().Trim()) + int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][4].ToString().Trim())) / 2);
                                                                            if (SF.KeyDetector(ref S_X1, ref S_Y1, DT_FIEK.Rows[0][0].ToString().Trim(), OCRRes.ParsedResults[0].TextOverlay.Lines, DT_FIEK.Rows[0][1].ToString().Trim(), int.Parse(DT_FIEK.Rows[0][5].ToString().Trim())) == true)
                                                                            {
                                                                                if (SF.KeyDetector(ref S_X2, ref S_Y2, DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][0].ToString().Trim(), OCRRes.ParsedResults[0].TextOverlay.Lines, DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][1].ToString().Trim(), int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][5].ToString().Trim())) == true)
                                                                                {
                                                                                    int XSM = Math.Abs(S_X1 - S_X2);
                                                                                    int XMM = Math.Abs(M_X1 - M_X2);
                                                                                    if (XSM == 0) { XSM = 1; }
                                                                                    if (XMM == 0) { XMM = 1; }
                                                                                    FI_X_Coe = (double)((double)XSM / (double)XMM);
                                                                                    int YSM = Math.Abs(S_Y1 - S_Y2);
                                                                                    int YMM = Math.Abs(M_Y1 - M_Y2);
                                                                                    if (YSM == 0) { YSM = 1; }
                                                                                    if (YMM == 0) { YMM = 1; }
                                                                                    FI_Y_Coe = (double)((double)YSM / (double)YMM);
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 6] = FI_X_Coe; // X Coefficient Front Image
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 7] = FI_Y_Coe; // Y Coefficient Front Image
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 8] = S_X1 - (M_X1 * FI_X_Coe); // Scan X Top-Left Front Image
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 9] = S_Y1 - (M_Y1 * FI_Y_Coe); ; // Scan Y Top-Left Front Image
                                                                                    ProContinue = true;
                                                                                }
                                                                            }
                                                                        }
                                                                        if (ProContinue == true)
                                                                        {
                                                                            // Front Image Color Picker :
                                                                            DataTable DT_FICP = new DataTable();
                                                                            DT_FICP = SQ.Get_DTable_TSQL("Select X,Y,R,G,B,Similarity From Template_10_FrontImage_ColorPicker Where (DID = '" + RW1[0].ToString().Trim() + "') Order By X,Y");
                                                                            if (DT_FICP.Rows != null)
                                                                            {
                                                                                if (DT_FICP.Rows.Count > 0)
                                                                                {
                                                                                    double IMG_IP = 0;
                                                                                    double IMG_BP = 0;
                                                                                    foreach (DataRow RW2 in DT_FICP.Rows)
                                                                                    {
                                                                                        int CPX = (int)((int.Parse(RW2[0].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 6]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 8]);
                                                                                        int CPY = (int)((int.Parse(RW2[1].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 7]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 9]);
                                                                                        Color CLS = ((Bitmap)Img_Front_Nor).GetPixel(CPX, CPY);
                                                                                        IMG_IP += SF.GetColorSimilarity(CLS, Color.FromArgb(int.Parse(RW2[2].ToString().Trim()), int.Parse(RW2[3].ToString().Trim()), int.Parse(RW2[4].ToString().Trim())));
                                                                                        IMG_BP += int.Parse(RW2[5].ToString().Trim());
                                                                                    }
                                                                                    IMG_IP /= DT_FICP.Rows.Count;
                                                                                    IMG_BP /= DT_FICP.Rows.Count;
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 2] = (int)IMG_IP;
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 15] = 1;
                                                                                }
                                                                            }
                                                                            // Back Image Functionality :
                                                                            if (IMG_Nor_Tag_Back == true)
                                                                            {
                                                                                if ((RW1[1].ToString().Trim() == "1") && ((RW1[2].ToString().Trim() == "0")))
                                                                                {
                                                                                    // Get OCR Result Of Back Image :
                                                                                    if (OCRRes_Back == null)
                                                                                    {
                                                                                        string OCRResultLocalBack = IDVOCR((Bitmap)Img_Back_Nor, "JPG");
                                                                                        OCRRes_Back = JsonConvert.DeserializeObject<OCR_Result>(OCRResultLocalBack);
                                                                                        OCR_TextResult_Back = OCRRes_Back.ParsedResults[0].ParsedText.Trim();
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("\r", " ");
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("\n", " ");
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("\t", " ");
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("  ", " ").Replace("  ", " "); ;
                                                                                        OCR_TextOrientation_Back = OCRRes_Back.ParsedResults[0].TextOrientation.Trim();
                                                                                        OCR_Last_Back = OCRRes_Back;
                                                                                    }
                                                                                    // Search For Key Back Image :
                                                                                    DataTable DT_FBEK = new DataTable();
                                                                                    DT_FBEK = SQ.Get_DTable_TSQL("Select KeyCode,Similarity,X1,Y1,Y2,KeyIndex From Template_09_BackImage_Elements Where (DID = '" + RW1[0].ToString().Trim() + "') And (KeyActive = '1') And (KeyCode <> '') Order By X1,Y1");
                                                                                    if (DT_FBEK.Rows != null)
                                                                                    {
                                                                                        if (DT_FBEK.Rows.Count > 0)
                                                                                        {
                                                                                            int[,] BIE_TemplateKey = new int[DT_FBEK.Rows.Count, 2];
                                                                                            int BIE_TemplateKey_Row = 0;
                                                                                            foreach (DataRow RW2 in DT_FBEK.Rows)
                                                                                            {
                                                                                                BIE_TemplateKey_Row++;
                                                                                                BIE_TemplateKey[BIE_TemplateKey_Row - 1, 0] = SF.GetWordsSimilarityInString(RW2[0].ToString().Trim(), OCR_TextResult_Back);
                                                                                                BIE_TemplateKey[BIE_TemplateKey_Row - 1, 1] = int.Parse(RW2[1].ToString().Trim());
                                                                                            }
                                                                                            double LastRate_Cal = 0;
                                                                                            double LastRate_Master = 0;
                                                                                            for (int i = 0; i < BIE_TemplateKey_Row; i++)
                                                                                            {
                                                                                                LastRate_Cal += BIE_TemplateKey[i, 0];
                                                                                                LastRate_Master += BIE_TemplateKey[i, 1];
                                                                                            }
                                                                                            LastRate_Cal /= BIE_TemplateKey_Row;
                                                                                            LastRate_Master /= BIE_TemplateKey_Row;
                                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 3] = (int)LastRate_Cal;
                                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 16] = 1;
                                                                                        }
                                                                                    }
                                                                                    if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 16] == 1)
                                                                                    {
                                                                                        // Get Back Image Scale :
                                                                                        double FI_X_CoeB = 0;
                                                                                        double FI_Y_CoeB = 0;
                                                                                        bool ProContinueB = false;
                                                                                        if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 3] > 0)
                                                                                        {
                                                                                            int M_X1 = 0; int M_Y1 = 0; int M_X2 = 0; int M_Y2 = 0;
                                                                                            int S_X1 = 0; int S_Y1 = 0; int S_X2 = 0; int S_Y2 = 0;
                                                                                            M_X1 = int.Parse(DT_FBEK.Rows[0][2].ToString().Trim());
                                                                                            M_Y1 = (int)((int.Parse(DT_FBEK.Rows[0][3].ToString().Trim()) + int.Parse(DT_FBEK.Rows[0][4].ToString().Trim())) / 2);
                                                                                            M_X2 = int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][2].ToString().Trim());
                                                                                            M_Y2 = (int)((int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][3].ToString().Trim()) + int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][4].ToString().Trim())) / 2);
                                                                                            if (SF.KeyDetector(ref S_X1, ref S_Y1, DT_FBEK.Rows[0][0].ToString().Trim(), OCRRes_Back.ParsedResults[0].TextOverlay.Lines, DT_FBEK.Rows[0][1].ToString().Trim(), int.Parse(DT_FBEK.Rows[0][5].ToString().Trim())) == true)
                                                                                            {
                                                                                                if (SF.KeyDetector(ref S_X2, ref S_Y2, DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][0].ToString().Trim(), OCRRes_Back.ParsedResults[0].TextOverlay.Lines, DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][1].ToString().Trim(), int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][5].ToString().Trim())) == true)
                                                                                                {
                                                                                                    int XSM = Math.Abs(S_X1 - S_X2);
                                                                                                    int XMM = Math.Abs(M_X1 - M_X2);
                                                                                                    if (XSM == 0) { XSM = 1; }
                                                                                                    if (XMM == 0) { XMM = 1; }
                                                                                                    FI_X_CoeB = (double)((double)XSM / (double)XMM);
                                                                                                    int YSM = Math.Abs(S_Y1 - S_Y2);
                                                                                                    int YMM = Math.Abs(M_Y1 - M_Y2);
                                                                                                    if (YSM == 0) { YSM = 1; }
                                                                                                    if (YMM == 0) { YMM = 1; }
                                                                                                    FI_Y_CoeB = (double)((double)YSM / (double)YMM);
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 10] = FI_X_CoeB; // X Coefficient Back Image
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 11] = FI_Y_CoeB; // Y Coefficient Back Image
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 12] = S_X1 - (M_X1 * FI_X_CoeB); // Scan X Top-Left Back Image
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 13] = S_Y1 - (M_Y1 * FI_Y_CoeB); ; // Scan Y Top-Left Back Image
                                                                                                    ProContinueB = true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if (ProContinueB == true)
                                                                                        {
                                                                                            // Back Image Color Picker :
                                                                                            DataTable DT_FICPB = new DataTable();
                                                                                            DT_FICPB = SQ.Get_DTable_TSQL("Select X,Y,R,G,B,Similarity From Template_11_BackImage_ColorPicker Where (DID = '" + RW1[0].ToString().Trim() + "') Order By X,Y");
                                                                                            if (DT_FICPB.Rows != null)
                                                                                            {
                                                                                                if (DT_FICPB.Rows.Count > 0)
                                                                                                {
                                                                                                    double IMG_IP = 0;
                                                                                                    double IMG_BP = 0;
                                                                                                    foreach (DataRow RW2 in DT_FICPB.Rows)
                                                                                                    {
                                                                                                        int CPX = (int)((int.Parse(RW2[0].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 10]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 12]);
                                                                                                        int CPY = (int)((int.Parse(RW2[1].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 11]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 13]);
                                                                                                        Color CLS = ((Bitmap)Img_Back_Nor).GetPixel(CPX, CPY);
                                                                                                        IMG_IP += SF.GetColorSimilarity(CLS, Color.FromArgb(int.Parse(RW2[2].ToString().Trim()), int.Parse(RW2[3].ToString().Trim()), int.Parse(RW2[4].ToString().Trim())));
                                                                                                        IMG_BP += int.Parse(RW2[5].ToString().Trim());
                                                                                                    }
                                                                                                    IMG_IP /= DT_FICPB.Rows.Count;
                                                                                                    IMG_BP /= DT_FICPB.Rows.Count;
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 4] = (int)IMG_IP;
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 17] = 1;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            // Get Optimal , Maximum , Minimum OCR Fields Count :
                                                                            int LastCounterPercent = 0;
                                                                            int OCRNorC = int.Parse(RW1[3].ToString().Trim());
                                                                            int OCRMinC = int.Parse(RW1[4].ToString().Trim());
                                                                            int OCRMaxC = int.Parse(RW1[5].ToString().Trim());
                                                                            int LineCount = OCRRes.ParsedResults[0].TextOverlay.Lines.Count;
                                                                            if (LineCount >= OCRMinC) { LastCounterPercent += 25; }
                                                                            if (LineCount <= OCRMaxC) { LastCounterPercent += 25; }
                                                                            if ((LineCount >= (OCRNorC - (int)((OCRNorC * NormalOcrLines_threshold) / 100))) && (LineCount <= (OCRNorC + (int)((OCRNorC * NormalOcrLines_threshold) / 100)))) { LastCounterPercent += 50; }
                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 5] = (int)LastCounterPercent;
                                                                            // Get Front / Back Image Calculation :
                                                                            if (RW1[1].ToString().Trim() == "1") { FIE_TemplateCode[FIE_TemplateCode_Row - 1, 19] = 1; }
                                                                            if (RW1[6].ToString().Trim() == "1") { FIE_TemplateCode[FIE_TemplateCode_Row - 1, 18] = 1; }
                                                                        }
                                                                    }
                                                                }
                                                                // Select Template With Maximum Rank
                                                                if ((FIE_TemplateCode.Length / 21) > 0)
                                                                {
                                                                    int Divider_Count = 0;
                                                                    double SumRank = 0;
                                                                    // Calculate Template Last Rank
                                                                    for (int i = 0; i < (FIE_TemplateCode.Length / 21); i++)
                                                                    {
                                                                        Divider_Count = 0;
                                                                        SumRank = 0;
                                                                        FIE_TemplateCode[i, 20] = 0;
                                                                        // Front Image Rank 
                                                                        if (FIE_TemplateCode[i, 18] == 1)
                                                                        {
                                                                            // Key Similarity
                                                                            if (FIE_TemplateCode[i, 14] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 1] * Rank_KeySimilarity_FI;
                                                                                Divider_Count += Rank_KeySimilarity_FI;
                                                                            }
                                                                            // Color Picker Similarity 
                                                                            if (FIE_TemplateCode[i, 15] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 2] * Rank_ColorSimilarity_FI;
                                                                                Divider_Count += Rank_ColorSimilarity_FI;
                                                                            }
                                                                        }
                                                                        // Back Image Rank 
                                                                        if (FIE_TemplateCode[i, 19] == 1)
                                                                        {
                                                                            // Key Similarity
                                                                            if (FIE_TemplateCode[i, 16] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 3] * Rank_KeySimilarity_BI;
                                                                                Divider_Count += Rank_KeySimilarity_BI;
                                                                            }
                                                                            // Color Picker Similarity 
                                                                            if (FIE_TemplateCode[i, 17] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 4] * Rank_ColorSimilarity_BI;
                                                                                Divider_Count += Rank_ColorSimilarity_BI;
                                                                            }
                                                                        }
                                                                        // OCR Line Number Rank
                                                                        SumRank += FIE_TemplateCode[i, 5] * Rank_OCRFielad;
                                                                        Divider_Count += Rank_OCRFielad;
                                                                        // Calculate Average
                                                                        try
                                                                        {
                                                                            FIE_TemplateCode[i, 20] = (int)Math.Round(SumRank / Divider_Count);
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                            FIE_TemplateCode[i, 20] = 0;
                                                                        }
                                                                    }
                                                                    // Select Template With Maximum Rank :
                                                                    var TemList = new List<TemplateList>();
                                                                    TemList.Clear();
                                                                    double MaxTemplateRank = 0;
                                                                    for (int i = 0; i < (FIE_TemplateCode.Length / 21); i++)
                                                                    {
                                                                        if (FIE_TemplateCode[i, 20] > MaxTemplateRank)
                                                                        {
                                                                            TemList.Clear();
                                                                            MaxTemplateRank = FIE_TemplateCode[i, 20];
                                                                            TemList.Add(new TemplateList()
                                                                            {
                                                                                TemplateID = FIE_TemplateCode[i, 0].ToString(),
                                                                                FrontImage_Key_Similarity = FIE_TemplateCode[i, 1],
                                                                                FrontImage_ColorPicker_Similarity = FIE_TemplateCode[i, 2],
                                                                                FrontImage_X_Coeficient = FIE_TemplateCode[i, 6],
                                                                                FrontImage_Y_Coeficient = FIE_TemplateCode[i, 7],
                                                                                FrontImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 8],
                                                                                FrontImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 9],
                                                                                BackImage_Key_Similarity = FIE_TemplateCode[i, 3],
                                                                                BackImage_ColorPicker_Similarity = FIE_TemplateCode[i, 4],
                                                                                BackImage_X_Coeficient = FIE_TemplateCode[i, 10],
                                                                                BackImage_Y_Coeficient = FIE_TemplateCode[i, 11],
                                                                                BackImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 12],
                                                                                BackImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 13],
                                                                                OCR_Line_Rank = FIE_TemplateCode[i, 5],
                                                                                TemplateAverageRank = FIE_TemplateCode[i, 20],
                                                                                TemplateAverageRank_Step2 = 0
                                                                            });
                                                                        }
                                                                        else
                                                                        {
                                                                            if (FIE_TemplateCode[i, 20] == MaxTemplateRank)
                                                                            {
                                                                                MaxTemplateRank = FIE_TemplateCode[i, 20];
                                                                                TemList.Add(new TemplateList()
                                                                                {
                                                                                    TemplateID = FIE_TemplateCode[i, 0].ToString(),
                                                                                    FrontImage_Key_Similarity = FIE_TemplateCode[i, 1],
                                                                                    FrontImage_ColorPicker_Similarity = FIE_TemplateCode[i, 2],
                                                                                    FrontImage_X_Coeficient = FIE_TemplateCode[i, 6],
                                                                                    FrontImage_Y_Coeficient = FIE_TemplateCode[i, 7],
                                                                                    FrontImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 8],
                                                                                    FrontImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 9],
                                                                                    BackImage_Key_Similarity = FIE_TemplateCode[i, 3],
                                                                                    BackImage_ColorPicker_Similarity = FIE_TemplateCode[i, 4],
                                                                                    BackImage_X_Coeficient = FIE_TemplateCode[i, 10],
                                                                                    BackImage_Y_Coeficient = FIE_TemplateCode[i, 11],
                                                                                    BackImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 12],
                                                                                    BackImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 13],
                                                                                    OCR_Line_Rank = FIE_TemplateCode[i, 5],
                                                                                    TemplateAverageRank = FIE_TemplateCode[i, 20],
                                                                                    TemplateAverageRank_Step2 = 0
                                                                                });
                                                                            }
                                                                        }
                                                                    }
                                                                    if (TemList.Count == 1)
                                                                    {
                                                                        LST_Template.TemplateID = TemList[0].TemplateID;
                                                                        LST_Template.FrontImage_Key_Similarity = TemList[0].FrontImage_Key_Similarity;
                                                                        LST_Template.FrontImage_ColorPicker_Similarity = TemList[0].FrontImage_ColorPicker_Similarity;
                                                                        LST_Template.FrontImage_X_Coeficient = TemList[0].FrontImage_X_Coeficient;
                                                                        LST_Template.FrontImage_Y_Coeficient = TemList[0].FrontImage_Y_Coeficient;
                                                                        LST_Template.FrontImage_X_TopLeft_Refrence = TemList[0].FrontImage_X_TopLeft_Refrence;
                                                                        LST_Template.FrontImage_Y_TopLeft_Refrence = TemList[0].FrontImage_Y_TopLeft_Refrence;
                                                                        LST_Template.BackImage_Key_Similarity = TemList[0].BackImage_Key_Similarity;
                                                                        LST_Template.BackImage_ColorPicker_Similarity = TemList[0].BackImage_ColorPicker_Similarity;
                                                                        LST_Template.BackImage_X_Coeficient = TemList[0].BackImage_X_Coeficient;
                                                                        LST_Template.BackImage_Y_Coeficient = TemList[0].BackImage_Y_Coeficient;
                                                                        LST_Template.BackImage_X_TopLeft_Refrence = TemList[0].BackImage_X_TopLeft_Refrence;
                                                                        LST_Template.BackImage_Y_TopLeft_Refrence = TemList[0].BackImage_Y_TopLeft_Refrence;
                                                                        LST_Template.OCR_Line_Rank = TemList[0].OCR_Line_Rank;
                                                                        LST_Template.TemplateAverageRank = TemList[0].TemplateAverageRank;
                                                                        TemplateFounded = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        foreach (TemplateList TL in TemList)
                                                                        {
                                                                            foreach (TemplateList TL2 in TemList)
                                                                            {
                                                                                if (TL.TemplateID != TL2.TemplateID)
                                                                                {
                                                                                    if (TL.TemplateAverageRank >= TL2.TemplateAverageRank)
                                                                                    {
                                                                                        if (TL.TemplateAverageRank == TL2.TemplateAverageRank)
                                                                                        {
                                                                                            if (TL.FrontImage_Key_Similarity >= TL2.FrontImage_Key_Similarity)
                                                                                            {
                                                                                                if (TL.FrontImage_Key_Similarity == TL2.FrontImage_Key_Similarity)
                                                                                                {
                                                                                                    if (TL.FrontImage_ColorPicker_Similarity >= TL2.FrontImage_ColorPicker_Similarity)
                                                                                                    {
                                                                                                        if (TL.FrontImage_ColorPicker_Similarity == TL2.FrontImage_ColorPicker_Similarity)
                                                                                                        {
                                                                                                            if (TL.BackImage_Key_Similarity >= TL2.BackImage_Key_Similarity)
                                                                                                            {
                                                                                                                if (TL.BackImage_Key_Similarity == TL2.BackImage_Key_Similarity)
                                                                                                                {
                                                                                                                    if (TL.BackImage_ColorPicker_Similarity >= TL2.BackImage_ColorPicker_Similarity)
                                                                                                                    {
                                                                                                                        if (TL.BackImage_ColorPicker_Similarity == TL2.BackImage_ColorPicker_Similarity)
                                                                                                                        {
                                                                                                                            if (TL.OCR_Line_Rank >= TL2.OCR_Line_Rank)
                                                                                                                            {
                                                                                                                                if (TL.OCR_Line_Rank == TL2.OCR_Line_Rank)
                                                                                                                                {
                                                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                                                    TL2.TemplateAverageRank_Step2 += 1;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                TL2.TemplateAverageRank_Step2 += 1;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            TL.TemplateAverageRank_Step2 += 1;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        TL2.TemplateAverageRank_Step2 += 1;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                TL2.TemplateAverageRank_Step2 += 1;
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            TL.TemplateAverageRank_Step2 += 1;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        TL2.TemplateAverageRank_Step2 += 1;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                TL2.TemplateAverageRank_Step2 += 1;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            TL.TemplateAverageRank_Step2 += 1;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        TL2.TemplateAverageRank_Step2 += 1;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        double MaxRank2 = 0;
                                                                        foreach (TemplateList TL in TemList)
                                                                        {
                                                                            if (TL.TemplateAverageRank_Step2 > MaxRank2)
                                                                            {
                                                                                LST_Template.TemplateID = TL.TemplateID;
                                                                                LST_Template.FrontImage_Key_Similarity = TL.FrontImage_Key_Similarity;
                                                                                LST_Template.FrontImage_ColorPicker_Similarity = TL.FrontImage_ColorPicker_Similarity;
                                                                                LST_Template.FrontImage_X_Coeficient = TL.FrontImage_X_Coeficient;
                                                                                LST_Template.FrontImage_Y_Coeficient = TL.FrontImage_Y_Coeficient;
                                                                                LST_Template.FrontImage_X_TopLeft_Refrence = TL.FrontImage_X_TopLeft_Refrence;
                                                                                LST_Template.FrontImage_Y_TopLeft_Refrence = TL.FrontImage_Y_TopLeft_Refrence;
                                                                                LST_Template.BackImage_Key_Similarity = TL.BackImage_Key_Similarity;
                                                                                LST_Template.BackImage_ColorPicker_Similarity = TL.BackImage_ColorPicker_Similarity;
                                                                                LST_Template.BackImage_X_Coeficient = TL.BackImage_X_Coeficient;
                                                                                LST_Template.BackImage_Y_Coeficient = TL.BackImage_Y_Coeficient;
                                                                                LST_Template.BackImage_X_TopLeft_Refrence = TL.BackImage_X_TopLeft_Refrence;
                                                                                LST_Template.BackImage_Y_TopLeft_Refrence = TL.BackImage_Y_TopLeft_Refrence;
                                                                                LST_Template.OCR_Line_Rank = TL.OCR_Line_Rank;
                                                                                LST_Template.TemplateAverageRank = TL.TemplateAverageRank;
                                                                                LST_Template.TemplateAverageRank_Step2 = TL.TemplateAverageRank_Step2;
                                                                            }
                                                                        }
                                                                        TemplateFounded = true;
                                                                        foreach (TemplateList TL in TemList)
                                                                        {
                                                                            if (LST_Template.TemplateID != TL.TemplateID)
                                                                            {
                                                                                if (LST_Template.TemplateAverageRank_Step2 == TL.TemplateAverageRank_Step2)
                                                                                {
                                                                                    TemplateFounded = false;
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    TemplateFounded = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                TemplateFounded = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            TemplateFounded = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //============================================================================================================================================================
                                                        //======================================================================== Back Image ========================================================================
                                                        //============================================================================================================================================================
                                                        string OCR_TextResult = "";
                                                        string OCR_TextOrientation = "0";
                                                        string OCR_TextResult_Back = "";
                                                        string OCR_TextOrientation_Back = "0";
                                                        OCR_TextResult = OCRRes.ParsedResults[0].ParsedText.Trim();
                                                        OCR_TextResult = OCR_TextResult.Replace("\r", " ");
                                                        OCR_TextResult = OCR_TextResult.Replace("\n", " ");
                                                        OCR_TextResult = OCR_TextResult.Replace("\t", " ");
                                                        OCR_TextResult = OCR_TextResult.Replace("  ", " ").Replace("  ", " "); ;
                                                        OCR_TextOrientation = OCRRes.ParsedResults[0].TextOrientation.Trim();
                                                        OCR_Last_Back = OCRRes;
                                                        // Searching for Template Only Back Image :
                                                        DataTable DT_FIE_All = new DataTable();
                                                        DataTable DT_FIEK = new DataTable();
                                                        DT_FIE_All = SQ.Get_DTable_TSQL("Select DID,FrontImage,OnlyBackImageBarcode,ONOCR,LNOCR,MNOCR,BackImage From Template_06_BasicConfiguration Where (DTID = '" + TemplateSearchCode + "') And (BackImage = '1') Order By DID");
                                                        if (DT_FIE_All.Rows != null)
                                                        {
                                                            if (DT_FIE_All.Rows.Count > 0)
                                                            {
                                                                double[,] FIE_TemplateCode = new double[DT_FIE_All.Rows.Count, 21];
                                                                int FIE_TemplateCode_Row = 0;
                                                                foreach (DataRow RW1 in DT_FIE_All.Rows)
                                                                {
                                                                    FIE_TemplateCode_Row++;
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 0] = int.Parse(RW1[0].ToString().Trim()); //Document ID ------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 1] = 0; // Back Image Key Similarity Average -----------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 2] = 0; // Back Image Color Picker Similarity Average --------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 3] = 0; // Front Image Key Similarity Average ----------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 4] = 0; // Front Image Color Picker Similarity Average -------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 5] = 0; // OCR Line Number Average Rank ----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 6] = 0; // X Coefficient Back Image --------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 7] = 0; // Y Coefficient Back Image --------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 8] = 0; // Scan X Top-Left Back Image ------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 9] = 0; // Scan Y Top-Left Back Image ------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 10] = 0; // X Coefficient Front Image ------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 11] = 0; // Y Coefficient Front Image ------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 12] = 0; // Base X Top-Left Front Image ----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 13] = 0; // Base Y Top-Left Front Image ----------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 14] = 0; // Founded Back Image -------------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 15] = 0; // Founded Back Image Color Picker ------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 16] = 0; // Founded Front Image ------------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 17] = 0; // Founded Front Image Color Picker -----------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 18] = 0; // Calculate Back Image -----------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 19] = 0; // Calculate Front Image ----------------------------> OK
                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 20] = 0; // Template Last Rank -------------------------------> OK
                                                                    // Search For Key Back Image :
                                                                    DT_FIEK = new DataTable();
                                                                    DT_FIEK = SQ.Get_DTable_TSQL("Select KeyCode,Similarity,X1,Y1,Y2,KeyIndex From Template_09_BackImage_Elements Where (DID = '" + RW1[0].ToString().Trim() + "') And (KeyActive = '1') And (KeyCode <> '') Order By X1,Y1");
                                                                    if (DT_FIEK.Rows != null)
                                                                    {
                                                                        if (DT_FIEK.Rows.Count > 0)
                                                                        {
                                                                            int[,] FIE_TemplateKey = new int[DT_FIEK.Rows.Count, 2];
                                                                            int FIE_TemplateKey_Row = 0;
                                                                            foreach (DataRow RW2 in DT_FIEK.Rows)
                                                                            {
                                                                                FIE_TemplateKey_Row++;
                                                                                FIE_TemplateKey[FIE_TemplateKey_Row - 1, 0] = SF.GetWordsSimilarityInString(RW2[0].ToString().Trim(), OCR_TextResult);
                                                                                FIE_TemplateKey[FIE_TemplateKey_Row - 1, 1] = int.Parse(RW2[1].ToString().Trim());
                                                                            }
                                                                            double LastRate_Cal = 0;
                                                                            double LastRate_Master = 0;
                                                                            for (int i = 0; i < FIE_TemplateKey_Row; i++)
                                                                            {
                                                                                LastRate_Cal += FIE_TemplateKey[i, 0];
                                                                                LastRate_Master += FIE_TemplateKey[i, 1];
                                                                            }
                                                                            LastRate_Cal /= FIE_TemplateKey_Row;
                                                                            LastRate_Master /= FIE_TemplateKey_Row;
                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 1] = (int)LastRate_Cal;
                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 14] = 1;
                                                                        }
                                                                    }
                                                                    if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 14] == 1)
                                                                    {
                                                                        // Get Back Image Scale :
                                                                        double FI_X_Coe = 0;
                                                                        double FI_Y_Coe = 0;
                                                                        bool ProContinue = false;
                                                                        if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 1] > 0)
                                                                        {
                                                                            int M_X1 = 0; int M_Y1 = 0; int M_X2 = 0; int M_Y2 = 0;
                                                                            int S_X1 = 0; int S_Y1 = 0; int S_X2 = 0; int S_Y2 = 0;
                                                                            M_X1 = int.Parse(DT_FIEK.Rows[0][2].ToString().Trim());
                                                                            M_Y1 = (int)((int.Parse(DT_FIEK.Rows[0][3].ToString().Trim()) + int.Parse(DT_FIEK.Rows[0][4].ToString().Trim())) / 2);
                                                                            M_X2 = int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][2].ToString().Trim());
                                                                            M_Y2 = (int)((int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][3].ToString().Trim()) + int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][4].ToString().Trim())) / 2);
                                                                            if (SF.KeyDetector(ref S_X1, ref S_Y1, DT_FIEK.Rows[0][0].ToString().Trim(), OCRRes.ParsedResults[0].TextOverlay.Lines, DT_FIEK.Rows[0][1].ToString().Trim(), int.Parse(DT_FIEK.Rows[0][5].ToString().Trim())) == true)
                                                                            {
                                                                                if (SF.KeyDetector(ref S_X2, ref S_Y2, DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][0].ToString().Trim(), OCRRes.ParsedResults[0].TextOverlay.Lines, DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][1].ToString().Trim(), int.Parse(DT_FIEK.Rows[DT_FIEK.Rows.Count - 1][5].ToString().Trim())) == true)
                                                                                {
                                                                                    int XSM = Math.Abs(S_X1 - S_X2);
                                                                                    int XMM = Math.Abs(M_X1 - M_X2);
                                                                                    if (XSM == 0) { XSM = 1; }
                                                                                    if (XMM == 0) { XMM = 1; }
                                                                                    FI_X_Coe = (double)((double)XSM / (double)XMM);
                                                                                    int YSM = Math.Abs(S_Y1 - S_Y2);
                                                                                    int YMM = Math.Abs(M_Y1 - M_Y2);
                                                                                    if (YSM == 0) { YSM = 1; }
                                                                                    if (YMM == 0) { YMM = 1; }
                                                                                    FI_Y_Coe = (double)((double)YSM / (double)YMM);
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 6] = FI_X_Coe; // X Coefficient Back Image
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 7] = FI_Y_Coe; // Y Coefficient Back Image
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 8] = S_X1 - (M_X1 * FI_X_Coe); // Scan X Top-Left Back Image
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 9] = S_Y1 - (M_Y1 * FI_Y_Coe); ; // Scan Y Top-Left Back Image
                                                                                    ProContinue = true;
                                                                                }
                                                                            }
                                                                        }
                                                                        if (ProContinue == true)
                                                                        {
                                                                            // Back Image Color Picker :
                                                                            DataTable DT_FICP = new DataTable();
                                                                            DT_FICP = SQ.Get_DTable_TSQL("Select X,Y,R,G,B,Similarity From Template_11_BackImage_ColorPicker Where (DID = '" + RW1[0].ToString().Trim() + "') Order By X,Y");
                                                                            if (DT_FICP.Rows != null)
                                                                            {
                                                                                if (DT_FICP.Rows.Count > 0)
                                                                                {
                                                                                    double IMG_IP = 0;
                                                                                    double IMG_BP = 0;
                                                                                    foreach (DataRow RW2 in DT_FICP.Rows)
                                                                                    {
                                                                                        int CPX = (int)((int.Parse(RW2[0].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 6]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 8]);
                                                                                        int CPY = (int)((int.Parse(RW2[1].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 7]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 9]);
                                                                                        Color CLS = ((Bitmap)Img_Back_Nor).GetPixel(CPX, CPY);
                                                                                        IMG_IP += SF.GetColorSimilarity(CLS, Color.FromArgb(int.Parse(RW2[2].ToString().Trim()), int.Parse(RW2[3].ToString().Trim()), int.Parse(RW2[4].ToString().Trim())));
                                                                                        IMG_BP += int.Parse(RW2[5].ToString().Trim());
                                                                                    }
                                                                                    IMG_IP /= DT_FICP.Rows.Count;
                                                                                    IMG_BP /= DT_FICP.Rows.Count;
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 2] = (int)IMG_IP;
                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 15] = 1;
                                                                                }
                                                                            }
                                                                            // Front Image Functionality :
                                                                            if (IMG_Nor_Tag_Back == true)
                                                                            {
                                                                                if ((RW1[1].ToString().Trim() == "1") && ((RW1[2].ToString().Trim() == "0")))
                                                                                {
                                                                                    // Get OCR Result Of Front Image :
                                                                                    if (OCRRes_Back == null)
                                                                                    {
                                                                                        string OCRResultLocalBack = IDVOCR((Bitmap)Img_Front_Nor, "JPG");
                                                                                        OCRRes_Back = JsonConvert.DeserializeObject<OCR_Result>(OCRResultLocalBack);
                                                                                        OCR_TextResult_Back = OCRRes_Back.ParsedResults[0].ParsedText.Trim();
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("\r", " ");
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("\n", " ");
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("\t", " ");
                                                                                        OCR_TextResult_Back = OCR_TextResult_Back.Replace("  ", " ").Replace("  ", " "); ;
                                                                                        OCR_TextOrientation_Back = OCRRes_Back.ParsedResults[0].TextOrientation.Trim();
                                                                                        OCR_Last_Front = OCRRes_Back;
                                                                                    }
                                                                                    // Search For Key Front Image :
                                                                                    DataTable DT_FBEK = new DataTable();
                                                                                    DT_FBEK = SQ.Get_DTable_TSQL("Select KeyCode,Similarity,X1,Y1,Y2,KeyIndex From Template_08_FrontImage_Elements Where (DID = '" + RW1[0].ToString().Trim() + "') And (KeyActive = '1') And (KeyCode <> '') Order By X1,Y1");
                                                                                    if (DT_FBEK.Rows != null)
                                                                                    {
                                                                                        if (DT_FBEK.Rows.Count > 0)
                                                                                        {
                                                                                            int[,] BIE_TemplateKey = new int[DT_FBEK.Rows.Count, 2];
                                                                                            int BIE_TemplateKey_Row = 0;
                                                                                            foreach (DataRow RW2 in DT_FBEK.Rows)
                                                                                            {
                                                                                                BIE_TemplateKey_Row++;
                                                                                                BIE_TemplateKey[BIE_TemplateKey_Row - 1, 0] = SF.GetWordsSimilarityInString(RW2[0].ToString().Trim(), OCR_TextResult_Back);
                                                                                                BIE_TemplateKey[BIE_TemplateKey_Row - 1, 1] = int.Parse(RW2[1].ToString().Trim());
                                                                                            }
                                                                                            double LastRate_Cal = 0;
                                                                                            double LastRate_Master = 0;
                                                                                            for (int i = 0; i < BIE_TemplateKey_Row; i++)
                                                                                            {
                                                                                                LastRate_Cal += BIE_TemplateKey[i, 0];
                                                                                                LastRate_Master += BIE_TemplateKey[i, 1];
                                                                                            }
                                                                                            LastRate_Cal /= BIE_TemplateKey_Row;
                                                                                            LastRate_Master /= BIE_TemplateKey_Row;
                                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 3] = (int)LastRate_Cal;
                                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 16] = 1;
                                                                                        }
                                                                                    }
                                                                                    if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 16] == 1)
                                                                                    {
                                                                                        // Get Front Image Scale :
                                                                                        double FI_X_CoeB = 0;
                                                                                        double FI_Y_CoeB = 0;
                                                                                        bool ProContinueB = false;
                                                                                        if (FIE_TemplateCode[FIE_TemplateCode_Row - 1, 3] > 0)
                                                                                        {
                                                                                            int M_X1 = 0; int M_Y1 = 0; int M_X2 = 0; int M_Y2 = 0;
                                                                                            int S_X1 = 0; int S_Y1 = 0; int S_X2 = 0; int S_Y2 = 0;
                                                                                            M_X1 = int.Parse(DT_FBEK.Rows[0][2].ToString().Trim());
                                                                                            M_Y1 = (int)((int.Parse(DT_FBEK.Rows[0][3].ToString().Trim()) + int.Parse(DT_FBEK.Rows[0][4].ToString().Trim())) / 2);
                                                                                            M_X2 = int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][2].ToString().Trim());
                                                                                            M_Y2 = (int)((int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][3].ToString().Trim()) + int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][4].ToString().Trim())) / 2);
                                                                                            if (SF.KeyDetector(ref S_X1, ref S_Y1, DT_FBEK.Rows[0][0].ToString().Trim(), OCRRes_Back.ParsedResults[0].TextOverlay.Lines, DT_FBEK.Rows[0][1].ToString().Trim(), int.Parse(DT_FBEK.Rows[0][5].ToString().Trim())) == true)
                                                                                            {
                                                                                                if (SF.KeyDetector(ref S_X2, ref S_Y2, DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][0].ToString().Trim(), OCRRes_Back.ParsedResults[0].TextOverlay.Lines, DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][1].ToString().Trim(), int.Parse(DT_FBEK.Rows[DT_FBEK.Rows.Count - 1][5].ToString().Trim())) == true)
                                                                                                {
                                                                                                    int XSM = Math.Abs(S_X1 - S_X2);
                                                                                                    int XMM = Math.Abs(M_X1 - M_X2);
                                                                                                    if (XSM == 0) { XSM = 1; }
                                                                                                    if (XMM == 0) { XMM = 1; }
                                                                                                    FI_X_CoeB = (double)((double)XSM / (double)XMM);
                                                                                                    int YSM = Math.Abs(S_Y1 - S_Y2);
                                                                                                    int YMM = Math.Abs(M_Y1 - M_Y2);
                                                                                                    if (YSM == 0) { YSM = 1; }
                                                                                                    if (YMM == 0) { YMM = 1; }
                                                                                                    FI_Y_CoeB = (double)((double)YSM / (double)YMM);
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 10] = FI_X_CoeB; // X Coefficient Front Image
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 11] = FI_Y_CoeB; // Y Coefficient Front Image
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 12] = S_X1 - (M_X1 * FI_X_CoeB); // Scan X Top-Left Front Image
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 13] = S_Y1 - (M_Y1 * FI_Y_CoeB); ; // Scan Y Top-Left Front Image
                                                                                                    ProContinueB = true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        if (ProContinueB == true)
                                                                                        {
                                                                                            // Front Image Color Picker :
                                                                                            DataTable DT_FICPB = new DataTable();
                                                                                            DT_FICPB = SQ.Get_DTable_TSQL("Select X,Y,R,G,B,Similarity From Template_10_FrontImage_ColorPicker Where (DID = '" + RW1[0].ToString().Trim() + "') Order By X,Y");
                                                                                            if (DT_FICPB.Rows != null)
                                                                                            {
                                                                                                if (DT_FICPB.Rows.Count > 0)
                                                                                                {
                                                                                                    double IMG_IP = 0;
                                                                                                    double IMG_BP = 0;
                                                                                                    foreach (DataRow RW2 in DT_FICPB.Rows)
                                                                                                    {
                                                                                                        int CPX = (int)((int.Parse(RW2[0].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 10]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 12]);
                                                                                                        int CPY = (int)((int.Parse(RW2[1].ToString().Trim()) * FIE_TemplateCode[FIE_TemplateCode_Row - 1, 11]) + FIE_TemplateCode[FIE_TemplateCode_Row - 1, 13]);
                                                                                                        Color CLS = ((Bitmap)Img_Front_Nor).GetPixel(CPX, CPY);
                                                                                                        IMG_IP += SF.GetColorSimilarity(CLS, Color.FromArgb(int.Parse(RW2[2].ToString().Trim()), int.Parse(RW2[3].ToString().Trim()), int.Parse(RW2[4].ToString().Trim())));
                                                                                                        IMG_BP += int.Parse(RW2[5].ToString().Trim());
                                                                                                    }
                                                                                                    IMG_IP /= DT_FICPB.Rows.Count;
                                                                                                    IMG_BP /= DT_FICPB.Rows.Count;
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 4] = (int)IMG_IP;
                                                                                                    FIE_TemplateCode[FIE_TemplateCode_Row - 1, 17] = 1;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                            // Get Optimal , Maximum , Minimum OCR Fields Count :
                                                                            int LastCounterPercent = 0;
                                                                            int OCRNorC = int.Parse(RW1[3].ToString().Trim());
                                                                            int OCRMinC = int.Parse(RW1[4].ToString().Trim());
                                                                            int OCRMaxC = int.Parse(RW1[5].ToString().Trim());
                                                                            int LineCount = OCRRes.ParsedResults[0].TextOverlay.Lines.Count;
                                                                            if (LineCount >= OCRMinC) { LastCounterPercent += 25; }
                                                                            if (LineCount <= OCRMaxC) { LastCounterPercent += 25; }
                                                                            if ((LineCount >= (OCRNorC - (int)((OCRNorC * NormalOcrLines_threshold) / 100))) && (LineCount <= (OCRNorC + (int)((OCRNorC * NormalOcrLines_threshold) / 100)))) { LastCounterPercent += 50; }
                                                                            FIE_TemplateCode[FIE_TemplateCode_Row - 1, 5] = (int)LastCounterPercent;
                                                                            // Get Front / Back Image Calculation :
                                                                            if (RW1[1].ToString().Trim() == "1") { FIE_TemplateCode[FIE_TemplateCode_Row - 1, 19] = 1; }
                                                                            if (RW1[6].ToString().Trim() == "1") { FIE_TemplateCode[FIE_TemplateCode_Row - 1, 18] = 1; }
                                                                        }
                                                                    }
                                                                }
                                                                // Select Template With Maximum Rank
                                                                if ((FIE_TemplateCode.Length / 21) > 0)
                                                                {
                                                                    int Divider_Count = 0;
                                                                    double SumRank = 0;
                                                                    // Calculate Template Last Rank
                                                                    for (int i = 0; i < (FIE_TemplateCode.Length / 21); i++)
                                                                    {
                                                                        Divider_Count = 0;
                                                                        SumRank = 0;
                                                                        FIE_TemplateCode[i, 20] = 0;
                                                                        // Back Image Rank 
                                                                        if (FIE_TemplateCode[i, 18] == 1)
                                                                        {
                                                                            // Key Similarity
                                                                            if (FIE_TemplateCode[i, 14] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 1] * Rank_KeySimilarity_FI;
                                                                                Divider_Count += Rank_KeySimilarity_FI;
                                                                            }
                                                                            // Color Picker Similarity 
                                                                            if (FIE_TemplateCode[i, 15] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 2] * Rank_ColorSimilarity_FI;
                                                                                Divider_Count += Rank_ColorSimilarity_FI;
                                                                            }
                                                                        }
                                                                        // Front Image Rank 
                                                                        if (FIE_TemplateCode[i, 19] == 1)
                                                                        {
                                                                            // Key Similarity
                                                                            if (FIE_TemplateCode[i, 16] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 3] * Rank_KeySimilarity_BI;
                                                                                Divider_Count += Rank_KeySimilarity_BI;
                                                                            }
                                                                            // Color Picker Similarity 
                                                                            if (FIE_TemplateCode[i, 17] == 1)
                                                                            {
                                                                                SumRank += FIE_TemplateCode[i, 4] * Rank_ColorSimilarity_BI;
                                                                                Divider_Count += Rank_ColorSimilarity_BI;
                                                                            }
                                                                        }
                                                                        // OCR Line Number Rank
                                                                        SumRank += FIE_TemplateCode[i, 5] * Rank_OCRFielad;
                                                                        Divider_Count += Rank_OCRFielad;
                                                                        // Calculate Average
                                                                        try
                                                                        {
                                                                            FIE_TemplateCode[i, 20] = (int)Math.Round(SumRank / Divider_Count);
                                                                        }
                                                                        catch (Exception)
                                                                        {
                                                                            FIE_TemplateCode[i, 20] = 0;
                                                                        }
                                                                    }
                                                                    // Select Template With Maximum Rank :
                                                                    var TemList = new List<TemplateList>();
                                                                    TemList.Clear();
                                                                    double MaxTemplateRank = 0;
                                                                    for (int i = 0; i < (FIE_TemplateCode.Length / 21); i++)
                                                                    {
                                                                        if (FIE_TemplateCode[i, 20] > MaxTemplateRank)
                                                                        {
                                                                            TemList.Clear();
                                                                            MaxTemplateRank = FIE_TemplateCode[i, 20];
                                                                            TemList.Add(new TemplateList()
                                                                            {
                                                                                TemplateID = FIE_TemplateCode[i, 0].ToString(),
                                                                                BackImage_Key_Similarity = FIE_TemplateCode[i, 1],
                                                                                BackImage_ColorPicker_Similarity = FIE_TemplateCode[i, 2],
                                                                                BackImage_X_Coeficient = FIE_TemplateCode[i, 6],
                                                                                BackImage_Y_Coeficient = FIE_TemplateCode[i, 7],
                                                                                BackImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 8],
                                                                                BackImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 9],
                                                                                FrontImage_Key_Similarity = FIE_TemplateCode[i, 3],
                                                                                FrontImage_ColorPicker_Similarity = FIE_TemplateCode[i, 4],
                                                                                FrontImage_X_Coeficient = FIE_TemplateCode[i, 10],
                                                                                FrontImage_Y_Coeficient = FIE_TemplateCode[i, 11],
                                                                                FrontImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 12],
                                                                                FrontImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 13],
                                                                                OCR_Line_Rank = FIE_TemplateCode[i, 5],
                                                                                TemplateAverageRank = FIE_TemplateCode[i, 20],
                                                                                TemplateAverageRank_Step2 = 0
                                                                            });
                                                                        }
                                                                        else
                                                                        {
                                                                            if (FIE_TemplateCode[i, 20] == MaxTemplateRank)
                                                                            {
                                                                                MaxTemplateRank = FIE_TemplateCode[i, 20];
                                                                                TemList.Add(new TemplateList()
                                                                                {
                                                                                    TemplateID = FIE_TemplateCode[i, 0].ToString(),
                                                                                    BackImage_Key_Similarity = FIE_TemplateCode[i, 1],
                                                                                    BackImage_ColorPicker_Similarity = FIE_TemplateCode[i, 2],
                                                                                    BackImage_X_Coeficient = FIE_TemplateCode[i, 6],
                                                                                    BackImage_Y_Coeficient = FIE_TemplateCode[i, 7],
                                                                                    BackImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 8],
                                                                                    BackImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 9],
                                                                                    FrontImage_Key_Similarity = FIE_TemplateCode[i, 3],
                                                                                    FrontImage_ColorPicker_Similarity = FIE_TemplateCode[i, 4],
                                                                                    FrontImage_X_Coeficient = FIE_TemplateCode[i, 10],
                                                                                    FrontImage_Y_Coeficient = FIE_TemplateCode[i, 11],
                                                                                    FrontImage_X_TopLeft_Refrence = FIE_TemplateCode[i, 12],
                                                                                    FrontImage_Y_TopLeft_Refrence = FIE_TemplateCode[i, 13],
                                                                                    OCR_Line_Rank = FIE_TemplateCode[i, 5],
                                                                                    TemplateAverageRank = FIE_TemplateCode[i, 20],
                                                                                    TemplateAverageRank_Step2 = 0
                                                                                });
                                                                            }
                                                                        }
                                                                    }
                                                                    if (TemList.Count == 1)
                                                                    {
                                                                        LST_Template.TemplateID = TemList[0].TemplateID;
                                                                        LST_Template.FrontImage_Key_Similarity = TemList[0].FrontImage_Key_Similarity;
                                                                        LST_Template.FrontImage_ColorPicker_Similarity = TemList[0].FrontImage_ColorPicker_Similarity;
                                                                        LST_Template.FrontImage_X_Coeficient = TemList[0].FrontImage_X_Coeficient;
                                                                        LST_Template.FrontImage_Y_Coeficient = TemList[0].FrontImage_Y_Coeficient;
                                                                        LST_Template.FrontImage_X_TopLeft_Refrence = TemList[0].FrontImage_X_TopLeft_Refrence;
                                                                        LST_Template.FrontImage_Y_TopLeft_Refrence = TemList[0].FrontImage_Y_TopLeft_Refrence;
                                                                        LST_Template.BackImage_Key_Similarity = TemList[0].BackImage_Key_Similarity;
                                                                        LST_Template.BackImage_ColorPicker_Similarity = TemList[0].BackImage_ColorPicker_Similarity;
                                                                        LST_Template.BackImage_X_Coeficient = TemList[0].BackImage_X_Coeficient;
                                                                        LST_Template.BackImage_Y_Coeficient = TemList[0].BackImage_Y_Coeficient;
                                                                        LST_Template.BackImage_X_TopLeft_Refrence = TemList[0].BackImage_X_TopLeft_Refrence;
                                                                        LST_Template.BackImage_Y_TopLeft_Refrence = TemList[0].BackImage_Y_TopLeft_Refrence;
                                                                        LST_Template.OCR_Line_Rank = TemList[0].OCR_Line_Rank;
                                                                        LST_Template.TemplateAverageRank = TemList[0].TemplateAverageRank;
                                                                        TemplateFounded = true;
                                                                    }
                                                                    else
                                                                    {
                                                                        foreach (TemplateList TL in TemList)
                                                                        {
                                                                            foreach (TemplateList TL2 in TemList)
                                                                            {
                                                                                if (TL.TemplateID != TL2.TemplateID)
                                                                                {
                                                                                    if (TL.TemplateAverageRank >= TL2.TemplateAverageRank)
                                                                                    {
                                                                                        if (TL.TemplateAverageRank == TL2.TemplateAverageRank)
                                                                                        {
                                                                                            if (TL.BackImage_Key_Similarity >= TL2.BackImage_Key_Similarity)
                                                                                            {
                                                                                                if (TL.BackImage_Key_Similarity == TL2.BackImage_Key_Similarity)
                                                                                                {
                                                                                                    if (TL.BackImage_ColorPicker_Similarity >= TL2.BackImage_ColorPicker_Similarity)
                                                                                                    {
                                                                                                        if (TL.BackImage_ColorPicker_Similarity == TL2.BackImage_ColorPicker_Similarity)
                                                                                                        {
                                                                                                            if (TL.FrontImage_Key_Similarity >= TL2.FrontImage_Key_Similarity)
                                                                                                            {
                                                                                                                if (TL.FrontImage_Key_Similarity == TL2.FrontImage_Key_Similarity)
                                                                                                                {
                                                                                                                    if (TL.FrontImage_ColorPicker_Similarity >= TL2.FrontImage_ColorPicker_Similarity)
                                                                                                                    {
                                                                                                                        if (TL.FrontImage_ColorPicker_Similarity == TL2.FrontImage_ColorPicker_Similarity)
                                                                                                                        {
                                                                                                                            if (TL.OCR_Line_Rank >= TL2.OCR_Line_Rank)
                                                                                                                            {
                                                                                                                                if (TL.OCR_Line_Rank == TL2.OCR_Line_Rank)
                                                                                                                                {
                                                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                                                    TL2.TemplateAverageRank_Step2 += 1;
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                                                }
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                TL2.TemplateAverageRank_Step2 += 1;
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            TL.TemplateAverageRank_Step2 += 1;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        TL2.TemplateAverageRank_Step2 += 1;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                TL2.TemplateAverageRank_Step2 += 1;
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            TL.TemplateAverageRank_Step2 += 1;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        TL2.TemplateAverageRank_Step2 += 1;
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    TL.TemplateAverageRank_Step2 += 1;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                TL2.TemplateAverageRank_Step2 += 1;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            TL.TemplateAverageRank_Step2 += 1;
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        TL2.TemplateAverageRank_Step2 += 1;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        double MaxRank2 = 0;
                                                                        foreach (TemplateList TL in TemList)
                                                                        {
                                                                            if (TL.TemplateAverageRank_Step2 > MaxRank2)
                                                                            {
                                                                                LST_Template.TemplateID = TL.TemplateID;
                                                                                LST_Template.FrontImage_Key_Similarity = TL.FrontImage_Key_Similarity;
                                                                                LST_Template.FrontImage_ColorPicker_Similarity = TL.FrontImage_ColorPicker_Similarity;
                                                                                LST_Template.FrontImage_X_Coeficient = TL.FrontImage_X_Coeficient;
                                                                                LST_Template.FrontImage_Y_Coeficient = TL.FrontImage_Y_Coeficient;
                                                                                LST_Template.FrontImage_X_TopLeft_Refrence = TL.FrontImage_X_TopLeft_Refrence;
                                                                                LST_Template.FrontImage_Y_TopLeft_Refrence = TL.FrontImage_Y_TopLeft_Refrence;
                                                                                LST_Template.BackImage_Key_Similarity = TL.BackImage_Key_Similarity;
                                                                                LST_Template.BackImage_ColorPicker_Similarity = TL.BackImage_ColorPicker_Similarity;
                                                                                LST_Template.BackImage_X_Coeficient = TL.BackImage_X_Coeficient;
                                                                                LST_Template.BackImage_Y_Coeficient = TL.BackImage_Y_Coeficient;
                                                                                LST_Template.BackImage_X_TopLeft_Refrence = TL.BackImage_X_TopLeft_Refrence;
                                                                                LST_Template.BackImage_Y_TopLeft_Refrence = TL.BackImage_Y_TopLeft_Refrence;
                                                                                LST_Template.OCR_Line_Rank = TL.OCR_Line_Rank;
                                                                                LST_Template.TemplateAverageRank = TL.TemplateAverageRank;
                                                                                LST_Template.TemplateAverageRank_Step2 = TL.TemplateAverageRank_Step2;
                                                                            }
                                                                        }
                                                                        TemplateFounded = true;
                                                                        foreach (TemplateList TL in TemList)
                                                                        {
                                                                            if (LST_Template.TemplateID != TL.TemplateID)
                                                                            {
                                                                                if (LST_Template.TemplateAverageRank_Step2 == TL.TemplateAverageRank_Step2)
                                                                                {
                                                                                    TemplateFounded = false;
                                                                                    break;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    TemplateFounded = false;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                TemplateFounded = false;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            TemplateFounded = false;
                                                        }
                                                    }
                                                    //=============================================================================================================================================================
                                                    //================================================================= Element Convert To Values =================================================================
                                                    //=============================================================================================================================================================
                                                    if (TemplateFounded == true)
                                                    {
                                                        if (LST_Template.TemplateAverageRank >= Rank_TemplateAcceptedMinmumRank)
                                                        {
                                                            // Get Front Image Elements :


                                                            // Get Front Image Barcode :


                                                            // Get Back Image Elements :


                                                            // Get Back Image Barcode :


                                                            // Save Data To Sql :


                                                            // Rotation Image :
                                                            //if (IMG_Nor_Tag_Front == true)
                                                            //{
                                                            //    Img_Front_Nor = ImageRotation((Bitmap)Img_Front_Nor, OCR_TextOrientation);
                                                            //}
                                                            //if (IMG_Nor_Tag_Back == true)
                                                            //{
                                                            //    if ((OCR_TextOrientation_Back != "0") || (OCR_TextOrientation_Back != ""))
                                                            //    {
                                                            //        Img_Back_Nor = ImageRotation((Bitmap)Img_Back_Nor, OCR_TextOrientation_Back);
                                                            //    }
                                                            //    else
                                                            //    {
                                                            //        Img_Back_Nor = ImageRotation((Bitmap)Img_Back_Nor, OCR_TextOrientation);
                                                            //    }
                                                            //}
                                                            //if (IMG_IR_Tag_Front == true)
                                                            //{
                                                            //    Img_Front_IR = ImageRotation((Bitmap)Img_Front_IR, OCR_TextOrientation);
                                                            //}

                                                            //if (IMG_IR_Tag_Back == true)
                                                            //{
                                                            //    if ((OCR_TextOrientation_Back != "0") || (OCR_TextOrientation_Back != ""))
                                                            //    {
                                                            //        Img_Back_IR = ImageRotation((Bitmap)Img_Back_IR, OCR_TextOrientation_Back);
                                                            //    }
                                                            //    else
                                                            //    {
                                                            //        Img_Back_IR = ImageRotation((Bitmap)Img_Back_IR, OCR_TextOrientation);
                                                            //    }
                                                            //}
                                                            //if (IMG_UV_Tag_Front == true)
                                                            //{
                                                            //    Img_Front_UV = ImageRotation((Bitmap)Img_Front_UV, OCR_TextOrientation);
                                                            //}
                                                            //if (IMG_UV_Tag_Back == true)
                                                            //{
                                                            //    if ((OCR_TextOrientation_Back != "0") || (OCR_TextOrientation_Back != ""))
                                                            //    {
                                                            //        Img_Back_UV = ImageRotation((Bitmap)Img_Back_UV, OCR_TextOrientation_Back);
                                                            //    }
                                                            //    else
                                                            //    {
                                                            //        Img_Back_UV = ImageRotation((Bitmap)Img_Back_UV, OCR_TextOrientation);
                                                            //    }
                                                            //}
                                                            // Crop Front Image :


                                                            // Crop Back Image :


                                                            // Save Result Images :
                                                            string SIMGP = "";
                                                            if (IMG_Nor_Tag_Front == true)
                                                            {
                                                                SIMGP = "";
                                                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/2.jpg");
                                                                Img_Front_Nor.Save(SIMGP, ImageFormat.Jpeg);
                                                            }
                                                            if (IMG_Nor_Tag_Back == true)
                                                            {
                                                                SIMGP = "";
                                                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/3.jpg");
                                                                Img_Back_Nor.Save(SIMGP, ImageFormat.Jpeg);
                                                            }
                                                            if (IMG_IR_Tag_Front == true)
                                                            {
                                                                SIMGP = "";
                                                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/4.jpg");
                                                                Img_Front_IR.Save(SIMGP, ImageFormat.Jpeg);
                                                            }
                                                            if (IMG_IR_Tag_Back == true)
                                                            {
                                                                SIMGP = "";
                                                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/5.jpg");
                                                                Img_Back_IR.Save(SIMGP, ImageFormat.Jpeg);
                                                            }
                                                            if (IMG_UV_Tag_Front == true)
                                                            {
                                                                SIMGP = "";
                                                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/6.jpg");
                                                                Img_Front_UV.Save(SIMGP, ImageFormat.Jpeg);
                                                            }
                                                            if (IMG_UV_Tag_Back == true)
                                                            {
                                                                SIMGP = "";
                                                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/7.jpg");
                                                                Img_Back_UV.Save(SIMGP, ImageFormat.Jpeg);
                                                            }
                                                            // Save Face Image :
                                                            if (FaceDetected == true)
                                                            {
                                                                try
                                                                {
                                                                    Rectangle rct = FaceDetector_FB(Selected_IMG);
                                                                    Image FCIMG = CropImage(Selected_IMG, rct);
                                                                    SIMGP = "";
                                                                    SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/1.jpg");
                                                                    FCIMG.Save(SIMGP, ImageFormat.Jpeg);
                                                                }
                                                                catch (Exception)
                                                                { }
                                                            }
                                                            // Update Last Status :
                                                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '6',[Status_Text] = 'Done',[ErrorMessage] = '',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                                        }
                                                        else
                                                        {
                                                            // Template Rank no Accept
                                                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '7',[Status_Text] = 'Failed',[ErrorMessage] = 'Template rank failed',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                                        }
                                                    }
                                                    else
                                                    {   // Template not found
                                                        SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '7',[Status_Text] = 'Failed',[ErrorMessage] = 'Template not found',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                                    }
                                                }
                                                else
                                                {   // OCR Return Error Message
                                                    SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '7',[Status_Text] = 'Failed',[ErrorMessage] = '" + OCRRes.ParsedResults[0].ErrorMessage.Trim() + "\r\n" + OCRRes.ParsedResults[0].ErrorDetails.Trim() + "',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                                }
                                            }
                                            else
                                            {   // Processing Error
                                                SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '7',[Status_Text] = 'Failed',[ErrorMessage] = 'Is errored on processing',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                            }
                                        }
                                        else
                                        {   // Exit Code Error
                                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '7',[Status_Text] = 'Failed',[ErrorMessage] = 'Exit code : " + OCRRes.OCRExitCode.ToString() + "',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                        }
                                    }
                                    else
                                    {   // No Face Detected or Back Image Not Recognized.
                                        SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'Images not valid',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                    }
                                }
                                else
                                {   // Applicarion Don't Have Front or Back Image For OCR.
                                    SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'OCR image not found',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                                }
                            }
                            else
                            {   // DataTable Return Multy or Zero Row.
                                SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'DT return ununic row application',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                            }
                        }
                        else
                        {   // DataTable Return Null Rows.
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'DT return null application',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                    }
                    else
                    {   // DataTable Return No 1 Record For Template Searching Code.
                        SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'DT return ununic row setting',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                    }
                }
                else
                {   // DataTable Return null Record For Template Searching Code.
                    SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'DT return null etting',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                }
            }
            catch (Exception e)
            {   // Structure Error.
                SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[ErrorMessage] = 'Local exception error',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
            }
        }
        //-----------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------
    }
}