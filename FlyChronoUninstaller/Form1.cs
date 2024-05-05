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
