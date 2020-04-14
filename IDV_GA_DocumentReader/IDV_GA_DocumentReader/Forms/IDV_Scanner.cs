﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;

namespace IDV_GA_DocumentReader.Forms
{

    public partial class IDV_Scanner : Form
    {
        string FileLog = "";
        private Control THC;
        bool Cam_Init_1 = false;
        bool Cam_Init_2 = true;
        bool Cam_Init_3 = false;
        bool Init_Activity_Complete = false;
        bool FirstLoad = true;
        //--------------------------------------------------------
        string ServerAddress = "http://52.64.239.184:8080";
        string UserID = "1";
        //--------------------------------------------------------
        ListBox SelectedPluginList = new ListBox();
        ListBox AvailablePluginList = new ListBox();
        ListBox RFIDAvailDataItems = new ListBox();
        ListBox RFIDSelectDataItems = new ListBox();
        ListBox UHFAvailDataItems = new ListBox();
        ListBox UHFSelectDataItems = new ListBox();

        int Activity_Now = 0;
        bool SystemRunNew = false;

        public IDV_Scanner()
        {
            InitializeComponent();
            THC = new Control();
            THC.CreateControl();
        }

        private void IDV_Scanner_Load(object sender, EventArgs e)
        {
            this.Top = 0;
            this.Left = 0;
            this.WindowState = FormWindowState.Maximized;
            PL_01_Splash.Visible = false;
            PL_02_Splash.Visible = false;
            PL_03_Splash.Visible = false;
            PL_04_Splash.Visible = false;
            Lbl_ErrorPublic.Text = "";
            SystemRunNew = false;
            if (System.IO.Directory.Exists(Application.StartupPath + @"\Logs") == false) { System.IO.Directory.CreateDirectory(Application.StartupPath + @"\Logs"); }
            FileLog = Application.StartupPath + @"\Logs\" + DateTime.Now.ToString("ddMMyyyy-HHmm") + ".txt";
            File.AppendAllText(FileLog, "-------------------------------------------------------------------------" + "\r\n");
            File.AppendAllText(FileLog, "- Run Software -> " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + "\r\n");
            File.AppendAllText(FileLog, "-------------------------------------------------------------------------" + "\r\n");
            timer1.Interval = 1000;
            timer1.Start();
            timer2.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            PL_01_Splash.Left = (this.Width / 2) - (PL_01_Splash.Width / 2);
            PL_01_Splash.Top = (this.Height / 2) - (PL_01_Splash.Height / 2);
            File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Initial System Startup ..." + "\r\n");
            Splash_Lbl_1.Text = "Initial System Startup ...";
            Splash_Lbl_2.Text = "";
            Splash_Btn_Exit.Visible = false;
            Splash_Btn_Retry.Visible = false;
            PL_01_Splash.Visible = true;
            timer2.Interval = 1000;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            Lbl_ErrorPublic.Text = "";
            Splash_Lbl_1.Text = "Initial System Startup ...";
            Splash_Lbl_2.Text = "";
            Splash_Btn_Retry.Visible = false;
            Splash_Btn_Exit.Visible = false;
            Cam_Init_1 = false;
            Cam_Init_2 = true;
            Cam_Init_3 = false;
            Init_Activity_Complete = false;
            FirstLoad = true;
            int ReloadInit = 0;
            Application.DoEvents();
            try
            {
                File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Connect To Camera Driver" + "\r\n");
                MMM.Readers.FullPage.Reader.EnableLogging(true, 1, -1, "HLNonBlockingExample.Net.log");
                MMM.Readers.ErrorCode lResult = MMM.Readers.ErrorCode.NO_ERROR_OCCURRED;
                Microsoft.Win32.SystemEvents.PowerModeChanged += new Microsoft.Win32.PowerModeChangedEventHandler(OnPowerModeChanged);
                lResult = MMM.Readers.FullPage.Reader.Initialise(new MMM.Readers.FullPage.DataDelegate(DataCallbackThreadHelper), new MMM.Readers.FullPage.EventDelegate(EventCallbackThreadHelper), new MMM.Readers.ErrorDelegate(ErrorCallbackThreadHelper), new MMM.Readers.FullPage.CertificateDelegate(CertificateCallbackThreadHelper), true, false);
                if (lResult != MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                {
                    Splash_Lbl_1.Text = "Error! Initialise Failed";
                    Splash_Lbl_2.Text = lResult.ToString();
                    ReloadInit = 1;
                    File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Initialise Failed" + "\r\n");
                }
                MMM.Readers.FullPage.Reader.SetWarningCallback(new MMM.Readers.WarningDelegate(WarningCallbackThreadHelper));
            }
            catch (System.DllNotFoundException except)
            {
                Splash_Lbl_1.Text = "Error! Initialise Failed";
                File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Initialise Failed" + "\r\n");
                Splash_Lbl_2.Text = except.Message;
                ReloadInit = 1;
            }
            if (ReloadInit == 0)
            {
                while (Init_Activity_Complete == false)
                {
                    if ((Cam_Init_1 == true) && (Cam_Init_2 == true) && (Cam_Init_3 == true)) { Init_Activity_Complete = true; }
                    Application.DoEvents();
                }
                if ((Cam_Init_1 == true) && (Cam_Init_2 == true) && (Cam_Init_3 == true))
                {
                    MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                    Splash_Lbl_1.Text = "Congratulations! Initialise Successfully";
                    Splash_Lbl_2.Text = "Applying RFID Chip Setting ...";
                    File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Congratulations! Initialise Successfully" + "\r\n");
                    File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Applying RFID Chip Setting ..." + "\r\n");
                    Application.DoEvents();
                    MMM.Readers.FullPage.ReaderState state = MMM.Readers.FullPage.Reader.GetState();
                    if (state == MMM.Readers.FullPage.ReaderState.READER_NOT_INITIALISED || state == MMM.Readers.FullPage.ReaderState.READER_ERRORED || state == MMM.Readers.FullPage.ReaderState.READER_FATAL_ERRORED || state == MMM.Readers.FullPage.ReaderState.READER_TERMINATED || state == MMM.Readers.FullPage.ReaderState.READER_READING)
                    {
                        Splash_Lbl_1.Text = "Error! RFID Chip Setting Failed";
                        Splash_Lbl_2.Text = "Scanner not respone to system request. Please restart software [ Code:101 ]";
                        Splash_Btn_Exit.Visible = true;
                        Splash_Btn_Retry.Visible = false;
                        File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Error! RFID Chip Setting Failed" + "\r\n");
                        File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Scanner not respone to system request. Please restart software [ Code:101 ]" + "\r\n");
                    }
                    else
                    {
                        DataToSend_Reload();
                        RFIDSelectAll();
                        Application.DoEvents();
                        MMM.Readers.FullPage.ReaderSettings lSettings;
                        if (MMM.Readers.FullPage.Reader.GetSettings(out lSettings) == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                        {
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.DOCMARKERS;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.CODELINE;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.CHECKSUM;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.IRIMAGE;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.VISIBLEIMAGE;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.UVIMAGE;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.PHOTOIMAGE;
                            lSettings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.SMARTCARD;
                            lSettings.puDataToSend.send &= ~MMM.Readers.FullPage.DataSendSet.Flags.SECURITYCHECK;
                            lSettings.puDataToSend.send &= ~MMM.Readers.FullPage.DataSendSet.Flags.SWIPE;
                            lSettings.puDataToSend.send &= ~MMM.Readers.FullPage.DataSendSet.Flags.AAMVA;
                            lSettings.puDataToSend.special &= ~MMM.Readers.FullPage.DataSendSet.Flags.SECURITYCHECK;
                            lSettings.puDataToSend.special |= MMM.Readers.FullPage.DataSendSet.Flags.IRIMAGE;
                            lSettings.puDataToSend.special |= MMM.Readers.FullPage.DataSendSet.Flags.VISIBLEIMAGE;
                            lSettings.puCameraSettings.ir.puUseAntiGlare = true;
                            lSettings.puCameraSettings.vis.puUseAntiGlare = true;
                            lSettings.puCameraSettings.ir.puUseSequentialImaging = false;
                            lSettings.puCameraSettings.vis.puUseSequentialImaging = false;
                            lSettings.puCameraSettings.flap = 0;
                            lSettings.puCameraSettings.flap |= 0x08;
                            lSettings.puCameraSettings.ir.puAmbientRemoval = MMM.Readers.Modules.Camera.AmbientRemovalMethod.MRAS_HARDWARE_AMBIENT_REMOVAL;
                            lSettings.puCameraSettings.vis.puAmbientRemoval = MMM.Readers.Modules.Camera.AmbientRemovalMethod.MRAS_HARDWARE_AMBIENT_REMOVAL;
                            lSettings.puCameraSettings.uv.puAmbientRemoval = MMM.Readers.Modules.Camera.AmbientRemovalMethod.MRAS_HARDWARE_AMBIENT_REMOVAL;
                            lSettings.puImageSettings.useVisibleForBarcode = 0;
                            lSettings.puImageSettings.useVisibleForBarcode = 1;
                            lSettings.puDataToSend.send &= ~MMM.Readers.FullPage.DataSendSet.Flags.BARCODEIMAGE;
                            lSettings.puDataToSend.send &= ~MMM.Readers.FullPage.DataSendSet.Flags.UHF;
                            lSettings.puDataToSend.progress = 0;
                            lSettings.puDataToSend.imageFormat = MMM.Readers.FullPage.ImageFormats.RTE_JPEG;
                            lSettings.puCameraSettings.scaleFactor = 100;
                            lSettings.puRFIDSettings.puRFProcessSettings.puRFApplicationMode |= (int)MMM.Readers.Modules.RF.RF_APPLICATION_MODE.EPASSPORT;
                            lSettings.puRFIDSettings.puRFProcessSettings.puRFApplicationMode &= ~(int)MMM.Readers.Modules.RF.RF_APPLICATION_MODE.EID;
                            lSettings.puRFIDSettings.puRFProcessSettings.puRFApplicationMode &= ~(int)MMM.Readers.Modules.RF.RF_APPLICATION_MODE.EDL;
                            lSettings.puRFIDSettings.puRFProcessSettings.puRFApplicationMode &= ~(int)MMM.Readers.Modules.RF.RF_APPLICATION_MODE.IDL;
                            StoreUHFOrder(ref lSettings.puDataToSend.uhf);
                            StoreRFIDOrder(ref lSettings.puDataToSend.rfid);
                            StorePlugins();
                            FirstLoad = false;
                            Wait(2000);
                            if (textBox1.Text.IndexOf("DOC_ON_WINDOW") > 0) { Wait(4000); }
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_ASLEEP, true);
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                            MMM.Readers.ErrorCode EE = MMM.Readers.FullPage.Reader.UpdateSettings(lSettings);
                            Splash_Lbl_2.Text = "RFID Chip Setting Ready to use";
                            File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "RFID Chip Setting Ready to use" + "\r\n");
                            Application.DoEvents();
                            Wait(1000);
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_ASLEEP, true);
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                            DoScan();
                        }
                        else
                        {
                            Splash_Lbl_1.Text = "Error! RFID Chip Setting Failed";
                            Splash_Lbl_2.Text = "Scanner not respone to system request. Please restart software [ Code:102 ]";
                            Splash_Btn_Exit.Visible = true;
                            Splash_Btn_Retry.Visible = false;
                            File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Error! RFID Chip Setting Failed" + "\r\n");
                            File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Scanner not respone to system request. Please restart software [ Code:102 ]" + "\r\n");
                        }

                    }
                }
                else
                {
                    Splash_Lbl_1.Text = "Error! Initialise Failed";
                    Splash_Lbl_2.Text = "Please Retry Initialise System";
                    File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Initialise Failed" + "\r\n");
                    File.AppendAllText(FileLog, "- System Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Please Retry Initialise System" + "\r\n");
                    Splash_Btn_Exit.Visible = true;
                    Splash_Btn_Retry.Visible = false;
                }
            }
            else
            {
                Splash_Btn_Exit.Visible = true;
                Splash_Btn_Retry.Visible = false;
            }
        }

