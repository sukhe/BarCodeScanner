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

namespace BarCodeScanner
{

    public partial class MainForm : Form
    {
        public static List<CargoDoc> cargodocs = new List<CargoDoc>();
        public static string[] doclist;
        public static Settings settings = new Settings();
        Stream ReceiveStream = null;
        StreamReader sr = null;
        public static string CurrentPath;
//        public static Config 

        CasioScanner cs;
        public delegate void AddScan(string s);
        public AddScan addItem;

        public static ScanMode scanmode;

        public static DocListForm doclistform;
        public static DataTable doctable;
        public static DataTableReader docreader;
        public static int currentdocrow;
        public static int currentdoccol;

        public static ProductListForm productlistform;
        public static DataTable producttable;
        public static DataTableReader productreader;

        public static XCodeListForm xcodelistform;
        public static DataTable xcodetable;
        public static DataTableReader xcodereader;

        public static List<string> log = new List<string>();

//        public static DataTable doctable;
//        public static DataTableReader docreader;


        private void LogErr(string Label, Exception ex)
        {
            if (ex.InnerException == null)
            {
                log.Add(System.DateTime.Now.ToString()+" "+Label + " " + ex.Message);
                MessageBox.Show(Label + " " + ex.Message);
            }
            else
            {
                log.Add(System.DateTime.Now.ToString()+" "+Label + " " + ex.Message + " " + ex.InnerException.Message);
                MessageBox.Show(Label + " " + ex.Message + " " + ex.InnerException.Message);
            }
        }


        private void Log(string Label)
        {
            log.Add(System.DateTime.Now.ToString()+" "+Label);
        }

        private void LogShow(string Label)
        {
            log.Add(System.DateTime.Now.ToString()+" "+Label);
            MessageBox.Show(Label);
        }

        private void LogSave() 
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

        /// <summary>
        /// Проверяем наличие файла настроек и каталогов. При отсутствии - загружаем/создаём.
        /// Составляем список имеющихся на сканере документов.
        /// </summary>
        /// <returns>Истина если всё хорошо; ложь, если что-то не в порядке</returns>
        private Boolean TestFilesAndDirs()
        {
            CurrentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)+@"\";
            
            if (!File.Exists(CurrentPath + "settings.xml"))
            {
                if (!DownloadSettings())
                {
                    LogShow("Файл настроек не найден!");
                    return false;
                }
            }
            else
            {
                settings = Settings.LoadFromFile(CurrentPath + "settings.xml");
            }

            /*            if (!Directory.Exists(CurrentPath+@"\log")
                            Directory.CreateDirectory(CurrentPath+@"\log"); */

            if (!Directory.Exists(CurrentPath+"doc"))
                Directory.CreateDirectory(CurrentPath+"doc");

//            doclist = Directory.GetFiles(CurrentPath + "doc", "*_*_*.xml");
// Сделать загрузку данных из файлов, чтобы в списке были даты, контрагенты и т.п.
// Вообще, это надо в отдельную функцию засунуть, чтобы суперюзер мог перечитать список

            Config.scannerNumber = "02";
            Config.server = "192.168.10.213";
            Config.scannerNumber = "02";
            //Config.scannerNumber = GetScannerID();
            if (Config.scannerNumber == "") return false;
            else return true;
        }

        private Boolean PingServer(string serverAddress)
        {
            int timeout = 5000;
            Boolean result = false;
            OpenNETCF.Net.NetworkInformation.Ping ping = new OpenNETCF.Net.NetworkInformation.Ping();
            OpenNETCF.Net.NetworkInformation.PingReply reply = ping.Send(serverAddress, timeout);
            try
            {
//                OpenNETCF.Net.NetworkInformation.PingReply reply = ping.Send(serverAddress, timeout);
                if (reply.Status == OpenNETCF.Net.NetworkInformation.IPStatus.Success)
                {
                    result = true;
                }
            }
            catch 
            {
                Log("MF.PingServer.Status " + reply.Status.ToString());
            }
            return result;
        }

        private string GetScannerID()
        {
            string mac = "";
            string number = "";
            foreach (OpenNETCF.Net.NetworkInformation.NetworkInterface ni in OpenNETCF.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Description == "SDIO86861")
                {
                    mac = ni.GetPhysicalAddress().ToString();
                    break;
                }
            }

