using AssureTec.AssureID.Web.SDK;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace IDV_ScannerWS.Modules
{
    public class AssureID
    {
        //--------------------------------------------------------------------------
        SQL_Tranceiver SQ = new SQL_Tranceiver();
        PublicFunctions PB = new PublicFunctions();
        //--------------------------------------------------------------------------
        string Username = "";
        string Password = "";
        string AssureIdConnectEndpoint = "";
        Guid _subscriptionId;
        //--------------------------------------------------------------------------
        private AssureIDServiceClient _assureIdServiceClient;
        //--------------------------------------------------------------------------
        private const string Manufacturer = "xxx";
        private const string Model = "xxx";
        private const string SerialNumber = "xxx";
        private const bool HasMagneticStripeReader = false;
        private const bool HasContactlessChipReader = false;
        private DeviceType _deviceType;
        private DeviceInfo _deviceInfo;
        private DocumentSettings _documentSettings;
        private SensorType? _sensorType = null;
        private CroppingExpectedSize? _croppingExpectedSize = null;
        private CroppingMode? _croppingMode = null;
        //private bool _frontWhiteImgFlag, _frontIrImgFlag, _frontUvImgFlag, _backWhiteImgFlag, _backIrImgFlag, _backUvImgFlag;
        //private string _frontWhiteImgFilePath, _frontIrImgFilePath, _frontUvImgFilePath, _backWhiteImgFilePath, _backIrImgFilePath, _backUvImgFilePath;
        //--------------------------------------------------------------------------
        public AssureIDServiceClient AssureIdServiceClient(string endpoint, string username, string password)
        {
            try
            {
                var serverAddress = new Uri(endpoint);
                var networkCredential = new NetworkCredential(username, password);
                return new AssureIDServiceClient(serverAddress, networkCredential);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool CheckSubscription()
        {
            try
            {
                var subscriptions = _assureIdServiceClient.GetSubscriptions(false);
                if (subscriptions.Length > 0)
                {
                    var isSubscriptionActive = subscriptions.Any(s => s.Id == _subscriptionId);

                    if (isSubscriptionActive)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static Bitmap SetId2ImagResolution(Bitmap bitmap)
        {
            var w = (bitmap.Width >= bitmap.Height) ? bitmap.Width : bitmap.Height;
            var dpi = (int)(w / 4.134);
            bitmap.SetResolution(dpi, dpi);
            return bitmap;
        }

        public void GetData(string AppID)
        {
            try
            {
                DataTable DT = new DataTable();
                DT = SQ.Get_DTable_TSQL("Select * From AD_AssureID");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        Username = DT.Rows[0][0].ToString().Trim();
                        Password = DT.Rows[0][1].ToString().Trim();
                        AssureIdConnectEndpoint = DT.Rows[0][2].ToString().Trim();
                        _subscriptionId = new Guid(DT.Rows[0][3].ToString().Trim());
                        if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(AssureIdConnectEndpoint) && _subscriptionId != Guid.Empty)
                        {
                            _assureIdServiceClient = AssureIdServiceClient(AssureIdConnectEndpoint, Username, Password);

                            if (!CheckSubscription())
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                        _sensorType = SensorType.Scanner;
                        _croppingMode = CroppingMode.None;
                        _croppingExpectedSize = CroppingExpectedSize.ID1;
                        _deviceType = new DeviceType(Manufacturer, Model, (SensorType)_sensorType);
                        _deviceInfo = new DeviceInfo(_deviceType, SerialNumber, HasMagneticStripeReader, HasContactlessChipReader);
                        _documentSettings = new DocumentSettings(_deviceInfo);
                        _documentSettings.ImageCroppingExpectedSize = (CroppingExpectedSize)_croppingExpectedSize;
                        _documentSettings.ImageCroppingMode = (CroppingMode)_croppingMode;
                        _documentSettings.SubscriptionId = _subscriptionId;
                        var documentInstanceId = _assureIdServiceClient.PostDocumentInstance(_documentSettings);
                        DocumentSide documentSide;
                        LightSource lightSource;
                        // Ready Image :
                        string BaseFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID);
                        if (Directory.Exists(BaseFolder) == false) { Directory.CreateDirectory(BaseFolder); }
                        if (Directory.Exists(BaseFolder + "\\" + "Scanned") == false) { Directory.CreateDirectory(BaseFolder + "\\" + "Scanned"); }
                        if (Directory.Exists(BaseFolder + "\\" + "Result") == false) { Directory.CreateDirectory(BaseFolder + "\\" + "Result"); }
                        bool WI_F_F = false; string WI_F_A = "";
                        bool WI_B_F = false; string WI_B_A = "";
                        var filePath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "2" + ".jpg");
                        if (File.Exists(filePath1) == true)
                        {
                            WI_F_F = true;
                            WI_F_A = filePath1;
                        }
                        var filePath2 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "3" + ".jpg");
                        if (File.Exists(filePath2) == true)
                        {
                            WI_B_F = true;
                            WI_B_A = filePath2;
                        }
                        if (WI_F_F == false) { return; }
                        if (WI_B_F == false) { return; }
                        if (WI_F_F)
                        {
                            documentSide = DocumentSide.Front;
                            lightSource = LightSource.White;
                            var bitmap = new Bitmap(WI_F_A);
                            {
                                _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                            }
                        }
                        if (WI_B_F)
                        {
                            documentSide = DocumentSide.Back;
                            lightSource = LightSource.White;
                            using (var bitmap = new Bitmap(WI_B_A))
                            {
                                _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                            }
                        }
                        var document = _assureIdServiceClient.GetDocument(documentInstanceId);
                        var result = document.Result;
                        SQ.Execute_TSql("Delete From US_DL_02_Alerts Where (App_ID = '" + AppID + "')");
                        SQ.Execute_TSql("Delete From US_DL_03_Documents Where (App_ID = '" + AppID + "')");
                        //if (result != AuthenticationResult.Passed && result != AuthenticationResult.Failed)
                        //{
                        if (document.Alerts.Length > 0)
                        {
                            foreach (var alert in document.Alerts)
                            {
                                if (alert.Result == result)
                                {
                                    try
                                    {
                                        SQ.Execute_TSql("Insert Into US_DL_02_Alerts Values ('" + AppID + "','" + alert.Key.Trim() + "')");
                                    }
                                    catch (Exception) { }
                                }
                            }
                        }
                        //}
                        int DocCounter = 0;
                        if (document.Fields.Length > 0)
                        {
                            foreach (var field in document.Fields)
                            {
                                if (field.Name == "Photo" || field.Name == "Signature")
                                {
                                    switch (field.Name)
                                    {
                                        case "Photo":
                                            {
                                                try
                                                {
                                                    Bitmap FIMG = _assureIdServiceClient.GetDocumentFieldImage(document.InstanceId, "Photo");
                                                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/1.jpg");
                                                    FIMG.Save(filePath, ImageFormat.Jpeg);
                                                }
                                                catch (Exception) { }
                                                break;
                                            }
                                        case "Signature":
                                            {
                                                Bitmap SIMG = _assureIdServiceClient.GetDocumentFieldImage(document.InstanceId, "Signature");
                                                var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/8.jpg");
                                                SIMG.Save(filePath, ImageFormat.Jpeg);
                                                break;
                                            }
                                    }
                                }
                                else
                                {
                                    DocCounter++;
                                    try
                                    {
                                        SQ.Execute_TSql("Insert Into US_DL_03_Documents Values ('" + AppID + "','" + DocCounter.ToString() + "','" + field.Name.Trim() + "','" + field.Value.ToString().Trim() + "')");
                                    }
                                    catch (Exception) { }
                                }
                            }
                            try
                            {
                                if (document.Classification.Type.FullName.ToString().Trim() != "")
                                {
                                    DocCounter++;
                                    try
                                    {
                                        SQ.Execute_TSql("Insert Into US_DL_03_Documents Values ('" + AppID + "','" + DocCounter.ToString() + "','Document Type','" + document.Classification.Type.FullName.ToString().Trim() + "')");
                                    }
                                    catch (Exception) { }
                                }
                            }
                            catch (Exception) { }
                        }
                        // Save Last Image to Result Folder :
                        bool VF = false; bool VB = false;
                        bool IF = false; bool IB = false;
                        bool UF = false; bool UB = false;
                        Image VFI = null; Image VBI = null;
                        Image IFI = null; Image IBI = null;
                        Image UFI = null; Image UBI = null;
                        string FileNameIMG = "";
                        FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/2.jpg");
                        if (File.Exists(FileNameIMG) == true) { VF = true; VFI = new Bitmap(FileNameIMG); }
                        FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/3.jpg");
                        if (File.Exists(FileNameIMG) == true) { VB = true; VBI = new Bitmap(FileNameIMG); }
                        FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/4.jpg");
                        if (File.Exists(FileNameIMG) == true) { IF = true; IFI = new Bitmap(FileNameIMG); }
                        FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/5.jpg");
                        if (File.Exists(FileNameIMG) == true) { IB = true; IBI = new Bitmap(FileNameIMG); }
                        FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/6.jpg");
                        if (File.Exists(FileNameIMG) == true) { UF = true; UFI = new Bitmap(FileNameIMG); }
                        FileNameIMG = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/7.jpg");
                        if (File.Exists(FileNameIMG) == true) { UB = true; UBI = new Bitmap(FileNameIMG); }
                        if (document.Classification.OrientationChanged == true)
                        {
                            try { VFI.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                            try { VBI.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                            try { IFI.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                            try { IBI.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                            try { UFI.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                            try { UBI.RotateFlip(RotateFlipType.Rotate180FlipNone); } catch (Exception) { }
                        }
                        string SIMGP = "";
                        if (document.Classification.PresentationChanged == false)
                        {
                            if (VF == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/2.jpg");
                                VFI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (VB == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/3.jpg");
                                VBI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (IF == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/4.jpg");
                                IFI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (IB == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/5.jpg");
                                IBI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (UF == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/6.jpg");
                                UFI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (UB == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/7.jpg");
                                UBI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                        }
                        else
                        {
                            if (VF == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/3.jpg");
                                VFI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (VB == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/2.jpg");
                                VBI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (IF == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/5.jpg");
                                IFI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (IB == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/4.jpg");
                                IBI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (UF == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/7.jpg");
                                UFI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                            if (UB == true)
                            {
                                SIMGP = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/6.jpg");
                                UBI.Save(SIMGP, ImageFormat.Jpeg);
                            }
                        }
                        // Set Application Status :
                        if (result == AuthenticationResult.Unknown)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '3',[Status_Text] = 'Unknown',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                        if (result == AuthenticationResult.Attention)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '4',[Status_Text] = 'Attention',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                        if (result == AuthenticationResult.Caution)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '5',[Status_Text] = 'Caution',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                        if (result == AuthenticationResult.Skipped)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '6',[Status_Text] = 'Skipped',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                        if (result == AuthenticationResult.Failed)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '7',[Status_Text] = 'Failed',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                        if (result == AuthenticationResult.Passed)
                        {
                            SQ.Execute_TSql("Update US_DL_01_Application Set [Status_Code] = '8',[Status_Text] = 'Passed',[Update_Date] = '" + PB.Get_Date() + "',[Update_Time] = '" + PB.Get_Time() + "' Where (ApplicationID = '" + AppID + "')");
                        }
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception) { return; }
        }

        private void SetImages(string AppID)
        {
            var documentInstanceId = _assureIdServiceClient.PostDocumentInstance(_documentSettings);
            DocumentSide documentSide;
            LightSource lightSource;
            string BaseFolder = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID);
            if (Directory.Exists(BaseFolder) == false) { Directory.CreateDirectory(BaseFolder); }
            if (Directory.Exists(BaseFolder + "\\" + "Scanned") == false) { Directory.CreateDirectory(BaseFolder + "\\" + "Scanned"); }
            if (Directory.Exists(BaseFolder + "\\" + "Result") == false) { Directory.CreateDirectory(BaseFolder + "\\" + "Result"); }
            bool WI_F_F = false; string WI_F_A = "";
            bool WI_B_F = false; string WI_B_A = "";
            bool II_F_F = false; string II_F_A = "";
            bool II_B_F = false; string II_B_A = "";
            bool UI_F_F = false; string UI_F_A = "";
            bool UI_B_F = false; string VI_B_A = "";
            var filePath1 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "2" + ".jpg");
            if (File.Exists(filePath1) == true)
            {
                WI_F_F = true;
                WI_F_A = filePath1;
            }
            var filePath2 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "3" + ".jpg");
            if (File.Exists(filePath2) == true)
            {
                WI_B_F = true;
                WI_B_A = filePath2;
            }
            var filePath3 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "4" + ".jpg");
            if (File.Exists(filePath3) == true)
            {
                II_F_F = true;
                II_F_A = filePath3;
            }
            var filePath4 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "5" + ".jpg");
            if (File.Exists(filePath4) == true)
            {
                II_B_F = true;
                II_B_A = filePath4;
            }
            var filePath5 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "6" + ".jpg");
            if (File.Exists(filePath5) == true)
            {
                UI_F_F = true;
                UI_F_A = filePath5;
            }
            var filePath6 = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Scanned/" + "7" + ".jpg");
            if (File.Exists(filePath6) == true)
            {
                UI_B_F = true;
                VI_B_A = filePath6;
            }
            if (WI_F_F)
            {
                documentSide = DocumentSide.Front;
                lightSource = LightSource.White;
                var bitmap = new Bitmap(WI_F_A);
                {
                    _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                }
            }
            if (WI_B_F)
            {
                documentSide = DocumentSide.Back;
                lightSource = LightSource.White;
                using (var bitmap = new Bitmap(WI_B_A))
                {
                    _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                }
            }
            if (II_F_F)
            {
                documentSide = DocumentSide.Front;
                lightSource = LightSource.NearInfrared;
                using (var bitmap = new Bitmap(II_F_A))
                {
                    _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                }
            }
            if (II_B_F)
            {
                documentSide = DocumentSide.Back;
                lightSource = LightSource.NearInfrared;
                using (var bitmap = new Bitmap(II_B_A))
                {
                    _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                }
            }
            if (UI_F_F)
            {
                documentSide = DocumentSide.Front;
                lightSource = LightSource.UltravioletA;
                using (var bitmap = new Bitmap(UI_F_A))
                {
                    _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                }
            }
            if (UI_B_F)
            {
                documentSide = DocumentSide.Back;
                lightSource = LightSource.UltravioletA;
                using (var bitmap = new Bitmap(VI_B_A))
                {
                    _assureIdServiceClient.PostDocumentImage(documentInstanceId, documentSide, lightSource, bitmap);
                }
            }
            var document = _assureIdServiceClient.GetDocument(documentInstanceId);
            if ((_croppingMode == CroppingMode.Automatic) || (_croppingMode == CroppingMode.Always) || document.Classification.PresentationChanged || document.Classification.OrientationChanged)
            {
                if (WI_F_F)
                {
                    Image image = _assureIdServiceClient.GetDocumentImage(document.InstanceId, DocumentSide.Front, LightSource.White);
                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/2.jpg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                if (WI_B_F)
                {
                    Image image = _assureIdServiceClient.GetDocumentImage(document.InstanceId, DocumentSide.Back, LightSource.White);
                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/3.jpg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                if (II_F_F)
                {
                    Image image = _assureIdServiceClient.GetDocumentImage(document.InstanceId, DocumentSide.Front, LightSource.NearInfrared);
                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/4.jpg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                if (II_B_F)
                {
                    Image image = _assureIdServiceClient.GetDocumentImage(document.InstanceId, DocumentSide.Back, LightSource.NearInfrared);
                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/5.jpg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                if (UI_F_F)
                {
                    Image image = _assureIdServiceClient.GetDocumentImage(document.InstanceId, DocumentSide.Front, LightSource.UltravioletA);
                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/6.jpg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
                if (UI_B_F)
                {
                    Image image = _assureIdServiceClient.GetDocumentImage(document.InstanceId, DocumentSide.Back, LightSource.UltravioletA);
                    var filePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Photos/Acuant/" + AppID + "/Result/7.jpg");
                    image.Save(filePath, ImageFormat.Jpeg);
                }
            }
        }
    }
}