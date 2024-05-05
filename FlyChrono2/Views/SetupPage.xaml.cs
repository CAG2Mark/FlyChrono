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
