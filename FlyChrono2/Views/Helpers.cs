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
