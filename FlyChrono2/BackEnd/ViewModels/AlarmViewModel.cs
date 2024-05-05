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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace FlyChrono2.BackEnd.ViewModels
{
    public class AlarmViewModel : INotifyPropertyChanged
    {
        #region Properties

        private ObservableCollection<AlarmModel> _alarms = new ObservableCollection<AlarmModel>();
        public ObservableCollection<AlarmModel> Alarms
        {
            get => _alarms;
            set { _alarms = value; OnPropertyChanged();}
        }

        private AlarmModel _preAlarmModel = new AlarmModel();
        public AlarmModel PreAlarmModel
        {
            get => _preAlarmModel;
            set { _preAlarmModel = value; OnPropertyChanged();}
        }

        public bool NoAlarms
        {
            get => Alarms.Count == 0;
        }

        #endregion

        #region events

        public EventHandler AlarmAdded;
        public EventHandler AlarmFailedToAdd;

        #endregion

        public AlarmViewModel()
        {
            if (GlobalVars.IsUpdating) return;

            Deserialize();
        }

        #region commands
        private bool _canExecute = true;

        private RelayCommand _confirmAlarmCommand;
        public ICommand ConfirmAlarmCommand
        {
            get
            {
                if (_confirmAlarmCommand == null)
                {
                    _confirmAlarmCommand = new RelayCommand(param => this.ConfirmAlarm(),
                        param => _canExecute);
                }
                return _confirmAlarmCommand;
            }
        }

        private RelayCommand _deleteAlarmCommand;
        public ICommand DeleteAlarmCommand
        {
            get
            {
                if (_deleteAlarmCommand == null)
                {
                    _deleteAlarmCommand = new RelayCommand(param => this.DeleteAlarm(param),
                        param => _canExecute);
                }
                return _deleteAlarmCommand;
            }
        }
        #endregion

        #region delete alarm

        public void DeleteAlarm(object param)
        {
            var alarmToDelete = (AlarmModel)param;
            Alarms.Remove(alarmToDelete);

            OnPropertyChanged(nameof(NoAlarms));

            Serialize();
        }

        #endregion

        #region add alarm

        public void ConfirmAlarm()
        {
            if (!string.IsNullOrEmpty(PreAlarmModel.AlarmName))
            {
                Alarms.Add(PreAlarmModel);
                PreAlarmModel = new AlarmModel();

                // update the no alarms text
                OnPropertyChanged(nameof(NoAlarms));

                Serialize();

                AlarmAdded.Invoke(this, new EventArgs());

                Console.WriteLine(Alarms.Count);

                return;
            }

            AlarmFailedToAdd.Invoke(this, new EventArgs());
        }

        #endregion

        #region serialisation

        public void Serialize()
        {
            
            try
            {
                Console.WriteLine("serializing");
                FileStream outFile = File.Create(@"C:\CAG2 Software\FlyChrono" + @"\alarms.xml");
                XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<AlarmModel>));
                formatter.Serialize(outFile, Alarms);

                outFile.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to serialize");
                /*
                MessageBox.Show("Unable to save alarms. The error is as follows:" +
                                Environment.NewLine + Environment.NewLine + ex.ToString());

                return;*/
            }
        }

        public void Deserialize()
        {
            var path = @"C:\CAG2 Software\FlyChrono" + @"\alarms.xml";

            if (!File.Exists(path)) return;

            XmlSerializer serializer =
                new XmlSerializer(typeof(ObservableCollection<AlarmModel>));


            serializer.UnknownNode += (o, e) =>
                MessageBox.Show(
                "Your alarms could not be loaded correctly (Error 1)."
                + Environment.NewLine
                + Environment.NewLine
                + "Error:" + Environment.NewLine
                + "Unknown Attribute");

            serializer.UnknownAttribute += (o, e) =>
                MessageBox.Show(
                "Your alarms could not be loaded correctly (Error 2)."
                + Environment.NewLine
                + Environment.NewLine
                + "Error:" + Environment.NewLine
                + "Unknown Node");

            FileStream fs = new FileStream(path, FileMode.Open);

            Alarms = (ObservableCollection<AlarmModel>)serializer.Deserialize(fs);
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
