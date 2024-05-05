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

namespace FlyChrono2.BackEnd.Models
{
    public class FlightDataModel : INotifyPropertyChanged
    {

        #region time-related properties

        private DateTime _realZulu;
        public DateTime RealZulu
        {
            get => _realZulu;
            set { _realZulu = value; OnPropertyChanged(); }
        }

        private DateTime _simZulu;
        public DateTime SimZulu
        {
            get => _simZulu;
            set { _simZulu = value; OnPropertyChanged(); }
        }

        #endregion

        #region other properties

        public double Altitude { get; set; }
        public double GroundSpeed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsConnected { get; set; }
        public bool OnGround { get; set; }
        #endregion
        public bool PausedOrInMenu { get; set; }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