            foreach (Scanner t in settings.Scanners)
            {
                if ((t.MAC == mac) && (mac != ""))
                {
                    number = t.Nomer;
                    break;
                }
            }
            if (number == "")
            {
                LogShow(@"[MF05] Неопознанный сканер!");
//                Close();
            }
            return number;
        }


        /// <summary>
        /// Загрузка файла настроек с сервера. Делаем указанное количество попыток.
        /// Критерий правильности загрузки - в полученном конфиге количество пользователей больше нуля
        /// </summary>
        /// <returns>Истина если загрузилось; ложь, если нет</returns>
        private Boolean DownloadSettings()
        {
            Boolean result = false;
            int repeat = 3;      // количество повторов для считывания настроек по сети

            string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/Settings");
            s = DeleteNameSpace(s);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            while (repeat>0 && result == false) {
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
                    LogErr(@"[MF02]", ex);
                    return false; 
                }
            }
            return result;
        }

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
                    cargodocs.Add(CargoDoc.LoadFromFile(s));
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogErr(@"[MF03]", ex);
                return false;
            }
            return result;
        }

        public MainForm()
        {
            InitializeComponent();

            if (TestFilesAndDirs())
            {
                CargoDoc cargodoc = new CargoDoc();
                MainForm.scanmode = ScanMode.Doc;

                cs = new CasioScanner();
                cs.Scanned += OnScan;
                cs.Open();
                //            addItem = new AddScan(ListItemAdd); // назначение делегата для потокобезопасного вызова процедуры
                addItem = new AddScan(GetXML);

                dataGrid1.Focus();
            }
            else Close();
        }

        private Boolean existDoc(string s)
        {
            return doclist.Contains(s);
        }

        public void GetXML(string barcod)
        {
            switch (scanmode) 
            {
                case (ScanMode.Doc):
                    if (existDoc(CurrentPath + @"doc\" + barcod.Replace(" ", "_") + "_" + Config.scannerNumber + ".xml"))
                    {
                        int space = barcod.IndexOf(" ");
                        string data_raw = barcod.Remove(0, space);
                        string data = data_raw.Substring(7, 2) + "." + data_raw.Substring(5, 2) + "." + data_raw.Substring(3, 2);
                        LogShow("Документ № " + barcod.Substring(0,space) + " от " + data + " уже загружен!");
                    }
                    else
                    {
                        if (PingServer(Config.server))
                        {
                            DownloadXML(barcod);
                            LoadAllDataFromXml();
                            ReloadDocTable();
                        }
                        else
                        {
                            LogShow("[MF.GetXML] Нет связи с сервером!");
                        }
                    }
                    break;
                case (ScanMode.BarCod):
                    ScanBarCode(barcod);
                    break;
                case (ScanMode.Nothing):
                    break;
            }
        }

        public void ScanBarCode(string barcod)
        {
            string bar = barcod.Substring(0,5);
            Boolean find_product = false;
            int i = 0;
            try
            {
                foreach (Product p in cargodocs[currentdocrow].TotalProducts)
                {
                    if (p.PID == bar)
                    {
                        find_product = true;
                        if (Convert.ToInt16(p.ScannedBar) > Convert.ToInt16(p.Quantity))
                        {
                            LogShow("Превышение количества продукции с кодом " + bar);
                        }
                        else
                        {
                            if (Convert.ToInt16(p.ScannedBar) == Convert.ToInt16(p.Quantity))
                            {
                               LogShow("Достигнуто нужное количество продукции с кодом " + bar);
                            }
                            cargodocs[currentdocrow].TotalProducts[i].ScannedBar = (Convert.ToInt16(p.ScannedBar) + 1).ToString();
                            producttable.Rows[currentdocrow].ItemArray[3] = (Convert.ToInt16(p.ScannedBar) + 1).ToString();

                            XCode x = new XCode();

                            x.Data = nowcolData(System.DateTime.Now.ToString());
                            x.Fio = Config.userName;
                            x.DData = "";
                            x.DFio = "";
                            x.PID = p.PID;
                            x.ScanCode = barcod;
                            x.ScanFrom = "";
                            x.ScanTo = "";
                            x.ScannerID = Config.scannerNumber;

                            var xl = new List<XCode>();
                            xl.AddRange(cargodocs[currentdocrow].XCodes);
                            xl.Add(x);
                            cargodocs[currentdocrow].XCodes = xl.ToArray();

                            if (xcodelistform != null && xcodelistform.Visible)
                            {
                                xcodetable.AcceptChanges();
                                xcodelistform.ReloadXCodeTable();
                            }
                            else productlistform.ReloadProductTable();
//                            xcodelistform.Refresh();
                            break;
                        }
                    }
                    i++;
                }
            }
            catch (Exception ex)
            {
               LogErr(@"[MF04]",ex);
            }
            if (!find_product) LogShow("В этом заказе нет продукции с кодом " + bar);
        }


/*        void ListItemAdd(string text)
        {
            listBox1.Items.Add(text);
        } */

        private void OnScan(object sender, ScannedDataEventArgs e)
        {
            this.Invoke(addItem, (e.Data).ToString());
//            listBox1.Invoke(GetXML, (e.Data).ToString());
        }

        private string GetTime()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CustomTime));
                string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/Time");
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
                LogShow("[MF.GetTime] "+ex);
                return "";
            }
        }

        public Boolean DownloadXML(string barcod)
        {
//            if (GetTime()!="") 
            try
            {
/*                string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + barcod.Replace(" ","_")+"_"+Config.scannerNumber);
//                string s = GetHTTP("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + barcod.Replace(" ", "_") + "_" + Config.scannerNumber);
                s = DeleteNameSpace(s);
                s = DeleteNil(s);*/
//                listBox1.Items.Add("Получено " + s.Length.ToString() + " байт данных");

                string s;
                int i = 0;
                do
                {
                    s = sRestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + barcod.Replace(" ", "_") + "_" + Config.scannerNumber);
                    s = DeleteNameSpace(s);
                    s = DeleteNil(s);
                    if (s == "<CargoDoc>")
                    {
                        i++;
                        System.Threading.Thread.Sleep(1000);
                    } else i = 3;
                } while (i < 3);
                
                XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));
                CargoDoc cd = new CargoDoc();

