using FlyChrono2.BackEnd;
using FlyChrono2.BackEnd.Models;
using FlyChrono2.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyChrono2.CustomControls
{
    /// <summary>
    /// Interaction logic for AlarmPanel.xaml
    /// </summary>
    public partial class AlarmPanel : UserControl, INotifyPropertyChanged, IDisposable
    {
        public static readonly DependencyProperty AssignedAlarmProperty = DependencyProperty.Register(
            "AssignedAlarm", typeof(AlarmModel), typeof(AlarmPanel), new PropertyMetadata((o, e) => ((AlarmPanel)o).AssignedAlarmChanged()));

        private string _ringsAtText;
        public string RingsAtText
        {
            get => _ringsAtText;
            set { _ringsAtText = value; OnPropertyChanged(); }
        }

        #region animations

        public async void MakeSmall()
        {
            if (EditingAlarm) ToggleEditPanel();

            Helpers.PlayStoryboard(this, (Storyboard)Resources["MakeSmallPanel"]);

            await Helpers.Wait(200);

            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;
        }

        public void MakeSmallInstant()
        {
            DeleteButton.Visibility = Visibility.Collapsed;
            EditButton.Visibility = Visibility.Collapsed;

            DeleteButton.Opacity = 0;
            EditButton.Opacity = 0;

            MainGrid.Margin = new Thickness(8,8,8, 0);
            RepeatsTextBlock.Opacity = 0;

            MarginGrid.Margin = new Thickness(8, 0, 0, 0);
        }

        public async void MakeBig()
        {

            var sb = (Storyboard)Resources["MakeBigPanel"];
            Helpers.PlayStoryboard(this, sb);

            await Helpers.Wait(600);

            DeleteButton.Visibility = Visibility.Visible;
            EditButton.Visibility = Visibility.Visible;
        }

        #endregion

        public AlarmPanel()
        {
            if (GlobalVars.IsUpdating) return;

            InitializeComponent();

            checkRingTimer.Elapsed += checkRing;
            checkRingTimer.Start();

            Loaded += (o, e) =>
            {
                if (GlobalVars.IsSmallWindow) MakeSmallInstant();
            };
        }

        private bool hasSetHandlers = false;

        public void AssignedAlarmChanged()
        {
            if (AssignedAlarm == null) return;

            OnPropertyChanged(nameof(AssignedAlarm));

            if (!hasSetHandlers)
            {
                hasSetHandlers = true;
                AssignedAlarm.PropertyChanged += (o, e) => UpdateString();
                AssignedAlarm.TtgRingCondition.PropertyChanged += (o, e) => UpdateString();
                AssignedAlarm.SimZuluRingCondition.PropertyChanged += (o, e) => UpdateString();
                AssignedAlarm.RealZuluRingCondition.PropertyChanged += (o, e) => UpdateString();

                AssignedAlarm.MakeSmall += (o, e) => MakeSmall();
                AssignedAlarm.MakeBig += (o, e) => MakeBig();
            }



            UpdateString();

        }

        public void UpdateString()
        {
            GlobalVars.GlobalAlarmViewModel.Serialize();

            if (AssignedAlarm == null) return;
            Console.WriteLine("updating string");

            switch (AssignedAlarm.Mode)
            {
                case AlarmMode.ByETA:
                    RingsAtText = AssignedAlarm.TtgRingCondition.RingHour + "h " + PadTime(AssignedAlarm.TtgRingCondition.RingMinute) + "m from destination";
                    break;
                case AlarmMode.ByDTG:
                    RingsAtText = AssignedAlarm.DtgRingCondition + "nm from destination";
                    break;
                case AlarmMode.BySimZulu:
                    RingsAtText = PadTime(AssignedAlarm.SimZuluRingCondition.RingHour) + ":" + PadTime(AssignedAlarm.SimZuluRingCondition.RingMinute) + "z simulator time";
                    break;
                case AlarmMode.ByRealZulu:
                    RingsAtText = PadTime(AssignedAlarm.RealZuluRingCondition.RingHour) + ":" + PadTime(AssignedAlarm.RealZuluRingCondition.RingMinute) + "z real time";
                    break;
            }
        }

        public string PadTime(object param)
        {
            var str = param.ToString();
            return str.PadLeft(2, '0');
        }

        public AlarmModel AssignedAlarm
        {
            get => (AlarmModel)GetValue(AssignedAlarmProperty);
            set => SetValue(AssignedAlarmProperty, value);
        }

        public bool EditingAlarm { get; set; } = false;

        Timer checkRingTimer = new Timer(100);

        private void checkRing(object sender, ElapsedEventArgs e)
        {
            if (disposedValue) return;

            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (AssignedAlarm == null) return;

                    //UpdateString();

                    if (AssignedAlarm.CheckRing())
                    {
                        checkRingTimer.Stop();
                        ring();
                    }
                }
                );
            }
            catch (TaskCanceledException)
            {
                Dispatcher.InvokeShutdown();
            }
        }

        private void ring()
        {
            if (AssignedAlarm == null) return;

            var popup = new AlarmPopup(AssignedAlarm.AlarmName);
            popup.Show();

            popup.OnDismissed += (o, e) =>
            {
                AssignedAlarm.AlarmRinged();
                checkRingTimer.Start();
            };
            popup.OnSnoozed += (o, e) =>
            {
                var timeToRing = DateTime.UtcNow.AddMinutes(Properties.Settings.Default.SnoozeMinutes);

                var snoozeTimer = new Timer(1000);
                snoozeTimer.Elapsed += (o1, e1) =>
                {
                    if (timeToRing.Minute == DateTime.UtcNow.Minute &&
                        timeToRing.Second == DateTime.UtcNow.Second)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ring();
                            snoozeTimer.Stop();
                        }
                        );
                    }
                };

                snoozeTimer.Start();
            };
        }

        private void ShowEditPanel(object sender, RoutedEventArgs e)
        {
            ToggleEditPanel();
        }

        private void ToggleEditPanel()
        {
            EditPanel.Visibility = EditingAlarm ? Visibility.Collapsed : Visibility.Visible;
            EditingAlarm = !EditingAlarm;
        }

        public event EventHandler<EventArgs> AlarmDeleting;

        private void DeleteAlarm(object sender, RoutedEventArgs e)
        {
            AlarmDeleting?.Invoke(this, new EventArgs());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                checkRingTimer.Stop();
                checkRingTimer = null;
                AssignedAlarm = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AlarmPanel()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
