﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace BarCodeScanner
{
    public static class Config
    {
        public static string scannerNumber = "";
        public static string scannerIp = "";
        public static string scannerMac = "";
        public static string userName = "";
        public static string batterylevel = "";
        public static Boolean superuser = false;
        public static string serverIp = "";
    }

    public enum ScanMode { Doc, BarCod, DelBarCod, Nothing }

}