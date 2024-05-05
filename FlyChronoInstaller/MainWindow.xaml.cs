using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlyChronoInstaller.Annotations;
using System.IO.Compression;
using System.Threading;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using File = System.IO.File;
using Path = System.IO.Path;

namespace FlyChronoInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private string _statusText = "Install size: 9.8MB";

        public string statusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public string appVersion => "2.0.1";
        public double givenDownloadSize => 2790;

        public string registryDirec => "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        public string appName => "FlyChrono";

        public string installPath => @"C:\CAG2 Software\FlyChrono";
        public string exePath => @"C:\CAG2 Software\FlyChrono\FlyChrono.exe";
        public string airportPath => @"C:\CAG2 Software\FlyChrono\airportsExtra.csv";
        public string icoPath => @"C:\CAG2 Software\FlyChrono\FlyChronoIcon.ico";
        public string uninstallPath => @"C:\CAG2 Software\Uninstalls\FlyChronouninstaller.exe";

        public string downloadUri => "https://flyapps.weebly.com/uploads/2/3/8/8/23886508/flychronolibs.zip";
        public string downloadPath => @"C:\CAG2 Software\Installer Temporary Files\chronolibs.zip";


        public MainWindow()
        {

            InitializeComponent();

            var stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(Properties.Resources.studiolicense));
            LicenseRichTextBox.Selection.Load(stream, DataFormats.Rtf);

        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ClickMinimize(object sender, RoutedEventArgs e)
        {


            WindowState = WindowState.Minimized;
        }

        private void tryCreateDirectory(string newDirectory)
        {
            if (Directory.Exists(newDirectory)) return;
            try
            {
                Directory.CreateDirectory(newDirectory);
            }
            catch (Exception)
            {
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void GoToPreInstall(object sender, RoutedEventArgs e)
        {
            MasterTabControl.SelectedIndex = 1;
        }

        private void Install(object sender, RoutedEventArgs e)
        {
            extractExe();
            downloadLibs();
        }

        private void downloadLibs()
        {
            statusText = "Initializing Libraries Download";

            tryCreateDirectory(@"C:\CAG2 Software\Installer Temporary Files");

            string tempFolder = @"C:\CAG2 Software\Installer Temporary Files\ChronoLibs";


            if (Directory.Exists(tempFolder))
                Directory.Delete(tempFolder, true);
            tryCreateDirectory(tempFolder);

            WebClient client = new WebClient();
            client.DownloadFileAsync(new Uri(downloadUri), downloadPath);

            client.DownloadProgressChanged += (o, e) =>
            {
                double bytesReceived = e.BytesReceived / 1000;
                double bytesToReceive = e.TotalBytesToReceive / 1000;
                string percentage;
                string text;

                if (e.TotalBytesToReceive > 1)
                {
                    percentage = Math.Round((double)bytesReceived / bytesToReceive * 100) + "%";
                    text = Math.Round(bytesReceived / 1000, 1) + "MB of " + Math.Round(bytesToReceive / 1000, 1) + "MB of libraries downloaded (" +
                           percentage + ")";
                }
                else
                {
                    percentage = Math.Round((double)bytesReceived / givenDownloadSize * 100) + "%";
                    text = Math.Round(bytesReceived / 1000, 1) + "MB of " + Math.Round(givenDownloadSize / 1000, 1) + "MB of libraries downloaded (" +
                           percentage + ")";
                }



                statusText = text;
            };

            client.DownloadFileCompleted += (o, e) =>
            {

                if (e.Cancelled)
                {
                    statusText = "Download failed";
                    InstallButton.Visibility = Visibility.Hidden;
                    return;
                }

                statusText = "Extracting libraries (this may take some time)";

                try
                {
                    ZipFile.ExtractToDirectory(downloadPath, tempFolder);

                    Copy(tempFolder, installPath);

                    Directory.Delete(tempFolder, true);
                }
                catch (IOException exception)
                {
                    MessageBox.Show(
                        "Unable to extract the necessary libraries. Please do NOT run FlyChrono, disable your antivirus and retry the installation.");
                    Application.Current.Shutdown();
                }




                finishInstallation();

                statusText = "Successfully Installed";

                InstallButton.Visibility = Visibility.Collapsed;
                FinishedStackPanel.Visibility = Visibility.Visible;


            };
        }

        void Copy(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
            }
        }

        private void extractExe()
        {
            foreach (var process in Process.GetProcessesByName("FlyChrono"))
            {
                process.Kill();
            }
            Thread.Sleep(100);
            InstallButton.Visibility = Visibility.Hidden;
            tryCreateDirectory(@"C:\CAG2 Software");
            tryCreateDirectory(installPath);
            tryCreateDirectory(@"C:\CAG2 Software\Uninstalls");
            File.WriteAllBytes(exePath, Properties.Resources.FlyChrono);
            File.WriteAllText(airportPath, Properties.Resources.airportsExtra);
            File.WriteAllBytes(uninstallPath, Properties.Resources.FlyChronoUninstaller);

            using (FileStream fileStream = new FileStream(icoPath, FileMode.Create)) //FileStream writes the icon file as WriteAllBytes does not work for icons
            {
                Properties.Resources.Icon0.Save(fileStream);
            }

            IWshRuntimeLibrary.WshShell wshShell = new IWshRuntimeLibrary.WshShell();

            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            IWshRuntimeLibrary.IWshShortcut desktopShortCut = default(IWshRuntimeLibrary.IWshShortcut);
            desktopShortCut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(desktopFolder + "\\FlyChrono.lnk");
            desktopShortCut.TargetPath = exePath;
            desktopShortCut.WorkingDirectory = installPath;
            desktopShortCut.Save();

            string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            IWshRuntimeLibrary.IWshShortcut startMenuShortCut = default(IWshRuntimeLibrary.IWshShortcut);
            startMenuShortCut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(startFolder + "\\FlyChrono.lnk");
            startMenuShortCut.TargetPath = exePath;
            startMenuShortCut.WorkingDirectory = installPath;
            startMenuShortCut.Save();

        }

        private void finishInstallation()
        {

            RegistryKey HKEY = (Registry.LocalMachine).OpenSubKey(registryDirec, true); //Declares the HKEY directory in the registry
            RegistryKey applicationKey = HKEY.CreateSubKey(appName); //Creates the "FlyChrono" key under the HKEY variable

            //Syntax: SetValue(Name, Value, Type)
            applicationKey.SetValue("DisplayName", appName, RegistryValueKind.String);
            applicationKey.SetValue("Publisher", "FlyApps", RegistryValueKind.String);
            applicationKey.SetValue("DisplayIcon", icoPath, RegistryValueKind.String);
            applicationKey.SetValue("DisplayVersion", appVersion, RegistryValueKind.String);
            applicationKey.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"), RegistryValueKind.String);
            applicationKey.SetValue("InstallLocation", exePath, RegistryValueKind.String);
            applicationKey.SetValue("UninstallString", uninstallPath, RegistryValueKind.String);
        }

        private void LaunchApp(object sender, RoutedEventArgs e)
        {
            Launch();
        }

        private void Launch()
        {
            Process.Start(exePath);
            Application.Current.Shutdown();
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CheckIfInstalled(object sender, RoutedEventArgs e)
        {
            if (!globalVariables.instantInstallBool) return;

            // prevent bug from preventing update from old flychrono
            if (AppDomain.CurrentDomain.BaseDirectory == @"C:\CAG2 Software\FlyChrono\") return;

            instantInstall();
        }

        public void instantInstall()
        {
            Hide();
            Thread.Sleep(100);

            extractExe();
            Thread.Sleep(100);

            Launch();
            Thread.Sleep(100);

            Application.Current.Shutdown();
        }
    }
}
