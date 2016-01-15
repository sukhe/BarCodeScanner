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
        static Settings settings = new Settings();
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

//        public static DataTable doctable;
//        public static DataTableReader docreader;


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
                    MessageBox.Show("Файл настроек не найден!");
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

            doclist = Directory.GetFiles(CurrentPath + "doc", "*_*_*.xml");
// Сделать загрузку данных из файлов, чтобы в списке были даты, контрагенты и т.п.
// Вообще, это надо в отдельную функцию засунуть, чтобы суперюзер мог перечитать список

            Config.scannerNumber = "02";
            return true;
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
                    if (ex.InnerException == null)
                        MessageBox.Show(@"[MF02] " + ex.Message);
                    else
                        MessageBox.Show(@"[MF02] " + ex.Message + " " + ex.InnerException.Message);
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
            try
            {
                foreach (string s in doclist)
                {
                    cargodocs.Add(CargoDoc.LoadFromFile(s));
                }
                result = true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    MessageBox.Show(@"[MF03] " + ex.Message);
                else
                    MessageBox.Show(@"[MF03] " + ex.Message + " " + ex.InnerException.Message);
                return false;
            }
            return result;
        }

        public MainForm()
        {
            InitializeComponent();

            TestFilesAndDirs();
            CargoDoc cargodoc = new CargoDoc();
            MainForm.scanmode = ScanMode.Doc;

            cs = new CasioScanner();
            cs.Scanned += OnScan;
            cs.Open();
//            addItem = new AddScan(ListItemAdd); // назначение делегата для потокобезопасного вызова процедуры
            addItem = new AddScan(GetXML);

            dataGrid1.Focus();
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
                        MessageBox.Show("Документ № " + barcod.Substring(0,space) + " от " + data + " уже загружен!");
                    }
                    else
                    {
                        DownloadXML(barcod);
                        LoadAllDataFromXml();
                        doclistform.ReloadDocTable();
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
                            MessageBox.Show("Превышение количества продукции с кодом " + bar);
                        }
                        else
                        {
                            if (Convert.ToInt16(p.ScannedBar) == Convert.ToInt16(p.Quantity))
                            {
                                MessageBox.Show("Достигнуто нужное количество продукции с кодом " + bar);
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
                if (ex.InnerException == null)
                    MessageBox.Show(@"[MF04] " + ex.Message);
                else
                    MessageBox.Show(@"[MF04] " + ex.Message + " " + ex.InnerException.Message);
                
            }
            if (!find_product) MessageBox.Show("В этом заказе нет продукции с кодом " + bar);
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

        public Boolean DownloadXML(string barcod)
        {
            try
            {
//                string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + barcod.Replace(" ","_")+"_"+Config.scannerNumber);
                string s = GetHTTP("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + barcod.Replace(" ", "_") + "_" + Config.scannerNumber);
                s = DeleteNameSpace(s);
                s = DeleteNil(s);
//                listBox1.Items.Add("Получено " + s.Length.ToString() + " байт данных");

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
                if (ex.InnerException == null)
                    MessageBox.Show(@"[MF01] "+ex.Message);
                else
                    MessageBox.Show(@"[MF01] "+ex.Message + " " + ex.InnerException.Message);
                return false;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (LoadAllDataFromXml())
            {
//                DocListForm d = new DocListForm();
//                d.Show();
//                if (doclistform == null) 
                doclistform = new DocListForm();
                doclistform.Show();
//                doclistform.Focus();
            }


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

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //
        }

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

/*            if (LoginForm.Dialog() == DialogResult.Abort) Close();
            else
            { */
            Config.userName = "Бендер О.И.";
            if (LoadAllDataFromXml())
            {
                if (doctable == null)
                    doctable = new DataTable();
                dataGrid1.DataSource = doctable;
                GetCustomers();
            }

//                this.statusBar1.Text = "Сканер №" + Config.scannerNumber + " / " + Config.userName;
/*            }
            if (Config.superuser)
            {
                this.BackColor = Color.Coral;
            } */
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
                MessageBox.Show(ex.Message);
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
            string sb;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 15000;
                request.ContentType = "text/xml;charset=utf-8";

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                sb = reader.ReadToEnd().ToString();
            }
            catch (Exception ex)
            {
                sb = ex.Message.ToString();
            }
            return sb;
        }

        private void PutHTTP(string url)
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
                req.Method = "PUT";
                req.ContentType = "text/xml; charset=utf-8";
                //req.ContentType = "application/json; charset=utf-8";
                req.Timeout = 60000;


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
                MessageBox.Show(ex.Message);
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
            req.Timeout = 60000;
            // We give the Request as "State" to the function, you'll see why

            string payload = "<xml>...</xml>";
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
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(string[]), "<xml>...</xml>");
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

            if (SystemState.PowerBatteryState == BatteryState.Critical)
            {
                //                MessageBox.Show("Низкий заряд батареи. Поставьте сканер на подзарядку.", "Внимание!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            };
        }

        private string DeleteNameSpace(string s) // применять только один раз! иначе откусывает лишнее
        {
            string begin = s.Substring(0, s.IndexOf(" "));
            return begin + ">" + s.Substring(s.IndexOf(">") + 1);
        }

        private string DeleteNil(string s)
        {
            return s.Replace(@" i:nil=""true""", "");
        }

        private void MainForm_Closing(object sender, CancelEventArgs e)
        {
            cs.Scanned -= OnScan;
            cs.Dispose();
            Dispose();
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

            doctable.Rows.Clear();

            foreach (CargoDoc d in cargodocs)
            {
                doctable.Rows.Add(new object[] { d.Number.Trim(), colData(d.Data), d.Partner });
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

            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            col1.Width = 80;
            col1.MappingName = doctable.Columns[0].ColumnName;
            col1.HeaderText = doctable.Columns[0].ColumnName;
            col1.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            col2.Width = 90;
            col2.MappingName = doctable.Columns[1].ColumnName;
            col2.HeaderText = doctable.Columns[1].ColumnName;
            col2.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            //            if (MainForm.cargodocs.Count > 9) col3.Width = 236;
            //            else 
            col3.Width = 260;
            col3.MappingName = doctable.Columns[2].ColumnName;
            col3.HeaderText = doctable.Columns[2].ColumnName;
            col3.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col3);

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
                button2_Click(this, e);
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

            string val = doctable.Rows[e.RowNum][0].ToString().Trim();
            if (val == "337")
                //                    e.BackBrush = new SolidBrush(Color.LightGreen);
                e.BackBrush = new SolidBrush(fullColor);
            else if (val == "336")
                //                    e.BackBrush = new SolidBrush(Color.Pink);
                e.BackBrush = new SolidBrush(partialColor);
            else
                e.BackBrush = new SolidBrush(dataGrid1.BackColor);

            //                e.ForeBrush = new SolidBrush(dataGrid1.ForeColor); 
            //            } 
        }

        private void dataGrid1_Click(object sender, EventArgs e)
        {
            //            var z = sender;
            //            var ee = e;
            currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
            currentdocrow = dataGrid1.CurrentCell.RowNumber;
            productlistform = new ProductListForm();
            productlistform.Show();
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
            currentdocrow = dataGrid1.CurrentCell.RowNumber;
        }

    }
}
