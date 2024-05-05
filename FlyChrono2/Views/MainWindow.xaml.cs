/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using FlyChrono2.BackEnd;
using FlyChrono2.BackEnd.ViewModels;
using FlyChrono2.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace FlyChrono2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public void setTimeout(Action action, int timeout)
        {
            Thread t = new Thread(
                () =>
                {
                    Thread.Sleep(timeout);
                    action.Invoke();
                }
            );
            t.Start();
        }


        public MainWindow()
        {
            //Properties.Settings.Default.Reset();
                if (GlobalVars.IsUpdating)
                {
                    GlobalVars.IsUpdating = true;
                    new UpdateWindow(GlobalVars.NewVersion).Show();
                    Close();
                    return;
                }




            if (!Properties.Settings.Default.FirstTimeHasSetup)
            {
                var setup = new SetupPage();
                setup.Show();

                Close();
                return;
            }

            else
            {

                InitializeComponent();

                Properties.Settings.Default.Reload();

                if (Properties.Settings.Default.SmallWindowDefault)
                {
                    GlobalVars.IsSmallWindow = true;

                    Helpers.PlayStoryboard(this, (Storyboard)Resources["SmallWindow"], true);

                    HomePage.MakeSmallWindow(true);
                    FlightPage.MakeSmallWindow();
                    AlarmsPage.MakeSmallWindow();
                    SettingsPage.MakeSmallWindow();

                    SmallWindowButton.Visibility = Visibility.Collapsed;
                    BigWindowButton.Visibility = Visibility.Visible;

                    Dispatcher.Invoke(() =>
                    {
                        TabControlListBox.Visibility = Visibility.Collapsed;
                    });
                }
            }
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            if (!Properties.Settings.Default.MinimizeWhenClosed) Helpers.ShutdownApp();
            else WindowState = WindowState.Minimized;
        }

        private void ClickMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void MakeWindowSmall(object sender, RoutedEventArgs e)
        {
            GlobalVars.IsSmallWindow = true;

            Helpers.PlayStoryboard(this,(Storyboard)Resources["SmallWindow"]);

            HomePage.MakeSmallWindow();
            FlightPage.MakeSmallWindow();
            AlarmsPage.MakeSmallWindow();
            SettingsPage.MakeSmallWindow();

            SmallWindowButton.Visibility = Visibility.Collapsed;
            BigWindowButton.Visibility = Visibility.Visible;

            await Helpers.Wait(200);

            Dispatcher.Invoke(() =>
            {
                TabControlListBox.Visibility = Visibility.Collapsed;
            });
        }

        private async void MakeWindowBig(object sender, RoutedEventArgs e)
        {
            GlobalVars.IsSmallWindow = false;

            Helpers.PlayStoryboard(this,(Storyboard)Resources["BigWindow"]);

                HomePage.MakeBigWindow();
            FlightPage.MakeBigWindow();
            AlarmsPage.MakeBigWindow();
            SettingsPage.MakeBigWindow();

            SmallWindowButton.Visibility = Visibility.Visible;
            BigWindowButton.Visibility = Visibility.Collapsed;

            await Helpers.Wait(600);

            Dispatcher.Invoke(() =>
            {
                TabControlListBox.Visibility = Visibility.Visible;
            });


        }

        private void PageChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlListBox.SelectedIndex == 2)
            {
                TimePage.RefreshTime();
            } 
        }
    }
}
