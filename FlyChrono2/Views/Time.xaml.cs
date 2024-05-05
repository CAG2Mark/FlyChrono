using FlyChrono2.BackEnd;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for Time.xaml
    /// </summary>
    public partial class Time : UserControl
    {
        public Time()
        {
            if (GlobalVars.IsUpdating) return;

            InitializeComponent();

            DataContext = GlobalVars.GlobalTimeSyncViewModel;
        }

        /// <summary>
        /// Sets the 'set current time' numericupdowns to the current time.
        /// </summary>
        public void RefreshTime()
        {
            var data = GlobalVars.GlobalTimeSyncViewModel;

            CustomHourControl.CurrentValue = data.Model.SimZulu.Hour;
            CustomMinuteControl.CurrentValue = data.Model.SimZulu.Minute;
        }
    }
}
