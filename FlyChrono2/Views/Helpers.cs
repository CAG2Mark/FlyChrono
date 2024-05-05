/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using FlyChrono2.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace FlyChrono2.Views
{
    public static class Helpers
    {
        public static void PlayStoryboard(FrameworkElement sender, Storyboard sb)
        {

            if (Properties.Settings.Default.ReducedAnimation)
            {
                sb.Begin(sender, true);

                sb.Seek(sender, new TimeSpan(0, 0, 0), TimeSeekOrigin.Duration);
            }
            else
            {
                sb.Begin(sender, true);
            }

        }

        public static void PlayStoryboard(FrameworkElement sender, Storyboard sb, bool instant)
        {
            
            if (instant)
            {
                sb.Begin(sender, true);

                sb.Seek(sender, TimeSpan.Zero, TimeSeekOrigin.Duration);
            }
            else
            {
                sb.Begin(sender, true);
            }

        }

        public static async Task Wait(int milliseconds)
        {
            if (!Properties.Settings.Default.ReducedAnimation)
                await Task.Delay(milliseconds);
        }

        public static void ShutdownApp()
        {
            GlobalVars.GlobalAlarmViewModel.Serialize();

            GlobalVars.nIcon.Visible = false;
            GlobalVars.nIcon.Icon.Dispose();
            GlobalVars.nIcon.Dispose();

            Properties.Settings.Default.Save();
            GlobalVars.GlobalTimeSyncViewModel.CloseConnection();
            GlobalVars.GlobalFlightViewModel.WriteClosedText();

            Application.Current.Shutdown();
        }
    }
}
