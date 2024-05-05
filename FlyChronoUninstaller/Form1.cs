/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlyChronoUninstaller
{
    public partial class Form1 : Form
    {
        public string installPath => @"C:\CAG2 Software\FlyChrono";
        public string exePath => @"C:\CAG2 Software\FlyChrono\FlyChrono.exe";
        public string icoPath => @"C:\CAG2 Software\FlyChrono\FlyChronoicon.ico";
        public string uninstallPath => @"C:\CAG2 Software\Uninstalls\FlyChronouninstaller.exe";

        public Form1()
        {
            InitializeComponent();
            ShowInTaskbar = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }



        private void Timer1_Tick_1(object sender, EventArgs e)
        {
            try
            {
                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                File.Delete(desktopFolder + "\\FlyChrono.lnk");

                string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                File.Delete(startFolder + "\\FlyChrono.lnk");

                Directory.Delete(installPath, true);
            }
            catch
            {

            }

            try
            {
                string InstallerRegLoc = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
                RegistryKey homeKey = (Registry.LocalMachine).OpenSubKey(InstallerRegLoc, true);
                RegistryKey appSubKey = homeKey.OpenSubKey("FlyChrono");
                if (null != appSubKey)
                {
                    homeKey.DeleteSubKey("FlyChrono");
                }
            }
            catch
            {

            }
            timer1.Stop();
            Application.Exit();
        }
    }
}