/*                using (var reader = new StringReader(s))
                {
                    cd = (CargoDoc)serializer.Deserialize(reader);
                }
                cd.SaveToFile(CurrentPath + @"doc\" + barcod.Replace(" ","_") + "_" +Config.scannerNumber+".xml");*/

                using (FileStream fs = File.OpenWrite(CurrentPath + @"doc\" + barcod.Replace(" ", "_") + "_" + Config.scannerNumber + ".xml"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(s);
                    fs.Write(info, 0, info.Length);
//                    listBox1.Items.Add("Получено " + info.Length.ToString() + " байт данных");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogErr(@"[MF01]",ex);
                return false;
            }
//            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
/*            CargoDoc d = new CargoDoc();
            d = MainForm.cargodocs[MainForm.currentdocrow];
            string n = d.Number.Trim();
            string t = MainForm.uncolData(d.Data);
            d.SaveToFile(MainForm.CurrentPath + @"doc\"+ n + "_" + t + "_" + Config.scannerNumber.ToString() + ".xml");*/
/*            if (PingServer("192.168.10.213")){
                Log("MF.SendDoc "+cargodocs[currentdocrow].Number);
            } else {
                Log("MF.SendDoc "+cargodocs[currentdocrow].Number + "Server not connect");
            }
            GetTime(); 
            LogShow("MF GetTime Done");*/
            string n = cargodocs[currentdocrow].Number.Trim();
            string t = uncolData(cargodocs[currentdocrow].Data);
            //pRestAPI(@"http://192.168.10.213/CargoDocService.svc/CargoDoc/" + n + "_" + t + "_" + Config.scannerNumber);
            if (PingServer("192.168.10.213")) {
//                MSDNPost
                SendDoc(@"http://192.168.10.213/CargoDocService.svc/CargoDoc/" + n + "_" + t + "_" + Config.scannerNumber);
                Log("MF.DocSended " + cargodocs[currentdocrow].Number);
            } else {
                Log("MF.DocNotSended "+cargodocs[currentdocrow].Number);
            }
        }

/*        private void button1_Click(object sender, EventArgs e)
        {
            if (LoadAllDataFromXml())
            {
//                DocListForm d = new DocListForm();
//                d.Show();
//                if (doclistform == null) 
                doclistform = new DocListForm();
                doclistform.Show();
//                doclistform.Focus();
            }*/


/*            DownloadXML("9237_20151118_02");
 * 
            string ndoc = "9237_20151118_02";
            string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + ndoc); */

//            s = "<CargoDoc><Data>2015-11-18T10:56:52+02:00</Data><DocId i:nil=\"true\"/><Error i:nil=\"true\"/><Number>9237     </Number><Partner>ТОВ \"БИТТЕХ";

/*            s = DeleteNameSpace(s);
            s = DeleteNil(s);

            listBox1.Items.Clear();
            listBox1.Items.Add("Получено " + s.Length.ToString() + " байт данных"); */

/*            using (FileStream fs = File.OpenWrite(CurrentPath + ndoc + ".xml"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(s);
                fs.Write(info, 0, info.Length);
            }*/

/*            XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));
            CargoDoc cd = new CargoDoc();
            using (var reader = new StringReader(s))
            {
                cd = (CargoDoc)serializer.Deserialize(reader);
            }
            cd.SaveToFile(CurrentPath + @"\doc\"+ndoc+@".xml"); */

