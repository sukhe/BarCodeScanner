using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
//using OpenNETCF.ToolHelp;
//using System.Runtime.InteropServices;

namespace BarCodeScanner
{
    static class Program
    {
/*        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);*/

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}