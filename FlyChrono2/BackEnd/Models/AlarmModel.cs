using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace FlyChrono2.BackEnd.Models
{

    public enum AlarmMode
    {
        ByETA = 0,
        ByDTG = 1,
        BySimZulu = 2,
        ByRealZulu = 3
    }

    public class AlarmModel : INotifyPropertyChanged
    {
        private string _alarmName;
        public string AlarmName
        {
            get => _alarmName;
            set { _alarmName = value; OnPropertyChanged(); }
        }

        private AlarmMode _mode = AlarmMode.ByETA;
        public AlarmMode Mode
        {
            get => _mode;
            set { _mode = value; OnPropertyChanged(); ringConditionChanged(); }
        }

        private AlarmTime _ttgRingCondition = new AlarmTime(0, 30);
        public AlarmTime TtgRingCondition
        {
            get => _ttgRingCondition;
            set { _ttgRingCondition = value; OnPropertyChanged(); }
        }

        private int _dtgRingCondition = 100;
        public int DtgRingCondition
        {
            get => _dtgRingCondition;
            set { _dtgRingCondition = value; OnPropertyChanged(); ringConditionChanged(); }
        }

        private AlarmTime _simZuluRingCondition = new AlarmTime(0, 0);
        public AlarmTime SimZuluRingCondition
        {
            get => _simZuluRingCondition;
            set { _simZuluRingCondition = value; OnPropertyChanged(); }
        }

        private AlarmTime _realZuluRingCondition = new AlarmTime(0,0);
        public AlarmTime RealZuluRingCondition
        {
            get => _realZuluRingCondition;
            set { _realZuluRingCondition = value; OnPropertyChanged(); }
        }

        private bool _alarmActive = true;
        public bool AlarmActive
        {
            get => _alarmActive;
            set { _alarmActive = value; OnPropertyChanged(); alarmActivated(); }
        }

        public bool WaitingForReset { get; set; } = false;


        private bool _alarmRepeats = false;
        public bool AlarmRepeats
        {
            get => _alarmRepeats;
            set { _alarmRepeats = value; OnPropertyChanged(); }
        }

        #region animations

        /// <summary>
        /// Invoke the event that will make the associated panel big or small.
        /// </summary>
        /// <param name="makeBig">Whether you want to make the panel big or small.</param>
        public void InvokeAnimation(bool makeBig)
        {
            (makeBig ? MakeBig : MakeSmall)?.Invoke(this, new EventArgs());
        }

        public event EventHandler MakeBig;
        public event EventHandler MakeSmall;

        #endregion

        public AlarmModel()
        {
        }

        public AlarmModel(AlarmMode alarmMode, object ringCondition)
        {
            if (GlobalVars.IsUpdating) return;

            Mode = alarmMode;

            switch (alarmMode)
            {
                case AlarmMode.ByETA:
                    TtgRingCondition = (AlarmTime)ringCondition;
                    break;
                case AlarmMode.ByDTG:
                    DtgRingCondition = (int)ringCondition;
                    break;
                case AlarmMode.BySimZulu:
                    SimZuluRingCondition = (AlarmTime)ringCondition;
                    break;
                case AlarmMode.ByRealZulu:
                    RealZuluRingCondition = (AlarmTime)ringCondition;
                    break;
            }
        }

        private void alarmActivated()
        {

        }

        [XmlIgnoreAttribute]
        public EventHandler RingConditionChanged;
        private void ringConditionChanged()
        {
            RingConditionChanged?.Invoke(this, new EventArgs());
        }


        public bool CheckRing()
        {

            if (!AlarmActive || WaitingForReset) return false;
            return CheckCondition();
        }

        public bool CheckCondition()
        {
            switch (Mode)
            {
                case AlarmMode.ByETA:
                    if (!GlobalVars.GlobalFlightViewModel.IsFlightSet ||
                        !GlobalVars.GlobalFlightViewModel.CanShowEta ||
                        GlobalVars.GlobalFlightViewModel.IsDepSameAsArrival || 
                        !GlobalVars.GlobalFlightDataModel.IsConnected) return false;

                    var ttgNow = Math.Round(GlobalVars.GlobalFlightViewModel.MinutesToGo);
                    if (TtgRingCondition.ToMinutes() >= ttgNow)
                        return true;
                    break;
                case AlarmMode.ByDTG:
                    if (!GlobalVars.GlobalFlightViewModel.IsFlightSet ||
                        GlobalVars.GlobalFlightViewModel.IsDepSameAsArrival ||
                        !GlobalVars.GlobalFlightDataModel.IsConnected) return false;

                    var dtg = Math.Round(GlobalVars.GlobalFlightViewModel.Dtg);
                    if (DtgRingCondition >= dtg)
                        return true;
                    break;
                case AlarmMode.BySimZulu:
                    if (!GlobalVars.GlobalFlightDataModel.IsConnected || GlobalVars.GlobalFlightDataModel.PausedOrInMenu) return false;

                    if (SimZuluRingCondition.ToMinutes() ==
                        GlobalVars.GlobalFlightDataModel.SimZulu.Hour * 60 +
                        GlobalVars.GlobalFlightDataModel.SimZulu.Minute)
                        return true;
                    break;
                case AlarmMode.ByRealZulu:
                    if (RealZuluRingCondition.ToMinutes() ==
                        DateTime.UtcNow.Hour * 60 +
                        DateTime.UtcNow.Minute)
                        return true;
                    break;
            }

            return false;
        }

        public void AlarmRinged()
        {
            if (AlarmRepeats)
            {
                WaitingForReset = true;
                waitForReset();
            }
            else AlarmActive = false;
        }

        private async void waitForReset()
        {
            while (CheckCondition())
            {
                await Task.Delay(3000);
            }

            await Task.Delay(100);

            WaitingForReset = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
