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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using FlyChrono2.BackEnd.Models;

namespace FlyChrono2.BackEnd.ViewModels
{
    public enum SyncStatus
    {
        Disconnected = 0,
        Waiting = 1,
        Syncing = 2,
        Synced = 3
    }
    public class TimeSyncViewModel : INotifyPropertyChanged
    {

        public void CloseConnection()
        {
            GlobalVars.CurrentSimConnection.closeConnection();
        }

        #region properties
        /// <summary>
        /// The model containing all sim-related and time-related information.
        /// </summary>
        public FlightDataModel Model
        {
            get => GlobalVars.GlobalFlightDataModel;
            set => GlobalVars.GlobalFlightDataModel = value;
        }

        /// <summary>
        /// Prevents the app recognising a pause right after the time has been synced
        /// </summary>
        public bool SyncLock { get; set; }

        /// <summary>
        /// Whether the application is waiting for the user to initiate a sync.
        /// </summary>
        public bool WaitingForUserSync { get;set; }


        private SyncStatus _currentSyncStatus;
        /// <summary>
        /// The current sync status.
        /// </summary>
        public SyncStatus CurrentSyncStatus
        {
            get => _currentSyncStatus;
            set { _currentSyncStatus = value; OnPropertyChanged(); SyncChanged?.Invoke(this, new SyncChangedEventArgs(CurrentSyncStatus)); }
        }

        /// <summary>
        /// Whether a user-initiated connection has failed or not. Reset upon connection.
        /// </summary>
        public bool FailedManualConnect { get; set; }

        /// <summary>
        /// Specifies whether or not the user has manually set the time.
        /// </summary>
        public bool UsingCustomTime { get; set; } = false;

        public DateTime DateTimeToSet { get; set; }

        private string _statusText;
        /// <summary>
        /// The text displayed on the home page.
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }


        private int _customMinute = 0;
        /// <summary>
        /// The custom minute the user has specified.
        /// </summary>
        public int CustomMinute
        {
            get => _customMinute;
            set { _customMinute = value; OnPropertyChanged(); }
        }

        private int _customHour = 0;
        /// <summary>
        /// The custom hour the user has specified.
        /// </summary>
        public int CustomHour
        {
            get => _customHour;
            set { _customHour = value; OnPropertyChanged(); }
        }

        private int _preCustomOffsetMinute = 0;
        /// <summary>
        /// The preCustomOffset minute the user has specified.
        /// </summary>
        public int PreCustomOffsetMinute
        {
            get => _preCustomOffsetMinute;
            set { _preCustomOffsetMinute = value; OnPropertyChanged(); }
        }

        private int _preCustomOffsetHour = 0;
        /// <summary>
        /// The preCustomOffset hour the user has specified.
        /// </summary>
        public int PreCustomOffsetHour
        {
            get => _preCustomOffsetHour;
            set { _preCustomOffsetHour = value; OnPropertyChanged(); }
        }

        public int CustomOffsetMinute { get; set; } = 0;
        public int CustomOffsetHour { get; set; } = 0;


        #endregion


        private Timer CheckTimeTimer = new Timer(5000);

        #region constructor

        /// <summary>
        /// Constructor for TimeSyncViewModel.
        /// </summary>
        public TimeSyncViewModel()
        {
            if (GlobalVars.IsUpdating) return;

            Model = new FlightDataModel
            {
                RealZulu = DateTime.UtcNow,
            };

            //Populate();


            var flightDataTimer = new System.Timers.Timer(100);
            flightDataTimer.Elapsed += flightDataTimerElapsed;
            flightDataTimer.Start();

            CheckTimeTimer.Elapsed += CheckTime;
            CheckTimeTimer.Start();
        }

        #endregion

        #region commands

        private bool _canExecute = true;

        private RelayCommand _syncButtonPressed;
        public ICommand SyncButtonPressed
        {
            get
            {
                if (_syncButtonPressed == null)
                {
                    _syncButtonPressed = new RelayCommand(param => this.SyncButtonPressedAction(),
                        param => _canExecute);
                }
                return _syncButtonPressed;
            }
        }

