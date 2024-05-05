using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlyChrono2.BackEnd.Models;
using FlyChrono2.BackEnd.ViewModels;

namespace FlyChrono2.BackEnd
{
    public static class GlobalVars
    {
        public static TimeSyncViewModel GlobalTimeSyncViewModel;

        public static AlarmViewModel GlobalAlarmViewModel;
        public static SimConnection CurrentSimConnection { get; set; }

        public static FlightDataModel GlobalFlightDataModel;

        public static FlightViewModel GlobalFlightViewModel;

        public static bool IsSmallWindow { get; set; } = false;

        public static System.Windows.Forms.NotifyIcon nIcon;

        public static bool IsUpdating = false;

        public static bool UpdateConfirmed = false;

        public static string NewVersion;
    }
}