//        }


/*        private void button2_Click(object sender, EventArgs e)
        {
            string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/Settings");

            s = DeleteNameSpace(s);

            XmlSerializer serializer = new XmlSerializer(typeof(Settings)); */

//            Settings set = new Settings(); 

/*            using (FileStream fs = File.OpenWrite(CurrentPath + "\\seti.xml"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(s);
                fs.Write(info, 0, info.Length);
            }
            */

/*            try
            {

                using (var reader = new StringReader(s))
                {

                    settings = (Settings)serializer.Deserialize(reader);
                }

            }
            catch (Exception ex)
            {
                s = ex.Message;
                if (ex.InnerException != null)
                {
                    s = s + " " + ex.InnerException.Message;
                }
            }

            listBox1.Items.Add("Места погрузки/разгрузки");
            foreach (Location l in settings.Locations) {
                listBox1.Items.Add(l.LID + " - " + l.Name);
            }

            listBox1.Items.Add("Пользователи");
            foreach (User u in settings.Users)
            {
                listBox1.Items.Add(u.FIO + " - " + u.Pwd);
            }

            listBox1.Items.Add("Сканеры");
            foreach (Scanner c in settings.Scanners)
            {
                listBox1.Items.Add(c.MAC + " - " + c.Nomer);
            }

            listBox1.Items.Add("Перемещения");
            foreach (Transfer t in settings.Transfers)
            {
                listBox1.Items.Add(t.Name + " - " + t.From + " - " + t.To);
            } */
            

//            Settings set = Settings.Deserialize(s);
            //MessageBox.Show("Нажата F2");
//            GetHTTP("http://192.168.10.213/CargoDocService.svc/Settings");
//            RestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + s);

//        }

/*        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
//                MessageBox.Show("Нажата F1 аппаратно");
                button1_Click(this,e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                button2_Click(this, e);
//                MessageBox.Show("Нажата F2 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                button3_Click(this, e);
//                MessageBox.Show("Нажата F3 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
//                MessageBox.Show("Нажата F4 аппаратно");
            }

        }*/

        private void MainForm_Load(object sender, EventArgs e)
        {
            Config.userName = "Бендер О.И.";
            if (LoginForm.Dialog() == DialogResult.Abort) Close();
            else
            {
            Config.scannerNumber = GetScannerID();
            if (Config.scannerNumber == "")
                Close();
            else
            {
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
                //MessageBox.Show("Сканер №" + Config.scannerNumber + " \nПользователь: " + Config.userName);
            }

//                this.statusBar1.Text = "Сканер №" + Config.scannerNumber + " / " + Config.userName;
            }
            if (Config.superuser)
            {
                this.BackColor = Color.Coral;
            }
        }

        #region HTTP

        private string StringGetWebPage(String uri)
        {
            const int bufSizeMax = 65536; // max read buffer size conserves memory
            const int bufSizeMin = 8192;  // min size prevents numerous small reads
            StringBuilder sb;




            // A WebException is thrown if HTTP request fails
            try 
            {

                // Create an HttpWebRequest using WebRequest.Create (see .NET docs)!
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Method = "GET";
                request.Timeout = 5000;

                // Execute the request and obtain the response stream
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                // Content-Length header is not trustable, but makes a good hint.
                // Responses longer than int size will throw an exception here!
                int length = (int)response.ContentLength;

                // Use Content-Length if between bufSizeMax and bufSizeMin
                int bufSize = bufSizeMin;
                if (length > bufSize)
                    bufSize = length > bufSizeMax ? bufSizeMax : length;

                // Allocate buffer and StringBuilder for reading response
                byte[] buf = new byte[bufSize];
                sb = new StringBuilder(bufSize);
                // Read response stream until end
                while ((length = responseStream.Read(buf, 0, buf.Length)) != 0)
                    sb.Append(Encoding.UTF8.GetString(buf, 0, length));

             }
             catch (Exception ex)
             {
                sb = new StringBuilder(ex.Message);
             }
         return sb.ToString();
         }

        private string GetHTTP(string url)
        {
            //                string url = txtURL.Text;
            //                string proxy = txtProxy.Text;
            string s = "";

            try
            {
                /*        if(!"".Equals(txtProxy.Text))
                        {
                            WebProxy proxyObject = new WebProxy(proxy, 80);

                            // Disable proxy use when the host is local.
                            proxyObject.BypassProxyOnLocal = true;

                            // HTTP requests use this proxy information.
                            GlobalProxySelection.Select = proxyObject;

                        }*/

                WebRequest req = WebRequest.Create(url);
                req.Timeout = 5000;
                WebResponse result = req.GetResponse();
                ReceiveStream = result.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                var sr = new StreamReader(ReceiveStream, encode);
                s = sr.ReadToEnd();

/*                XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));
                CargoDoc cd = new CargoDoc();

                using (FileStream fs = File.OpenWrite(CurrentPath + "cargon.xml"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(sr.ToString());
                    fs.Write(info, 0, info.Length);
                    listBox1.Items.Add("Получено "+info.Length.ToString()+" байт данных");
                } */
                return s;
            }
            catch (WebException ex)
            {
                string message = ex.Message;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (null != response)
                {
                    message = response.StatusDescription;
                    response.Close();
                }
//                this.listBox1.Items.Add(message);
            }
            catch (Exception ex)
            {
                LogErr(@"[MF.GetHTTP]",ex);
//                this.listBox1.Items.Add(ex.Message);
            }
            finally
            {
                if (ReceiveStream != null) ReceiveStream.Close();
                if (sr != null) sr.Close();
            }
            return s;
        }

        private string sRestAPI(string url)
        {
            string sb = "";
            try
            {
                Log("MF sRestAPI Begin");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 5000;
                request.ContentType = "text/xml;charset=utf-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Log("MF sRestAPI response");
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb = reader.ReadToEnd().ToString();
                Log("MF.sRestAPI = " + sb);
                response.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogErr(@"[MF.sRestAPI]", ex);
                sb = ex.Message.ToString();
            }
            return sb;
        }

        private string SendDoc(string url)
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
                        Log("[MF.SendDoc.Error] " + sb);
                        MessageBox.Show("[MF.SendDoc.Error] " + proto.Error);
                    }
                    else
                        if (proto.Message != null)
                        {
                            Log("[MF.SendDoc.Result] " + sb);
                            MessageBox.Show("Отправлено " + proto.Message + " штрихкодов по документу " + cargodocs[currentdocrow].Number.Trim());
                        }
                        else
                        {
                            LogShow("[MF.SendDoc.Error3]");
                        }
                }
                catch
                {
                    LogShow("[MF.SendDoc.Error2] " + sb);
                }
                