        private RelayCommand _setCustomTimeCommand;
        public ICommand SetCustomTimeCommand
        {
            get
            {
                if (_setCustomTimeCommand == null)
                {
                    _setCustomTimeCommand = new RelayCommand(param => SetCustomTime(),
                        param => _canExecute);
                }
                return _setCustomTimeCommand;
            }
        }

        private RelayCommand _applyOffsetCommand;
        public ICommand ApplyOffsetCommand
        {
            get
            {
                if (_applyOffsetCommand == null)
                {
                    _applyOffsetCommand = new RelayCommand(param => ApplyOffset(),
                        param => _canExecute);
                }
                return _applyOffsetCommand;
            }
        }

        #endregion

        /// <summary>
        /// Applies the user-specified offset.
        /// </summary>
        public async void ApplyOffset()
        {
            if (!Model.IsConnected || Model.PausedOrInMenu)
            {
                MessageBox.Show("Offset Applied.");
                return;
            }

            CustomOffsetHour = PreCustomOffsetHour;
            CustomOffsetMinute = PreCustomOffsetMinute;

            await Task.Delay(1000);

            Sync();
        }

        /// <summary>
        /// Sets the user-specified custom time.
        /// </summary>
        public void SetCustomTime()
        {
            if (!Model.IsConnected || Model.PausedOrInMenu)
            {
                MessageBox.Show("The simulator is not connected, loaded and/or paused.");
                return;
            }

            UsingCustomTime = true;

            GlobalVars.CurrentSimConnection.SetHour(CustomHour);
            GlobalVars.CurrentSimConnection.SetMinute(CustomMinute);
        }


        public void SyncButtonPressedAction()
        {
            if (!Model.IsConnected)
            {
                if (!GlobalVars.CurrentSimConnection.tryConnect())
                {
                    FailedManualConnect = true;
                    StatusText = "Failed to Connect to Simulator";
                }
                else
                {
                    Populate();
                }
            }

            if (WaitingForUserSync)
            {
                Sync();
                WaitingForUserSync = false;
            }
        }

        #region simulator interactions
        /// <summary>
        /// Occurs when the flight data timer has elapsed, usually at an interval of 1 second. Populates the model containing the flight data.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void flightDataTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!UsingCustomTime)
            {
                // just use the current time if the user hasn't specified a custom time/offset
                DateTimeToSet = DateTime.UtcNow.AddMinutes(CustomOffsetMinute).AddHours(CustomOffsetHour);
            }

