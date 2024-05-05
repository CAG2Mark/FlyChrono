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
using System.Xml.Serialization;

namespace FlyChrono2.BackEnd.Models
{
    public class AlarmTime : INotifyPropertyChanged
    {
        public AlarmTime()
        {
        }

        public AlarmTime(int hour, int minute)
        {
            RingHour = hour;
            RingMinute = minute;
        }


        private int _ringHour;
        public int RingHour { get => _ringHour; set { _ringHour = value;  OnPropertyChanged(); RingConditionChangedEvent?.Invoke(this, new EventArgs());  } }

        private int _ringMinute;
        public int RingMinute { get => _ringMinute; set { _ringMinute = value; OnPropertyChanged(); RingConditionChangedEvent?.Invoke(this, new EventArgs()); } }

        public delegate void RingConditionChanged(object sender, EventArgs e);
        public event RingConditionChanged RingConditionChangedEvent;

        public int ToMinutes()
        {
            return RingHour * 60 + RingMinute;
        }

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
