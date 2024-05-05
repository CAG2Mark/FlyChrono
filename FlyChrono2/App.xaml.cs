using FlyChrono2.BackEnd;
using FlyChrono2.BackEnd.ViewModels;
using FlyChrono2.Views;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FlyChrono2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        private const string Unique = "6ddd8b0f-ff38-4595-b470-b9b4b9f27836";

        [STAThread]
        public static void Main()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                // TODO: Implement updater code for open source version

                /*
                var version =
                    new WebClient().DownloadString(
                        "");
                */
                var version = "TODO";

                // if (version != FlyChrono2.Properties.Settings.Default.ApplicationVersion) {
                if (false) {
                    GlobalVars.IsUpdating = true;
                    GlobalVars.NewVersion = version;
                }
                else
                {
                    // initialise all global fields now so they dont get initialised when updating

                    GlobalVars.nIcon = new System.Windows.Forms.NotifyIcon();
                    GlobalVars.GlobalTimeSyncViewModel = new TimeSyncViewModel();
                    GlobalVars.GlobalAlarmViewModel = new AlarmViewModel();
                    GlobalVars.CurrentSimConnection = new SimConnection();
                    GlobalVars.GlobalFlightViewModel = new FlightViewModel();
                }
                
            }

            if (SingleInstance<App>.InitializeAsFirstInstance(Unique) || FlyChrono2.Properties.Settings.Default.IsRestarting)
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            MessageBox.Show("FlyChrono is already running.");

            return true;
        }

        #endregion

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            new ExceptionWindow(e.Exception).Show();

            if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();

        }
    }
}
