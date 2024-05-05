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
using System.ComponentModel;
using System.Diagnostics;
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

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for SetupPage.xaml
    /// </summary>
    public partial class SetupPage : Window, INotifyPropertyChanged
    {
        private int _currentPage = 0;
        public int CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        public SetupPage()
        {
            InitializeComponent();
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ClickMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

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

        private void PageForward(object sender, RoutedEventArgs e)
        {
            if (CurrentPage == 1 && FSUIPCCheckBox.IsChecked != true)
            {
                MessageBox.Show("Please confirm you have FSUIPC installed.");
            }
            else
            {
                CurrentPage++;
                updateButtons();

            }
        }

        private void PageBack(object sender, RoutedEventArgs e)
        {
            CurrentPage--;
            updateButtons();
        }

        private void DownloadFSUIPC(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.schiratti.com/dowson.html");
        }

        private void CompleteSetup(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.FirstTimeHasSetup = true;
            Properties.Settings.Default.Save();

            var window = new MainWindow();
            window.Show();

            Close();
        }

        private void updateButtons()
        {
            if (CurrentPage == SetupTabControl.Items.Count - 1) ForwardButton.Visibility = Visibility.Hidden;
            else ForwardButton.Visibility = Visibility.Visible;

            if (CurrentPage == 0) BackButton.Visibility = Visibility.Hidden;
            else BackButton.Visibility = Visibility.Visible;
        }
    }
}
