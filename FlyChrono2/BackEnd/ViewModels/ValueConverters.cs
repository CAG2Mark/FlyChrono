using FlyChrono2.BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FlyChrono2.BackEnd.ViewModels
{

    public class RepeatsValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Repeats" : "Rings Once";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectionStatusToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SyncStatus)value)
            {
                case SyncStatus.Disconnected:
                    return new BrushConverter().ConvertFrom("#A30000");
                case SyncStatus.Waiting:
                    return new BrushConverter().ConvertFrom("#ffc400");
                case SyncStatus.Syncing:
                    return new BrushConverter().ConvertFrom("#008a0e");
                case SyncStatus.Synced:
                    return new BrushConverter().ConvertFrom("#3acc00");
            }
            return new BrushConverter().ConvertFrom("#A30000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConnectionStatusToTextValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SyncStatus)value)
            {
                case SyncStatus.Disconnected:
                    return "Not Connected to Simulator";
                case SyncStatus.Waiting:
                    return "Waiting for Simulator";
                case SyncStatus.Syncing:
                    return "Syncing Time to Simulator";
                case SyncStatus.Synced:
                    return "Simulator Time Synced";
            }
            return "Not Connected to Simulator";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TimeToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var time = (DateTime)value;
            return time.Ticks == 0 ? "N/A" : time.ToString("HH:mm:ss");

            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntAlarmModeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AlarmMode)value;
        }

      
    }

    public class BoolToVisiblityValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible;
        }


    }

    public class BoolInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }


    }

}
