using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlyChrono2.CustomControls
{
    /// <summary>
    /// Interaction logic for airportFinder.xaml
    /// </summary>
    public partial class AirportFinder : Window, INotifyPropertyChanged
    {
        public AirportFinder()
        {
            InitializeComponent();
        }

        #region Dialog Methods

        private void DialogCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void DialogOK(object sender, RoutedEventArgs e)
        {
            if (AirportsListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an airport.");
                return;
            }

            selectedAirport = (airportItem)AirportsListBox.SelectedItem;
            DialogResult = true;
        }

        #endregion

        #region properties

        private List<airportItem> _airportItemsList;

        public List<airportItem> airportItemsList
        {
            get => _airportItemsList;
            set { _airportItemsList = value; OnPropertyChanged(); }
        }

        public airportItem selectedAirport { get; set; }

        #endregion

        public void listAirports(string param)
        {
            var addedIcaos = new List<string>();
            var __airportItemsList = new List<airportItem>();

            if (!File.Exists(@"C:\CAG2 Software\FlyChrono\airports.csv"))
            {
                MessageBox.Show(
                    "The airport database file was deleted and/or could not be found. Please re-install the application.");

                return;
            }

            string line;
            var file =
                new System.IO.StreamReader(@"C:\CAG2 Software\FlyChrono\airports.csv");
            while ((line = file.ReadLine()) != null)
            {
                var lineSplit = line.Split(',');
                var actualAirportName = lineSplit[3];


                // search airport name
                if (actualAirportName.ToLower().Contains(param.ToLower()))
                {
                    int index;

                    var newAirportItem = new airportItem();
                    newAirportItem.airportIcao = lineSplit[1].Replace(@"""", "");
                    newAirportItem.airportName = actualAirportName.Replace(@"""", "");

                    switch (lineSplit[2].Replace(@"""", ""))
                    {
                        case "heliport":
                            index = 0;
                            break;
                        case "small_airport":
                            index = 1;
                            break;
                        case "medium_airport":
                            index = 2;
                            break;
                        case "large_airport":
                            index = 3;
                            break;
                        default:
                            index = 0;
                            break;
                    }
                    newAirportItem.index = index;

                    addedIcaos.Add(lineSplit[1].Replace(@"""", ""));
                    __airportItemsList.Add(newAirportItem);
                }

                // search airport icao
                if (lineSplit[1].ToLower().Contains(param.ToLower()))
                {
                    int index;

                    var newAirportItem = new airportItem();
                    newAirportItem.airportIcao = lineSplit[1].Replace(@"""", "");
                    newAirportItem.airportName = actualAirportName.Replace(@"""", "");

                    switch (lineSplit[2].Replace(@"""", ""))
                    {
                        case "heliport":
                            index = 0;
                            break;
                        case "small_airport":
                            index = 1;
                            break;
                        case "medium_airport":
                            index = 2;
                            break;
                        case "large_airport":
                            index = 3;
                            break;
                        default:
                            index = 0;
                            break;
                    }
                    newAirportItem.index = index;

                    if (!addedIcaos.Contains(lineSplit[1].Replace(@"""", "")))
                    {
                        addedIcaos.Add(lineSplit[1].Replace(@"""", ""));
                        __airportItemsList.Add(newAirportItem);
                    }
                }

                // search airport city
                if (lineSplit[10].ToLower().Contains(param.ToLower()))
                {
                    int index;

                    var newAirportItem = new airportItem();
                    newAirportItem.airportIcao = lineSplit[1].Replace(@"""", "");
                    newAirportItem.airportName = actualAirportName.Replace(@"""", "");

                    switch (lineSplit[2].Replace(@"""", ""))
                    {
                        case "heliport":
                            index = 0;
                            break;
                        case "small_airport":
                            index = 1;
                            break;
                        case "medium_airport":
                            index = 2;
                            break;
                        case "large_airport":
                            index = 3;
                            break;
                        default:
                            index = 0;
                            break;
                    }
                    newAirportItem.index = index;

                    if (!addedIcaos.Contains(lineSplit[1].Replace(@"""", "")))
                    {
                        addedIcaos.Add(lineSplit[1].Replace(@"""", ""));
                        __airportItemsList.Add(newAirportItem);
                    }
                }
            }

            airportItemsList = __airportItemsList.OrderByDescending(d => d.index).ToList();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonSearch(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Length > 2)
            {
                listAirports(SearchBox.Text);
            }
            else
            {
                MessageBox.Show("The search term must be over 2 characters in length.");
            }
        }

        private void SearchBoxPressedKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SearchBox.Text.Length > 2)
                {
                    listAirports(SearchBox.Text);
                }
                else
                {
                    MessageBox.Show("The search term must be over 2 characters in length.");
                }
            }
        }

        private void AirportsListBox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AirportsListBox.SelectedItem == null)
            {
                return;
            }

            selectedAirport = (airportItem)AirportsListBox.SelectedItem;
            DialogResult = true;
        }
    }

    public class airportItem
    {
        public string airportName { get; set; }
        public string airportIcao { get; set; }

        public int index { get; set; }
    }
}
