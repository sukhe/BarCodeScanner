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

namespace BarCodeScanner
{

    public partial class MainForm : Form
    {
        public static List<CargoDoc> cargodocs = new List<CargoDoc>();  // структура в памяти, куда загружаются 
                                                                        // все отгрузочные документы, имеющиеся в сканере
        public static Settings settings = new Settings();               // сюда загружаются данные из файла настроек settings.xml
        public static string[] doclist;                                 // список всех отгрузочных документов (файлов в подкаталоге doc)
        public static string CurrentPath;                               // путь в файловой системе к каталогу программы

        CasioScanner cs;                                                // класс сканера, запускаемый в отдельном потоке
        public delegate void AddScan(string s);                         // 
        public AddScan addBarCode;

        public static ScanMode scanmode;                                // режим сканирования штрихкодов - документ, продукция и т.д.

        public static DataTable doctable;
        public static DataTableReader docreader;
        public static int currentdocrow;                                // номер текущей строки таблицы отгрузочных документов
        public static int currentdoccol;                                // номер текущей колонки таблицы отгрузочных документов

        public static ProductListForm productlistform;                  // форма для работы со списком продукции
        public static DataTable producttable;
        public static DataTableReader productreader;

        public static XCodeListForm xcodelistform;                      // форма для работы со списком штрих-кодов
        public static DataTable xcodetable;
        public static DataTableReader xcodereader;

        public static List<string> log = new List<string>();            // структура для хранения логов в памяти, при выходе из программы
                                                                        // записывается на диск
        public static Color fullColor;                                  // цвет фона для строк с полностью заполненным документом
        public static Color partialColor;                               // цвет фона для строк с частично заполненным документом

        private int serviceKeySequence;

        public MainForm()
        {
            InitializeComponent();

/*            var processes = OpenNETCF.ToolHelp.ProcessEntry.GetProcesses();
            foreach (OpenNETCF.ToolHelp.ProcessEntry process in processes)
            {
                if (process.ExeFile == "BarCodeScanner.exe")
                {
                    LogShow("[MF.StartDuplicate] Программа уже запущена!");
                    Close();
                }
            }*/

            if (TestFilesAndDirs())
            {
                CargoDoc cargodoc = new CargoDoc();
                MainForm.scanmode = ScanMode.Doc;

                cs = new CasioScanner();
                cs.Scanned += OnScan;
                cs.Open();
                addBarCode = new AddScan(BarCodeProcessing); // назначение делегата для потокобезопасного вызова процедуры

                //SystemLibNet.Api.SysSetBuzzerVolume(SystemLibNet.Def.B_SCANEND, SystemLibNet.Def.BUZZERVOLUME_MAX);

                dataGrid1.Focus();
            }
            else Close();
        }

/*        private Boolean existDoc(string s)
        {
            return doclist.Contains(s);
        } */

        private void MainForm_Load(object sender, EventArgs e)
        {
            Config.userName = "Мистер Х";
            Config.notping = true;

            serviceKeySequence = 0;
            if (LoginForm.Dialog() == DialogResult.Abort) Close();
            else
            {
/*            Config.scannerNumber = GetScannerID();
            if (Config.scannerNumber == "")
                Close();
            else
            {*/
                if (LoadAllDataFromXml())
                {
                    if (doctable == null)
                        doctable = new DataTable();
                    dataGrid1.DataSource = doctable;
                    GetCustomers();
                }
                Log("Start " + Config.userName + " " + Config.scannerNumber);
                labelInfo.Text = "Ск." + Config.scannerNumber + "/" + Config.userName;
                labelTime.Text = System.DateTime.Now.ToShortDateString().Substring(0, 5) + " " + System.DateTime.Now.ToShortTimeString();
//            }
            }
/*            if (Config.superuser)
            {
                this.BackColor = Color.Coral;
            }*/
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            // сделать сохранение xml ?
            Log("[MF.Closing] Штатный выход из программы");
            LogSave();
            if (cs != null)
            {
                cs.Scanned -= OnScan;
                cs.Dispose();
            }
            Dispose();
        }

