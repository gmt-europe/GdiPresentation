using System.Text;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GdiPresentation.Demo
{
    static class Program
    {
        public static RegistryKey BaseKey
        {
            get { return Registry.CurrentUser.CreateSubKey("Software\\GdiPresentation"); }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
