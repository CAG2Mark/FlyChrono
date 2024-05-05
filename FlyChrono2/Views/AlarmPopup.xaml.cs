using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for AlarmPopup.xaml
    /// </summary>
    public partial class AlarmPopup : Window
    {
        Timer timer = new Timer(600);
        bool flashStatus;
        public AlarmPopup()
        {
            InitializeComponent();
            
            timer.Elapsed += (o, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    FlashingBorder.Opacity = flashStatus ? 0.1 : 0;
                    flashStatus = !flashStatus;
                });
            };
            timer.Start();
        }

        MediaPlayer player;


        public AlarmPopup(string alarmName)
        {
            InitializeComponent();
            AlarmNameTextBlock.Text = alarmName;

            timer.Elapsed += (o, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    FlashingBorder.Opacity = flashStatus ? 0.1 : 0;
                    flashStatus = !flashStatus;
                });
            };
            timer.Start();

            if (File.Exists(Properties.Settings.Default.AlarmSound))
            {
                player = new MediaPlayer();
                player.Open(new Uri(Properties.Settings.Default.AlarmSound));

                player.Play();

                player.MediaEnded += (o, e) =>
                {
                    player.Position = TimeSpan.Zero;
                    player.Play();
                };
            }
        }

        public EventHandler OnDismissed;
        public EventHandler OnSnoozed;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 8;
            this.Top = desktopWorkingArea.Bottom - this.Height - 8;
        }

        private async void SnoozeAlarm(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            FlashingBorder.Opacity = 0;

            stopPlayer();

            OnSnoozed?.Invoke(this, new EventArgs());

            ControlsStackPanel.Visibility = Visibility.Collapsed;

            if (Properties.Settings.Default.SnoozeMinutes == 1) {
                MinutesTextBlock.Text = "minute";
            }

            BeginStoryboard((Storyboard)MainGrid.Resources["SnoozeStoryBoard"]);

            await Task.Delay(3100);
            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {


            OnDismissed?.Invoke(this, new EventArgs());
            stopPlayer();
            timer.Stop();

            Close();

       
        }

        private void stopPlayer()
        {
            if (player != null) player.Stop();
        }
    }
}
