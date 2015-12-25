using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BarCodeScanner
{
    static class Program
    {
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