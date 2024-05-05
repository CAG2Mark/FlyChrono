using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for ExceptionWindow.xaml
    /// </summary>
    public partial class ExceptionWindow : Window , INotifyPropertyChanged
    {
        private string _exception;

        public string exception
        {
            get => _exception;
            set { _exception = value; OnPropertyChanged(); }
        }

        private string direc = @"C:\CAG2 Software\FlyChrono\Logs";

        public ExceptionWindow()
        {
            InitializeComponent();
        }

        public ExceptionWindow(Exception ex)
        {
            InitializeComponent();

            var exStr = ex.ToString();
            exception = exStr;


            if (!Directory.Exists(direc)) Directory.CreateDirectory(direc);

            var time = DateTime.UtcNow;

            var fileName = time.Year + "-" + time.Month + "-" + time.Day + " " + time.Hour + " " + time.Minute + " " +
                           time.Second + "z";

            File.WriteAllText(direc + @"\" + fileName + ".txt", exStr);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RestartApp(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsRestarting = true;
            Properties.Settings.Default.Save();

            Process.Start(Application.ResourceAssembly.Location);

            Application.Current.Shutdown();



        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            Process.Start(direc);
        }

        private void CopyError(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(exception);
            MessageBox.Show("Error copied to clipboard.");
        }
    }
}

