/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using FlyChrono2.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlyChrono2.CustomControls
{
    [ContentProperty("ToggleBoxContent")]

    /// <summary>
    /// Interaction logic for ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl, INotifyPropertyChanged
    {
        public ToggleSwitch()
        {
            InitializeComponent();
        }

        #region dependencyPropertes

        public static DependencyProperty IsToggledProperty = DependencyProperty.Register("IsToggled", typeof(bool), typeof(ToggleSwitch),
            new PropertyMetadata(false, (o, e) => ((ToggleSwitch)o).toggleChanged()));

        public static DependencyProperty AlternateColorSchemeProperty = DependencyProperty.Register("AlternateColorScheme", typeof(bool), typeof(ToggleSwitch),
    new PropertyMetadata(false, (o, e) => ((ToggleSwitch)o).AlternateColorSchemeChanged()));

        public static DependencyProperty ToggleBoxContentProperty = DependencyProperty.Register("ToggleBoxContent", typeof(object), typeof(ToggleSwitch),
    new PropertyMetadata("", (o, e) => ((ToggleSwitch)o).textChanged()));

        public bool AlternateColorScheme
        {
            get => (bool)GetValue(AlternateColorSchemeProperty);
            set => SetValue(AlternateColorSchemeProperty, value);
        }

        public void AlternateColorSchemeChanged()
        {
            toggleBoxBackgroundTop.Background = AlternateColorScheme ?
                (SolidColorBrush)Application.Current.Resources["BorderColorDark2Brush"] : (SolidColorBrush)Application.Current.Resources["FancyRedBrush"];


        }

        public bool IsToggled
        {
            get
            {
                return (bool)GetValue(IsToggledProperty);
            }
            set
            {
                SetValue(IsToggledProperty, value);
            }
        }
        public object ToggleBoxContent
        {
            get
            {
                return (object)GetValue(ToggleBoxContentProperty);
            }
            set
            {
                SetValue(ToggleBoxContentProperty, value);
            }
        }

        #endregion


        #region callbacks

        public void toggleChanged()
        {
            OnPropertyChanged(nameof(IsToggled));
            animate();
        }

        public void textChanged()
        {
            OnPropertyChanged(nameof(ToggleBoxContent));
        }

        #endregion


        #region toggleEvents
        private void Clicked(object sender, MouseButtonEventArgs e)
        {
            IsToggled = !IsToggled;
            animate();
        }
        #endregion

        #region animation events

        private void animate()
        {
            if (IsToggled) animateIn(); else animateOut();
        }
        private void animateIn()
        {
            Helpers.PlayStoryboard(this, (Storyboard)MainGrid.FindResource("ToggleOn"));
        }

        private void animateOut()
        {
            Helpers.PlayStoryboard(this, (Storyboard)MainGrid.FindResource("ToggleOff"));
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Helpers.PlayStoryboard(this, (Storyboard)MainGrid.FindResource("MouseEnter"));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Helpers.PlayStoryboard(this, (Storyboard)MainGrid.FindResource("MouseLeave"));
        }
    }
}
