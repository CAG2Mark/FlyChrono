/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

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

