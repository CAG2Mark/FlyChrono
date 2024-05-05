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
