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
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window , INotifyPropertyChanged
    {
        private string _exception;

        public string exception
        {
            get => _exception;
            set { _exception = value; OnPropertyChanged(); }
        }

        private string direc = @"C:\CAG2 Software\FlyChrono\Logs";

        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public ExceptionWindow(Exception ex)
        {
            InitializeComponent();

            var exStr = ex.ToString();
            exception = exStr;


            if (!Directory.Exists(direc)) Directory.CreateDirectory(direc);

            var time = DateTime.UtcNow;

            var fileName = time.Year + "-" + time.Month + "-" + time.Day + " " + time.Hour + " " + time.Minute + " " +
                           time.Second + "z";

            File.WriteAllText(direc + @"\" + fileName + ".txt", exStr);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RestartApp(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsRestarting = true;
            Properties.Settings.Default.Save();

            Process.Start(Application.ResourceAssembly.Location);

            Application.Current.Shutdown();



        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(direc);
        }

        private void CopyError(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(exception);
            MessageBox.Show("Error copied to clipboard.");
        }
    }
}

