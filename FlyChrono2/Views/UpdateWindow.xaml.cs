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
using System.Windows.Shapes;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window, INotifyPropertyChanged
    {
        public string downloadUri = "TODO"; // TODO

        private string _version;

        public string version
        {
            get => _version;
            set { _version = value; OnPropertyChanged(); }
        }

        private string _percentage = "0%";
        public string percentage
        {
            get => _percentage;
            set { _percentage = value; OnPropertyChanged(); }
        }

        private double _width = 0;
        public double width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(); }
        }

        public UpdateWindow()
        {
            InitializeComponent();
        }

        public UpdateWindow(string version)
        {
            this.version = "New version: " + version;
            InitializeComponent();
        }

        public string downloadPath => @"C:\CAG2 Software\Installer Temporary Files\FlyChronoInstaller.exe";

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            downloadOverlay();
        }

        private void downloadOverlay()
        {
            tryCreateDirectory(@"C:\CAG2 Software");
            tryCreateDirectory(@"C:\CAG2 Software\Installer Temporary Files");

            WebClient client = new WebClient();

            client.DownloadFileAsync(new Uri(downloadUri), downloadPath);

            client.DownloadProgressChanged += (o, e) =>
            {
                if (e.TotalBytesToReceive != 0)
                {
                    percentage = Math.Round(Convert.ToDecimal((double)e.BytesReceived / e.TotalBytesToReceive * 100)) + "%";
                    width = (double)e.BytesReceived / e.TotalBytesToReceive * 233;
                }
            };

            client.DownloadFileCompleted += (o, e) =>
            {
                Process.Start(downloadPath, "instantInstall");
                Application.Current.Shutdown();
            };
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

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

