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

        private void CheckUpdates(object sender, RoutedEventArgs e)
        {
            var version =
                new WebClient().DownloadString(
                    "https://flyapps.weebly.com/uploads/2/3/8/8/23886508/flychrono2version.txt");
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
        }
    }
}
