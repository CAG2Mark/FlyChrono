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