//                LogShow("MF.SendDoc = " + sb);
                response.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                LogErr("[MF.SendDoc.GlobalError]", ex);
                sb = ex.Message.ToString();
            }
            return sb;
        }

        private string HTTPDecode(string input)
        {
            string ss = input.Replace("&lt;", "<");
            input = ss.Replace("&gt;", ">");
            ss = input.Substring(input.IndexOf("<string>")+8);
//            ss.
            input = ss.Substring(0,ss.Length-9);
            return input;
        }

        private void MSDNPost(string url)
        {
            // Create a request using a URL that can receive a post. 
            //            WebRequest request = WebRequest.Create("http://localhost:8888/CargoDocService.svc/CargoDoc/9237_20151118_02");
            WebRequest request = WebRequest.Create(url);
            //request.Timeout = 5000;
            //WebRequest.DefaultWebProxy = WebRequest.GetSystemWebProxy();

            //WebRequest.DefaultWebProxy = new WebProxy("http://127.0.0.1:8800", true);
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            CargoDoc cargodoc = CargoDoc.LoadFromFile(CurrentPath + @"doc\337_20151118_02.xml");
            //            CargoDoc cargodoc = CargoDoc.LoadFromFile(@"D:\WORK\CASIO\RestClient\2408.xml");
            //string postData = DeleteNameSpace2(cargodoc.Serialize());
            string postData = cargodoc.Serialize();

//            string postData = cargodocs[currentdocrow].Serialize();
//            listBox1.Items.Add(postData);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/xml;charset=utf-8";
            //            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            try
            {
                Log("MSDN.request");
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                Log("MSDN.response");
                WebResponse response = request.GetResponse();
                // Display the status.
                //                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                responseFromServer = HTTPDecode(responseFromServer);

                /*                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                                string responseFromServer = reader.ReadToEnd().ToString(); */

                // Display the content.
                //                listBox1.Items.Add(responseFromServer);
                //                Console.WriteLine(responseFromServer);
                // Clean up the streams.
/*                reader.Close();
                dataStream.Close();
                response.Close(); */
                LogShow(@"[MF.MSDN.End] " + responseFromServer);

            }
            catch (Exception ex)
            {
                LogErr(@"[MF.MSDN]", ex);
                //                listBox1.Items.Add(ex.Message);
                /*                Console.WriteLine(ex.Message);
                                if (ex.InnerException != null)
                                {
                //                    listBox1.Items.Add(ex.InnerException.Message);
                                   Console.WriteLine(ex.InnerException.Message);
                                }*/
            }
        }

        private string pRestAPI(string url)
        {
            string sb = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 5000;
                request.ContentType = "text/xml;charset=utf-8";

                string payload = cargodocs[currentdocrow].Serialize();
                byte[] postBytes = Encoding.UTF8.GetBytes(payload);
//                    .ASCII.GetBytes(str);
                request.ContentLength = postBytes.Length;
//                request.ContentType = "application/x-www-form-urlencoded";
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                sb = reader.ReadToEnd().ToString();

            }
            catch (Exception ex)
            {
                LogErr(@"[MF.pRestAPI]", ex);
                sb = ex.Message.ToString();
            }
            return sb;
        }

        private void PostHTTP(string url)
        {
            //                string url = txtURL.Text;
            //                string proxy = txtProxy.Text;

            try
            {
                /*        if(!"".Equals(txtProxy.Text))
                        {
                            WebProxy proxyObject = new WebProxy(proxy, 80);

                            // Disable proxy use when the host is local.
                            proxyObject.BypassProxyOnLocal = true;

                            // HTTP requests use this proxy information.
                            GlobalProxySelection.Select = proxyObject;

                        }*/

                WebRequest req = WebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "text/xml; charset=utf-8";
                //req.ContentType = "application/json; charset=utf-8";
                req.Timeout = 5000;


                WebResponse result = req.GetResponse();
                ReceiveStream = result.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                sr = new StreamReader(ReceiveStream, encode);

                // Read the stream into arrays of 30 characters
                // to add as items in the list box. Repeat until
                // buffer is read.
                Char[] read = new Char[30];
                int count = sr.Read(read, 0, 30);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
//                    this.listBox1.Items.Add(str);
                    count = sr.Read(read, 0, 30);
                }
            }
            catch (WebException ex)
            {
                LogErr(@"[MF.PostHTTP.1]", ex);
                string message = ex.Message;
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (null != response)
                {
                    message = response.StatusDescription;
                    response.Close();
                }
//                this.listBox1.Items.Add(message);
            }
            catch (Exception ex)
            {
                LogErr(@"[MF.PostHTTP.2]", ex);
//                MessageBox.Show(ex.Message);
//                this.listBox1.Items.Add(ex.Message);
            }
            finally
            {
                ReceiveStream.Close();
                sr.Close();
            }
        }

        private void RestAPI(string url)
        {
            // A REST Url like: http://host/api/contacts
//            String url = this.txtURL.Text;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            // You can skip this step for anonymous web services
//            req.Credentials = new System.Net.NetworkCredential("username", "password");
            // You can replace it with POST, PUT, DELETE
            req.Method = "POST";
            // Actually the content is empty but just for coherence...
            req.ContentType = "text/xml;charset=utf-8";
            // Tells the web service you want XML and not JSON
            req.Accept = "text/xml";
            // You never know how long it takes for the other side to do its job
            req.Timeout = 5000;
            // We give the Request as "State" to the function, you'll see why

//            string payload = "<xml>...</xml>";
            string payload = cargodocs[currentdocrow].Serialize();
            req.ContentLength = payload.Length;

/*            using (var client = new System.Net.WebClient())
            {
                client.UploadData(address, "PUT", data);
            } */

/*            string payload = "<xml>...</xml>";
            req.ContentLength = payload.Length;
            Stream dataStream = req.GetRequestStream();
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(string[]), payload);
            //string[] result = (string[])ser.Deserialize(ser);
//            Serialize(dataStream, payload);
            dataStream.Close(); */
            req.BeginGetResponse(new AsyncCallback(RespCallback), req);
        }

        private static void RespCallback(IAsyncResult asynchronousResult)
        {
/*            public Form1 f;
            public string url; */
            

            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)asynchronousResult.AsyncState;
            // Now you see why we wanted the Request :)
            System.Net.HttpWebResponse resp = (System.Net.HttpWebResponse)req.EndGetResponse(asynchronousResult);
            // This step can be done asynchronously aswell, it would be a good idea for large data amounts.
            System.IO.Stream respStream = resp.GetResponseStream();
            // The most important part here is the Schema url where "contract" is the namespace of the class Contact.
