/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using FlyChrono2.BackEnd;
using FlyChrono2.BackEnd.Models;
using FlyChrono2.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for Alarms.xaml
    /// </summary>
    public partial class Alarms : UserControl
    {
        public bool AlarmPanelShown { get; set; } = false;
        public Alarms()
        {
            if (GlobalVars.IsUpdating) return;

            InitializeComponent();
            DataContext = GlobalVars.GlobalAlarmViewModel;

            GlobalVars.GlobalAlarmViewModel.AlarmAdded += (o, e) => 
            {
                toggleAddAlarmPanel();
                //Dispatcher.Invoke(() => AlarmsListBox.Items.Refresh());
            };
            GlobalVars.GlobalAlarmViewModel.AlarmFailedToAdd += (o, e) => MessageBox.Show("Please fill in the required fields.");
        }

        private void ShowAlarmPanelClick(object sender, RoutedEventArgs e)
        {
            toggleAddAlarmPanel();
        }

        private void toggleAddAlarmPanel()
        {
            Helpers.PlayStoryboard(this,(Storyboard)MainGrid.Resources[
    AlarmPanelShown ? "CloseAlarmPanel" : "OpenAlarmPanel"]);

            AlarmPanelShown = !AlarmPanelShown;
        }

        private void DeleteAlarm(object sender, EventArgs e)
        {
            var senderPanel = (AlarmPanel)sender;
            var alarm = senderPanel.AssignedAlarm;

            if (GlobalVars.GlobalAlarmViewModel.DeleteAlarmCommand.CanExecute(alarm))
                GlobalVars.GlobalAlarmViewModel.DeleteAlarmCommand.Execute(alarm);

            senderPanel.Dispose();
        }

        private void AddAlarm(object sender, RoutedEventArgs e)
        {
        }


        public void MakeSmallWindow()
        {
            if (AlarmPanelShown) toggleAddAlarmPanel();
            
            Helpers.PlayStoryboard(this, (Storyboard)Resources["SmallWindowAlarms"]);

            foreach(AlarmModel alarm in AlarmsListBox.Items)
            {
                alarm.InvokeAnimation(false);
            }

            //var template = AlarmsListBox.ItemTemplate;
            //var border = (Border)template.FindName("AlarmPanelBorder", AlarmsListBox);
            //BeginStoryboard((Storyboard)border.Resources["AlarmPanelAnimation"]);
        }

        public void MakeBigWindow()
        {

            foreach (AlarmModel alarm in AlarmsListBox.Items)
            {
                alarm.InvokeAnimation(true);
            }

            Helpers.PlayStoryboard(this, (Storyboard)Resources["BigWindowAlarms"]);

        }
        #region mouseewheel speed

        // Courtesy of https://stackoverflow.com/questions/876994/adjust-flowdocumentreaders-scroll-increment-when-viewingmode-set-to-scroll


        private void HandleScrollSpeed(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (!(sender is DependencyObject))
                    return;

                ScrollViewer scrollViewer = (((DependencyObject)sender)).GetScrollViewer() as ScrollViewer;
                ListBox lbHost = sender as ListBox; //Or whatever your UI element is

                if (scrollViewer != null && lbHost != null)
                {
                    double scrollSpeed = 2;
                    //you may check here your own conditions
                    if (lbHost.Name == "SourceListBox" || lbHost.Name == "TargetListBox")
                        scrollSpeed = 2;

                    double offset = scrollViewer.VerticalOffset - (e.Delta * scrollSpeed / 6);
                    if (offset < 0)
                        scrollViewer.ScrollToVerticalOffset(0);
                    else if (offset > scrollViewer.ExtentHeight)
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
                    else
                        scrollViewer.ScrollToVerticalOffset(offset);

                    e.Handled = true;
                }
                else
                    throw new NotSupportedException("ScrollSpeed Attached Property is not attached to an element containing a ScrollViewer.");
            }
            catch (Exception ex)
            {
                //Do something...
            }
        }

        #endregion
    }

    public static class Extensions
    {
        public static DependencyObject GetScrollViewer(this DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }
    }
}
