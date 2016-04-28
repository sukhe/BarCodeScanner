using System;
using System.Collections.Generic;

namespace BarCodeScanner
{
    /// <summary>
    /// Event arguments for the <see cref="IBarcodeScanner.Scanned"/> event
    /// </summary>
    public class ScannedDataEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ScannedDataEventArgs"/>
        /// </summary>
        /// <param name="data"></param>
        public ScannedDataEventArgs(string data)
        {
            Data = data;
        }

        /// <summary>
        /// Gets the scanned data
        /// </summary>
        /// <remarks>
        /// Some scanners are capable of reading several barcodes at once,
        /// if the scanner is not capable of this then the barcode is obviously
        /// stored in the first index of the string array (<see cref="Data"/>[0])
        /// </remarks>
        public string Data { get; private set; }
    }
}