using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace FlyChrono2.CustomControls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl, INotifyPropertyChanged
    {

        public enum numberValueType
        {
            Integer = 1,
            Decimal = 2,
            Percentage = 3,
            Pixels = 4,
            Degrees = 5
        }
        #region dependencyProperties

        public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(Convert.ToDouble(1)));


        public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(Convert.ToDouble(10)));

        public static readonly DependencyProperty CurrentValueProperty =
DependencyProperty.Register("CurrentValue", typeof(double), typeof(NumericUpDown), new PropertyMetadata(Convert.ToDouble(1), (o, e) => ((NumericUpDown)o).setVal()));

        public static readonly DependencyProperty ValueTypeProperty =
DependencyProperty.RegisterAttached("ValueType", typeof(numberValueType), typeof(NumericUpDown), new PropertyMetadata(numberValueType.Integer));

        public static readonly DependencyProperty UpDownButtonsVisibleProperty =
            DependencyProperty.Register("UpDownButtonsVisible", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));

        public double MaxValue
        {
            get => (Double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public double MinValue
        {
            get => (Double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double CurrentValue
        {
            get => (Double)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }

        public bool buttonsVisible
        {
            get => (bool)GetValue(UpDownButtonsVisibleProperty);
            set => SetValue(UpDownButtonsVisibleProperty, value);
        }

        public numberValueType ValueType
        {
            get => (numberValueType)GetValue(ValueTypeProperty);
            set => SetValue(ValueTypeProperty, value);
        }

        #endregion



        public NumericUpDown()
        {
            InitializeComponent();

        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
            setVal();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (buttonsVisible == false)
            {
                RightGrid.Visibility = Visibility.Collapsed;
                RightColumnDefinition.Width = new GridLength(0);

                MainGrid.Resources["textBoxThickness"] = new Thickness(1, 1, 1, 1);
                MainGrid.Resources["textBoxCornerRadius"] = new CornerRadius(4, 4, 4, 4);
            }

            if (loaded == false)
            {
                setVal();
            }

            loaded = true;
        }
        bool loaded = false;
        private double currentNumber
        {


            get; set;
        }

        public event EventHandler onValueChanged;

        public void setVal() // sets the current value to the textbox
        {
            // Fires the onValueChanged event when the value is changed

            if (loaded)
            {
                onValueChanged?.Invoke(this, new EventArgs());
            }

            if (ValueType == numberValueType.Percentage)
            {
                double currentValPercent = Math.Round(CurrentValue * 100);
                NumberTextBox.Text = currentValPercent.ToString() + "%";
            }
            else if (ValueType == numberValueType.Pixels)
            {
                NumberTextBox.Text = CurrentValue.ToString() + "px";
            }
            else if (ValueType == numberValueType.Degrees)
            {
                NumberTextBox.Text = CurrentValue.ToString() + "°";
            }
            else
            {
                NumberTextBox.Text = CurrentValue.ToString();
            }
        }

        public void numberUpPressed(object sender, RoutedEventArgs e)
        {
            numberUp();

        }

        public void numberUp()
        {
            if (ValueType == numberValueType.Integer || ValueType == numberValueType.Degrees)
            {
                if (CurrentValue < MaxValue)
                {
                    CurrentValue++;
                    setVal();
                }
            }
            else if (ValueType == numberValueType.Decimal || ValueType == numberValueType.Pixels)
            {
                var newVal = Math.Round(CurrentValue + 1);
                if (newVal <= MaxValue)
                {
                    CurrentValue = newVal;
                    setVal();
                }
                else
                {
                    CurrentValue = MaxValue;
                    setVal();
                }
            }
            else if (ValueType == numberValueType.Percentage)
            {
                if (CurrentValue < MaxValue)
                {
                    CurrentValue = CurrentValue + 0.01;
                    setVal();
                }
            }
        }

        public void numberDownPressed(object sender, RoutedEventArgs e)
        {
            numberDown();
        }

        public void numberDown()
        {
            if (ValueType == numberValueType.Integer || ValueType == numberValueType.Degrees)
            {
                if (CurrentValue > MinValue)
                {
                    CurrentValue--;
                    setVal();
                }
            }
            else if (ValueType == numberValueType.Decimal || ValueType == numberValueType.Pixels)
            {
                var newVal = Math.Round(CurrentValue - 1);
                if (newVal >= MinValue)
                {
                    CurrentValue = newVal;
                    setVal();
                }
                else
                {
                    CurrentValue = MinValue;
                    setVal();

                }
            }
            else if (ValueType == numberValueType.Percentage)
            {
                if (CurrentValue > MinValue)
                {
                    CurrentValue = CurrentValue - 0.01;
                    setVal();
                }
            }
        }

        public void verifyNumber()
        {

            if (NumberTextBox.Text == "")
            {
                CurrentValue = 0;
                setVal();
            }
            else
            {
                try
                {
                    switch (ValueType)
                    {
                        case numberValueType.Integer:
                            {
                                double curVal1 = Convert.ToDouble(NumberTextBox.Text); // this var is mainly used for comparison
                                double curVal = Math.Round(curVal1);
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    setVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    setVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    setVal();
                                }

                                break;
                            }
                        case numberValueType.Decimal:
                            {
                                double curVal = Convert.ToDouble(NumberTextBox.Text); // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    setVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    setVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    setVal();
                                }

                                break;
                            }
                        case numberValueType.Percentage:
                            {
                                string curValStr = NumberTextBox.Text.Replace("%", "");
                                double curVal = Convert.ToDouble(curValStr) / 100; // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    setVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    setVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    setVal();
                                }

                                break;
                            }
                        case numberValueType.Pixels:
                            {
                                string curValStrTemp = NumberTextBox.Text.Replace("p", "");
                                string curValStr = curValStrTemp.Replace("x", "");
                                double curVal = Convert.ToDouble(curValStr); // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    setVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    setVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    setVal();
                                }

                                break;

                            }
                        case numberValueType.Degrees:
                            {
                                string curValStr = NumberTextBox.Text.Replace("°", "");

                                double curVal = Convert.ToDouble(curValStr); // this var is mainly used for comparison
                                if (curVal > MaxValue)
                                {
                                    CurrentValue = MaxValue;
                                    setVal();
                                }
                                else if (curVal < MinValue)
                                {
                                    CurrentValue = MinValue;
                                    setVal();
                                }
                                else
                                {
                                    CurrentValue = curVal;
                                    setVal();
                                }

                                break;
                            }

                    }
                }
                catch (Exception)
                {
                    NumberTextBox.Text = tempValue;
                }
            }
        }
        private string tempValue;

        public void setValue(object sender, RoutedEventArgs e)
        {
            verifyNumber();
        }

        public void setTempValue(object sender, RoutedEventArgs e)
        {

            tempValue = NumberTextBox.Text;
        }

        private void keyPressed(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                verifyNumber();
            }

        }

        private readonly Regex _regex = new Regex("[0-9.,-]+"); //regex that matches disallowed text
        private readonly Regex _regexInt = new Regex("[0-9-]+"); //regex that matches disallowed text
        private readonly Regex _regexPercent = new Regex("[0-9,.-^%]*$"); //regex that matches disallowed text
        private readonly Regex _regexDegree = new Regex("^[0-9-+°+-]*$");
        private readonly Regex _regexPixel = new Regex("^[0-9-+,.p-p+x-x]*$");
        private bool IsTextAllowed(string text)
        {
            switch (ValueType)
            {
                case numberValueType.Decimal:
                    return _regex.IsMatch(text);
                case numberValueType.Integer:
                    return _regexInt.IsMatch(text);
                case numberValueType.Percentage:
                    return _regexPercent.IsMatch(text);
                case numberValueType.Degrees:
                    return _regexDegree.IsMatch(text);
                case numberValueType.Pixels:
                    {
                        return _regexPixel.IsMatch(text);
                    }

            }
            return false;
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            e.Handled = !IsTextAllowed(e.Text);
        }

        #region styling

        private string _resourceName = "UniversalBorderColor";
        private bool _isSelected = false;

        private void ColorSelectionTextbox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            _isSelected = true;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorDark2Brush"];
        }

        private void ColorSelectionTextbox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _isSelected = false;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorBrush"];
        }

        private void ColorSelectionTextbox_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_isSelected) return;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorDark1Brush"];
        }

        private void ColorSelectionTextbox_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_isSelected) return;
            MainGrid.Resources[_resourceName] = Application.Current.Resources["BorderColorBrush"];
        }

        #endregion

        private void previewKeyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) numberUp();
            if (e.Key == Key.Down) numberDown();
        }
    }
}
