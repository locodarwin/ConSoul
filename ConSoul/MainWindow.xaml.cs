using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AW;



namespace ConSoul
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Timer, BG worker, instance definitions
        public IInstance _instance;
        public DispatcherTimer aTimer;
        BackgroundWorker m_Login;

        public struct Coords
        {
            public int x;
            public int y;
            public int z;
            public int yaw;
        };



        public MainWindow()
        {
            InitializeComponent();
            //this.Text = Globals.sAppName + " " + Globals.sVersion;

            LoadINI();


            Status("Ready to log into universe.");

            textBotname.Text = Globals.sBotName;
            textCitnum.Text = Convert.ToString(Globals.iCitNum);
            textPrivPass.Text = Globals.sPassword;
            textWorld.Text = Globals.sWorld;
            textCoords.Text = Globals.sCoords;
            textAvatar.Text = Convert.ToString(Globals.iAV);

            toolStatus.Background = Brushes.Salmon;
            butLogOut.IsEnabled = false;


            // The AW message queue timer
            aTimer = new DispatcherTimer();
            aTimer.Tick += new EventHandler(aTimer_Tick);
            aTimer.Interval = new TimeSpan(20);
            aTimer.Start();

            // Background tasking definitions for the universe & world login
            m_Login = new BackgroundWorker();
            m_Login.DoWork += new DoWorkEventHandler(m_LoginDoWork);
            m_Login.ProgressChanged += new ProgressChangedEventHandler(m_LoginProgress);
            m_Login.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_LoginCompleted);
            m_Login.WorkerReportsProgress = true;



        }



        public static class Globals
        {
            // Application parameters
            public static string sAppName = "ConSoul";
            public static string sVersion = "v1.0";
            public static string sByline = "Copyright © 2017 by Locodarwin";
            public static string sINI = "ConSoul.ini";

            // Login and positioning
            public static string sUnivLogin, sBotName, sPassword, sWorld, sCoords;
            public static int iPort, iCitNum, iXPos, iYPos, iZPos, iYaw, iAV;
            public static bool iCaretaker;

            // Permissions
            public static List<string> lAdminCmds, lOwners, lAdmins;

            // Status and logging
            public static string sLogfile;

            // Universe/world login states
            public static bool iInUniv = false;
            public static bool iInWorld = false;

            // Status markers
            //public static Logging LogStat = Logging.Status;

            // Command prefix
            public static string sComPrefix;

        }



        private void butLogIn_Click(object sender, RoutedEventArgs e)
        {
            // Grab the contents of the controls and put them into the globals
            Globals.sBotName = textBotname.Text;
            Globals.iCitNum = Convert.ToInt32(textCitnum.Text);
            Globals.sPassword = textPrivPass.Text;
            Globals.sWorld = textWorld.Text;
            Globals.sCoords = textCoords.Text;
            Coords coords = ConvertCoords(Globals.sCoords);
            Globals.iXPos = coords.x;
            Globals.iYPos = coords.y;
            Globals.iZPos = coords.z;
            Globals.iYaw = coords.yaw;
            Globals.iAV = Convert.ToInt32(textAvatar.Text);

            toolStatus.Background = Brushes.LightYellow;
            toolStatus.Content = "Logging In";
            butLogIn.IsEnabled = false;
            butConfig.IsEnabled = false;

            m_Login.RunWorkerAsync();


        }


        private void butLogOut_Click(object sender, RoutedEventArgs e)
        {
            if (Globals.iInUniv == false)
            {
                Status("Not in universe. Aborted.");
                return;
            }

            _instance.Dispose();
            Status("Logged out.");
            toolStatus.Background = Brushes.Salmon;
            toolStatus.Content = "Logged Out";
            butLogIn.IsEnabled = true;
            butConfig.IsEnabled = true;
            butLogOut.IsEnabled = false;

            Globals.iInUniv = false;
            Globals.iInWorld = false;
        }



        private void butConfig_Click(object sender, EventArgs e)
        {
            //System.Diagnostics.Process.Start(@"notepad.exe " + Globals.sINI);
            string appPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var p = new System.Diagnostics.Process();
            //p.StartInfo.FileName = "notepad.exe " + appPath + "\\" + Globals.sINI;
            p.StartInfo.FileName = appPath + "\\" + Globals.sINI;
            p.Start();
        }


        private void butChange_Click(object sender, RoutedEventArgs e)
        {
            Globals.iAV = Convert.ToInt32(textAvatar.Text);
            Status("Command issued: Change AV to " + Globals.iAV);
            _instance.Attributes.MyType = Globals.iAV;
            _instance.StateChange();
        }


        private void m_LoginDoWork(object sender, DoWorkEventArgs e)
        {

            // Check universe login state and abort if we're already logged in
            if (Globals.iInUniv == true)
            {
                m_Login.ReportProgress(0, "Already logged into universe!");
                return;
            }

            // Initalize the AW API?
            m_Login.ReportProgress(0, "Initializing the API instance.");
            _instance = new Instance();

            // install the event handlers
            m_Login.ReportProgress(0, "Installing events and callbacks.");
            //_instance.EventAvatarAdd += OnEventAvatarAdd;
            //_instance.EventAvatarDelete += OnEventAvatarDelete;
            _instance.EventChat += OnEventChat;


            // Set universe login parameters
            _instance.Attributes.LoginName = Globals.sBotName;
            _instance.Attributes.LoginOwner = Globals.iCitNum;
            _instance.Attributes.LoginPrivilegePassword = Globals.sPassword;
            //_instance.Attributes.LoginApplication = Globals.sBotDesc;

            // Log into universe
            //m_Login.ReportProgress(0, "Entering universe.");
            var rc = _instance.Login();
            if (rc != Result.Success)
            {
                m_Login.ReportProgress(0, "Unable to log in to universe (reason:" + rc + ").");
                return;
            }
            else
            {
                m_Login.ReportProgress(0, "Universe entry successful.");
                Globals.iInUniv = true;
            }

            // Enter world

            // Prepare for Caretaker mode if the option has been enabled
            if (Globals.iCaretaker == true)
            {
                _instance.Attributes.EnterGlobal = true;
                m_Login.ReportProgress(0, "Caretaker mode requested.");
            }

            //m_Login.ReportProgress(0, "Logging into world " + Globals.sWorld + ".");
            rc = _instance.Enter(Globals.sWorld);
            if (rc != Result.Success)
            {
                m_Login.ReportProgress(0, "Failed to log into world" + Globals.sWorld + " (reason:" + rc + ").");
                _instance.Dispose();
                Globals.iInUniv = false;
                return;
            }
            else
            {
                m_Login.ReportProgress(0, "Entered world " + Globals.sWorld + ".");
                Globals.iInWorld = true;

            }

            // Test caretaker mode (if requested)
            if (Globals.iCaretaker == true)
            {
                if (_instance.Attributes.WorldCaretakerCapability == true)
                {
                    m_Login.ReportProgress(0, "Caretaker mode confirmed.");
                }
                else
                {
                    m_Login.ReportProgress(0, "Caretaker mode denied.");
                }
            }

            // Commit the positioning and become visible
            m_Login.ReportProgress(0, "Changing position in world.");
            _instance.Attributes.MyX = Globals.iXPos;
            _instance.Attributes.MyY = Globals.iYPos;
            _instance.Attributes.MyZ = Globals.iZPos;
            _instance.Attributes.MyYaw = Globals.iYaw;
            _instance.Attributes.MyType = Globals.iAV;

            rc = _instance.StateChange();
            if (rc == Result.Success)
            {
                m_Login.ReportProgress(0, "Movement successful.");
            }
            else
            {
                m_Login.ReportProgress(0, "Movement aborted (reason: " + rc + ").");
            }
        }


        private void m_LoginProgress(object serder, ProgressChangedEventArgs e)
        {
            Status(e.UserState.ToString());
        }

        private void m_LoginCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Status("Error while performing background operation!");
            }

            if (Globals.iInWorld == true)
            {
                toolStatus.Background = Brushes.LightGreen;
                //toolLoggedIn.Text = "Logged In";
                butLogOut.IsEnabled = true;
                Status("Logged in.");
            }
            else
            {
                toolStatus.Background = Brushes.Salmon;
                //toolLoggedIn.Text = "Logged Out";
                butLogIn.IsEnabled = true;
                butConfig.IsEnabled = true;
                Status("Failed to log in.");
            }

        }

        // Timer fires this method to perform AW_WAIT()
        private void aTimer_Tick(object source, EventArgs e)
        {
            if (Globals.iInWorld)
            {
                Utility.Wait(0);
            }

        }

        // Dispose of the bot instance before exiting the main application form
        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance.Dispose();
            Status("The bot application was closed.");
        }


        public void Status(string sMessage)
        {
            toolStatus.Content = sMessage;
            
        }


    }
}
