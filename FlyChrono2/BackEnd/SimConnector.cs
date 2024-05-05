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
using FSUIPC;

namespace FlyChrono2.BackEnd
{

    public class SimConnection
    {
        #region Offsets

        Offset<int> groundSpeedOffset = new FSUIPC.Offset<int>(0x2B4);
        Offset<long> altitudeOffset = new FSUIPC.Offset<long>(0x570);

        Offset<byte> zuluHourOffset = new FSUIPC.Offset<byte>(0x23B);
        Offset<byte> zuluMinOffset = new FSUIPC.Offset<byte>(0x23C);
        Offset<byte> zuluSecOffset = new Offset<byte>(0x23A);

        Offset<short> simYearOffset = new Offset<short>(0x240);
        Offset<short> yearDayOffset = new Offset<short>(0x23E);

        Offset<long> latitudeOffset = new FSUIPC.Offset<long>(0x560);
        Offset<long> longitudeOffset = new FSUIPC.Offset<long>(0x568);

        Offset<short> onGroundOffset = new FSUIPC.Offset<short>(0x366);

        Offset<byte> inMenuOffset = new Offset<byte>(0x3365);
        Offset<short> pausedOffset = new Offset<short>(0x264);
        Offset<short> simRateOffset = new Offset<short>(0xC1A);

        #endregion

        /// <summary>
        /// Constructor for the SimConnection.
        /// </summary>
        public SimConnection()
        {
            // close connection to avoid a double connection
            closeConnection();
        }

        /// <summary>
        /// Tries to connect to FSUIPC.
        /// </summary>
        /// <returns>This is whether the application is connected to FSUIPC after this method has been ran.</returns>
        public bool tryConnect()
        {
            if (!Properties.Settings.Default.FirstTimeHasSetup) return false;
                
            try
            {
                if (FSUIPCConnection.IsOpen)
                {
                    return true;
                }
                FSUIPCConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                if (FSUIPCConnection.IsOpen)
                {
                    return true;
                }

                closeConnection();
                return false;
               
                
            }
        }

        /// <summary>
        /// Tries to close the connection.
        /// </summary>
        public void closeConnection()
        {
            if (FSUIPCConnection.IsOpen)
            {
                try
                {
                    FSUIPCConnection.Close();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Populates a given timeSyncModel's properties.
        /// </summary>
        /// <param name="timeSyncModel">The model to populate.</param>
        public void PopulateModel(FlightDataModel timeSyncModel)
        {

            timeSyncModel.RealZulu = DateTime.UtcNow;

            timeSyncModel.IsConnected = tryConnect();

            if (timeSyncModel.IsConnected)
            {
                try
                {
                    FSUIPCConnection.Process();
                }
                catch (FSUIPCException)
                {
                    closeConnection();
                    return;
                }
                

                timeSyncModel.GroundSpeed =
                    Convert.ToDouble(groundSpeedOffset.Value) / 33714.631111111;

                timeSyncModel.Altitude =
                    ((altitudeOffset.Value * 0.00000001640419947507 / 0.000000005 / (65536.0 * 65536.0) * 100) + 0.5) / 100;

                timeSyncModel.Latitude =
                    latitudeOffset.Value * 90.0 / (10001750.0 * 65536.0 * 65536.0);

                timeSyncModel.Longitude =
                    longitudeOffset.Value * 360.0 / (65536.0 * 65536.0 * 65536.0 * 65536.0);

                timeSyncModel.OnGround =
                    onGroundOffset.Value != 0;

                if (simYearOffset.Value != 0)
                {
                    timeSyncModel.SimZulu =
                        new DateTime(simYearOffset.Value, 1, 1, zuluHourOffset.Value, zuluMinOffset.Value, zuluSecOffset.Value)
                        .AddDays(yearDayOffset.Value - 1);
                }

                timeSyncModel.PausedOrInMenu =
                    inMenuOffset.Value != 0 || pausedOffset.Value != 0 || simRateOffset.Value != 256;

            }
            else
            {
                timeSyncModel.SimZulu =
                    new DateTime(0);
            }

            

        }

        /// <summary>
        /// Sets the second of the simulator to zero.
        /// </summary>
        public void ResetSecond()
        {
            // FSUIPC cannot directly set the second of the sim - it can only set it to 0.
            zuluSecOffset.Value = 0;
        }


        /// <summary>
        /// Sets the minute of the simulator.
        /// </summary>
        /// <param name="minute">The minute to set.</param>
        public void SetMinute(int minute)
        {
            if (minute < 0 || minute > 59)
            {
                throw new Exception("'Minute' can only be between 0 and 59 inclusive.");
            }

            zuluMinOffset.Value = Convert.ToByte(minute);
        }

        /// <summary>
        /// Sets the hour of the simulator.
        /// </summary>
        /// <param name="hour">The minute to set.</param>
        public void SetHour (int hour)
        {
            if (hour < 0 || hour > 23)
            {
                throw new Exception("'Hour' can only be between 0 and 23 inclusive.");
            }

            zuluHourOffset.Value = Convert.ToByte(hour);
        }

        public void SetCalendar(DateTime time)
        {
            simYearOffset.Value = (Int16)time.Year;
            yearDayOffset.Value = (Int16)time.DayOfYear;
        }
    }
}