        # region BeforeWork - проверки, обработки перед запуском основной программы

        /// <summary>
        /// Проверяем наличие файла настроек и каталога "doc". При отсутствии - загружаем/создаём.
        /// Определяем номер сканера
        /// </summary>
        /// <returns>Истина если всё хорошо; ложь, если что-то не в порядке</returns>
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

            /*            if (!Directory.Exists(CurrentPath+@"\log")
                            Directory.CreateDirectory(CurrentPath+@"\log"); */

            if (!Directory.Exists(CurrentPath + "doc"))
                Directory.CreateDirectory(CurrentPath + "doc");

            Config.serverIp = "192.168.10.213";
//            Config.scannerNumber = 
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
/*            if (number == "")
            {
                LogShow("[MF.GetScannerID] Неопознанный сканер");
                //                Close();
            }
            return number;*/
        }

        /// <summary>
        /// Загружаем в память все имеющиеся на сканере документы
        /// </summary>
        /// <returns>Истина если всё хорошо; ложь, если что-то не в порядке</returns>
        private Boolean LoadAllDataFromXml()
        {
            Boolean result = false;
            cargodocs.Clear();
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
                        LogErr("[MF.LoadAllDataFromXml.Doc] Не открывается документ " + s.Substring(s.IndexOf(@"\doc\") + 5), ex);
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
        /// <returns>Истина если загрузилось; ложь, если нет</returns>
        public static Boolean DownloadSettings()
        {
            Boolean result = false;
            int repeat = 3;      // количество повторов для считывания настроек по сети

            if (Config.notping || PingServer(Config.serverIp))
            {
                try
                {
                    string s = RestAPI_GET("http://"+Config.serverIp+"/CargoDocService.svc/Settings");
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
                MainForm.LogShow("Нет связи с сервером");
                return false;
            }
        }

        /// <summary>
        /// Загрузка отгрузочной накладной с сервера
        /// </summary>
        /// <returns>Истина если загрузилось; ложь, если нет</returns>
        public Boolean DownloadXML(string docnum)
        {
            //            if (GetTime()!="") 
            //string docnum = barcod.Replace(" ", "_") + "_" + Config.scannerNumber;
            if (Config.notping || PingServer(Config.serverIp))
            {
                try
                {
                    string s;
                    //string docnum = barcod.Replace(" ", "_") + "_" + Config.scannerNumber;
                    int i = 0;
//                    GetTime();
                    do
                    {
                        s = RestAPI_GET("http://" + Config.serverIp + "/CargoDocService.svc/CargoDoc/" + docnum);
                        s = DeleteNameSpace(s);
                        s = DeleteNil(s);
                        if (s == "<CargoDoc>")
                        {
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
                        if ((s.IndexOf("<Error>") > 0) && (s.IndexOf("</Error>") > 0))
                        {
                            string er = s.Substring(s.IndexOf("<Error>") + 7);
                            s = er.Substring(0, er.IndexOf("</Error>"));
                            LogShow("[MF.DownloadXML.3] Ошибка получения документа " + docnum + ": " + s);
                            return false;
                        }
                        else
                        {
                            try
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));
                                CargoDoc cd = new CargoDoc();
                                using (var reader = new StringReader(s))
                                {
                                    cd = (CargoDoc)serializer.Deserialize(reader);
                                    cd.SaveToFile(CurrentPath + @"doc\" + docnum + ".xml");
                                }
                            }
                            catch (Exception ex)
                            {
                                LogErr("[MF.DownloadXML.2] Документ получен с ошибками ", ex);
                            }

                            /*                    using (FileStream fs = File.OpenWrite(CurrentPath + @"doc\" + docnum + ".xml"))
                                                {
                                                    Byte[] info = new UTF8Encoding(true).GetBytes(s);
                                                    fs.Write(info, 0, info.Length);
                                                }*/
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
        /// Отправка отгрузочного документа на сервер
        /// </summary>
        private void UploadXML()
        {
            /*            if (PingServer("192.168.10.213")){
                            Log("MF.SendDoc "+cargodocs[currentdocrow].Number);
                        } else {
                            Log("MF.SendDoc "+cargodocs[currentdocrow].Number + "Server not connect");
                        }
                        GetTime(); 
                        LogShow("MF GetTime Done");*/
            if (dataGrid1.VisibleRowCount == 0)
            {
                LogShow("[MF.DocSendNothing]\n Нет документов для отправки");
            }
            else
            {
                //                GetTime();
                string n = cargodocs[currentdocrow].Number.Trim();
                string t = ConvertToYYYYMMDD(cargodocs[currentdocrow].Data);
                string x = n + "_" + t + "_" + Config.scannerNumber;
                string z = "";
                int xall = cargodocs[currentdocrow].XCodes.Length;
                string xgood = cargodocs[currentdocrow].ScannedBar;
                if (Config.notping || PingServer(Config.serverIp))
                { 
                try
                {
                    z = RestAPI_POST(@"http://" + Config.serverIp + "/CargoDocService.svc/CargoDoc/" + x);
                    if (z.Substring(0, 2) == "->")
                    {
                        z = z.Remove(0, 2);
                        //Log("[MF.DocSended] " + cargodocs[currentdocrow].Number);
                        //Log("[MF.DocSended] " + z);
                        //                            if (Convert.ToInt16(z) == Convert.ToInt16(cargodocs[currentdoccol].ScannedBar))
                        if (Convert.ToInt16(z) == xall)
                        {
                            Log("[MF.DocSended] " + z + "(without deleted " + xgood + ")");
                            if (Convert.ToInt16(xgood) == xall)
                                MessageBox.Show("Отправлено " + xgood + " штрихкодов по документу " + cargodocs[currentdocrow].Number.Trim());
                            else
                                MessageBox.Show("Отправлено " + xgood + " штрихкодов по документу " + cargodocs[currentdocrow].Number.Trim() + "(с удалёнными - " + xall.ToString() + ")");
                            try
                            {
                                File.Delete(CurrentPath + @"doc\" + x + ".xml");
                                LoadAllDataFromXml();
                                GetCustomers();
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
//                            LogShow("[MF.NotEqualQuantity] Сервер принял " + z + " штрихкодов, а отсканировано " + cargodocs[currentdoccol].ScannedBar);
                        }

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
        private string GetTime()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CustomTime));
                string s = RestAPI_GET("http://" + Config.serverIp + "/CargoDocService.svc/Time");
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
        /// Проверка доступности сервера по сети
        /// При этом не проверяется его работоспособность
        /// </summary>
        /// <returns>Истина - сервер доступен, ложь - нет</returns>
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
        /// <returns>Ответ сервера в виде строки</returns>
        public static string RestAPI_GET(string url)
        {
            string sb = "";
            try
            {
                MainForm.Log("[MF.RestAPI_GET.Begin]");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 5000;
                request.ContentType = "text/xml;charset=utf-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                MainForm.Log("[MF.RestAPI_GET.Response]");
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb = reader.ReadToEnd().ToString();
                MainForm.Log("[MF.RestAPI_GET.Reader] " + sb);
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
        /// Загрузка данных на сервер командой POST
        /// В ответ приходит или количество загруженных штрих-кодов или ошибка в заранее оговоренной форме (см. класс Protocol)
        /// </summary>
        /// <param name="url">Строка в формате БазовыйURLДляОтправкиДанных/НомерДокумента_Дата_НомерСканера </param>
        /// <returns>Ответ сервера - количество загруженных штрих-кодов или ошибка</returns>
        private string RestAPI_POST(string url)
        {
            string sb = "";
            try
            {
                //                Log("MF.SendDoc.Begin");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 5000;
                request.ContentType = "application/xml;charset=utf-8";

                string postData = cargodocs[currentdocrow].Serialize();
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //                Log("MF.SendDoc.Response");
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
//                            MessageBox.Show("Отправлено " + proto.Message + " штрихкодов по документу " + cargodocs[currentdocrow].Number.Trim());
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

                //                LogShow("MF.SendDoc = " + sb);
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

        # endregion

        #region DataGrid - загрузка данных в таблицу, обработка событий, раскраска

        /// <summary>
        /// Загрузка в экранную таблицу отгрузочных документов данных из соответствующей структуры в памяти
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
        private void GetCustomers()
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

            // ширина колонок
            dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            //            tableStyle.MappingName = MainForm.doctable.TableName;

            /*            vColumn.MappingName = "Name";
                        vColumn.HeaderText = "Наименование";*/

            DataGridTextBoxColumnColored col0 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            col0.Width = 80;
            col0.MappingName = doctable.Columns[0].ColumnName;
            col0.HeaderText = doctable.Columns[0].ColumnName;
            col0.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col0);

            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            col1.Width = 90;
            col1.MappingName = doctable.Columns[1].ColumnName;
            col1.HeaderText = doctable.Columns[1].ColumnName;
            col1.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
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
            //Определим класс аргумента события, делегат и само событие, 
            //необходимые для "общения" кода выполняющего прорисовку ячейки, с кодом, 
            //предоставляющим цвет для этой ячейки. 
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

            //А вот и переопределенный метод DataGridTextBoxColumn.Paint(), 
            //запрашивающий при помощи события (аргументов) цвет и передающий его 
            //базовому методу Paint(), в параметре backBrush. 
            //Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, 
            //используя при этом подставленный нами backBrush. 
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
        /// При щелчке на строке таблицы (или нажатии Enter) вызывается список продукции этого отгрузочного документа
        /// </summary>
        private void dataGrid1_Click(object sender, EventArgs e)
        {
            if (dataGrid1.VisibleRowCount != 0)
            {
                currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
                currentdocrow = dataGrid1.CurrentCell.RowNumber;

                productlistform = new ProductListForm();
                productlistform.ShowDialog();
            }
/*            else
            {
                Log("[MF.DataGrid.Empty.Table]");
                MessageBox.Show("В сканере нет загруженных документов!");
            }*/
            MainForm.scanmode = ScanMode.Doc;
        }

        /// <summary>
        /// При изменении текущей ячейки обновляем соответствующие переменные
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
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                panel3_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
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
            if ((e.KeyCode.GetHashCode() == 190) && (serviceKeySequence == 5))
            {
                MainForm.scanmode = ScanMode.Nothing;
                serviceKeySequence = 0;
                ServiceForm serv = new ServiceForm();
                serv.ShowDialog();
                serv.Close();
                LoadAllDataFromXml();
                GetCustomers();
                currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
                currentdocrow = dataGrid1.CurrentCell.RowNumber;
                MainForm.scanmode = ScanMode.Doc;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Nothing;
            UploadXML();
            MainForm.scanmode = ScanMode.Doc;
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Nothing;
            FromToForm f = new FromToForm();
            f.ShowDialog();
            MainForm.scanmode = ScanMode.Doc;
        }
       
/*        private void button3_Click(object sender, EventArgs e)
        {
            SetTime(GetTime());
        } */

        /// <summary>
        /// Выход из программы
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        # region Scan - обработка событий сканирования штрихкодов

        public static Boolean isXCodePresent(string barcod) 
        {
            foreach (XCode x in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
            {
                if (x.ScanCode == barcod && x.DData == "") return true;
            }
            return false;
        }

        /// <summary>
        /// Добавление отсканированного штрих-кода в таблицу на экране и стуктуры в памяти
        /// </summary>
        public static void ScanBarCode(string barcod)
        {
            string bar = barcod.Substring(0, 5);
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
                        if (Convert.ToInt16(p.ScannedBar) > Convert.ToInt16(p.Quantity))
                        {
                            MainForm.LogShow("Не добавлено! Уже достаточно продукции с кодом " + bar);
                            MainForm.Attention();
                        }
                        else
                        {
                            if (Convert.ToInt16(p.ScannedBar) == Convert.ToInt16(p.Quantity))
                            {
                                MainForm.LogShow("Достигнуто необходимое количество продукции с кодом " + bar); // предупреждаем
                                MainForm.Attention();                                
                            }

                            MainForm.cargodocs[MainForm.currentdocrow].TotalProducts[i].ScannedBar = (Convert.ToInt16(p.ScannedBar) + 1).ToString();
                            MainForm.cargodocs[MainForm.currentdocrow].ScannedBar = (Convert.ToInt16(MainForm.cargodocs[MainForm.currentdocrow].ScannedBar) + 1).ToString();
                            MainForm.producttable.Rows[MainForm.currentdocrow].ItemArray[3] = (Convert.ToInt16(p.ScannedBar) + 1).ToString();

                            XCode x = new XCode();

                            x.Data = ConvertToFullDataTime(System.DateTime.Now.ToString());
                            x.Fio = Config.userName;
                            x.DData = "";
                            x.DFio = "";
                            x.PID = p.PID;
                            x.ScanCode = barcod;
                            x.ScanFrom = "";
                            x.ScanTo = "";
                            x.ScannerID = Config.scannerNumber;

                            var xl = new List<XCode>();
                            xl.AddRange(MainForm.cargodocs[MainForm.currentdocrow].XCodes);
                            xl.Add(x);
                            MainForm.cargodocs[MainForm.currentdocrow].XCodes = xl.ToArray();
//                            MainForm.cargodocs[MainForm.currentdocrow].ScannedBar = 

                            if (MainForm.xcodelistform != null && MainForm.xcodelistform.Visible)
                            {
                                MainForm.xcodetable.AcceptChanges();
                                MainForm.xcodelistform.ReloadXCodeTable();
                                MainForm.xcodetable.AcceptChanges();
                            }
                            else
                            {
                                MainForm.producttable.AcceptChanges();
                                MainForm.productlistform.ReloadProductTable();
                                MainForm.producttable.AcceptChanges();
                            }
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
            if (!find_product) MainForm.LogShow("В этом заказе нет продукции с кодом " + bar);
        }

        /// <summary>
        /// Вызов обработчика при возникновении события считывания штрих-кода
        /// </summary>
        private void OnScan(object sender, ScannedDataEventArgs e)
        {
            this.Invoke(addBarCode, (e.Data).ToString());
        }

        /// <summary>
        /// Считанный штрих-код может быть штрих-кодом товара или штрих-кодом отгрузочного документа
        /// Здесь производится определение того, что это за штрих-код, и вызов соответствующих процедур
        /// </summary>
        public void BarCodeProcessing(string barcod)
        {
            switch (scanmode)
            {
                case (ScanMode.Doc):
                    string docnum = barcod.Replace(" ", "_") + "_" + Config.scannerNumber;
                    //                    if (existDoc(CurrentPath + @"doc\" + barcod.Replace(" ", "_") + "_" + Config.scannerNumber + ".xml"))
                    if (doclist.Contains(CurrentPath + @"doc\" + docnum + ".xml"))
                                                                 
                    {
                        int space = barcod.IndexOf(" ");
                        string data_raw = barcod.Remove(0, space);
                        string data = data_raw.Substring(7, 2) + "." + data_raw.Substring(5, 2) + "." + data_raw.Substring(3, 2);
                        LogShow("Документ № " + barcod.Substring(0, space) + " от " + data + " уже загружен!");
                    }
                    else
                    {
                        if (Config.notping || PingServer(Config.serverIp))
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
                            LogShow("[MF.BarCodeProcessing] Нет связи с сервером!");
                        }
                    }
                    break;
                case (ScanMode.BarCod):
                    ScanBarCode(barcod);
                    break;
                case (ScanMode.Nothing):
                    MessageBox.Show(barcod);
                    break;
            }
        }

        # endregion

        # region Log - протоколирование событий

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

        public static void Vibration()
        {
            Calib.SystemLibNet.Api.SysPlayVibrator(Calib.SystemLibNet.Def.B_ALARM, Calib.SystemLibNet.Def.VIBRATOR_DEFAULT, Calib.SystemLibNet.Def.VIBRATOR_DEFAULT, Calib.SystemLibNet.Def.VIBRATOR_DEFAULT);
        }

        public static void Speaker()
        {
            //Calib.SystemLibNet.Api.SysPlayBuzzer(Calib.SystemLibNet.Def.B_ALARM, Calib.SystemLibNet.Def.BUZ_DEFAULT, Calib.SystemLibNet.Def.BUZ_DEFAULT);
            Calib.SystemLibNet.Api.SysPlayBuzzer(Calib.SystemLibNet.Def.B_WARNING, Calib.SystemLibNet.Def.BUZ_DEFAULT, Calib.SystemLibNet.Def.BUZ_DEFAULT);
        }

        /// <summary>
        /// Добавление в лог-файл строки с отметкой времени плюс распарсенного сообщения об ошибке
        /// Сообщение об ошибке - на экран
        /// </summary>
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
        /// Добавление в лог-файл строки с отметкой времени плюс распарсенного сообщения об ошибке
        /// </summary>
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
        public static void Log(string LogString)
        {
            MainForm.log.Add(System.DateTime.Now.ToString() + " " + LogString);
        }

        /// <summary>
        /// Добавление в лог-файл строки с отметкой времени и показ такого-же сообщения во всплывающем окне
        /// </summary>
        public static void LogShow(string Label)
        {
            MainForm.log.Add(System.DateTime.Now.ToString() + " " + Label);
            MessageBox.Show(Label);
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
        }
        #endregion

        #region Utilities - маленькие вспомогательные функции

        /// <summary>
        /// Преобразование отметки времени из формата "2015-11-18T14:50:17+02:00" в формат "18.11.15"
        /// </summary>
        /// <returns>Строка с датой</returns>
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
        /// <returns>Строка с датой</returns>
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
        /// </summary>
        /// <returns>Строка с датой</returns>
        public static string ConvertToFullDataTime(string s)
        {
            string ss;
            try
            {
                ss = "20" + s.Substring(6, 2) + "-" + s.Substring(3, 2) + "-" + s.Substring(0, 2) + "T" + s.Substring(9, 8) + @"+02:00";
            }
            catch
            {
                ss = "";
            }
            return ss;
        }

        /// <summary>
        /// Удаляет из XML определения пространств имён
        /// Применять только один раз, иначе удаляет больше чем надо
        /// </summary>
        /// <returns>Строка</returns>
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
        /// <returns>Строка</returns>
        private string DeleteNil(string s)
        {
            return s.Replace(@" i:nil=""true""", "");
        }

        /// <summary>
        /// Заменяет HTTP-шные коды на обычные символы
        /// </summary>
        /// <returns>Строка</returns>
        private string HTTPDecode(string input)
        {
            string ss = input.Replace("&lt;", "<");
            input = ss.Replace("&gt;", ">");
            ss = input.Substring(input.IndexOf("<string>") + 8);
            input = ss.Substring(0, ss.Length - 9);
            return input;
        }

        #endregion

        /// <summary>
        /// Обновляет значение времени на экране и проверяет состояние батареи
        /// </summary>
        private void SetTime(string s)
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
            //MessageBox.Show(s + " " + DateTime.Now.ToString() );
            DateTime dt = new DateTime(year, month, day, hour, min, sec);
            OpenNETCF.WindowsCE.DateTimeHelper.LocalTime = dt;
            //System.Threading.Thread.Sleep(1000);            
            //MessageBox.Show(DateTime.Now.ToString());
        }

        /// <summary>
        /// Обновляет значение времени на экране и проверяет состояние батареи
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