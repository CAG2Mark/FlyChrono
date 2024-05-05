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
