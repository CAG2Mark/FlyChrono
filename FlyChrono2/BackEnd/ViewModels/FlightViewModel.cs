/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using FlyChrono2.BackEnd.Models;
using System;
using System.Timers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace FlyChrono2.BackEnd.ViewModels
{
    public class FlightViewModel : INotifyPropertyChanged
    {
        public FlightViewModel()
        {
            if (GlobalVars.IsUpdating) return;

            // update database file
            if (File.Exists(@"C:\CAG2 Software\FlyChrono\airportsExtra.csv"))
            {
                var extraAirports = File.ReadAllLines(@"C:\CAG2 Software\FlyChrono\airportsExtra.csv");

                try
                {
                    var revLine = extraAirports.ToList().Find(line => line.Contains("DATABASE-REVISION"));
                    int rev = Convert.ToInt32(revLine.Split(',')[1]);

                    if (rev != Properties.Settings.Default.DatabaseRevision) AddToDatabase(extraAirports);
                    Properties.Settings.Default.DatabaseRevision = rev;

                    Properties.Settings.Default.Save();
                }
                catch (Exception)
                {

                }
            }

            var flightDataTimer = new Timer(100);
            flightDataTimer.Elapsed += FlightDataTimerElapsed;

            flightDataTimer.Start();

            Directory.CreateDirectory(@"C:\CAG2 Software\AppLink");

            File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronoactive.txt", "true");

            ClearDepArrText();
        }

        #region database update

        private void AddToDatabase(string[] airports)
        {
            try
            {
                var database = File.ReadAllLines(@"C:\CAG2 Software\FlyChrono\airports.csv").ToList();

                foreach (string s in airports)
                {
                    if (s.Contains("DATABASE-REVISION")) continue;
                    database.Add(s);
                }

                File.WriteAllLines(@"C:\CAG2 Software\FlyChrono\airports.csv", database);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to update airport database. If you notice any missing airports, please re-install FlyChrono.");
            }
        }

        #endregion

        #region properties

        public static string notConnectedText = "n/a";
        public static string emptyTimeText = "--:--";
        public static string emptyZuluText = "--:--z";
        public static string emptyAirportText = "----";

        #region dep
        private string _preDep;
        public string PreDep
        {
            get => _preDep;
            set { _preDep = value; OnPropertyChanged(); UpdatedDep(); }
        }

        private string _dep = emptyAirportText;
        public string Dep
        {
            get => _dep;
            set { _dep = value; OnPropertyChanged(); }
        }

        private string _depName;
        public string DepName
        {
            get => _depName;
            set { _depName = value; OnPropertyChanged(); }
        }

        #endregion

        #region arr
        private string _preArr;
        public string PreArr
        {
            get => _preArr;
            set { _preArr = value; OnPropertyChanged(); UpdatedArr(); }
        }

        private string _arr = emptyAirportText;
        public string Arr
        {
            get => _arr;
            set { _arr = value; OnPropertyChanged(); }
        }

        private string _arrName;
        public string ArrName
        {
            get => _arrName;
            set { _arrName = value; OnPropertyChanged(); }
        }

        #endregion

        #region callsign
        private string _preCallsign;
        public string PreCallsign
        {
            get => _preCallsign;
            set { _preCallsign = value; OnPropertyChanged(); }
        }

        private string _callsign = notConnectedText;
        public string Callsign
        {
            get => _callsign;
            set { _callsign = value; OnPropertyChanged(); }
        }

        #endregion

        #region coords

        public double PreDepLat { get; set; }
        public double PreArrLat { get; set; }
        public double PreDepLong { get; set; }
        public double PreArrLong { get; set; }


        public double DepLat { get; set; }
        public double ArrLat { get; set; }
        public double DepLong { get; set; }
        public double ArrLong { get; set; }


        #endregion

        public bool IsValidDep { get; set; }
        public bool IsValidArr { get; set; }

        public int ShowETAAlt => Properties.Settings.Default.ShowEtaAltitude;
        public bool ShowETA { get; set; }

        public FlightDataModel FlightData
        {
            get => GlobalVars.GlobalFlightDataModel;
            set => GlobalVars.GlobalFlightDataModel = value;
        }

        #region flight related properties

        public double Distance { get; set; }
        private string _distanceText = notConnectedText;
        public string DistanceText
        {
            get => _distanceText;
            set { _distanceText = value; OnPropertyChanged(); }
        }

        public double Dtg { get; set; }
        private string _dtgText = notConnectedText;
        public string DtgText
        {
            get => _dtgText;
            set { _dtgText = value; OnPropertyChanged(); }
        }

        public DateTime EtaDateTime { get; set; }
        private string _etaText = emptyZuluText;
        public string EtaText
        {
            get => _etaText;
            set { _etaText = value; OnPropertyChanged(); }
        }

        public DateTime EtaSimDateTime { get; set; }
        private string _etaSimText = emptyZuluText;
        public string EtaSimText
        {
            get => _etaSimText;
            set { _etaSimText = value; OnPropertyChanged(); }
        }

        public double MinutesToGo { get; set; }
        private string _ttgText = emptyTimeText;
        public string TtgText
        {
            get => _ttgText;
            set { _ttgText = value; OnPropertyChanged(); }
        }

        private double _flightPercent;
        public double FlightPercent
        {
            get => _flightPercent;
            set { _flightPercent = value; OnPropertyChanged(); }
        }
        private string _flightPercentText;
        public string FlightPercentText
        {
            get => _flightPercentText;
            set { _flightPercentText = value; OnPropertyChanged(); }
        }


        public bool IsDepSameAsArrival { get; set; }
        public bool IsFlightSet { get; set; }
        public bool CanShowEta { get; set; }

        #endregion

        #endregion

        #region timer elapsed actions

        private void FlightDataTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // create the directories and text files for sharing flight data.
            try
            {
                Directory.CreateDirectory(@"C:\CAG2 Software\AppLink");

                sharedDep = File.ReadAllText(@"C:\CAG2 Software\AppLink\flylivedep.txt");
                sharedArr = File.ReadAllText(@"C:\CAG2 Software\AppLink\flylivearr.txt");
                sharedCallsign = File.ReadAllText(@"C:\CAG2 Software\AppLink\flylivecallsign.txt");
                flyLiveActive = File.ReadAllText(@"C:\CAG2 Software\AppLink\flyliveactive.txt");

            }

            // runs if the files are locked or cannot be accessed for some reason
            catch (Exception)
            {
                sharedDep = "";
                sharedArr = "";
                sharedCallsign = "";
                flyLiveActive = "false";
            }

            // detects any changes in the data
            if ((sharedDep != _sharedDep && sharedDep != "") ||
                (sharedArr != _sharedArr && sharedArr != "") ||
                sharedCallsign != _sharedCallsign) SetFlightFromSharedData();

            // detects any changes in the data
            _sharedDep = sharedDep;
            _sharedArr = sharedArr;
            _sharedCallsign = sharedCallsign;
            _flyLiveActive = flyLiveActive;

            if (!FlightData.IsConnected)
            {
                EtaSimText = emptyZuluText;
                EtaText = emptyZuluText;
                DtgText = notConnectedText;
                TtgText = emptyTimeText;
                FlightPercentText = "0%";
                return;
            }

            if (FlightData.Altitude > ShowETAAlt) CanShowEta = true;

            // set DTG
            if (IsFlightSet && !IsDepSameAsArrival)
            {
                // determine and set distance
                Dtg = GetDistance(FlightData.Latitude, FlightData.Longitude, ArrLat, ArrLong);
                DtgText = Math.Round(Dtg).ToString() + "nm";

                FlightPercent = Distance - Dtg >= 0 ? (Distance - Dtg) / Distance : 0;
                FlightPercentText = Math.Round(FlightPercent * 100) + "%";
            }
            // set ETA and TTG
            if (
                CanShowEta &&
                !IsDepSameAsArrival &&
                IsFlightSet
                )
            {

                if (FlightData.GroundSpeed > 0.1)
                {
                    var hoursToGo = Dtg / FlightData.GroundSpeed;
                    MinutesToGo = hoursToGo * 60;
                    var secondsToGo = MinutesToGo * 60;

                    var timeToGo = new TimeSpan((long)Math.Round(hoursToGo * TimeSpan.TicksPerHour));

                    TtgText = timeToGo.ToString(@"hh\:mm");

                    // sim eta calculation

                    var simTimeNow = FlightData.SimZulu;
                    EtaSimDateTime = simTimeNow.AddSeconds(secondsToGo);
                    EtaSimText = EtaSimDateTime.ToString("HH:mm") + "z";

                    // real eta calculation

                    var realTime = DateTime.UtcNow;
                    EtaDateTime = realTime.AddSeconds(secondsToGo);
                    EtaText = EtaDateTime.ToString("HH:mm") + "z";


                }
            }

            
        }

        private void SetFlightFromSharedData()
        {
            PreDep = sharedDep;
            PreArr = sharedArr;
            PreCallsign = sharedCallsign;

            SetFlight();
        }

        private string sharedDep = "";
        private string sharedArr = "";
        private string sharedCallsign = "";
        private string flyLiveActive = "false";

        private string _sharedDep = "";
        private string _sharedArr = "";
        private string _sharedCallsign = "";
        private string _flyLiveActive = "false";

        public double GetFlightPercent(double currentDist, double finalDist)
        {
            double inversePercent = currentDist / finalDist;
            double percent = 1 - inversePercent;
            if (percent >= 0) return percent;
            return 0;
        }

        public double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 3440.065; // In nautical miles
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * 2 * Math.Asin(Math.Sqrt(a));
        }

        public static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        #endregion

        #region setting airports

        private bool _canExecute = true;
        private RelayCommand _applyFlightDataCommand;
        public ICommand ApplyFlightDataCommand
        {
            get
            {
                if (_applyFlightDataCommand == null)
                {
                    _applyFlightDataCommand = new RelayCommand(param => ApplyFlightData(),
                        param => _canExecute);
                }
                return _applyFlightDataCommand;
            }
        }

        public EventHandler FlightDataApplied;

        #region flight sharing

        public void WriteClosedText()
        {
            Directory.CreateDirectory(@"C:\CAG2 Software\AppLink");

            try
            {
                File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronoactive.txt", "false");
            }
            catch (Exception) { }

            ClearDepArrText();
        }

        private void ClearDepArrText()
        {
            try
            {
                File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronodep.txt", "");
                File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronoarr.txt", "");
                File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronocallsign.txt", "");
            }
            catch (Exception) { }
        }

        #endregion


        public void SetFlight()
        {
            Dep = PreDep;
            Arr = PreArr;
            DepLat = PreDepLat;
            DepLong = PreDepLong;
            ArrLat = PreArrLat;
            ArrLong = PreArrLong;
            Callsign = string.IsNullOrEmpty(PreCallsign) ? notConnectedText : PreCallsign;

            Distance = GetDistance(DepLat, DepLong, ArrLat, ArrLong);

            IsDepSameAsArrival = Distance == 0;
            DistanceText = IsDepSameAsArrival ? notConnectedText : Math.Round(Distance) + "nm";

            IsFlightSet = true;

            CanShowEta = FlightData.Altitude > ShowETAAlt;
        }


        public void ApplyFlightData()
        {

            if (IsValidDep && IsValidArr)
            {

                SetFlight();

                FlightDataApplied?.Invoke(this, new EventArgs());


                if (Properties.Settings.Default.ShareFlightWithFlyLive)
                {
                    Directory.CreateDirectory(@"C:\CAG2 Software\AppLink");

                    try
                    {
                        File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronodep.txt", Dep);
                        File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronoarr.txt", Arr);
                        File.WriteAllText(@"C:\CAG2 Software\AppLink\flychronocallsign.txt", PreCallsign);
                    }
                    catch (Exception) { }
                }


                if (IsDepSameAsArrival) MessageBox.Show("You have set the departure airport to be the same as the arrival airport. ETA and distance calculations will not be available.");
            }
            else
            {
                MessageBox.Show("Please fill in the required data.");
            }
        }
        public void UpdatedDep()
        {
            if (PreDep.Length < 3) return;

            Airport dep = GetIcao(PreDep);
            if (dep.airportName == null)
            {
                IsValidDep = false;
                DepName = "";
                return;
            }

            IsValidDep = true;

            DepName = dep.airportName;

            PreDepLat = dep.latitude;
            PreDepLong = dep.longitude;
        }

        public void UpdatedArr()
        {
            if (PreArr.Length < 3) return;

            Airport arr = GetIcao(PreArr);
            if (arr.airportName == null)
            {
                IsValidArr = false;
                ArrName = "";
                return;
            }

            IsValidArr = true;

            ArrName = arr.airportName;

            PreArrLat = arr.latitude;
            PreArrLong = arr.longitude;
        }

        public Airport GetIcao(string icaoStr)
        {
            if (icaoStr.Length <= 2)
            {
                return new Airport(); // return a null airport if the length is less than 3
            }

            string line;
            string airportLine = "";
            string icao = @"""" + icaoStr + @"""";
            bool isDone = false;

            if (!File.Exists(@"C:\CAG2 Software\FlyChrono\airports.csv"))
            {
                MessageBox.Show(
                    "The airport database file was deleted and/or could not be found. Please re-install the application.");
                
            }
            var file =
    new System.IO.StreamReader(@"C:\CAG2 Software\FlyChrono\airports.csv");
            while ((line = file.ReadLine()) != null)
            {
                if (line.Substring(3, 26).Contains(icao) && !isDone)
                {
                    airportLine = line;
                    //MessageBox.Show(line);
                    isDone = true;
                }
            }

            if (!isDone || airportLine == "")
            {
                return new Airport();
            }

            var airportSplit = Regex.Split(airportLine, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            List<string> airportDetails = airportSplit.ToList<String>();

            string latStr = airportDetails[4].Replace(@"""", "");
            string longStr = airportDetails[5].Replace(@"""", "");
            string airportName = airportDetails[3].Replace(@"""", "");

            return new Airport(
                Convert.ToDouble(latStr, new NumberFormatInfo() { NumberDecimalSeparator = "." }),
                Convert.ToDouble(longStr, new NumberFormatInfo() { NumberDecimalSeparator = "." }), airportName);
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
}
