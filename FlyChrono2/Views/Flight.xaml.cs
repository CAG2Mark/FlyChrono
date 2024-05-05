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
using FlyChrono2.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for Flight.xaml
    /// </summary>
    public partial class Flight : UserControl
    {
        public Flight()
        {
            if (GlobalVars.IsUpdating) return;

            InitializeComponent();

            DataContext = GlobalVars.GlobalFlightViewModel;
            GlobalVars.GlobalFlightViewModel.FlightDataApplied += (o, e) => 
            {
                DepartureEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FancyRedBrush"];
                FlightIcon.Fill = (SolidColorBrush)Application.Current.Resources["FancyRedBrush"];
                ArrivalEllipse.Fill = (SolidColorBrush)Application.Current.Resources["FancyRedBrush"];
                FlightDispatcherTabControl.SelectedIndex = 0;
            };
        }

        private void FlightDispatcherToggle(object sender, RoutedEventArgs e)
        {
            FlightDispatcherTabControl.SelectedIndex = FlightDispatcherTabControl.SelectedIndex == 0 ? 1 : 0;
        }

        private void CheckInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "^[a-zA-Z0-9-]+$") || e.Text.Contains(" "))
                e.Handled = true;
        }

        private void CallsignChanged(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Contains(",");
        }

        private void FindDep(object sender, RoutedEventArgs e)
        {
            var airportFinderDialog = new AirportFinder();

            if (airportFinderDialog.ShowDialog() == true)
            {
                var ViewModel = (FlightViewModel)DataContext;
                ViewModel.PreDep = airportFinderDialog.selectedAirport.airportIcao.ToUpper();
            }
        }

        private void FindArr(object sender, RoutedEventArgs e)
        {
            var airportFinderDialog = new AirportFinder();

            if (airportFinderDialog.ShowDialog() == true)
            {
                var ViewModel = (FlightViewModel)DataContext;
                ViewModel.PreArr = airportFinderDialog.selectedAirport.airportIcao.ToUpper();
            }
        }

        private void FlightDispatcherTabControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
                e.Handled = true;
        }

        public void MakeSmallWindow()
        {
            Helpers.PlayStoryboard(this, (Storyboard)Resources["SmallWindowFlight"]);
            FlightDispatcherToggleSmall.Visibility = Visibility.Visible;
        }

        public void MakeBigWindow()
        {
            var sb = (Storyboard)Resources["BigWindowFlight"];
            Helpers.PlayStoryboard(this, sb);

            FlightDispatcherToggleSmall.Visibility = Visibility.Collapsed;
        }
    }
}
