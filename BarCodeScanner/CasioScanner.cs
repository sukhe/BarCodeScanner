using System;
using System.Text;
using System.Threading;
using Calib;

namespace BarCodeScanner
{

    public class CasioScanner : IDisposable
    {
        private static readonly object syncLock = new object();
        private volatile bool running;
        private Thread scanningThread;

        /// <summary>
        /// Gets the status of the scanner
        /// </summary>
        public static ScannerStatus Status; // { get; private set; }

        /// <summary>
        /// Event raised when new data has been scanned
        /// </summary>
        public event EventHandler<ScannedDataEventArgs> Scanned;

        /// <summary>
        /// Opens the connection to the scanner
        /// </summary>
        public void Open()
        {
            if (Status == ScannerStatus.Opened)
                return;

            lock (syncLock)
            {
//               var result = OBReadLibNet.Api.OBROpen(Handle.GetValueOrDefault(IntPtr.Zero),
//                                                     OBReadLibNet.Def.OBR_ALL | OBReadLibNet.Def.OBR_OUT_ON | OBReadLibNet.Def.OBR_OUT_ON);
/*               var result = OBReadLibNet.Api.OBROpen(IntPtr.Zero,
                            OBReadLibNet.Def.OBR_ALL | OBReadLibNet.Def.OBR_OUT_ON | OBReadLibNet.Def.OBR_OUT_ON);
               CheckCasioResult(result); */
/*                var result = OBReadLibNet.Api.OBRClose();
                CheckCasioResult(result);*/
                var result = OBReadLibNet.Api.OBRLoadConfigFile(); //ini File read default value set
                CheckCasioResult(result);
                result = OBReadLibNet.Api.OBRSetDefaultSymbology(); //1D(OBR) driver mode will be ini File vallue
                CheckCasioResult(result);
                result = OBReadLibNet.Api.OBRSetScanningKey(OBReadLibNet.Def.OBR_TRIGGERKEY_L | OBReadLibNet.Def.OBR_TRIGGERKEY_R | OBReadLibNet.Def.OBR_CENTERTRIGGER);
                CheckCasioResult(result);
                result = OBReadLibNet.Api.OBRSetScanningCode(OBReadLibNet.Def.OBR_ALL);
                CheckCasioResult(result);
                result = OBReadLibNet.Api.OBRSetBuffType(OBReadLibNet.Def.OBR_BUFOBR);
                //1D(OBR) driver mode will be OBR_BUFOBR
                CheckCasioResult(result);
                result = OBReadLibNet.Api.OBRSetScanningNotification(OBReadLibNet.Def.OBR_EVENT, IntPtr.Zero);
                //result = OBReadLibNet.Api.OBRSetScanningNotification(OBReadLibNet.Def.OBR_EVENT, Handle);
                //1D(OBR) driver mode will be OBR_EVENT
                CheckCasioResult(result);

/*                result = OBReadLibNet.Api.OBRSetBuzzer(OBReadLibNet.Def.OBR_BUZON); //enable sound notification
                CheckCasioResult(result);*/

                result = OBReadLibNet.Api.OBRSetVibrator(OBReadLibNet.Def.OBR_VIBOFF); //enable vibration notification
                CheckCasioResult(result);

/*                if (Handle == null)
                {*/
                result = OBReadLibNet.Api.OBROpen(Handle, 0); //OBRDRV open
                CheckCasioResult(result);
//                }

                result = OBReadLibNet.Api.OBRClearBuff();
                CheckCasioResult(result);

                OBReadLibNet.Api.OBRSaveConfigFile();

                Status = ScannerStatus.Opened;

                scanningThread = new Thread(GetScannerStatusWorker) {IsBackground = true};
                scanningThread.Start();
                scanningThread.Name = "BarCodeScannerBackground";
            }
        }