//            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(string[]), "http://schemas.datacontract.org/2004/07/contract");
//            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(string[]), "<xml>...</xml>");
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(string[]), cargodocs[currentdocrow].Serialize());
            string[] result = (string[])ser.Deserialize(respStream);
            // Do something useful...

            // Для доступа к GUI
/*            foreach (string s in result)
            {
                f.listBox1.Invoke(new Action(AddText), new object[] { String.Format("{0} {1}", s) });
            } */

        }

        #endregion


/*        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Нажата F3");
            //LoadXml(@"2419.xml");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Нажата F4");
            Close();
        }*/


/*        private void button5_Click(object sender, EventArgs e)
        { 
//            string p = CurrentPath + @"\2419.xml";
//            if ( CargoDoc.LoadFromFile(p , out CargoDocDB ) == true)
            {  

//            }
//            d.Close();
//            Close();
        } */

/*        private void button6_Click(object sender, EventArgs e)
        {
            foreach (string s in doclist)
            {
//                listBox1.Items.Add(s);
            }
        }*/

/*        private void LoadXml(string filename)
        {
            string sb;
            CargoDoc cargo = new CargoDoc();

            try
            {
                cargo = CargoDoc.LoadFromFile(CurrentPath + "\\" +filename);
                listBox1.Items.Add(cargo.Partner);
                listBox1.Items.Add(cargo.Number);
                cargodocs.Add(cargo);
            }
            catch (Exception ex)
            {
                sb = ex.Message.ToString();
            }
        }*/

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelTime.Text = System.DateTime.Now.ToShortDateString().Substring(0, 5) + " " + System.DateTime.Now.ToShortTimeString();
            if (SystemState.PowerBatteryState == BatteryState.Critical)
            {
                MessageBox.Show("Низкий заряд батареи. Поставьте сканер на подзарядку.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            };
        }

        private string DeleteNameSpace(string s) // применять только один раз! иначе откусывает лишнее
        {
            string ss = "";
            if (s.IndexOf("<Cargo") >= 0)
            {
                ss = s.Substring(s.IndexOf("<Cargo"));
//                string begin = ss.Substring(0, ss.IndexOf(" "));
//                return begin + ">" + ss.Substring(ss.IndexOf(">") + 1);
            }
            else 
            if (s.IndexOf("<Settings") >= 0)
            {
                ss = s.Substring(s.IndexOf("<Settings"));
//                string begin = ss.Substring(0, ss.IndexOf(" "));
//                return begin + ">" + ss.Substring(ss.IndexOf(">") + 1);
//                return s.Substring(s.IndexOf("<Users"));
            } 
            else
            if (s.IndexOf("<CustomTime") >= 0)
            {
                ss = s.Substring(s.IndexOf("<CustomTime"));
//                string begin = ss.Substring(0, ss.IndexOf(" "));
//                return begin + ">" + ss.Substring(ss.IndexOf(">") + 1);
                //                return s.Substring(s.IndexOf("<Users"));
            };
            if (ss != "")
            {
                string begin = ss.Substring(0, ss.IndexOf(" "));
                return begin + ">" + ss.Substring(ss.IndexOf(">") + 1);
            }
            return "";
        }

        private string DeleteNil(string s)
        {
            return s.Replace(@" i:nil=""true""", "");
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            if (cs != null)
            {
                cs.Scanned -= OnScan;
                cs.Dispose();
            }
            Dispose();
            LogSave();
        }

        private void DocListForm_KeyDown(object sender, KeyEventArgs e)
        {

        }

        public void ReloadDocTable()
        {
            //            DataTable table = new DataTable();
            //            dataGrid1.DataSource = table;
            //            DataTableReader reader = new DataTableReader(GetCustomers(table, dataGrid1));
            //            MainForm.doctable.Clear();

            //            MainForm.doctable.Load(MainForm.docreader);

            //            MainForm.doclistform.dataGrid1.Refresh();
            //            MainForm.doclistform.dataGrid1.Update();

            //            GetCustomers();

/*            int q;
            int b;*/

            doctable.Rows.Clear();
            foreach (CargoDoc d in cargodocs)
            {
/*                q = 0;
                b = 0;

                foreach (Product p in d.TotalProducts)
                {
                    q += Convert.ToInt16(p.Quantity);
                    b += Convert.ToInt16(p.ScannedBar);
                }

                d.Quantity = q.ToString();
                d.ScannedBar = b.ToString();*/
                doctable.Rows.Add(new object[] { d.Number.Trim(), colData(d.Data), d.Partner, d.Quantity, d.ScannedBar });
            }
            doctable.AcceptChanges();
        }

        public static string colData(string s)
        {
            string ss;
            try
            {
                ss = s.Substring(8, 2) + '.' + s.Substring(5, 2) + '.' + s.Substring(2, 2);
            }
            catch
            {
                ss = "01.01.01";
            }
            return ss;
        }

        public static string uncolData(string s)
        {
            string ss;
            try
            {
                ss = s.Substring(0, 4) + s.Substring(5, 2) + s.Substring(8, 2);
            }
            catch
            {
                ss = "01.01.01";
            }
            return ss;
        }

        public static string nowcolData(string s)
        {
            string ss;
            try
            {
                ss = "20"+s.Substring(6, 2) + "-" + s.Substring(3, 2) + "-" + s.Substring(0, 2) + "T" + s.Substring(9,8)+@"+02:00";
            }
            catch
            {
                ss = "01.01.01";
            }
            return ss;
        }

        private void GetCustomers()
        {
            //            AddDataToTable();

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


            /*            table.Rows.Add(new object[] { 0, "Mary" });
                        table.Rows.Add(new object[] { 1, "Andy" });
                        table.Rows.Add(new object[] { 2, "Peter" });*/

            doctable.AcceptChanges();
            //            return MainForm.doctable;
        }

        # region Buttons/Events
/*        private void DocListForm_Closed(object sender, EventArgs e)
        {
            Tag = "Closed";
        }

        private void DocListForm_GotFocus(object sender, EventArgs e)
        {
            Tag = "GotFocus";
        }

        private void DocListForm_Activated(object sender, EventArgs e)
        {
            Tag = "Activated";
        }

        private void DocListForm_Deactivate(object sender, EventArgs e)
        {
            Tag = "Deactivate";
        }

        private void DocListForm_LostFocus(object sender, EventArgs e)
        {
            Tag = "LostFocus";
        }*/

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                //                MessageBox.Show("Нажата F1 аппаратно");
                //                var z = sender;
                //                var ee = e;
                /*                MainForm.productlistform = new ProductListForm();
                                MainForm.productlistform.Show();*/
                dataGrid1_Click(sender, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                //                MessageBox.Show("Нажата F1 аппаратно");
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                //button2_Click(this, e);
                //                MessageBox.Show("Нажата F2 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                //button3_Click(this, e);
                //                MessageBox.Show("Нажата F3 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
                //                MessageBox.Show("Нажата F4 аппаратно");
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            Tag = "Paint";
        }

        private void dataGrid1_Paint(object sender, PaintEventArgs e)
        {
            //      dataGrid1.
        }

        #endregion
        // разукрашивание ячеек в нужный цвет
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

        private void OnBackgroundEventHandler(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {

            Color fullColor = new Color();
            Color partialColor = new Color();

            partialColor = Color.FromArgb(255, 127, 127);
            fullColor = Color.FromArgb(127, 255, 127);

            /*            if (e.RowNum == dataGrid1.CurrentRowIndex)
                        {
                            e.BackBrush = new SolidBrush(dataGrid1.SelectionBackColor);
                            e.ForeBrush = new SolidBrush(dataGrid1.SelectionForeColor);
                        }
                        else
                        {*/
            /*                int divVal = e.RowNum / 2;
                            if (divVal * 2 != e.RowNum) e.BackBrush = new SolidBrush(partialColor);
                            else e.BackBrush = new SolidBrush(fullColor);*/

            //e.ForeBrush = new SolidBrush(dataGrid1.ForeColor);
            e.ForeBrush = new SolidBrush(Color.Black);

//            string val = doctable.Rows[e.RowNum][0].ToString().Trim();

            int q = Convert.ToInt16(doctable.Rows[e.RowNum][3]);
            int b = Convert.ToInt16(doctable.Rows[e.RowNum][4]);
            /*            int q = 5;
                        int b = 5; */

            if ((b < q) && (b != 0))
                e.BackBrush = new SolidBrush(partialColor);
            else if (b >= q)
                e.BackBrush = new SolidBrush(fullColor);
            else
                e.BackBrush = new SolidBrush(Color.White);

/*            if (val == "337")
                //                    e.BackBrush = new SolidBrush(Color.LightGreen);
                e.BackBrush = new SolidBrush(fullColor);
            else if (val == "336")
                //                    e.BackBrush = new SolidBrush(Color.Pink);
                e.BackBrush = new SolidBrush(partialColor);
            else
                e.BackBrush = new SolidBrush(dataGrid1.BackColor); */

            //                e.ForeBrush = new SolidBrush(dataGrid1.ForeColor); 
            //            } 
        }

        private void dataGrid1_Click(object sender, EventArgs e)
        {
            if (dataGrid1.VisibleRowCount != 0)
            {
                currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
                currentdocrow = dataGrid1.CurrentCell.RowNumber;

                productlistform = new ProductListForm();
                productlistform.Show();
            }
            else
            {
                Log("MF.DataGrid.Empty.Table");
                MessageBox.Show("В сканере нет загруженных документов!");
            }
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
            currentdocrow = dataGrid1.CurrentCell.RowNumber;
        }

        private void labelInfo_ParentChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(GetTime());
            OpenNETCF.
        }

                
    }
}