        private void DoScan()
        {
            try
            {
                bool ExitLoop = false;
                PL_02_Splash.Left = (this.Width / 2) - (PL_02_Splash.Width / 2);
                PL_02_Splash.Top = (this.Height / 2) - (PL_02_Splash.Height / 2);
                PL_03_Splash.Left = (this.Width / 2) - (PL_03_Splash.Width / 2);
                PL_03_Splash.Top = (this.Height / 2) - (PL_03_Splash.Height / 2);
                PL_04_Splash.Left = (this.Width / 2) - (PL_04_Splash.Width / 2);
                PL_04_Splash.Top = (this.Height / 2) - (PL_04_Splash.Height / 2);
                //----------------------------------------------------------------
                Activity_Now = 1;
                // 1 : Show First Page And Wait For Insert Passport .
                // 2 : Readding Data And Send to Webservice .
                // 3 : Show Result Tick .
                // 4 : Wait For Remove Passport From Scanner .
                // 5 : Wait For Response From Scanner .
                //----------------------------------------------------------------
                SystemRunNew = true;
                while (ExitLoop == false)
                {
                    switch (Activity_Now)
                    {
                        case 1:
                            {
                                MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                                PL_01_Splash.Visible = false;
                                PL_02_Splash.Visible = true;
                                PL_03_Splash.Visible = false;
                                PL_04_Splash.Visible = false;
                                Application.DoEvents();
                                File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Wait For Passport" + "\r\n");
                                Wait(1000);
                                Activity_Now = 5;
                                MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_ENABLED, true);
                                break;
                            }
                        //-------------------------------------------------------------------------------------------------------
                        case 2:
                            {
                                PL_01_Splash.Visible = false;
                                PL_02_Splash.Visible = false;
                                PL_03_Splash.Visible = true;
                                PL_04_Splash.Visible = false;
                                File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Proccess Passport" + "\r\n");
                                Activity_Now = 5;
                                break;
                            }
                        //-------------------------------------------------------------------------------------------------------
                        case 3:
                            {
                                int StatusCode = 0;
                                File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Proccess End" + "\r\n");
                                pictureBox3.Image = null;
                                LBLD_1.Text = "";
                                LBLD_2.Text = "";
                                LBLD_3.Text = "";
                                LBLD_4.Text = "";
                                LBLD_5.Text = "";
                                T_OK.Visible = false;
                                T_Error.Visible = false;
                                try
                                { if (rfImage.Image != null) { pictureBox3.Image = rfImage.Image; } else { if (photoImage.Image != null) { pictureBox3.Image = photoImage.Image; } } }
                                catch (Exception) { }
                                try
                                {
                                    if ((lblRFForenames.Text != null) && (lblRFForenames.Text != "")) { LBLD_1.Text = lblRFForenames.Text.Trim().ToUpper(); }
                                    if (LBLD_1.Text.Trim() == "") { LBLD_1.Text = lblForenames.Text.Trim().ToUpper(); }
                                }
                                catch (Exception) { }
                                try
                                {
                                    if ((lblRFSurname.Text != null) && (lblRFSurname.Text != "")) { LBLD_2.Text = lblRFSurname.Text.Trim().ToUpper(); }
                                    if (LBLD_2.Text.Trim() == "") { LBLD_2.Text = lblSurname.Text.Trim().ToUpper(); }
                                }
                                catch (Exception) { }
                                try
                                {
                                    if ((lblRFNationality.Text != null) && (lblRFNationality.Text != "")) { LBLD_3.Text = lblRFNationality.Text.Trim().ToUpper(); }
                                    if (LBLD_3.Text.Trim() == "") { LBLD_3.Text = lblNationality.Text.Trim().ToUpper(); }
                                }
                                catch (Exception) { }
                                try
                                {
                                    if ((lblRFSex.Text != null) && (lblRFSex.Text != "")) { LBLD_4.Text = lblRFSex.Text.Trim().ToUpper(); }
                                    if (LBLD_4.Text.Trim() == "") { LBLD_4.Text = lblSex.Text.Trim().ToUpper(); }
                                }
                                catch (Exception) { }
                                try
                                {
                                    if ((lblRFDocNumber.Text != null) && (lblRFDocNumber.Text != "")) { LBLD_5.Text = lblRFDocNumber.Text.Trim().ToUpper(); }
                                    if (LBLD_5.Text.Trim() == "") { LBLD_5.Text = lblDocumentNumber.Text.Trim().ToUpper(); }
                                }
                                catch (Exception) { }
                                int WIDEROK = 0;
                                try
                                {
                                    if (lblRFSurname.Text.Trim().ToUpper() != lblSurname.Text.Trim().ToUpper()) { WIDEROK = 1; }
                                    if (lblRFForenames.Text.Trim().ToUpper() != lblForenames.Text.Trim().ToUpper()) { WIDEROK = 1; }
                                    if (lblRFNationality.Text.Trim().ToUpper() != lblNationality.Text.Trim().ToUpper()) { WIDEROK = 1; }
                                    if (lblRFSex.Text.Trim().ToUpper() != lblSex.Text.Trim().ToUpper()) { WIDEROK = 1; }
                                    if (lblRFDateOfBirth.Text.Trim().ToUpper() != lblDateOfBirth.Text.Trim().ToUpper()) { WIDEROK = 1; }
                                    if (lblRFDocNumber.Text.Trim().ToUpper() != lblDocumentNumber.Text.Trim().ToUpper()) { WIDEROK = 1; }
                                    if (lblBACStatus.Text.Trim().ToUpper() != "TS_SUCCESS") { WIDEROK = 1; }
                                }
                                catch (Exception)
                                {
                                    WIDEROK = 1;
                                }
                                if (WIDEROK == 1) { T_Error.Visible = true; StatusCode = 2; } else { T_OK.Visible = true; StatusCode = 1; }
                                try
                                {
                                    if (WIDEROK == 1)
                                    {
                                        if ((lblRFSurname.Text.Trim() == "") && (lblSurname.Text.Trim() != "")) { StatusCode = 3; }
                                        if ((lblRFForenames.Text.Trim() == "") && (lblForenames.Text.Trim() != "")) { StatusCode = 3; }
                                        if ((lblRFNationality.Text.Trim() == "") && (lblNationality.Text.Trim() != "")) { StatusCode = 3; }
                                        if ((lblRFSex.Text.Trim() == "") && (lblSex.Text.Trim() != "")) { StatusCode = 3; }
                                        if ((lblRFDateOfBirth.Text.Trim() == "") && (lblDateOfBirth.Text.Trim() != "")) { StatusCode = 3; }
                                        if ((lblRFDocNumber.Text.Trim() == "") && (lblDocumentNumber.Text.Trim() != "")) { StatusCode = 3; }
                                    }
                                }
                                catch (Exception) { }
                                string EXODATE = "";
                                try
                                {
                                    if (lblRF_ExpireDate.Text.Trim() != "") { EXODATE = lblRF_ExpireDate.Text.Trim(); }
                                    if(EXODATE=="")
                                    {
                                        if (lblMRZ_ExpireDate.Text.Trim() != "") { EXODATE = lblMRZ_ExpireDate.Text.Trim(); }
                                    }
                                }
                                catch (Exception) { EXODATE = ""; }
                                if (StatusCode == 1) 
                                {
                                    if(EXODATE.Trim()!="")
                                    {
                                        // Hamid Said When Expires SHoulbe be in Refered .
                                        if (ExpiredDateCheck(EXODATE, Get_Date()) == false) { StatusCode = 3; WIDEROK = 1; }
                                    }
                                    else
                                    {
                                        StatusCode = 3; // No Expire Date => Reffered
                                        WIDEROK = 1;
                                    }
                                }
                                if (WIDEROK == 1) { T_Error.Visible = true; } else { T_OK.Visible = true; }
                                if (LBLD_1.Text.Trim() == "") { LBLD_1.Text = "NA"; }
                                if (LBLD_2.Text.Trim() == "") { LBLD_2.Text = "NA"; }
                                if (LBLD_3.Text.Trim() == "") { LBLD_3.Text = "NA"; }
                                if (LBLD_4.Text.Trim() == "") { LBLD_4.Text = "NA"; }
                                if (LBLD_5.Text.Trim() == "") { LBLD_5.Text = "NA"; }
                                if (lblDocumentType.Text.Trim().ToLower() == "unknown document") { lblDocumentType.Text = "Passport"; }
                                SendToServer(StatusCode);
                                PL_01_Splash.Visible = false;
                                PL_02_Splash.Visible = false;
                                PL_03_Splash.Visible = false;
                                PL_04_Splash.Visible = true;
                                Activity_Now = 5;
                                break;
                            }
                        //-------------------------------------------------------------------------------------------------------
                        case 4:
                            {
                                Wait(1000);
                                Activity_Now = 1;
                                File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Doc Removed" + "\r\n");
                                break;
                            }
                            //-------------------------------------------------------------------------------------------------------
                    }
                    Application.DoEvents();
                }
            }
            catch (Exception)
            { }
        }

        public string Get_Date()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
                string Txt = DateTime.Now.ToString("dd/MM/yyyy");
                return Txt.ToString().Trim();
            }
            catch
            {
                return "";
            }
        }

        public bool ExpiredDateCheck(string BaseDate, string MaxDate)
        {
            try
            {
                bool Res = false;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
                DateTime BDate = DateTime.Parse(BaseDate);
                DateTime MDate = DateTime.Parse(MaxDate);
                if (BDate.Date >= MDate.Date)
                {
                    Res = true;
                }
                return Res;
            }
            catch (Exception)
            {
                return false;
            }            
        }
        private void SendToServer(int StatusCode)
        {
            try
            {
                string BaseID = "0";
                string Document_ID = "1";
                if (lblDocumentType.Text.Trim().ToLower() != "passport") { Document_ID = "2"; } else { Document_ID = "1"; }
                File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Send To Server - Start" + "\r\n");
                if (LBLD_1.Text.Trim() == "") { StatusCode = 2; }
                if (LBLD_2.Text.Trim() == "") { StatusCode = 2; }
                BaseID = Scanner_HttpPost("BaseInfo", "UserCode=" + UserID + "&SDK_Code=1&Document_ID=" + Document_ID + "&FirstName=" + LBLD_1.Text.Trim() + "&LastName=" + LBLD_2.Text.Trim() + "&StatusCode=" + StatusCode.ToString() + "&DocNo=" + LBLD_5.Text.Trim());
                BaseID = BaseID.Replace("\"", "").Trim();
                if (BaseID != "ERR")
                {
                    File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Base ID = " + BaseID + "\r\n");
                    string R1 = Scanner_HttpPost("GA_01_MRZ_Data", "BaseID=" + BaseID + "&Document_Type=" + lblDocumentType.Text.Trim() + "&Forenames=" + lblForenames.Text.Trim() + "&Surname=" + lblSurname.Text.Trim() + "&Nationality=" + lblNationality.Text.Trim() + "&Sex=" + lblSex.Text.Trim() + "&Date_of_Birth=" + lblDateOfBirth.Text.Trim() + "&Document_Number=" + lblDocumentNumber.Text.Trim() + "&MRZ=" + richTextBoxCodeline.Text.Trim() + "&ExpireDate=" + lblMRZ_ExpireDate.Text.Trim());
                    File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "G1 = " + R1 + "\r\n");
                    string R2 = Scanner_HttpPost("GA_02_ChipData", "BaseID=" + BaseID + "&Forenames=" + lblRFForenames.Text.Trim() + "&Surname=" + lblRFSurname.Text.Trim() + "&Nationality=" + lblRFNationality.Text.Trim() + "&Sex=" + lblRFSex.Text.Trim() + "&Date_of_Birth=" + lblRFDateOfBirth.Text.Trim() + "&Document_Number=" + lblRFDocNumber.Text.Trim() + "&MRZ=" + lblRFCodeline.Text.Trim() + "&Air_Baud_Rate=" + lblAirBaudRate.Text.Trim() + "&Chip_ID=" + lblChipId.Text.Trim() + "&BAC=" + lblBACStatus.Text.Trim() + "&SAC=" + lblSACStatus.Text.Trim() + "&ExpireDate=" + lblRF_ExpireDate.Text.Trim());
                    File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "G2 = " + R2 + "\r\n");
                    string R3 = "";
                    foreach (ListViewItem anItem in validatedList.Items)
                    {
                        R3 = "";
                        R3 = Scanner_HttpPost("GA_03_Validate", "BaseID=" + BaseID + "&Item_Code=" + anItem.SubItems[0].Text.Trim() + "&Validate=" + anItem.SubItems[1].Text.Trim());
                        File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "G3 = " + anItem.SubItems[0].Text.Trim() + " -> " + R3 + "\r\n");
                    }
                    File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "G3 = " + R3 + "\r\n");
                    for (int i = 1; i < 6; i++)
                    {
                        switch (i)
                        {
                            case 1:
                                {
                                    if (photoImage.Image != null)
                                    {
                                        if (Directory.Exists(Application.StartupPath + @"\Photos") == false) { Directory.CreateDirectory(Application.StartupPath + @"\Photos"); }
                                        photoImage.Image.Save(Application.StartupPath + @"\Photos\FC.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                        var client = new RestClient(ServerAddress + "/api/GA_04_PhotoUploader?BaseID=" + BaseID);
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "multipart/form-data");
                                        request.AddFile("files[testfile1.pot]", Application.StartupPath + @"\Photos\FC.jpeg");
                                        IRestResponse response = client.Execute(request);
                                        if (File.Exists(Application.StartupPath + @"\Photos\FC.jpeg") == true) { File.Delete(Application.StartupPath + @"\Photos\FC.jpeg"); }
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    if (irImage.Image != null)
                                    {
                                        if (Directory.Exists(Application.StartupPath + @"\Photos") == false) { Directory.CreateDirectory(Application.StartupPath + @"\Photos"); }
                                        irImage.Image.Save(Application.StartupPath + @"\Photos\IR.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                        var client = new RestClient(ServerAddress + "/api/GA_04_PhotoUploader?BaseID=" + BaseID);
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "multipart/form-data");
                                        request.AddFile("files[testfile1.pot]", Application.StartupPath + @"\Photos\IR.jpeg");
                                        IRestResponse response = client.Execute(request);
                                        if (File.Exists(Application.StartupPath + @"\Photos\IR.jpeg") == true) { File.Delete(Application.StartupPath + @"\Photos\IR.jpeg"); }
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    if (uvImage.Image != null)
                                    {
                                        if (Directory.Exists(Application.StartupPath + @"\Photos") == false) { Directory.CreateDirectory(Application.StartupPath + @"\Photos"); }
                                        uvImage.Image.Save(Application.StartupPath + @"\Photos\UV.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                        var client = new RestClient(ServerAddress + "/api/GA_04_PhotoUploader?BaseID=" + BaseID);
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "multipart/form-data");
                                        request.AddFile("files[testfile1.pot]", Application.StartupPath + @"\Photos\UV.jpeg");
                                        IRestResponse response = client.Execute(request);
                                        if (File.Exists(Application.StartupPath + @"\Photos\UV.jpeg") == true) { File.Delete(Application.StartupPath + @"\Photos\UV.jpeg"); }
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    if (rfImage.Image != null)
                                    {
                                        if (Directory.Exists(Application.StartupPath + @"\Photos") == false) { Directory.CreateDirectory(Application.StartupPath + @"\Photos"); }
                                        rfImage.Image.Save(Application.StartupPath + @"\Photos\RF.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                        var client = new RestClient(ServerAddress + "/api/GA_04_PhotoUploader?BaseID=" + BaseID);
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "multipart/form-data");
                                        request.AddFile("files[testfile1.pot]", Application.StartupPath + @"\Photos\RF.jpeg");
                                        IRestResponse response = client.Execute(request);
                                        if (File.Exists(Application.StartupPath + @"\Photos\RF.jpeg") == true) { File.Delete(Application.StartupPath + @"\Photos\RF.jpeg"); }
                                    }
                                    break;
                                }
                            case 5:
                                {
                                    if (visibleImage.Image != null)
                                    {
                                        if (Directory.Exists(Application.StartupPath + @"\Photos") == false) { Directory.CreateDirectory(Application.StartupPath + @"\Photos"); }
                                        visibleImage.Image.Save(Application.StartupPath + @"\Photos\NM.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                        var client = new RestClient(ServerAddress + "/api/GA_04_PhotoUploader?BaseID=" + BaseID);
                                        client.Timeout = -1;
                                        var request = new RestRequest(Method.POST);
                                        request.AddHeader("Content-Type", "multipart/form-data");
                                        request.AddFile("files[testfile1.pot]", Application.StartupPath + @"\Photos\NM.jpeg");
                                        IRestResponse response = client.Execute(request);
                                        if (File.Exists(Application.StartupPath + @"\Photos\NM.jpeg") == true) { File.Delete(Application.StartupPath + @"\Photos\NM.jpeg"); }
                                    }
                                    break;
                                }
                        }
                    }
                    if (Directory.Exists(Application.StartupPath + @"\Photos") == true) { Directory.Delete(Application.StartupPath + @"\Photos", true); }
                }
                else { File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Send To Server - Error - BaseID" + "\r\n"); }
            }
            catch (Exception) { File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Send To Server - Error" + "\r\n"); }
            File.AppendAllText(FileLog, "- Scan Log : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Send To Server - End" + "\r\n");
        }

        public string Scanner_HttpPost(string API, string DValue)
        {
            string Res = "ERR";
            try
            {
                string SRVR = ServerAddress + "/";
                var client = new RestClient(SRVR + "api/" + API + "?" + DValue);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                IRestResponse response = client.Execute(request);
                Res = response.Content.ToString().Trim();
            }
            catch (Exception) { Res = "ERR"; }
            return Res;
        }

        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public void Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive)
            {
                Application.DoEvents();
            }

        }

        private void DataToSend_Reload()
        {
            MMM.Readers.FullPage.ReaderSettings lSettings;
            MMM.Readers.ErrorCode lErrorCode = MMM.Readers.FullPage.Reader.GetSettings(out lSettings);
            if (lErrorCode == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
            {
                if ((lSettings.puDataToSend.special & MMM.Readers.FullPage.DataSendSet.Flags.UVIMAGE) != 0) { lSettings.puCameraSettings.uv.puAmbientRemoval = MMM.Readers.Modules.Camera.AmbientRemovalMethod.MRAS_BASIC_AMBIENT_REMOVAL; }
                PopulateUHFLists(lSettings.puDataToSend.uhf);
                PopulateRFIDLists(lSettings.puDataToSend.rfid);
                PopulatePluginLists();
            }
        }

        private void PopulatePluginLists()
        {
            int lPluginNameLength = 1;
            for (int i = 0; (lPluginNameLength > 0); i++)
            {
                String lPluginName = "";
                MMM.Readers.ErrorCode lErrorCode =
                    MMM.Readers.FullPage.Reader.GetPluginName(ref lPluginName, i);

                lPluginNameLength = lPluginName.Length;

                if (lPluginNameLength > 0)
                {
                    bool lEnabled = false;

                    MMM.Readers.FullPage.Reader.IsPluginEnabled(lPluginName, ref lEnabled);
                    if (lEnabled)
                        SelectedPluginList.Items.Add(lPluginName);
                    else
                        AvailablePluginList.Items.Add(lPluginName);
                }
            }
        }

        private void StorePlugins()
        {
            foreach (String lPluginName in AvailablePluginList.Items)
                MMM.Readers.FullPage.Reader.EnablePlugin(lPluginName, false);

            foreach (String lPluginName in SelectedPluginList.Items)
                MMM.Readers.FullPage.Reader.EnablePlugin(lPluginName, true);
        }

        private void PopulateUHFLists(MMM.Readers.FullPage.UHFDataToSend aDataToSend)
        {
            ListBox lCurrentList = UHFAvailDataItems;
            for (int lOrder = 0; lOrder < 3; lOrder++)
            {
                if (lOrder > 0)
                    lCurrentList = UHFSelectDataItems;

                if (aDataToSend.puEPC == lOrder)
                    lCurrentList.Items.Add("EPC Tag");

                if (aDataToSend.puTagID == lOrder)
                    lCurrentList.Items.Add("Tag ID");
            }
        }

        private void StoreUHFOrder(ref MMM.Readers.FullPage.UHFDataToSend aDataToSend)
        {
            for (int lListCtrlIndex = 0; lListCtrlIndex < 2; lListCtrlIndex++)
            {
                ListBox lListbox = (lListCtrlIndex == 0) ? UHFAvailDataItems : UHFSelectDataItems;

                for (int lIndex = 0; lIndex < lListbox.Items.Count; lIndex++)
                {
                    if (!(lListbox.Items[lIndex] is String))
                        continue;

                    int lOrder = (lListCtrlIndex > 0) ? lIndex + 1 : 0;
                    String lItem = lListbox.Items[lIndex] as String;

                    if (lItem == "EPC Tag")
                        aDataToSend.puEPC = lOrder;

                    else if (lItem == "Tag ID")
                        aDataToSend.puTagID = lOrder;
                }
            }
        }

        private void PopulateRFIDLists(MMM.Readers.FullPage.RFIDDataToSend aDataToSend)
        {
            ListBox lCurrentList = RFIDAvailDataItems;
            for (int lOrder = 0; lOrder < 50; lOrder++)
            {
                if (lOrder > 0)
                    lCurrentList = RFIDSelectDataItems;

                if (aDataToSend.puEFComFile == lOrder)
                    lCurrentList.Items.Add("EF.COM file");

                if (aDataToSend.puEFSodFile == lOrder)
                    lCurrentList.Items.Add("EF.SOD file");

                if (aDataToSend.puAirBaudRate == lOrder)
                    lCurrentList.Items.Add("Air Baud Rate");

                if (aDataToSend.puActiveAuthentication == lOrder)
                    lCurrentList.Items.Add("Active Authentication");

                if (aDataToSend.puValidateDocSignerCert == lOrder)
                    lCurrentList.Items.Add("Validate D/S Cert.");

                if (aDataToSend.puChipID == lOrder)
                    lCurrentList.Items.Add("Chip ID");

                if (aDataToSend.puDG1MRZData == lOrder)
                    lCurrentList.Items.Add("DG1 MRZ");

                if (aDataToSend.puDG1DataEDL == lOrder)
                    lCurrentList.Items.Add("DG1 Data eDL");

                if (aDataToSend.puDG2FaceJPEG == lOrder)
                    lCurrentList.Items.Add("DG2 Photo");

                if (aDataToSend.puDG3Fingerprints == lOrder)
                    lCurrentList.Items.Add("DG3 Fingerprints");

                if (aDataToSend.puDG6FaceJPEG == lOrder)
                    lCurrentList.Items.Add("DG6 eDL Photo");

                if (aDataToSend.puDG7Fingerprints == lOrder)
                    lCurrentList.Items.Add("DG7 eDL Fingerprints");

                if (aDataToSend.puCrosscheckEFComEFSod == lOrder)
                    lCurrentList.Items.Add("Crosscheck EF.COM / EF.SOD");

                for (int lDataGroup = 0; lDataGroup < 16; lDataGroup++)
                {
                    if (aDataToSend.puDGFile[lDataGroup + 1] == lOrder)
                        lCurrentList.Items.Add(String.Format("DG{0} file", lDataGroup + 1));

                    if (aDataToSend.puValidateDG[lDataGroup + 1] == lOrder)
                        lCurrentList.Items.Add(String.Format("DG{0} validate", lDataGroup + 1));
                }

                for (int lDataGroup = 0; lDataGroup < 22; lDataGroup++)
                {
                    if (aDataToSend.puDGFileEID[lDataGroup + 1] == lOrder)
                        lCurrentList.Items.Add(String.Format("DG{0}_eID file", lDataGroup + 1));

                    if (aDataToSend.puValidateDGEID[lDataGroup + 1] == lOrder)
                        lCurrentList.Items.Add(String.Format("DG{0}_eID validate", lDataGroup + 1));
                }

                for (int lDataGroup = 0; lDataGroup < 14; lDataGroup++)
                {
                    if (aDataToSend.puDGFileEDL[lDataGroup + 1] == lOrder)
                        lCurrentList.Items.Add(String.Format("DG{0}_eDL file", lDataGroup + 1));

                    if (aDataToSend.puValidateDGEDL[lDataGroup + 1] == lOrder)
                        lCurrentList.Items.Add(String.Format("DG{0}_eDL validate", lDataGroup + 1));
                }

                if (aDataToSend.puValidateSignature == lOrder)
                    lCurrentList.Items.Add("Validate Signature");

                if (aDataToSend.puValidateSignedAttrs == lOrder)
                    lCurrentList.Items.Add("Validate Signed Attrs");

                if (aDataToSend.puGetBACStatus == lOrder)
                    lCurrentList.Items.Add("BAC Status");

                if (aDataToSend.puGetSACStatus == lOrder)
                    lCurrentList.Items.Add("SAC Status");
            }
        }

        private void StoreRFIDOrder(ref MMM.Readers.FullPage.RFIDDataToSend aDataToSend)
        {
            // To avoid having this switch twice, we'll just go around this for both list controls with a loop.
            // For the available list control, we want to set the order number to 0 for all items in it. For the
            // selected list control, we want to set the order number to the index in the list, plus 1.
            for (int lListCtrlIndex = 0; lListCtrlIndex < 2; lListCtrlIndex++)
            {
                ListBox lListbox = (lListCtrlIndex == 0) ? RFIDAvailDataItems : RFIDSelectDataItems;

                for (int lIndex = 0; lIndex < lListbox.Items.Count; lIndex++)
                {
                    if (!(lListbox.Items[lIndex] is String))
                        continue;

                    int lOrder = (lListCtrlIndex > 0) ? lIndex + 1 : 0;
                    String lItem = lListbox.Items[lIndex] as String;

                    if (lItem == "EF.COM file")
                        aDataToSend.puEFComFile = lOrder;

                    else if (lItem == "EF.SOD file")
                        aDataToSend.puEFSodFile = lOrder;

                    else if (lItem == "Air Baud Rate")
                        aDataToSend.puAirBaudRate = lOrder;

                    else if (lItem == "Active Authentication")
                        aDataToSend.puActiveAuthentication = lOrder;

                    else if (lItem == "Validate D/S Cert.")
                        aDataToSend.puValidateDocSignerCert = lOrder;

                    else if (lItem == "Chip ID")
                        aDataToSend.puChipID = lOrder;

                    else if (lItem == "DG1 MRZ")
                        aDataToSend.puDG1MRZData = lOrder;

                    else if (lItem == "DG1 Data eDL")
                        aDataToSend.puDG1DataEDL = lOrder;

                    else if (lItem == "DG2 Photo")
                        aDataToSend.puDG2FaceJPEG = lOrder;

                    else if (lItem == "DG3 Fingerprints")
                        aDataToSend.puDG3Fingerprints = lOrder;

                    else if (lItem == "DG6 eDL Photo")
                        aDataToSend.puDG6FaceJPEG = lOrder;

                    else if (lItem == "DG7 eDL Fingerprints")
                        aDataToSend.puDG7Fingerprints = lOrder;

                    else if (lItem == "Crosscheck EF.COM / EF.SOD")
                        aDataToSend.puCrosscheckEFComEFSod = lOrder;

                    else if (lItem == "Validate Signature")
                        aDataToSend.puValidateSignature = lOrder;

                    else if (lItem == "Validate Signed Attrs")
                        aDataToSend.puValidateSignedAttrs = lOrder;

                    else if (lItem == "BAC Status")
                        aDataToSend.puGetBACStatus = lOrder;

                    else if (lItem == "SAC Status")
                        aDataToSend.puGetSACStatus = lOrder;

                    else if (lItem.Contains("DG") && !(lItem.Contains("_eID") || lItem.Contains("_eDL")))
                    {
                        String lDGString = "";
                        foreach (Char lChar in lItem)
                        {
                            if (Char.IsNumber(lChar))
                                lDGString += lChar;
                        }
                        int lDataGroup = Int32.Parse(lDGString);

                        if (lItem.Contains("file") && lDataGroup <= aDataToSend.puDGFile.Length)
                            aDataToSend.puDGFile[lDataGroup] = lOrder;
                        else if (lItem.Contains("validate") && lDataGroup <= aDataToSend.puDGFile.Length)
                            aDataToSend.puValidateDG[lDataGroup] = lOrder;
                    }

                    else if (lItem.Contains("_eID"))
                    {
                        String lDGString = "";
                        foreach (Char lChar in lItem)
                        {
                            if (Char.IsNumber(lChar))
                                lDGString += lChar;
                        }
                        int lDataGroup = Int32.Parse(lDGString);

                        if (lItem.Contains("file") && lDataGroup <= aDataToSend.puDGFileEID.Length)
                            aDataToSend.puDGFileEID[lDataGroup] = lOrder;
                        else if (lItem.Contains("validate") && lDataGroup <= aDataToSend.puDGFileEID.Length)
                            aDataToSend.puValidateDGEID[lDataGroup] = lOrder;
                    }
                    else if (lItem.Contains("_eDL"))
                    {
                        String lDGString = "";
                        foreach (Char lChar in lItem)
                        {
                            if (Char.IsNumber(lChar))
                                lDGString += lChar;
                        }
                        int lDataGroup = Int32.Parse(lDGString);

                        if (lItem.Contains("file") && lDataGroup <= aDataToSend.puDGFileEDL.Length)
                            aDataToSend.puDGFileEDL[lDataGroup] = lOrder;
                        else if (lItem.Contains("validate") && lDataGroup <= aDataToSend.puDGFileEDL.Length)
                            aDataToSend.puValidateDGEDL[lDataGroup] = lOrder;
                    }
                }
            }
        }

        private void RFIDSelectAll()
        {
            Object[] lRawItems = new Object[RFIDAvailDataItems.Items.Count];
            RFIDAvailDataItems.Items.CopyTo(lRawItems, 0);
            RFIDAvailDataItems.Items.Clear();
            foreach (Object lRawItem in lRawItems) { if (lRawItem is String) { RFIDSelectDataItems.Items.Add(lRawItem); } }
        }

        void DataCallbackThreadHelper(MMM.Readers.FullPage.DataType aDataType, object aData)
        {
            if (THC.InvokeRequired)
            {
                THC.Invoke(
                    new MMM.Readers.FullPage.DataDelegate(DataCallback),
                    new object[] { aDataType, aData }
                );
            }
            else
            {
                DataCallback(aDataType, aData);
            }
        }

        void HighlightCodelineCheckDigits(MMM.Readers.CodelineData aCodeline)
        {
            for (int loop = 0; loop < aCodeline.CheckDigitDataListCount; loop++)
            {
                MMM.Readers.CodelineCheckDigitData lCDData = aCodeline.CheckDigitDataList[loop];
                int lIndex = lCDData.puCodelinePos;
                for (int line = 1; line < lCDData.puCodelineNumber; line++)
                {
                    switch (line)
                    {
                        case 1:
                            lIndex += aCodeline.Line1.Length;
                            ++lIndex;
                            break;
                        case 2:
                            lIndex += aCodeline.Line2.Length;
                            ++lIndex;
                            break;
                    }
                }
                richTextBoxCodeline.Select(lIndex, 1);
                if (lCDData.puValueExpected == lCDData.puValueRead)
                    richTextBoxCodeline.SelectionColor = Color.Green;
                else
                    richTextBoxCodeline.SelectionColor = Color.Red;
                richTextBoxCodeline.DeselectAll();
            }
        }

        void DataCallback(MMM.Readers.FullPage.DataType aDataType, object aData)
        {
            try
            {
                LogDataItem(aDataType, aData);
                if (aData != null)
                {
                    switch (aDataType)
                    {
                        case MMM.Readers.FullPage.DataType.CD_CODELINE_DATA:
                            {
                                MMM.Readers.CodelineData codeline = (MMM.Readers.CodelineData)aData;
                                richTextBoxCodeline.Text = codeline.Line1 + "\n" + codeline.Line2 + "\n" + codeline.Line3;
                                HighlightCodelineCheckDigits(codeline);
                                lblSurname.Text = codeline.Surname;
                                lblForenames.Text = codeline.Forenames;
                                lblNationality.Text = codeline.Nationality;
                                lblSex.Text = codeline.Sex;
                                lblDateOfBirth.Text = codeline.DateOfBirth.Day.ToString("00") + "/" + codeline.DateOfBirth.Month.ToString("00") + "/" + codeline.DateOfBirth.Year.ToString("00");
                                lblDocumentNumber.Text = codeline.DocNumber;
                                lblDocumentType.Text = codeline.DocType;
                                lblMRZ_ExpireDate.Text = codeline.ExpiryDate.Day.ToString("00") + "/" + codeline.ExpiryDate.Month.ToString("00") + "/" + "20" + codeline.ExpiryDate.Year.ToString("00");
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEIR:
                            {
                                irImage.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEVIS:
                            {
                                visibleImage.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEPHOTO:
                            {
                                photoImage.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEUV:
                            {
                                uvImage.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEIRREAR:
                            {
                                //irImageRear.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEVISREAR:
                            {
                                //visibleImageRear.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_IMAGEUVREAR:
                            {
                                //uvImageRear.Image = aData as Bitmap;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCAIRBAUD:
                            {
                                lblAirBaudRate.Text = aData as string;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_BACKEY_CORRECTION:
                            {
                                bool lTemp = THC.InvokeRequired;
                                {
                                    System.Text.StringBuilder lStringBuilder = aData as System.Text.StringBuilder;
                                    if (lStringBuilder != null)
                                    {
                                        //FormBACKeyCorrection lForm = new FormBACKeyCorrection();
                                        //lForm.SetCodeline(lStringBuilder.ToString());
                                        //if (lForm.ShowDialog() == DialogResult.OK)
                                        //{
                                        //    lStringBuilder.Replace(
                                        //        lStringBuilder.ToString(),
                                        //        lForm.GetCodeline()
                                        //    );
                                        //}
                                    }
                                }
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_CODELINE_DATA:
                            {
                                MMM.Readers.CodelineData codeline = (MMM.Readers.CodelineData)aData;
                                lblRFCodeline.Text = codeline.Data;
                                lblRFSurname.Text = codeline.Surname;
                                lblRFForenames.Text = codeline.Forenames;
                                lblRFNationality.Text = codeline.Nationality;
                                lblRFSex.Text = codeline.Sex;
                                lblRFDateOfBirth.Text = codeline.DateOfBirth.Day.ToString("00") + "/" + codeline.DateOfBirth.Month.ToString("00") + "/" + codeline.DateOfBirth.Year.ToString("00");
                                lblRFDocNumber.Text = codeline.DocNumber;
                                lblRF_ExpireDate.Text = codeline.ExpiryDate.Day.ToString("00") + "/" + codeline.ExpiryDate.Month.ToString("00") + "/" + "20" + codeline.ExpiryDate.Year.ToString("00");
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_PHOTO:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_EDL_PHOTO:
                            {
                                byte[] lInputBuffer = aData as byte[];
                                try
                                {
                                    System.IO.Stream streamBuffer = new System.IO.MemoryStream();
                                    streamBuffer.Write(lInputBuffer, 0, lInputBuffer.Length);
                                    streamBuffer.Seek(0, System.IO.SeekOrigin.Begin);
                                    rfImage.Image = new Bitmap(streamBuffer);
                                }
                                catch (Exception imgExcept)
                                {
                                    byte[] lOutputBuffer = null;
                                    MMM.Readers.Modules.Imaging.ConvertFormat(MMM.Readers.FullPage.ImageFormats.RTE_BMP, lInputBuffer, out lOutputBuffer);
                                    if (lOutputBuffer != null)
                                    {
                                        try
                                        {
                                            System.IO.Stream j2kStream = new System.IO.MemoryStream();
                                            j2kStream.Write(lOutputBuffer, 0, lOutputBuffer.Length);
                                            j2kStream.Seek(0, System.IO.SeekOrigin.Begin);
                                            rfImage.Image = new Bitmap(j2kStream);
                                        }
                                        catch (Exception decodeExcept)
                                        {
                                            //System.Windows.Forms.MessageBox.Show(decodeExcept.ToString());
                                        }
                                    }
                                }
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCCHIPID:
                            {
                                lblChipId.Text = aData as string;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG15_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG16_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG15_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG16_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG17_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG18_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG19_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG20_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG21_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG22_VALIDATE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_VALIDATE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCSIGNEDATTRS_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCSIGNEDATTRS_VALIDATE_CARD_SECURITY_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCSIGNEDATTRS_VALIDATE_CHIP_SECURITY_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCSIGNATURE_VALIDATE:
                        case MMM.Readers.FullPage.DataType.CD_SCSIGNATURE_VALIDATE_CARD_SECURITY_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCSIGNATURE_VALIDATE_CHIP_SECURITY_FILE:
                        case MMM.Readers.FullPage.DataType.CD_VALIDATE_DOC_SIGNER_CERT:
                            {
                                ListViewItem thisItem = validatedList.Items.Add(aDataType.ToString());

                                MMM.Readers.Modules.RF.ValidationCode lValidationResult = (MMM.Readers.Modules.RF.ValidationCode)aData;
                                thisItem.SubItems.Add(lValidationResult.ToString());
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_ACTIVE_AUTHENTICATION:
                        case MMM.Readers.FullPage.DataType.CD_SCBAC_STATUS:
                        case MMM.Readers.FullPage.DataType.CD_SAC_STATUS:
                        case MMM.Readers.FullPage.DataType.CD_SCTERMINAL_AUTHENTICATION_STATUS:
                        case MMM.Readers.FullPage.DataType.CD_SCCHIP_AUTHENTICATION_STATUS:
                            {
                                ListViewItem thisItem = validatedList.Items.Add(aDataType.ToString());

                                MMM.Readers.Modules.RF.TriState lState = (MMM.Readers.Modules.RF.TriState)aData;
                                thisItem.SubItems.Add(lState.ToString());
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG15_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG16_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG15_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG16_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG17_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG18_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG19_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG20_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG21_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG22_FILE_EID:
                        case MMM.Readers.FullPage.DataType.CD_SCEF_COM_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCEF_SOD_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCEF_CVCA_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCEF_CARD_SECURITY_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCEF_CHIP_SECURITY_FILE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_FILE_EDL:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_FILE_EDL:
                            {
                                byte[] lFileData = aData as byte[];

                                ListViewItem thisItem = dataFileList.Items.Add(aDataType.ToString());
                                thisItem.SubItems.Add(lFileData.Length.ToString() + " bytes");
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_UHF_EPC:
                            {
                                String lEPCString = "";
                                foreach (byte lByte in (byte[])aData)
                                {
                                    if (lEPCString.Length != 0)
                                        lEPCString += "-";
                                    lEPCString += lByte.ToString("X4");
                                }
                                //EPCField.Text = lEPCString;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_UHF_TAGID:
                            {
                                if (aData is MMM.Readers.FullPage.UHFTagIDData)
                                {
                                    //DisplayUHFTagID(
                                    //    (MMM.Readers.FullPage.UHFTagIDData)aData);
                                }
                                break;
                            }
                        #region eID
                        // eID related data for reference
                        case MMM.Readers.FullPage.DataType.CD_SCDG2_EID_ISSUING_ENTITY:
                            {
                                MMM.Readers.FullPage.EIDASIssuingEntity data = (MMM.Readers.FullPage.EIDASIssuingEntity)aData;
                                int x = 0;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG3_EID_VALIDITY_PERIOD:
                            {
                                MMM.Readers.FullPage.EIDASValidityPeriod data = (MMM.Readers.FullPage.EIDASValidityPeriod)aData;
                                int x = 0;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG9_EID_PLACE_OF_BIRTH:
                            {
                                MMM.Readers.FullPage.EIDASGeneralPlace data = (MMM.Readers.FullPage.EIDASGeneralPlace)aData;
                                int x = 0;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG17_EID_PLACE_OF_RESIDENCE:
                            {
                                MMM.Readers.FullPage.EIDASPlaceOfResidence data = (MMM.Readers.FullPage.EIDASPlaceOfResidence)aData;
                                int x = 0;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG12_EID_OPTIONAL_DATA_R:
                        case MMM.Readers.FullPage.DataType.CD_SCDG14_EID_WRITTEN_SIGNATURE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG18_EID_MUNICIPALITY_ID:
                        case MMM.Readers.FullPage.DataType.CD_SCDG21_EID_OPTIONAL_DATA_RW:
                            {
                                byte[] data = (byte[])aData;
                                int x = 0;
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_EID_DOCUMENT_TYPE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG4_EID_GIVEN_NAMES:
                        case MMM.Readers.FullPage.DataType.CD_SCDG5_EID_FAMILY_NAMES:
                        case MMM.Readers.FullPage.DataType.CD_SCDG6_EID_NOM_DE_PLUME:
                        case MMM.Readers.FullPage.DataType.CD_SCDG7_EID_ACADEMIC_TITLE:
                        case MMM.Readers.FullPage.DataType.CD_SCDG8_EID_DATE_OF_BIRTH:
                        case MMM.Readers.FullPage.DataType.CD_SCDG10_EID_NATIONALITY:
                        case MMM.Readers.FullPage.DataType.CD_SCDG11_EID_SEX:
                        case MMM.Readers.FullPage.DataType.CD_SCDG13_EID_BIRTH_NAME:
                        case MMM.Readers.FullPage.DataType.CD_SCDG19_EID_RESIDENCE_PERMIT_1:
                        case MMM.Readers.FullPage.DataType.CD_SCDG20_EID_RESIDENCE_PERMIT_2:
                            {
                                string test = aData.ToString();
                                break;
                            }
                        #endregion
                        case MMM.Readers.FullPage.DataType.CD_SCDG1_EDL_DATA:
                            {
                                MMM.Readers.FullPage.EDLDataGroup1Data data = (MMM.Readers.FullPage.EDLDataGroup1Data)aData;
                                break;
                            }
                    }
                    switch (aDataType)
                    {
                        case MMM.Readers.FullPage.DataType.CD_SCBAC_STATUS:
                            {
                                MMM.Readers.Modules.RF.TriState lState = (MMM.Readers.Modules.RF.TriState)aData;
                                this.lblBACStatus.Text = lState.ToString();
                                break;
                            }
                        case MMM.Readers.FullPage.DataType.CD_SAC_STATUS:
                            {
                                MMM.Readers.Modules.RF.TriState lState = (MMM.Readers.Modules.RF.TriState)aData;
                                this.lblSACStatus.Text = lState.ToString();
                                break;
                            }
                    }
                }
            }
            catch (Exception e)
            {
                Lbl_ErrorPublic.Text = e.ToString();
            }
        }

        void EventCallbackThreadHelper(MMM.Readers.FullPage.EventCode aEventType)
        {
            if (THC.InvokeRequired)
            {
                THC.Invoke(
                    new MMM.Readers.FullPage.EventDelegate(EventCallback),
                    new object[] { aEventType }
                );
            }
            else
            {
                EventCallback(aEventType);
            }
        }

        void EventCallback(MMM.Readers.FullPage.EventCode aEventType)
        {
            try
            {
                LogEvent(aEventType);
                switch (aEventType)
                {
                    case MMM.Readers.FullPage.EventCode.SETTINGS_INITIALISED:
                        {
                            MMM.Readers.FullPage.ReaderSettings settings;
                            MMM.Readers.ErrorCode errorCode = MMM.Readers.FullPage.Reader.GetSettings(out settings);
                            if (errorCode == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED)
                            {
                                settings.puDataToSend.send |= MMM.Readers.FullPage.DataSendSet.Flags.DOCMARKERS;
                                settings.puDataToSend.special = MMM.Readers.FullPage.DataSendSet.Flags.VISIBLEIMAGE | MMM.Readers.FullPage.DataSendSet.Flags.IRIMAGE;
                                MMM.Readers.FullPage.Reader.UpdateSettings(settings);
                                MMM.Readers.FullPage.Reader.EnableLogging(true, settings.puLoggingSettings.logLevel, (int)settings.puLoggingSettings.logMask, "HLNonBlockingExample.Net.log");
                                Cam_Init_1 = true;
                            }
                            else
                            {
                                Lbl_ErrorPublic.Text = "GetSettings failure, check for Settings structure mis-match. Error:" + errorCode.ToString();
                                Init_Activity_Complete = true;
                            }
                            break;
                        }
                    case MMM.Readers.FullPage.EventCode.DEVICE_CONNECTED:
                        {
                            Cam_Init_2 = true;
                            break;
                        }
                    case MMM.Readers.FullPage.EventCode.PLUGINS_INITIALISED:
                        {
                            int lIndex = 0;
                            string lPluginName = "";
                            while (MMM.Readers.FullPage.Reader.GetPluginName(ref lPluginName, lIndex) == MMM.Readers.ErrorCode.NO_ERROR_OCCURRED && lPluginName.Length > 0)
                            {
                                ++lIndex;
                            }
                            Cam_Init_3 = true;
                            break;
                        }
                    case MMM.Readers.FullPage.EventCode.DOC_REMOVED:
                        {
                            if (SystemRunNew == true)
                            {
                                if (MMM.Readers.FullPage.Reader.GetState() != MMM.Readers.FullPage.ReaderState.READER_DISABLED)
                                {
                                    MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                                }
                                Activity_Now = 4;
                            }
                            break;
                        }
                    case MMM.Readers.FullPage.EventCode.DOC_ON_WINDOW:
                        {
                            if (FirstLoad == true)
                            {
                                if (MMM.Readers.FullPage.Reader.GetState() != MMM.Readers.FullPage.ReaderState.READER_DISABLED)
                                {
                                    MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_DISABLED, true);
                                    return;
                                }
                            }
                            if (SystemRunNew == true)
                            {
                                Activity_Now = 2;
                            }
                            Clear();
                            break;
                        }
                    case MMM.Readers.FullPage.EventCode.READING_DATA:
                        {

                            break;
                        }
                    case MMM.Readers.FullPage.EventCode.END_OF_DOCUMENT_DATA:
                        {
                            if (SystemRunNew == true)
                            {
                                Activity_Now = 3;
                            }
                            break;
                        }
                }
                UpdateState(MMM.Readers.FullPage.Reader.GetState());
            }
            catch (Exception e)
            {
                LogError(0, e.Message);
            }
        }

        void Clear()
        {
            richTextBoxCodeline.Text = "";
            lblSurname.Text = "";
            lblForenames.Text = "";
            lblNationality.Text = "";
            lblSex.Text = "";
            lblDateOfBirth.Text = "";
            lblDocumentNumber.Text = "";
            lblDocumentType.Text = "";
            irImage.Image = null;
            visibleImage.Image = null;
            photoImage.Image = null;
            uvImage.Image = null;
            lblRFCodeline.Text = "";
            lblRFSurname.Text = "";
            lblRFForenames.Text = "";
            lblRFNationality.Text = "";
            lblRFSex.Text = "";
            lblRFDateOfBirth.Text = "";
            lblRFDocNumber.Text = "";
            rfImage.Image = null;
            validatedList.Items.Clear();
            dataFileList.Items.Clear();
            lblChipId.Text = "";
            lblAirBaudRate.Text = "";
            lblBACStatus.Text = "";
            lblSACStatus.Text = "";
            lblMRZ_ExpireDate.Text = "";
            lblRF_ExpireDate.Text = "";
        }

        void ErrorCallbackThreadHelper(MMM.Readers.ErrorCode aErrorCode, string aErrorMessage)
        {
            if (THC.InvokeRequired)
            {
                THC.Invoke(
                    new MMM.Readers.ErrorDelegate(ErrorCallback),
                    new object[] { aErrorCode, aErrorMessage }
                );
            }
            else
            {
                ErrorCallback(aErrorCode, aErrorMessage);
            }
        }

        void ErrorCallback(MMM.Readers.ErrorCode aErrorCode, string aErrorMessage)
        {
            switch (aErrorCode)
            {
                case MMM.Readers.ErrorCode.ERROR_READER_NOT_CONNECTED:
                    {
                        Cam_Init_1 = false;
                        Cam_Init_2 = false;
                        Cam_Init_3 = false;
                        Init_Activity_Complete = true;
                        break;
                    }
            }
            if (aErrorMessage.IndexOf("Datagroup") < 0) { Lbl_ErrorPublic.Text = aErrorMessage.ToString(); }
            LogError(aErrorCode, aErrorMessage);
        }

        void WarningCallbackThreadHelper(MMM.Readers.WarningCode aWarningCode, string aWarningMessage)
        {
            if (THC.InvokeRequired)
            {
                THC.Invoke(
                    new MMM.Readers.WarningDelegate(WarningCallback),
                    new object[] { aWarningCode, aWarningMessage }
                );
            }
            else
            {
                WarningCallback(aWarningCode, aWarningMessage);
            }
        }

        void WarningCallback(MMM.Readers.WarningCode aWarningCode, string aWarningMessage)
        {
            LogWarning(aWarningCode, aWarningMessage);
        }

        bool CertificateCallbackThreadHelper(byte[] aCertIdentifier, MMM.Readers.Modules.RF.CertType aCertType, out byte[] aCertBuffer)
        {
            if (THC.InvokeRequired)
            {
                aCertBuffer = null;
                object[] lParams = new object[]
                {
                    aCertIdentifier, aCertType, aCertBuffer
                };
                bool lResult = (bool)THC.Invoke(new MMM.Readers.FullPage.CertificateDelegate(CertificateCallback), lParams);
                aCertBuffer = (byte[])lParams[2];
                return lResult;
            }
            else
            {
                return CertificateCallback(aCertIdentifier, aCertType, out aCertBuffer);
            }
        }

        bool CertificateCallback(byte[] aCertIdentifier, MMM.Readers.Modules.RF.CertType aCertType, out byte[] aCertBuffer)
        {
            bool lSuccess = false;
            OpenFileDialog fileSelector = new OpenFileDialog();
            aCertBuffer = null;
            fileSelector.Title = "Open external certificate: " + aCertType.ToString();
            fileSelector.InitialDirectory = "c:\\certs\\";
            fileSelector.Filter = "Certs and keys|*.cer;*.der;*.cvcert;*.pkcs8;*.bin|All files (*.*)|*.*";
            fileSelector.FilterIndex = 1;
            fileSelector.RestoreDirectory = true;
            if (fileSelector.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    System.IO.Stream fs = null;
                    if ((fs = fileSelector.OpenFile()) != null)
                    {
                        aCertBuffer = new byte[fs.Length];
                        fs.Read(aCertBuffer, 0, aCertBuffer.Length);
                        fs.Close();
                        lSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    Lbl_ErrorPublic.Text = "Error: Could not read file from disk. Original error: " + ex.Message;
                }
            }
            return lSuccess;
        }

        void LogDataItem(MMM.Readers.FullPage.DataType aDataType, object aData)
        {
            if (aDataType == MMM.Readers.FullPage.DataType.CD_SWIPE_MSR_DATA)
            {
                MMM.Readers.Modules.Swipe.MsrData msrData = (MMM.Readers.Modules.Swipe.MsrData)aData;
                LogDataItem("MSR_TRACK_1", msrData.Track1);
                LogDataItem("MSR_TRACK_2", msrData.Track2);
                LogDataItem("MSR_TRACK_3", msrData.Track3);
            }
            else if (aDataType == MMM.Readers.FullPage.DataType.CD_AAMVA_DATA)
            {
                MMM.Readers.AAMVAData aamvaData = (MMM.Readers.AAMVAData)aData;
                LogDataItem("AAMVA_FULL_NAME", aamvaData.Parsed.FullName);
                LogDataItem("AAMVA_LICENCE_NUMBER", aamvaData.Parsed.LicenceNumber);
            }
            else if (aDataType > MMM.Readers.FullPage.DataType.CD_PLUGIN)
            {
                MMM.Readers.FullPage.PluginData pluginData = (MMM.Readers.FullPage.PluginData)aData;
                ListViewItem thisItem = dataFileList.Items.Add(pluginData.puDataFormat.ToString());
                string lInfo = pluginData.puFeatureName + " " + pluginData.puFieldName + ": ";
                if (pluginData.puData is string)
                    LogDataItem(aDataType.ToString(), lInfo + (string)pluginData.puData);
                else if (pluginData.puData is byte[])
                    LogDataItem(aDataType.ToString(), lInfo + ((byte[])pluginData.puData).Length + " bytes");
                else
                    LogDataItem(aDataType.ToString(), lInfo + aData);
            }
            else
            {
                LogDataItem(aDataType.ToString(), aData);
            }
        }

        void LogDataItem(string aDataType, object aData)
        {
            File.AppendAllText(FileLog, "- LogDataItem : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + aData.ToString() + "\r\n");
            textBox1.Text += "- LogDataItem : " + aData.ToString() + "\r\n";
        }

        void LogWarning(MMM.Readers.WarningCode aWarningCode, string aWarningMessage)
        {
            File.AppendAllText(FileLog, "- LogWarning : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + aWarningMessage.ToString() + "\r\n");
            textBox1.Text += "- LogWarning : " + aWarningMessage.ToString() + "\r\n";
        }

        void LogError(MMM.Readers.ErrorCode aErrorCode, string aErrorMessage)
        {
            File.AppendAllText(FileLog, "- LogError : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + aErrorMessage.ToString() + "\r\n");
            textBox1.Text += "- LogError : " + aErrorMessage.ToString() + "\r\n";
        }

        void LogEvent(MMM.Readers.FullPage.EventCode aEventType)
        {
            File.AppendAllText(FileLog, "- LogEvent : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + aEventType.ToString() + "\r\n");
            textBox1.Text += "- LogEvent : " + aEventType.ToString() + "\r\n";
        }

        void UpdateState(MMM.Readers.FullPage.ReaderState state)
        {
            File.AppendAllText(FileLog, "- UpdateState : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + state.ToString() + "\r\n");
            textBox1.Text += "- UpdateState : " + state.ToString() + "\r\n";
        }

        private MMM.Readers.FullPage.ReaderState prPreviousState = MMM.Readers.FullPage.ReaderState.READER_DISABLED;
        private void OnPowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Resume:
                    {
                        File.AppendAllText(FileLog, "- PowerModeChanged : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Resume" + "\r\n");
                        System.Threading.Thread.Sleep(5000);
                        MMM.Readers.FullPage.Reader.SetState(prPreviousState, true);
                        UpdateState(prPreviousState);
                        break;
                    }
                case Microsoft.Win32.PowerModes.Suspend:
                    {
                        File.AppendAllText(FileLog, "- PowerModeChanged : " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm") + " - " + "Suspend" + "\r\n");
                        MMM.Readers.FullPage.ReaderState lCurrentState = MMM.Readers.FullPage.Reader.GetState();
                        prPreviousState = lCurrentState;
                        if ((lCurrentState != MMM.Readers.FullPage.ReaderState.READER_NOT_INITIALISED) && (lCurrentState != MMM.Readers.FullPage.ReaderState.READER_ERRORED) && (lCurrentState != MMM.Readers.FullPage.ReaderState.READER_TERMINATED) && (lCurrentState != MMM.Readers.FullPage.ReaderState.READER_SUSPENDED))
                        {
                            MMM.Readers.FullPage.Reader.SetState(MMM.Readers.FullPage.ReaderState.READER_SUSPENDED, true);
                            UpdateState(MMM.Readers.FullPage.ReaderState.READER_SUSPENDED);
                            do
                            {
                                System.Threading.Thread.Sleep(10);
                                lCurrentState = MMM.Readers.FullPage.Reader.GetState();
                            }
                            while (lCurrentState == prPreviousState);
                        }
                    }
                    break;
            }
        }

        private void Splash_Btn_Retry_Click(object sender, EventArgs e)
        {
            Lbl_ErrorPublic.Text = "";
            Splash_Btn_Exit.Visible = false;
            Splash_Btn_Retry.Visible = false;
            Splash_Lbl_1.Text = "Initial System Startup ...";
            Splash_Lbl_2.Text = "";
            Application.DoEvents();
            timer2.Interval = 1000;
            timer2.Start();
        }

        private void Splash_Btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void IDV_Scanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MMM.Readers.FullPage.Reader.Shutdown();
                File.AppendAllText(FileLog, "-------------------------------------------------------------------------" + "\r\n");
                File.AppendAllText(FileLog, "- Close Software -> " + DateTime.Now.ToString("dd/MM/yyyy - HH:mm" + "\r\n"));
                File.AppendAllText(FileLog, "-------------------------------------------------------------------------" + "\r\n");
            }
            catch (System.DllNotFoundException)
            { }
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Activity_Now = 1;
        }
    }
}