        /// <summary>
        /// Closes the connection to the scanner
        /// </summary>
        public void Close()
        {
            if (Status != ScannerStatus.Opened)
                return;

            lock (syncLock)
            {
                running = false;
                scanningThread.Abort();
                scanningThread = null;

                var result = OBReadLibNet.Api.OBRClose();
                CheckCasioResult(result);

                Status = ScannerStatus.Closed;
            }
        }

        /// <summary>
        /// Manually initiate the scan
        /// </summary>
        /// <remarks>
        /// The scanned barcode will be received in the <see cref="ScannedDataEventArgs.Data"/>
        /// parameter when the <see cref="IBarcodeScanner.Scanned"/> event is fired
        /// </remarks>
/*        public void Scan()
        {
            throw new NotSupportedException();
        } */

        /// <summary>
        /// The handle to the window in which notifications are sent (required for certain scanner API's)
        /// </summary>
        // public IntPtr Handle { get; set; }
        public static IntPtr Handle;

        /// <summary>
        ///     Clean up resources used by the scanner device
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        private void CheckCasioResult(int p)
        {
            if (p != OBReadLibNet.Def.OBR_OK)
            {
                switch (p)
                {
                    case (OBReadLibNet.Def.OBR_NONDT):
                        throw new Exception("[CS01] Error end");
//                        break;
                    case (OBReadLibNet.Def.OBR_PON):
                        throw new Exception("[CS02] Already open - Перезагрузите сканер кнопкой RESET сзади");
//                        break;
                    case (OBReadLibNet.Def.OBR_POF):
                        throw new Exception("[CS03] Not open");
//                        break;
                    case (OBReadLibNet.Def.OBR_PRM):
                        throw new Exception("[CS04] Parameter error");
//                        break;
                    case (OBReadLibNet.Def.OBR_NOT_DEVICE):
                        throw new Exception("[CS05] OBR Driver(Scanner) device is not available");
//                        break;
                    case (OBReadLibNet.Def.OBR_NOT_DEVICE_DECODE):
                        throw new Exception("[CS06] OBR Driver(decode) device is not available");
//                        break;
                    case (OBReadLibNet.Def.OBR_ERROR_HOTKEY):
                        throw new Exception("[CS07] RegisterHotKey error");
//                        break;
                    default:
                        throw new Exception("[CS08] Unknown error: " + p.ToString("X"));
                }
            }
        }

        private void GetScannerStatusWorker()
        {
            running = true;
            try
            {
                while (running)
                {
                    SystemLibNet.Api.SysWaitForEvent(Handle, OBReadLibNet.Def.OBR_NAME_EVENT, 2000
                        /*timeout SystemLibNet.Def.INFINITE*/); //Wait event

                    int size = 0, code = 0;
                    byte number = 0, len = 0;
                    var result = OBReadLibNet.Api.OBRGetStatus(ref size, ref number);
                    CheckCasioResult(result);

                    if (number > 0)
                    {
                        var buffer = new byte[size];
                        result = OBReadLibNet.Api.OBRGets(buffer, ref code, ref len);
                        CheckCasioResult(result);

                        result = OBReadLibNet.Api.OBRClearBuff();
                        CheckCasioResult(result);

                        if (Scanned != null)
                        {
                            var barcode = Encoding.Default.GetString(buffer, 0, buffer.Length).Trim();
//                            BarcodeTypes bt = (BarcodeTypes)code;
//                            var barcodeData = new BarcodeData {Text = barcode, BarcodeType = bt};
//                            var barcodeData = new BarcodeData { Text = barcode, BarcodeType = DecodeType(code) };
                            //var barcodeData = new BarcodeData { Text = barcode };
                            //Scanned.Invoke(this, new ScannedDataEventArgs(new[] {barcodeData}));
                            //Scanned.Invoke(this, new ScannedDataEventArgs(new[] { barcode }));
                            Scanned.Invoke(this, new ScannedDataEventArgs( barcode ));
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
            }
            finally
            {
                running = false;
                Close();
            }
        }
    }
}