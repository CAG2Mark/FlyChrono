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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : System.Windows.Controls.UserControl
    {
        public static MenuItem CloseMenuItem = new MenuItem("Close FlyChrono", (o, e) => Helpers.ShutdownApp());
        public static MenuItem ShowMenuItem = new MenuItem("Show FlyChrono", (o, e) => {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
            Application.Current.MainWindow.Activate();
        });

        public static MenuItem[] items = new MenuItem[] { ShowMenuItem, CloseMenuItem };
        ContextMenu menu = new ContextMenu(items);

        public Home()
        {
            if (GlobalVars.IsUpdating) return;

            this.DataContext = GlobalVars.GlobalTimeSyncViewModel;

            InitializeComponent();



            // initialise the tray icon
            nIcon.Icon = Properties.Resources.Icon0;
            nIcon.Visible = true;
            nIcon.Text = "FlyChrono";
            nIcon.ContextMenu = menu;

            nIcon.DoubleClick += (o, e) => {

                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Activate();

            };

            // when the sync status is changed, update the notify icon
            GlobalVars.GlobalTimeSyncViewModel.SyncChanged += syncChanged;
        }

        private void syncChanged(object sender, SyncChangedEventArgs e)
        {

            // null checks - when the app closes, the icons or the notifyicon will may null.
            if (Properties.Resources.Icon0 == null ||
                Properties.Resources.Icon1 == null ||
                Properties.Resources.Icon2 == null ||
                Properties.Resources.Icon3 == null) return;

            if (nIcon == null) return;

            try
            {
                // convert sync status to an icon for the notify icon.
                switch (e.CurrentSyncStatus)
                {
                    case SyncStatus.Disconnected:
                        nIcon.Icon = Properties.Resources.Icon0;
                        break;
                    case SyncStatus.Waiting:
                        nIcon.Icon = Properties.Resources.Icon1;
                        break;
                    case SyncStatus.Syncing:
                        nIcon.Icon = Properties.Resources.Icon2;
                        break;
                    case SyncStatus.Synced:
                        nIcon.Icon = Properties.Resources.Icon3;
                        break;
                }
            }
            catch (Exception) { }
        }

        // miniwindow animations
        public void MakeSmallWindow()
        {
            Helpers.PlayStoryboard(this, (Storyboard)Resources["SmallWindowHome"]);
        }

        public void MakeSmallWindow(bool instant)
        {
            Helpers.PlayStoryboard(this, (Storyboard)Resources["SmallWindowHome"], instant);
        }


        public void MakeBigWindow()
        {
            Helpers.PlayStoryboard(this, (Storyboard)Resources["BigWindowHome"]);
        }

        #region notifyicon

        public System.Windows.Forms.NotifyIcon nIcon { get => GlobalVars.nIcon; set => GlobalVars.nIcon = value; }



        #endregion

    }
}
