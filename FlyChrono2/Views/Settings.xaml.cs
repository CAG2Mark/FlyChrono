/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using FlyChrono2.BackEnd;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            if (GlobalVars.IsUpdating) return;
            InitializeComponent();
        }

        private void OpenFacebook(object sender, MouseEventArgs e)
        {
            Process.Start("https://www.facebook.com/flyapps");
        }

        public void MakeSmallWindow()
        {
            
            SmallWindowWarning.Visibility = Visibility.Visible;
            Helpers.PlayStoryboard(this,(Storyboard)Resources["SmallWindowSettings"]);
        }

        public async void MakeBigWindow()
        {
            Helpers.PlayStoryboard(this, (Storyboard)Resources["BigWindowSettings"]);
            await  Helpers.Wait(800);
            SmallWindowWarning.Visibility = Visibility.Collapsed;
        }

        private void ChangeAlarmSound(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.FileName = Properties.Settings.Default.AlarmSound;
            dialog.Filter = "Audio Files|*.mp3; *.wav";

            if (dialog.ShowDialog() == true)
            {
                Properties.Settings.Default.AlarmSound = dialog.FileName;
            }

        }

        private void ShowLicense(object sender, RoutedEventArgs e)
        {
            var licenseWindow = new License();
            licenseWindow.ShowDialog();
        }
        private void ShowSourceCode(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/CAG2Mark/FlyChrono");
        }

        private void CheckUpdates(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not yet implemented for the open source version.");
            /*
            var version =
                new WebClient().DownloadString(
                    "TODO");
            if (version != Properties.Settings.Default.ApplicationVersion)
            {
                if (MessageBox.Show("New version found: " + version + ". Install?", "Update Found",
                        MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                {
                    new UpdateWindow(version).Show();

                    Application.Current.MainWindow.Close();
                }

            }
            else
            {
                MessageBox.Show("No new version found.");
            }
            */
        }
    }
}
