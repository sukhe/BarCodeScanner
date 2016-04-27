using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BarCodeScanner
{

    /// <summary>
    /// Класс для хранения текущих параметров сканера
    /// </summary>
    public static class Config
    {
        public static string scannerNumber = "";
        public static string scannerIp = "";
        public static string scannerMac = "";
        public static string userName = "";
//        public static string batterylevel = "";
//        public static Boolean superuser = false;
        public static string serverIp = "192.168.10.213";
//        public static Boolean notping = true;
//        public static string transfer = "";
        public static string transferFrom = "";
        public static string transferTo = "";
        public static string transferFromLid = "";
        public static string transferToLid = "";
        public static int maxLogSize = 200000;
    }

    /// <summary>
    /// Режим сканирования штрихкода
    /// </summary>
    //public enum ScanMode { Doc, BarCod, DelBarCod, Nothing }
    public enum ScanMode { Doc, BarCod, Nothing }
}
