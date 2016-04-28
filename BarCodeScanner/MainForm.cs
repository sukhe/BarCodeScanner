using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;
using System.Net;
using System.IO;
using Microsoft.WindowsMobile;
using Microsoft.WindowsMobile.Status;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace BarCodeScanner
{

    public partial class MainForm : Form
    {

        // >> часть кода, необходимая для вызова Reset
        [DllImport("coredll.dll", SetLastError = true)]
        static extern int SetSystemPowerState(string psState, int StateFlags, int Options);
        const int POWER_FORCE = 4096;
        const int POWER_STATE_RESET = 0x00800000;
        // << 

        # region Variables - объявление переменных

        /// <summary>
        /// Структура в памяти, хранящая во время работы все отгрузочные документы, имеющиеся в сканере
        /// </summary>
        public static List<CargoDoc> cargodocs = new List<CargoDoc>();
        /// <summary>
        /// Структура в памяти, хранящая данные из файла настроек settings.xml
        /// </summary>
        public static Settings settings = new Settings();
        /// <summary>
        /// Список всех отгрузочных документов (файлов в подкаталоге doc)
        /// </summary>
        public static string[] doclist;
        /// <summary>
        /// Путь в файловой системе к каталогу программы
        /// </summary>
        public static string CurrentPath;
        /// <summary>
        /// Класс для низкоуровнего сканирования штрихкодов, запускаемый в отдельном потоке
        /// </summary>
        CasioScanner cs;
        /// <summary>
        /// Делегат для потокобезопасного вызова из cs процедуры addBarCode, 
        /// срабатывающей в основном потоке программы при сканировании штрихкода
        /// </summary>
        public delegate void AddScan(string s);
        /// <summary>
        /// Процедура в основном потоке программы, которая вызывается из потока cs при сканировании штрихкода
        /// </summary>
        public AddScan addBarCode;
        /// <summary>
        /// Режим сканирования штрихкодов - документ, продукция и т.д.
        /// </summary>
        public static ScanMode scanmode;
        /// <summary>
        /// Отображаемая на экране таблица отгрузочных документов 
        /// </summary>
        public static DataTable doctable;
        /// <summary>
        /// Номер текущей строки таблицы отгрузочных документов
        /// </summary>
        public static int currentdocrow;
        /// <summary>
        /// Номер текущей колонки таблицы отгрузочных документов
        /// </summary>
        public static int currentdoccol;
        /// <summary>
        /// Форма для работы со списком продукции
        /// </summary>
        public static ProductListForm productlistform;
        /// <summary>
        /// Отображаемая на экране таблица продукции
        /// </summary>
        public static DataTable producttable;
        /// <summary>
        /// Форма для работы со списком штрихкодов
        /// </summary>
        public static XCodeListForm xcodelistform;
        /// <summary>
        /// Отображаемая на экране таблица штрихкодов
        /// </summary>
        public static DataTable xcodetable;
        /// <summary>
        /// Структура для хранения логов в памяти, при выходе из программы логи записываются на диск
        /// </summary>
        public static List<string> log = new List<string>();
        /// <summary>
        /// Цвет фона для строк с полностью заполненным документом
        /// </summary>
        public static Color fullColor;
        /// <summary>
        /// Цвет фона для строк с частично заполненным документом
        /// </summary>
        public static Color partialColor;
        /// <summary>
        /// Счётчик последовательности нажатия клавиш для входа в сервисный режим
        /// </summary>
        private int serviceKeySequence;

        # endregion

        # region Load and Close MainForm - загрузка и выгрузка основной формы

        /// <summary>
        /// Конструктор для основной формы приложения.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            if (TestFilesAndDirs())
            {
                CargoDoc cargodoc = new CargoDoc();

                cs = new CasioScanner();
                cs.Scanned += OnScan;
                cs.Open();
                addBarCode = new AddScan(BarCodeProcessing);
                dataGrid1.Focus();
            }
            else Close();
        }

        /// <summary>
        /// Обработчик события загрузки основной формы
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            serviceKeySequence = 0;
            if (LoginForm.Dialog() == DialogResult.Abort) Close(); // проверка пароля
            else
            {
                if (LoadAllDataFromXml())
                {
                    if (doctable == null)
                        doctable = new DataTable();
                    dataGrid1.DataSource = doctable;
                    CreateScreenTable();
                }
                MainForm.scanmode = ScanMode.Doc;

                Log("Start " + Config.userName + " " + Config.scannerNumber);

                labelInfo.Text = "Ск." + Config.scannerNumber + "/" + Config.userName;
                labelTime.Text = System.DateTime.Now.ToShortDateString().Substring(0, 5) + " " + System.DateTime.Now.ToShortTimeString();
            }
        }

        /// <summary>
        /// Обработчик события закрытия основной формы
        /// </summary>
        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            Log("[MF.Closing] Штатный выход из программы");
            LogSave();
            if (cs != null)
            {
                cs.Scanned -= OnScan;
                cs.Dispose();
            }
            Dispose();
        }

        # endregion

        # region BeforeWork - проверки, обработки перед запуском основной программы

        /// <summary>
        /// Проверяем наличие файла настроек и каталога "doc". Определяем номер сканера.
        /// </summary>
        /// <returns>true - всё нормально, можно запускать программу; false - что-то не в порядке</returns>
        private Boolean TestFilesAndDirs()
        {

            string s = "";

            if ((SystemState.PowerBatteryState & BatteryState.NotPresent) == BatteryState.NotPresent)
                s = "Батарея неисправна!";
            if ((SystemState.PowerBatteryState & BatteryState.Critical) == BatteryState.Critical)
                s = "Критическое состояние батареи!";
            if (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength == BatteryLevel.VeryLow)
                s += "Низкий уровень заряда батареи!";

            CurrentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + @"\";

            if (!File.Exists(CurrentPath + "settings.xml"))
            {
                WakeUp1C();
                if (!DownloadSettings())
                {
                    LogShow("[MF.TestFilesAndDirs] Файл настроек не найден и не загружен!");
                    return false;
                }
            }
            else
            {
                settings = Settings.LoadFromFile(CurrentPath + "settings.xml");
            }

            if (!Directory.Exists(CurrentPath + "doc"))
                Directory.CreateDirectory(CurrentPath + "doc");

            try
            {
                Config.serverIp = settings.CommonSettings.ServerIP;
                Config.baseUrl = settings.CommonSettings.BaseUrl;
                Config.logLevel = Convert.ToInt16(settings.CommonSettings.LogLevel);
            }
            catch
            {
            }

            GetScannerID();
            if (Config.scannerNumber == "")
            {
                LogShow("[MF.GetScannerID] Неопознанный сканер");
                return false;
            }
            else return true;
        }

        /// <summary>
        /// Определяем номер сканера по мак-адресу сетевого интерфейса, сравнивая его с файлом настроек
        /// </summary>
        public static void GetScannerID()
        {
            foreach (OpenNETCF.Net.NetworkInformation.NetworkInterface ni in OpenNETCF.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Description == "SDIO86861")
                {
                    Config.scannerMac = ni.GetPhysicalAddress().ToString();
                    Config.scannerIp = ni.CurrentIpAddress.ToString();
                    break;
                }
            }

            foreach (Scanner t in settings.Scanners)
            {
                if ((t.MAC == Config.scannerMac) && (Config.scannerMac != ""))
                {
                    Config.scannerNumber = t.Nomer;
                    break;
                }
            }
        }

        /// <summary>
        /// Загружаем в память все имеющиеся на сканере документы
        /// </summary>
        /// <returns>true - файлы загрузились; false - что-то не в порядке</returns>
        private Boolean LoadAllDataFromXml()
        {
            Boolean result = false;
            cargodocs.Clear();
            // файлы документов имеют название вида "НомерДокумента_ДатаДокумента_НомерСканера.xml"
            doclist = Directory.GetFiles(CurrentPath + "doc", "*_*_*.xml");
            string ss = "";
            try
            {
                foreach (string s in doclist)
                {
                    ss = s;
                    try
                    {
                        cargodocs.Add(CargoDoc.LoadFromFile(s));
                    }
                    catch (Exception ex)
                    {
                        LogErr("[MF.LoadAllDataFromXml.Doc] Не открывается документ №" + s.Substring(s.IndexOf(@"\doc\") + 5), ex);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogErr("[MF.LoadAllDataFromXml.Error] ", ex);
                return false;
            }
            return result;
        }

        #endregion

        # region Network - работа с сетью (обмен данными, пинги)

        /// <summary>
        /// Загрузка файла настроек с сервера. Делаем указанное количество попыток.
        /// Критерий правильности загрузки - в полученном конфиге количество пользователей больше нуля
        /// </summary>
        /// <returns>true - настройки загрузились; false - нет</returns>
        public static Boolean DownloadSettings()
        {
            Boolean result = false;
            int repeat = 3;      // количество повторов для считывания настроек по сети

            if (TestConnect1C())
            {
                try
                {
                    string s = RestAPI_GET("http://"+Config.serverIp+ Config.baseUrl + "/Settings");
                    s = MainForm.DeleteNameSpace(s);
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));

                    while (repeat > 0 && result == false)
                    {
                        try
                        {
                            using (var reader = new StringReader(s))
                            {
                                settings = (Settings)serializer.Deserialize(reader);
                            }
                            if (settings.Users.Length > 0)
                            {
                                settings.SaveToFile(CurrentPath + "settings.xml");
                                result = true;
                            }
                            repeat -= 1;
                        }
                        catch (Exception ex)
                        {
                            MainForm.LogErr("[MF.DownloadSettings.1] ", ex);
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MainForm.LogErr("[MF.DownloadSettings.2] ", ex);
                    return false;
                }
                return result;
            }
            else
            {
                MainForm.LogShow("[MF.DownloadSettings.3] Нет связи с сервером");
                return false;
            }
        }

        /// <summary>
        /// Загрузка отгрузочной накладной с сервера
        /// </summary>
        /// <param name="docnum">Строка в формате НомерДокумента_ДатаДокумента_НомерСканера </param>
        /// <returns>true - документ загрузился; false - нет</returns>
        public Boolean DownloadXML(string docnum)
        {
            if (TestConnect1C())
            {
                try
                {
                    string s;
                    int i = 0;
                    do
                    {
                        s = RestAPI_GET("http://" + Config.serverIp + Config.baseUrl + "/CargoDoc/" + docnum);
                        s = DeleteNameSpace(s);
                        s = DeleteNil(s);
                        if (s == "<CargoDoc>") // Пришёл пустой документ. Такое бывает, если не успел отработать запрос к 1С. 
                        {                      // Делаем паузу и повторяем попытку загрузки
                            i++;
                            System.Threading.Thread.Sleep(1000);
                        }
                        else i = 3;
                    } while (i < 3);

                    if (s == "<CargoDoc>")
                    {
                        LogShow("[MF.DownloadXML.1] Получен пустой документ " + docnum);
                        return false;
                    }
                    else
                        // В ответ на запрос 1С может вернуть сообщение об одной из заранее предусмотренных ошибок
                        if ((s.IndexOf("<Error>") > 0) && (s.IndexOf("</Error>") > 0))
                        {
                            string er = s.Substring(s.IndexOf("<Error>") + 7);
                            s = er.Substring(0, er.IndexOf("</Error>"));
                            LogShow("[MF.DownloadXML.3] Ошибка получения документа " + docnum + ": " + s);
                            return false;
                        }
                        else // ошибок нет, делаем разбор документа
                        {
                            try
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));
                                CargoDoc cd = new CargoDoc();
                                using (var reader = new StringReader(s))
                                {
                                    cd = (CargoDoc)serializer.Deserialize(reader);
                                    string x = cd.Number.Trim() + "_" + ConvertToYYYYMMDD(cd.Data) + "_" + Config.scannerNumber;
                                    cd.SaveToFile(CurrentPath + @"doc\" + x + ".xml"); // разобранный документ сохраняем в виде XML файла
                                }
                            }
                            catch (Exception ex)
                            {
                                LogErr("[MF.DownloadXML.2] Документ получен с ошибками ", ex);
                            }

                        }
                    return true;
                }
                catch (Exception ex)
                {
                    LogErr("[MF.DownloadXML.4]", ex);
                    return false;
                }
            }
            else
            {
                MainForm.LogShow("[MF.DownloadXML.5] Нет связи с сервером");
                return false;
            }
        }

        /// <summary>
        /// Отправка отгрузочного документа со сканера на сервер
        /// </summary>
        private void UploadXML()
        {
            if (dataGrid1.VisibleRowCount == 0)
            {
                LogShow("[MF.DocSendNothing]\n Нет документов для отправки");
            }
            else
            {
                WakeUp1C();
                string n = cargodocs[currentdocrow].Number.Trim();
                string t = ConvertToYYYYMMDD(cargodocs[currentdocrow].Data);
                string x = n + "_" + t + "_" + Config.scannerNumber;
                string z = "";
                int xall = cargodocs[currentdocrow].XCodes.Length; // всего штрихкодов в текущем документе

                string pid;
                int i;
                int totalbydoc = 0; // количество неудалённых штрихкодов по текущему документу
                foreach (Product p in cargodocs[currentdocrow].TotalProducts)
                {
                    pid = p.PID;
                    i = 0;
                    foreach (XCode xx in cargodocs[currentdocrow].XCodes)
                    {
                        if (pid == xx.PID && xx.DData == "") i++;
                    }
                    totalbydoc += i;
                    p.ScannedBar = i.ToString();
                }
                cargodocs[currentdocrow].ScannedBar = totalbydoc.ToString();

                string xgood = cargodocs[currentdocrow].ScannedBar;
                if (TestConnect1C())
                { 
                try
                {
                    z = RestAPI_POST_Zip(@"http://" + Config.serverIp + Config.baseUrl + "/CargoDocZip/" + x);
                    if (z.Substring(0, 2) == "->") // маркер, означающий нормальную передачу документа
                    {
                        z = z.Remove(0, 2); // получаем количество штрихкодов, принятых 1С
                        if (Convert.ToInt16(z) == xall)
                        {
                            Log("[MF.DocSended] " + z + "(without deleted " + xgood + ")");
                            if (Convert.ToInt16(xgood) == xall)
                                MessageBox.Show("Отправлено " + xgood + " штрихкодов по документу №" + cargodocs[currentdocrow].Number.Trim());
                            else
                                MessageBox.Show("Отправлено " + xgood + " штрихкодов по документу №" + cargodocs[currentdocrow].Number.Trim() + "(с удалёнными - " + xall.ToString() + ")");
                            try
                            {
                                // после подтверждения передачи, документ на сканере удаляется
                                File.Delete(CurrentPath + @"doc\" + x + ".xml"); 
                                LoadAllDataFromXml();
                                CreateScreenTable();
                                currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
                                currentdocrow = dataGrid1.CurrentCell.RowNumber;
                            }
                            catch (Exception ex)
                            {
                                LogErrShow("[MF.DocNotDeleted] Документ №" + cargodocs[currentdocrow].Number + " не удалён", "Документ №" + cargodocs[currentdocrow].Number + " не удалён", ex);
                            }
                        }
                        else
                        {
                            LogShow("[MF.NotEqualQuantity] Сервер принял " + z + " штрихкодов, а отсканировано " + cargodocs[currentdoccol].ScannedBar);
                        }

                    }
                    else
                    {
                        //LogShow(z);
                    }
                }
                catch (Exception ex)
                {
                    LogErrShow("[MF.DocNotSended] Документ №" + cargodocs[currentdocrow].Number + " не отправлен ", "Документ №" + cargodocs[currentdocrow].Number + " не отправлен ", ex);
                }
                } 
                else
                {
                    LogShow("[MF.DocNotSendPing] Нет связи с сервером");
                }
            }
        }

        /// <summary>
        /// Получение текущего времени с сервера
        /// </summary>
        /// <returns>Дата и время в текстовом виде -> 27.01.2016 14:46:38</returns>
        public static string GetTime()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CustomTime));
                string s = RestAPI_GET("http://" + Config.serverIp + Config.baseUrl + "/Time");
                s = DeleteNameSpace(s);
                CustomTime customtime = new CustomTime();
                using (var reader = new StringReader(s))
                {
                    customtime = (CustomTime)serializer.Deserialize(reader);
                }
                return customtime.Time.ToString();
            }
            catch (Exception ex)
            {
                Log("[MF.GetTime] " + ex);
                return "";
            }
        }

        /// <summary>
        /// Разбудить 1С. Чтобы заранее поднялась из свопа и приготовилась обслуживать запросы.
        /// </summary>
        /// <returns>true - проснулась, false - нет</returns>
        public static Boolean WakeUp1C()
        {
            Boolean result = false;
            int repeat = 3;
            while (repeat > 0 && result == false)
            {
                result = TestConnect1C();
                if (!result)
                {
                    System.Threading.Thread.Sleep(1000);
                    repeat++;
                }
            }
            return result;
        }

        /// <summary>
        /// Проверка связи с 1С
        /// </summary>
        /// <returns>true - связь есть, false - связи нет</returns>
        public static Boolean TestConnect1C()
        {
            string ss;
            try
            { 
                string s = RestAPI_GET("http://" + Config.serverIp + Config.baseUrl + "/Test1C");
                if (s.IndexOf("</string>") >= 0) // если 1С работоспособна, она отвечает XML сообщением, содержащим текст "<string>OK</string>"
                {
                    ss = s.Substring(s.IndexOf(">")+1);
                    s = ss.Substring(0,ss.IndexOf("</string>"));
                    if (s == "OK") return true;
                    else return false;
                } else return false; 
            }
            catch (Exception ex)
            {
                Log("[MF.TestConnect1C] " + ex);
                return false;
            }
        }

        /// <summary>
        /// Проверка доступности сервера по сети.
        /// При этом не проверяется только доступность (ping), но не работоспособность.
        /// </summary>
        /// <param name="serverAddress">IP адрес сервера</param>
        /// <returns>true - сервер доступен, false - нет</returns>
        public static Boolean PingServer(string serverAddress)
        {
            int timeout = 5000;
            Boolean result = false;
            try
            {
                OpenNETCF.Net.NetworkInformation.Ping ping = new OpenNETCF.Net.NetworkInformation.Ping();
                OpenNETCF.Net.NetworkInformation.PingReply reply = ping.Send(serverAddress, timeout);
                if (reply.Status == OpenNETCF.Net.NetworkInformation.IPStatus.Success)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                MainForm.LogErr("[MF.PingServer.Status] ",ex);
            }
            return result;
        }

        /// <summary>
        /// Получение данных с сервера командой GET
        /// </summary>
        /// <param name="url">URL запрашиваемого документа</param> 
        /// <returns>Ответ сервера в виде текстовой строки</returns>
        public static string RestAPI_GET(string url)
        {
            string sb = "";
            try
            {
                MainForm.Log("[MF.RestAPI_GET.Begin]");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 30000;
                request.ContentType = "text/xml;charset=utf-8";
                GlobalProxySelection.Select = new WebProxy("http://" + Config.serverIp.ToString() + ":80");
                //request.Proxy = GlobalProxySelection.GetEmptyWebProxy();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MainForm.Log("[MF.RestAPI_GET.Response]");
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb = reader.ReadToEnd().ToString();
                MainForm.Log("[MF.RestAPI_GET.Reader] Получено " + sb.Length.ToString() + "байт");
                response.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                MainForm.LogErr("[MF.RestAPI_GET.Error] ", ex);
                sb = ex.Message.ToString();
            }
            return sb;
        }

        /// <summary>
        /// Загрузка данных на сервер командой POST.
        /// В ответ приходит или количество загруженных штрихкодов или ошибка в заранее оговоренной форме (см. класс Protocol)
        /// </summary>
        /// <param name="url">URL передаваемого документа в формате БазовыйURLДляОтправкиДанных/НомерДокумента_Дата_НомерСканера </param>
        /// <returns>Ответ сервера - количество загруженных штрихкодов или ошибка</returns>
        private string RestAPI_POST(string url)
        {
            string sb = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 30000;
                request.ContentType = "application/xml;charset=utf-8";
                //request.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                GlobalProxySelection.Select = new WebProxy("http://" + Config.serverIp.ToString() + ":80");

                string postData = cargodocs[currentdocrow].Serialize();
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb = HTTPDecode(reader.ReadToEnd().ToString());

                try
                {
                    Protocol proto = new Protocol();
                    XmlSerializer serializer = new XmlSerializer(typeof(Protocol));
                    using (var r = new StringReader(sb))
                    {
                        proto = (Protocol)serializer.Deserialize(r);
                    }
                    if (proto.Error != null)
                    {
                        Log("[MF.RestAPI_POST.Error] " + sb);
                        MessageBox.Show("[MF.RestAPI_POST.Error] " + proto.Error);
                    }
                    else
                        if (proto.Message != null)
                        {
                            Log("[MF.RestAPI_POST.Result] " + sb);
                            sb = "->" + proto.Message;
                        }
                        else
                        {
                            LogShow("[MF.RestAPI_POST.Error.3]");
                        }
                }
                catch
                {
                    LogShow("[MF.RestAPI_POST.Error.2] " + sb);
                }

                response.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogErr("[MF.RestAPI_POST.GlobalError]", ex);
                sb = ex.Message.ToString();
            }
            return sb;
        }


        /// <summary>
        /// Копирование данных из одного потока в другой (у .Net 3.5 Compact Framework нет встроенных средств копирования потоков)
        /// </summary>
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            input.Position = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

        /// <summary>
        /// Загрузка на сервер командой POST данных, упакованных GZip-ом.
        /// Неупакованные данные не всегда влезают в буфер отправки (max 64KB для .Net 3.5 Compact Framework)
        /// </summary>
        /// <param name="url">URL передаваемого документа в формате БазовыйURLДляОтправкиДанных/НомерДокумента_Дата_НомерСканера </param>         
        /// <returns>Ответ сервера - количество загруженных штрихкодов или ошибка</returns>
        private string RestAPI_POST_Zip(string url)
        {
            string sb = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 30000;
                //request.ContentType = "application/xml;charset=utf-8";
                request.ContentType = "application/zip;charset=utf-8";
                //request.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                GlobalProxySelection.Select = new WebProxy("http://" + Config.serverIp.ToString() + ":80");

                request.AutomaticDecompression = DecompressionMethods.GZip;
                //request.SendChunked = false;
                //request.AllowWriteStreamBuffering = false;
                //request.TransferEncoding = "gzip";

                //                string docid = url.Substring(url.IndexOf("/CargoDocZip/")+13);

                //CargoDoc cargodoc = CargoDoc.LoadFromFile(@"D:\WORK\CASIO\RestClient\72_20160408_03.xml");
                //                string postData = docid+cargodoc.Serialize();
                string postData = cargodocs[currentdocrow].Serialize();
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);


                //                using (FileStream fs = File.OpenWrite(CurrentPath + @"doc\_" + cargodocs[currentdocrow].Number.ToString() + ".xml"))
                //                {
                //                    Byte[] info = new UTF8Encoding(true).GetBytes();
                //                    fs.Write(byteArray, 0, byteArray.Length);
                //                }

                //request.ContentLength = byteArray.Length;
                //Stream dataStream = request.GetRequestStream();

                //              вот эти 4 строчки вместо последующих двух - посылка зипованного документа
                request.Headers.Add("Content-Encoding", "gzip");


                //dataStream.Position = 0;
                MemoryStream ms = new MemoryStream();
                MemoryStream ms2 = new MemoryStream();

                /*                using (FileStream fileStream = File.Open(@"D:\WORK\CASIO\RestClient\62_20160401_03.gz", FileMode.Create))
                                { */

