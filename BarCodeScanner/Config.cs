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
        public static string serverIp = "192.168.10.213";
        public static string baseUrl = "/CargoDocService.svc";
        /// <summary>
        /// Уровень протоколирования: 0 - рабочий режим, 1 - разработка (подробные логи)
        /// </summary>
        public static int logLevel = 1;
        public static string transferFrom = "";
        public static string transferTo = "";
        public static string transferFromLid = "";
        public static string transferToLid = "";
        public static int maxLogSize = 200000;
    }

    /// <summary>
    /// Режим сканирования штрихкода
    /// </summary>
    public enum ScanMode { Doc, BarCod, Nothing }

}
