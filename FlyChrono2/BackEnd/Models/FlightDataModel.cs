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