/*                using (GZipStream zipStream = new GZipStream(ms, CompressionMode.Compress))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(postData);
                    zipStream.Write(buffer, 0, buffer.Length);
                    ms.Position = 0;
                    CopyStream(ms, ms2);
                }*/


//public static byte[] Compress(byte[] raw)
//    {
	using (MemoryStream memory = new MemoryStream())
	{
	    using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
	    {
		    gzip.Write(byteArray, 0, byteArray.Length);
	    }
        CopyStream(memory, ms2);
//	    return memory.ToArray();
	}
//    }



// >> unzip test

                ms2.Position = 0;
                byte[] byteArray2 = ms2.GetBuffer();
                request.ContentLength = byteArray2.Length;
                Stream dataStream = request.GetRequestStream();

                dataStream.Write(byteArray2, 0, byteArray2.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb = HTTPReplace(reader.ReadToEnd().ToString());

                try
                {
                    Protocol proto = new Protocol();
                    XmlSerializer serializer = new XmlSerializer(typeof(Protocol));
                    using (var r = new StringReader(sb))
                    {
                        proto = (Protocol)serializer.Deserialize(r);
                    }
                    if (proto.Error != null)
                    {
                        Log("[MF.RestAPI_POSTZ.Error] " + sb);
                        MessageBox.Show("[MF.RestAPI_POSTZ.Error] " + proto.Error);
                    }
                    else
                        if (proto.Message != null)
                        {
                            Log("[MF.RestAPI_POSTZ.Result] " + sb);
                            sb = "->" + proto.Message; // вставляем маркер, что всё ОК
                        }
                        else
                        {
                            LogShow("[MF.RestAPI_POSTZ.Error.3]");
                        }
                }
                catch
                {
                    LogShow("[MF.RestAPI_POSTZ.Error.2] " + sb);
                }
                response.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogErr("[MF.RestAPI_POSTZ.GlobalError]", ex);
                sb = ex.Message.ToString();
            }
            return sb;

        }

        # endregion

        #region DataGrid - загрузка данных в таблицу, обработка событий, раскраска

        /// <summary>
        /// Загрузка данных в экранную таблицу отгрузочных документов из соответствующей структуры в памяти
        /// </summary>
        public void ReloadDocTable()
        {
            doctable.Rows.Clear();
            foreach (CargoDoc d in cargodocs)
            {
                doctable.Rows.Add(new object[] { d.Number.Trim(), ConvertToDDMMYY(d.Data), d.Partner, d.Quantity, d.ScannedBar });
            }
            doctable.AcceptChanges();
        }

        /// <summary>
        /// Формирование экранной таблицы отгрузочных документов
        /// </summary>
        private void CreateScreenTable()
        {
            fullColor = new Color();
            partialColor = new Color();
            partialColor = Color.FromArgb(255, 127, 127);
            fullColor = Color.FromArgb(127, 255, 127);

            if (doctable.Columns.Count == 0)
            {

                DataColumn Number = doctable.Columns.Add("№ док.", typeof(string));
                doctable.Columns.Add("Дата", typeof(string));
                doctable.Columns.Add("Контрагент", typeof(string));
                doctable.Columns.Add("Надо", typeof(string));
                doctable.Columns.Add("Уже", typeof(string));

                // Set the ID column as the primary key column.
                doctable.PrimaryKey = new DataColumn[] { Number };
            }

            ReloadDocTable();

            // описываем колонки
            dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();

            DataGridTextBoxColumnColored col0 = new DataGridTextBoxColumnColored();
            col0.Width = 80;
            col0.MappingName = doctable.Columns[0].ColumnName;
            col0.HeaderText = doctable.Columns[0].ColumnName;
            col0.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col0);

            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            col1.Width = 90;
            col1.MappingName = doctable.Columns[1].ColumnName;
            col1.HeaderText = doctable.Columns[1].ColumnName;
            col1.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            if (MainForm.cargodocs.Count > 9) 
              col2.Width = 236;
            else 
              col2.Width = 252;
            col2.MappingName = doctable.Columns[2].ColumnName;
            col2.HeaderText = doctable.Columns[2].ColumnName;
            col2.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            col3.Width = 0;
            col3.MappingName = doctable.Columns[3].ColumnName;
            col3.HeaderText = doctable.Columns[3].ColumnName;
            tableStyle.GridColumnStyles.Add(col3);

            DataGridTextBoxColumn col4 = new DataGridTextBoxColumn();
            col4.Width = 0;
            col4.MappingName = doctable.Columns[4].ColumnName;
            col4.HeaderText = doctable.Columns[4].ColumnName;
            tableStyle.GridColumnStyles.Add(col4);

            dataGrid1.TableStyles.Add(tableStyle);

            doctable.AcceptChanges();
        }

        /// <summary>
        /// Определяем класс для работы с цветными строками экранной таблицы отгрузочных документов
        /// </summary>
        public class DataGridTextBoxColumnColored : DataGridTextBoxColumn
        {
            /// <summary>
            /// Определим класс аргумента события, делегат и само событие, необходимые для "общения" кода выполняющего прорисовку ячейки, 
            /// с кодом, предоставляющим цвет для этой ячейки. 
            /// </summary>
            public class NeedBackgroundEventArgs : EventArgs
            {
                private int FRowNum;
                private Brush FBackBrush;
                private Brush FForeBrush;
                private CurrencyManager FSource;

                public int RowNum { get { return FRowNum; } }
                public Brush BackBrush { get { return FBackBrush; } set { FBackBrush = value; } }
                public Brush ForeBrush { get { return FForeBrush; } set { FForeBrush = value; } }
                public CurrencyManager Source { get { return FSource; } }

                public NeedBackgroundEventArgs(CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush)
                {
                    this.FRowNum = rowNum;
                    this.FBackBrush = BackBrush;
                    this.FForeBrush = foreBrush;
                    this.FSource = source;
                }
            }
            public delegate void NeedBackgroundEventHandler(object sender, NeedBackgroundEventArgs e);
            public event NeedBackgroundEventHandler NeedBackground;

            /// <summary>
            /// Переопределенный метод DataGridTextBoxColumn.Paint(), запрашивающий при помощи события цвет и передающий его 
            /// базовому методу Paint(), в параметре backBrush. 
            /// Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, используя при этом подставленный нами backBrush. 
            /// </summary>
            protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
            {
                NeedBackgroundEventArgs e = new NeedBackgroundEventArgs(source, rowNum, backBrush, foreBrush);
                if (NeedBackground != null) NeedBackground(this, e);
                base.Paint(g, bounds, source, rowNum, e.BackBrush, e.ForeBrush, alignToRight);
            }
        }

        /// <summary>
        /// Раскрашивание в нужный цвет ячеек экранной таблицы отгрузочных документов
        /// в зависимости от степени готовности документа
        /// </summary>
        private void OnBackgroundEventHandler(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {
            Color fullColor = new Color();
            Color partialColor = new Color();

            partialColor = Color.FromArgb(255, 127, 127);
            fullColor = Color.FromArgb(127, 255, 127);

            e.ForeBrush = new SolidBrush(Color.Black);

            int q = Convert.ToInt16(doctable.Rows[e.RowNum][3]);
            int b = Convert.ToInt16(doctable.Rows[e.RowNum][4]);

            if ((b < q) && (b != 0))
                e.BackBrush = new SolidBrush(partialColor);
            else if (b >= q)
                e.BackBrush = new SolidBrush(fullColor);
            else
                e.BackBrush = new SolidBrush(Color.White);
        }

        /// <summary>
        /// При щелчке на строке таблицы (или нажатии Enter) вызывается форма со списком продукции этого отгрузочного документа
        /// </summary>
        private void dataGrid1_Click(object sender, EventArgs e)
        {
            if (dataGrid1.VisibleRowCount != 0)
            {
                currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
                currentdocrow = dataGrid1.CurrentCell.RowNumber;

                productlistform = new ProductListForm();
                productlistform.ShowDialog();
                LogSave();
            }
            MainForm.scanmode = ScanMode.Doc;
        }

        /// <summary>
        /// Если перешли на другую ячейку - нужно обновить переменные, хранящие её позицию
        /// </summary>
        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
            currentdocrow = dataGrid1.CurrentCell.RowNumber;
        }

        #endregion

        #region ButtonClick - обработка нажатия клавиш

        /// <summary>
        /// Обработка нажатий аппаратных клавиш
        /// </summary>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                MainForm.scanmode = ScanMode.BarCod;
                dataGrid1_Click(sender, e);
                serviceKeySequence = 0; 
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
                serviceKeySequence = 0;
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                panel3_Click(this, e);
                serviceKeySequence = 0;
            }

            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                button3_Click(this, e);
                serviceKeySequence = 0;
            } 
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
                serviceKeySequence = 0;
            }

            // отработка нажатия .1111. - запуск сервисной формы
            if ((e.KeyCode.GetHashCode() == 190) && (serviceKeySequence == 0))
            {
                serviceKeySequence++;
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.D1) && (serviceKeySequence >= 1) && (serviceKeySequence <= 4))
            {
                serviceKeySequence++;
            }
            if ((e.KeyCode.GetHashCode() == 190) && (serviceKeySequence == 5)) // запуск сервисной формы по .1111.
            {
                MainForm.scanmode = ScanMode.Nothing;
                serviceKeySequence = 0;
                ServiceForm serv = new ServiceForm();
                serv.ShowDialog();
                LoadAllDataFromXml();
                CreateScreenTable();
                currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
                currentdocrow = dataGrid1.CurrentCell.RowNumber;
                MainForm.scanmode = ScanMode.Doc;
            }
            // отработка нажатия ..11.. - перезагрузка
            if ((e.KeyCode.GetHashCode() == 190) && (serviceKeySequence == 3))
            {
                SoftReset();
            }

        }

        /// <summary>
        /// Выгрузка документов из сканера на сервер
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Nothing;
            UploadXML();
            MainForm.scanmode = ScanMode.Doc;
        }

        /// <summary>
        /// Выбор направления перемещения продукции
        /// </summary>
        private void panel3_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Nothing;
            FromToForm f = new FromToForm();
            f.ShowDialog();
            MainForm.scanmode = ScanMode.Doc;
            labelFrom.Text = Config.transferFrom;
            labelTo.Text = Config.transferTo;
        }
       
        /// <summary>
        /// Выход из программы
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        # region Scan - обработка событий сканирования штрихкодов

        /// <summary>
        /// Проверка того, что такой штрихкод уже сканировался (имеется в документах на сканере)
        /// </summary>
        /// <param name="barcod">Номер штрихкода</param>
        /// <returns>true - такой штрихкод уже сканировался, false - ещё не сканировался</returns>
        public static Boolean isXCodePresent(string barcod) 
        {
            foreach (XCode x in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
            {
                if (x.ScanCode == barcod && x.DData == "") return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление отсканированного штрихкода в таблицу на экране и стуктуры в памяти
        /// </summary>
        /// <param name="barcod">Номер штрихкода</param>
        public static void ScanBarCode(string barcod)
        {
            string bar = barcod.Substring(0, 5); // первые 5 символов штрихкода - тип продукции
            Boolean find_product = false;
            int i = 0;

            if (isXCodePresent(barcod))
            {
                MainForm.Attention();
                MainForm.LogShow("Штрихкод " + barcod + " уже есть в этом документе "); // предупреждаем
                return;
            }

            try
            {
                // Ищем в документе продукцию, соответствующую штрихкоду
                foreach (Product p in MainForm.cargodocs[MainForm.currentdocrow].TotalProducts)
                {
                    if (p.PID == bar)
                    {
                        find_product = true;

                        // Если уже набрано нужное количество штрихкодов - не даём добавлять
                        if (Convert.ToInt16(p.ScannedBar) == Convert.ToInt16(p.Quantity))
                        {
                            MainForm.Attention();
                            MainForm.LogShow("Не добавлено! Уже достаточно продукции с кодом " + bar);
                        }
                        else
                        {
                            if (Convert.ToInt16(p.ScannedBar) == Convert.ToInt16(p.Quantity) - 1)
                            {
                                MainForm.Attention(); // предупреждаем, что больше сканировать не нужно - это последний штрихкод по этому типу продукции
                                MainForm.LogShow("Достигнуто необходимое количество продукции с кодом " + bar);
                            }

                            MainForm.cargodocs[MainForm.currentdocrow].TotalProducts[i].ScannedBar = (Convert.ToInt16(p.ScannedBar) + 1).ToString();
                            MainForm.cargodocs[MainForm.currentdocrow].ScannedBar = (Convert.ToInt16(MainForm.cargodocs[MainForm.currentdocrow].ScannedBar) + 1).ToString();
                            MainForm.producttable.Rows[MainForm.currentdocrow].ItemArray[3] = (Convert.ToInt16(p.ScannedBar) + 1).ToString();

                            XCode x = new XCode();

                            x.Data = ConvertToFullDataTime(System.DateTime.Now.ToString());
                            x.FIO = Config.userName;
                            x.DData = "";
                            x.DFIO = "";
                            x.PID = p.PID;
                            x.ScanCode = barcod;
                            x.ScanFrom = Config.transferFromLid;
                            x.ScanTo = Config.transferToLid;
                            x.ScannerID = Config.scannerNumber;

                            var xl = new List<XCode>();
                            xl.AddRange(MainForm.cargodocs[MainForm.currentdocrow].XCodes);
                            xl.Add(x);
                            MainForm.cargodocs[MainForm.currentdocrow].XCodes = xl.ToArray();

                            if (MainForm.xcodelistform != null && MainForm.xcodelistform.Visible)
                            {
                                MainForm.xcodetable.AcceptChanges();
                                MainForm.xcodelistform.ReloadXCodeTable();
                                MainForm.xcodelistform.Refresh();
                            }
                            else
                            {
                                MainForm.producttable.AcceptChanges();
                                MainForm.productlistform.ReloadProductTable();
                                MainForm.productlistform.Refresh();
                            }
                            Application.DoEvents();
                            i++;
                            break;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MainForm.LogErr("[MF.ScanBarCode]", ex);
            }
            if (!find_product)
            {
                MainForm.Attention();
                MainForm.LogShow("В этом заказе нет продукции с кодом " + bar);
            }
        }

        /// <summary>
        /// Вызов обработчика в основном потоке при возникновении события считывания штрихкода в потоке cs
        /// </summary>
        private void OnScan(object sender, ScannedDataEventArgs e)
        {
            this.Invoke(addBarCode, (e.Data).ToString());
        }

        /// <summary>
        /// Считанный штрихкод может быть штрихкодом товара или штрихкодом отгрузочного документа.
        /// Здесь производится определение того, что это за штрихкод, и вызов соответствующих процедур
        /// </summary>
        /// <param name="code">Номер штрихкода</param>
        public void BarCodeProcessing(string code)
        {
            string barcod = "";
            switch (scanmode)
            {
                case (ScanMode.Doc): // открыта форма со списком документов, значит считываем штрихкод с номером документа
                    WakeUp1C();
                    // сканер иногда добавляет после номера штрихкода произвольные символы, поэтому обрезаем лишнее
                    if (code.Length > 15) barcod = code.Substring(0, 15);
                    else barcod = code;
                    string docnum = barcod.Replace(" ", "_") + "_" + Config.scannerNumber;

                    // проверяем, нет-ли уже такого документа, чтобы не затереть отсканированные ранее штрихкоды
                    // в принципе, 1С не должна-бы отдавать документ, если он уже загружен, но лучше проверить
                    if (doclist.Contains(CurrentPath + @"doc\" + docnum + ".xml")) 
                    {
                        int space = barcod.IndexOf(" ");
                        string data_raw = barcod.Remove(0, space);
                        string data = data_raw.Substring(7, 2) + "." + data_raw.Substring(5, 2) + "." + data_raw.Substring(3, 2);
                        LogShow("Документ № " + barcod.Substring(0, space) + " от " + data + " уже загружен!");
                    }
                    else
                    {
                        if (TestConnect1C()) 
                        {
                            SetTime(GetTime());
                            if (DownloadXML(docnum))
                            {
                                LoadAllDataFromXml();
                                ReloadDocTable();
                            }
                        }
                        else
                        {
                            LogShow("[MF.BarCodeProcessing] 1C не отвечает!");
                        }
                    }
                    break;
                case (ScanMode.BarCod): // открыта форма со списком продукции, значит считываем штрихкод изделия
                    if (code.Length > 15) barcod = code.Substring(0, 15);
                    else barcod = code;
                    // отгрузка по одному документу может происходить в разных местах, поэтому нужно выбрать откуда/куда грузим
                    if (Config.transferFromLid == "" || Config.transferToLid == "")
                    {
                        MainForm.Attention();
                        LogShow("[MF.BarCodeProcessing] Не выбрано откуда/куда грузится товар. Штрихкод не добавлен");
                    }
                    else
                    {
                        ScanBarCode(barcod);
                    }
                    break;
                case (ScanMode.Nothing): // показываем считанный штрихкод без обработки (например, если мы находимся в сервисном режиме)
                    MessageBox.Show(code);
                    break;
            }
        }

        # endregion

        # region Log - протоколирование событий

        /// <summary>
        /// Тройной писк и срабатывание вибромотора, чтобы пользователь не пропустил сообщение
        /// </summary>
        public static void Attention()
        {
            MainForm.Speaker();
            MainForm.Vibration();
            System.Threading.Thread.Sleep(100);
            MainForm.Speaker();
            MainForm.Vibration();
            System.Threading.Thread.Sleep(100);
            MainForm.Speaker();
            MainForm.Vibration();
        }

        /// <summary>
        /// Включение вибромотора в режиме ALARM
        /// </summary>
        public static void Vibration()
        {
            Calib.SystemLibNet.Api.SysPlayVibrator(Calib.SystemLibNet.Def.B_ALARM, Calib.SystemLibNet.Def.VIBRATOR_DEFAULT, Calib.SystemLibNet.Def.VIBRATOR_DEFAULT, Calib.SystemLibNet.Def.VIBRATOR_DEFAULT);
        }

        /// <summary>
        /// Писк динамика в режиме WARNING
        /// </summary>
        public static void Speaker()
        {
            Calib.SystemLibNet.Api.SysPlayBuzzer(Calib.SystemLibNet.Def.B_WARNING, Calib.SystemLibNet.Def.BUZ_DEFAULT, Calib.SystemLibNet.Def.BUZ_DEFAULT);
        }

        /// <summary>
        /// Добавление в лог-файл сообщения с отметкой времени и распарсенного сообщения об ошибке.
        /// Также выдаётся сообщение об ошибке на экран
        /// </summary>
        /// <param name="LogString">Строчка, которая запишется в лог-файл</param>
        /// <param name="МessageString">Строчка, которая отобразится на экране</param>
        /// <param name="ex">Исключение (exception)</param>
        public static void LogErrShow(string LogString, string MessageString, Exception ex)
        {
            if (ex.InnerException == null)
            {
                MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString + " " + ex.Message);
            }
            else
            {
                MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString + " " + ex.Message + " " + ex.InnerException.Message);
            }
            MessageBox.Show(MessageString);
        }

        /// <summary>
        /// Добавление в лог-файл сообщения с отметкой времени и распарсенного сообщения об ошибке.
        /// </summary>
        /// <param name="LogString">Строчка, которая запишется в лог-файл</param>
        /// <param name="ex">Исключение (exception)</param>         
        public static void LogErr(string LogString, Exception ex)
        {
            if (ex.InnerException == null)
            {
                MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString + " " + ex.Message);
            }
            else
            {
                MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString + " " + ex.Message + " " + ex.InnerException.Message);
            }
        }

        /// <summary>
        /// Добавление в лог-файл строки с отметкой времени
        /// </summary>
        /// <param name="LogString">Строчка, которая запишется в лог-файл</param>         
        public static void Log(string LogString)
        {
            MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString);
        }

        /// <summary>
        /// Добавление в лог-файл строки с отметкой времени и показ такого-же сообщения во всплывающем окне
        /// </summary>
        /// <param name="LogString">Строчка, которая запишется в лог-файл</param> 
        public static void LogShow(string LogString)
        {
            MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString);
            MessageBox.Show(LogString);
        }

        /// <summary>
        /// Сохранение лог-файла на диск
        /// </summary>
        public static void LogSave()
        {
            using (StreamWriter w = File.AppendText(CurrentPath + "log.txt"))
            {
                foreach (string s in log)
                {
                    w.WriteLine(s);
                }
            }
            log.Clear();

            FileInfo f = new FileInfo(CurrentPath + "log.txt");
            if (f.Length > Config.maxLogSize) // обрезаем лог-файл
            {
                File.Copy(CurrentPath + "log.txt", CurrentPath + "log_old.txt", true);
                File.Delete(CurrentPath + "log.txt");
            }

        }

        #endregion

        #region Utilities - маленькие вспомогательные функции

        /// <summary>
        /// Преобразование отметки времени из формата "2015-11-18T14:50:17+02:00" в формат "18.11.15"
        /// </summary>
        /// <param name="s">Строка даты в формате "2015-11-18T14:50:17+02:00"</param>
        /// <returns>Строка даты в формате "18.11.15"</returns>
        public static string ConvertToDDMMYY(string s)
        {
            string ss;
            try
            {
                ss = s.Substring(8, 2) + '.' + s.Substring(5, 2) + '.' + s.Substring(2, 2);
            }
            catch
            {
                ss = "";
            }
            return ss;
        }

        /// <summary>
        /// Преобразование отметки времени из формата "2015-11-18T14:50:17+02:00" в формат "20151118"
        /// </summary>
        /// <param name="s">Строка даты в формате "2015-11-18T14:50:17+02:00"</param>
        /// <returns>Строка даты в формате "20151118"</returns>
        public static string ConvertToYYYYMMDD(string s)
        {
            string ss;
            try
            {
                ss = s.Substring(0, 4) + s.Substring(5, 2) + s.Substring(8, 2);
            }
            catch
            {
                ss = "";
            }
            return ss;
        }

        /// <summary>
        /// Преобразование отметки времени из формата "27.01.16 14:27:48" в формат "2016-01-27T14:27:48+02:00"
        /// (исходное значение часов может состоять из одной или двух цифр)
        /// </summary>
        /// <param name="s">Строка даты в формате "27.01.16 14:27:48"</param>
        /// <returns>Строка даты в формате "2016-01-27T14:27:48+02:00"</returns>
        public static string ConvertToFullDataTime(string s)
        {
            string ss;
            try
            {
                ss = "20" + s.Substring(6, 2) + "-" + s.Substring(3, 2) + "-" + s.Substring(0, 2) + "T" + s.Substring(9) + @"+02:00";
            }
            catch
            {
                ss = "";
            }
            return ss;
        }

        /// <summary>
        /// Преобразование отметки времени из формата "2015-11-18T14:50:17+02:00" в формат "18.11.15 14:50:17"
        /// </summary>
        /// <param name="s">Строка даты в формате "2015-11-18T14:50:17+02:00"</param>
        /// <returns>Строка даты в формате "18.11.15 14:50:17"</returns>
        public static string ConvertToDDMMYYhhmmss(string s)
        {
            string ss;
            try
            {
                if (s[12] == ':')
                {
                    ss = s.Substring(8, 2) + '.' + s.Substring(5, 2) + '.' + s.Substring(2, 2) + ' ' +
                        s.Substring(11, 1) + ':' + s.Substring(13, 2) + ':' + s.Substring(16, 2);
                } else {
                    ss = s.Substring(8, 2) + '.' + s.Substring(5, 2) + '.' + s.Substring(2, 2) + ' ' +
                        s.Substring(11, 2) + ':' + s.Substring(14, 2) + ':' + s.Substring(17, 2);
                }
            }
            catch
            {
                ss = "";
            }
            return ss;
        }

        /// <summary>
        /// Удаляет из XML определения пространств имён.
        /// Применять только один раз, иначе удаляет больше чем надо
        /// </summary>
        /// <param name="s">Строка в формате XML</param> 
        /// <returns>Строка XML без пространства имён</returns>
        public static string DeleteNameSpace(string s)
        {
            string ss = "";
            if (s.IndexOf("<Cargo") >= 0)
            {
                ss = s.Substring(s.IndexOf("<Cargo"));
            }
            else
                if (s.IndexOf("<Settings") >= 0)
                {
                    ss = s.Substring(s.IndexOf("<Settings"));
                }
                else
                    if (s.IndexOf("<CustomTime") >= 0)
                    {
                        ss = s.Substring(s.IndexOf("<CustomTime"));
                    };
            if (ss != "")
            {
                string begin = ss.Substring(0, ss.IndexOf(" "));
                return begin + ">" + ss.Substring(ss.IndexOf(">") + 1);
            }
            return "";
        }

        /// <summary>
        /// Заменяет в XML значения nil на ""
        /// </summary>
        /// <param name="s">Строка в формате XML</param> 
        /// <returns>Строка XML без nil-ов</returns>
        private string DeleteNil(string s)
        {
            return s.Replace(@" i:nil=""true""", "");
        }

        /// <summary>
        /// Заменяет HTTP-шные коды знаков "меньше" и "больше" на обычные символы
        /// </summary>
        /// <param name="input">Строка</param> 
        /// <returns>Строка</returns>
        private string HTTPDecode(string input)
        {
            string ss = input.Replace("&lt;", "<");
            input = ss.Replace("&gt;", ">");
            ss = input.Substring(input.IndexOf("<string>") + 8);
            input = ss.Substring(0, ss.Length - 9);
            return input;
        }

        /// <summary>
        /// Заменяет HTTP-шные коды знаков "меньше" и "больше" на обычные символы; удаляет табуляции и переводы строк
        /// </summary>
        /// <param name="input">Строка</param> 
        /// <returns>Строка</returns>
        private string HTTPReplace(string input)
        {
            string ss = input.Replace("&lt;", "<");
            input = ss.Replace("&gt;", ">");
            ss = input.Replace("\t", "");
            input = ss.Replace("\n", "");
            ss = input.Substring(0,input.Length-9);
            input = ss.Substring(ss.IndexOf(">")+1);
            return input;
        }

        /// <summary>
        /// Устанавливает системное время
        /// </summary>
        /// <param name="s">Строка даты в формате </param> 
        public static void SetTime(string s)
        {
            if (s == "") return;
            int year, month, day, hour, min, sec;
            string[] ss = s.Split(' ');
            string[] data = ss[0].Split('.');
            string[] time = ss[1].Split(':');
            year = Convert.ToInt16(data[2]);
            month = Convert.ToInt16(data[1]);
            day = Convert.ToInt16(data[0]);
            hour = Convert.ToInt16(time[0]);
            min = Convert.ToInt16(time[1]);
            sec = Convert.ToInt16(time[2]);
            DateTime dt = new DateTime(year, month, day, hour, min, sec);
            OpenNETCF.WindowsCE.DateTimeHelper.LocalTime = dt;
        }

        /// <summary>
        /// Если нужно - добавляет впереди нолик до двухсимвольной строки
        /// </summary>
        /// <param name="i">Строка вида "01" или "1"</param> 
        /// <returns>Строка вида "01"</returns>         
        public static string AddZeroIfNeed(int i)
        {
            string s = i.ToString();
            if (s.Length == 1)
                return "0" + s;
            else return s;
        }

        /// <summary>
        /// Периодически вызывается из таймера. Обновляет значение времени на экране и проверяет состояние батареи
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            labelTime.Text = System.DateTime.Now.ToShortDateString().Substring(0, 5) + " " + System.DateTime.Now.ToShortTimeString();

            if (System.DateTime.Now.Minute % 5 == 0 ) // раз в 5 минут сообщать о заряде батареи
            if ((SystemState.PowerBatteryState & BatteryState.Charging) != BatteryState.Charging)
            {
                if ((SystemState.PowerBatteryState & BatteryState.NotPresent) == BatteryState.NotPresent)
                {
                    MainForm.Attention();
                    MessageBox.Show("Батарея неисправна!");
                }
                if ((SystemState.PowerBatteryState & BatteryState.Critical) == BatteryState.Critical)
                {
                    MainForm.Attention();
                    MessageBox.Show("Критическое состояние батареи!");
                }
                if (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength == BatteryLevel.VeryLow)
                {
                    MainForm.Attention();
                    MessageBox.Show("Низкий заряд батареи. Поставьте сканер на подзарядку.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                };
            }
        }

        /// <summary>
        /// Вызывает форму для ручного ввода номера документа
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Nothing;
            serviceKeySequence = 0;
            DocNumEnter doc = new DocNumEnter();
            doc.Owner = this;
            this.Tag = "";
            doc.ShowDialog();
            MainForm.scanmode = ScanMode.Doc;
            if (this.Tag.ToString() != "")
            {
                BarCodeProcessing(this.Tag.ToString());
                this.Tag = "";
            }
        }

        /// <summary>
        /// Перезагрузка сканера (Reset)
        /// </summary>
        public static void SoftReset()
        {
            SetSystemPowerState(null, POWER_STATE_RESET, POWER_FORCE);
        }

        #endregion

    }
}

/*
.
if ((e.KeyCode == System.Windows.Forms.Keys.Decimal) && (serviceKeySequence == 0))
RButton | MButton | Back | ShiftKey | Space | F17  = 2+4+8+16+32+128
e.KeyCode.ToString()	"Back, Menu, LMenu"	string = 8+18+164
e.KeyCode.GetHashCode()	190	int

System.Windows.Forms.Keys.Decimal = 110


1
e.KeyCode.GetHashCode()	49	int
e.KeyCode.ToString()	"D1"	string


Fn
e.KeyCode.ToString()	"227"	string
e.KeyCode.GetHashCode()	227	int


Fn+A
e.KeyCode.GetHashCode()	16	int
e.KeyCode.ToString()	"ShiftKey"	string

A
e.KeyCode.ToString()	"228"	string
e.KeyCode.GetHashCode()	228	int



CLR
e.KeyCode.GetHashCode()	8	int
e.KeyCode.ToString()	"Back"	string

Fn+CLR
e.KeyCode.ToString()	"Escape"	string
e.KeyCode.GetHashCode()	27	int
*/