            Populate();
        }

        /// <summary>
        /// Populates the flight data model.
        /// </summary>
        public void Populate()
        {
            if (GlobalVars.IsUpdating) return;

            if (Model != null && GlobalVars.CurrentSimConnection != null)
            GlobalVars.CurrentSimConnection.PopulateModel(Model);

            if (!Model.IsConnected)
            {
                CurrentSyncStatus = SyncStatus.Disconnected;
                if (!FailedManualConnect) StatusText = "Not Connected To Simulator";
            }
            else
            {
                OnModelPopulated();
            }
        }

        public bool AllowAutoSync
        {
            get => !UsingCustomTime && Properties.Settings.Default.AutoSync;
        }

        public void OnModelPopulated()
        {
            if (SyncLock) return;

            // Check for if sim is paused and not syncing
            if (Model.PausedOrInMenu && CurrentSyncStatus != SyncStatus.Syncing)
            {
                CheckPaused();
                if (IsPaused)
                {
                    CurrentSyncStatus = SyncStatus.Waiting; // not syncing or synced - waiting
                    StatusText = "Waiting for Simulator";
                }
            }
            // Check if sim isn't paused but isn't syncing or synced
            else if (!IsPaused && (CurrentSyncStatus != SyncStatus.Syncing && CurrentSyncStatus != SyncStatus.Synced))
            {
                if (AllowAutoSync)
                {
                    Sync();
                }
                else
                {
                    StatusText = "Ready to Sync";
                    WaitingForUserSync = true;
                }
                
            }
        }

        public bool IsPaused;
        public bool CanCheckPaused = true;

        /// <summary>
        /// Makes sure the simulator is actually paused by making sure the time paused exceeds 1 second.
        /// </summary>
        public async void CheckPaused()
        {
            if (!CanCheckPaused) return;

            CanCheckPaused = false;

            for (var i = 0; i < 10; i++)
            {
                await Task.Delay(100);
                if (!Model.PausedOrInMenu)
                {
                    CanCheckPaused = true;
                    IsPaused = false;
                    return;
                }
            }

            IsPaused = true;
            CanCheckPaused = true;
        }


        #endregion

        #region check time

        /// <summary>
        /// Checks whether the time has deviated too far from the real zulu time and corrects it if it is.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckTime(object sender, ElapsedEventArgs e)
        {
            // check whether it should actually run
            if (!Properties.Settings.Default.ConstantlyCheckTime ||
                CurrentSyncStatus == SyncStatus.Syncing ||
                CurrentSyncStatus == SyncStatus.Disconnected ||
                CurrentSyncStatus == SyncStatus.Waiting)
                return;

            
            // gets the seconds deviated
            var secondsDeviated = (int)(Math.Abs(Model.SimZulu.Ticks - DateTimeToSet.Ticks) / TimeSpan.TicksPerSecond);
            // gets the seconds the simulator time is allowed to deviate
            var deviationLimitSeconds = (11 - Properties.Settings.Default.TimeCorrectionAggression) * 30;

            // if the time deviation (rounded down) is greater than the allowed deviation limit
            if (secondsDeviated >= deviationLimitSeconds)
            {
                Sync();
                StatusText = "Automatically Correcting Time";
            }
        }

        #endregion

        public event EventHandler<SyncChangedEventArgs> SyncChanged;

        // this is the datetime to set for the advanced time sync
        private DateTime newTimeToSet => DateTimeToSet.AddMinutes(1);

        #region syncing
        /// <summary>
        /// Sets the hour, second and minute of the simulator.
        /// </summary>
        public async void Sync()
        {
            UsingCustomTime = false;
            SyncLock = true;

            GlobalVars.CurrentSimConnection.SetCalendar(DateTime.UtcNow);

            if (!Properties.Settings.Default.AdvancedSync)
            {
                #region standard sync
                Console.WriteLine("syncing");

                // sync hour

                CurrentSyncStatus = SyncStatus.Syncing;
                
                StatusText = "Syncing Time to Simulator";

                GlobalVars.CurrentSimConnection.SetHour(DateTimeToSet.Hour);


                // wait for simulator to finish loading

                await Task.Delay(1000);

                Console.WriteLine(Model.PausedOrInMenu);

                while (Model.PausedOrInMenu)
                {
                    Console.WriteLine(Model.PausedOrInMenu);
                    await Task.Delay(100);
                }

                Console.WriteLine("syncing minute");

                await Task.Delay(1000);

                // sync minute

                GlobalVars.CurrentSimConnection.SetMinute(DateTimeToSet.Minute);

                CurrentSyncStatus = SyncStatus.Synced;
                StatusText = "Simulator Time Synced";
            }

            #endregion

            else
            {
                #region advanced sync
                Console.WriteLine("syncing advanced");

                // sync hour

                CurrentSyncStatus = SyncStatus.Syncing;
                StatusText = "Syncing Time to Simulator";

                GlobalVars.CurrentSimConnection.ResetSecond();

                await Task.Delay(2000);

                GlobalVars.CurrentSimConnection.SetHour(newTimeToSet.Hour);

                // wait for simulator to finish loading

                await Task.Delay(1000);

                while (Model.PausedOrInMenu)
                {
                    await Task.Delay(100);
                }

                await Task.Delay(1050);

                // sync minute

                GlobalVars.CurrentSimConnection.SetMinute(newTimeToSet.Minute);
                Console.WriteLine("syncing minute to" + newTimeToSet.Minute);
                
                // wait until the real time second is 0 then reset the simulator second to 0 so the second is in sync
                while (DateTime.UtcNow.Second != 0)
                {
                    await Task.Delay(100);
                }

                Console.WriteLine("second set");
                GlobalVars.CurrentSimConnection.ResetSecond();

                CurrentSyncStatus = SyncStatus.Synced;
                StatusText = "Simulator Time Synced";
            }
            #endregion

            await Task.Delay(5000);

            SyncLock = false;

        }

        #endregion



        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Invokes the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class SyncChangedEventArgs : EventArgs
    {
        public SyncStatus CurrentSyncStatus { get; set; }

        public SyncChangedEventArgs(SyncStatus param)
        {
            CurrentSyncStatus = param;
        }
    }
}
