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
        string CurrentPath;

        /// <summary>
        /// Проверяем наличие файла настроек и каталогов. При отсутствии - загружаем/создаём.
        /// Составляем список имеющихся на сканере документов.
        /// </summary>
        /// <returns>Истина если всё хорошо; ложь, если что-то не в порядке</returns>
        private Boolean TestFilesAndDirs()
        {
            CurrentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (!File.Exists(CurrentPath+@"\settings.xml"))
                if (!DownloadSettings())
                {
                    MessageBox.Show("Файл настроек не найден!");
                    return false;
                }

            /*            if (!Directory.Exists(CurrentPath+@"\log")
                            Directory.CreateDirectory(CurrentPath+@"\log"); */

            if (!Directory.Exists(CurrentPath+@"\doc"))
                Directory.CreateDirectory(CurrentPath+@"\doc");

            doclist = Directory.GetFiles(CurrentPath + @"\doc", "*_*_*.xml");
// Сделать загрузку данных из файлов, чтобы в списке были даты, контрагенты и т.п.
// Вообще, это надо в отдельную функцию засунуть, чтобы суперюзер мог перечитать список
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
                        settings.SaveToFile(CurrentPath + @"settings.xml");
                        result = true;
                    }
                    repeat -= 1;
                }
                catch (Exception ex)
                {
                    s = ex.Message;
                    if (ex.InnerException != null)
                    {
                        s = s + " " + ex.InnerException.Message;
                        MessageBox.Show(s);
                    }
                    result = false;
                }
            }
            return result;
        }

        private Boolean LoadAllDataFromXml()
        {
            Boolean result = false;
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
                string err = ex.Message;
                if (ex.InnerException != null)
                {
                    err += " " + ex.InnerException.Message;
                }
                MessageBox.Show(err);
            }
            return result;
        }

        public MainForm()
        {
            InitializeComponent();
            TestFilesAndDirs();
            CargoDoc cargodoc = new CargoDoc();
/*            this.KeyPreview = true;
            this.KeyUp += new KeyEventHandler(this.OnKeyUp);
            HBConfig();*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ndoc = "9237_20151118_02";

//            GetHTTP("http://192.168.10.213/CargoDocService.svc/CargoDoc/"+s);
            //GetHTTP("http://192.168.10.213");

            string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + ndoc);

//            s = "<CargoDoc><Data>2015-11-18T10:56:52+02:00</Data><DocId i:nil=\"true\"/><Error i:nil=\"true\"/><Number>9237     </Number><Partner>ТОВ \"БИТТЕХ";

            s = DeleteNameSpace(s);
            s = DeleteNil(s);

            listBox1.Items.Clear();
            listBox1.Items.Add("Получено " + s.Length.ToString() + " байт данных");

/*            using (FileStream fs = File.OpenWrite(CurrentPath + ndoc + ".xml"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(s);
                fs.Write(info, 0, info.Length);
            }*/

            XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));

            CargoDoc cd = new CargoDoc();

            using (var reader = new StringReader(s))
            {

                cd = (CargoDoc)serializer.Deserialize(reader);
            }

            cd.SaveToFile(CurrentPath + @"\doc\"+ndoc+@".xml");
//            cargodoc = cd;

/*            listBox1.Items.Clear();

            listBox1.Items.Add(cd.Data);
            listBox1.Items.Add(cd.DocID);
            listBox1.Items.Add(cd.Partner);

            foreach (Product p in cd.TotalProducts)
            {
                listBox1.Items.Add(p.PName + " - " + p.Quantity);
            }

            foreach (XCode x in cd.XCodes)
            {
                listBox1.Items.Add(x.ScanCode + " - " + x.Fio);
            }   */

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = sRestAPI("http://192.168.10.213/CargoDocService.svc/Settings");

            s = DeleteNameSpace(s);

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

//            Settings set = new Settings(); 

/*            using (FileStream fs = File.OpenWrite(CurrentPath + "\\seti.xml"))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(s);
                fs.Write(info, 0, info.Length);
            }
            */
            try
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
            }
            



//            Settings set = Settings.Deserialize(s);
            //MessageBox.Show("Нажата F2");
//            GetHTTP("http://192.168.10.213/CargoDocService.svc/Settings");
//            RestAPI("http://192.168.10.213/CargoDocService.svc/CargoDoc/" + s);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Нажата F3");
            //LoadXml(@"2419.xml");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Нажата F4");
            Close();
        }

/*        private void HBConfig()
        {
            try
            {
                hwb1 = new HardwareButton();
                hwb2 = new HardwareButton();
                hwb3 = new HardwareButton();
                hwb4 = new HardwareButton();
                hwb1.AssociatedControl = this;
                hwb2.AssociatedControl = this;
                hwb3.AssociatedControl = this;
                hwb4.AssociatedControl = this;
                hwb1.HardwareKey = HardwareKeys.ApplicationKey1;
                hwb4.HardwareKey = HardwareKeys.ApplicationKey4;
                hwb2.HardwareKey = HardwareKeys.ApplicationKey2;
                hwb3.HardwareKey = HardwareKeys.ApplicationKey3;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message + " Check if the hardware button is physically available on this device.");
            }
        } 

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch ((HardwareKeys)e.KeyCode)
            {
                case HardwareKeys.ApplicationKey1:
                    statusBar1.Text = "Button 1 pressed.";
                    break;

                case HardwareKeys.ApplicationKey2:
                    statusBar1.Text = "Button 2 pressed.";
                    break;

                case HardwareKeys.ApplicationKey3:
                    statusBar1.Text = "Button 3 pressed.";
                    break;

                case HardwareKeys.ApplicationKey4:
                    statusBar1.Text = "Button 4 pressed.";
                    break;

                default:
                    break;
            }
        } */

        private void Form1_KeyDown(object sender, KeyEventArgs e)
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

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (LoginForm.Dialog() == DialogResult.Abort) Close();
            else
            {
                this.statusBar1.Text = "Сканер №" + Config.scannerNumber + " / " + Config.userName;
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

        private void GetHTTP(string url)
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
                WebResponse result = req.GetResponse();
                ReceiveStream = result.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                var sr = new StreamReader(ReceiveStream, encode);

                XmlSerializer serializer = new XmlSerializer(typeof(CargoDoc));
                CargoDoc cd = new CargoDoc();

                using (FileStream fs = File.OpenWrite(CurrentPath + "\\cargon.xml"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(sr.ToString());
                    fs.Write(info, 0, info.Length);
                    listBox1.Items.Add("Получено "+info.Length.ToString()+" байт данных");
                }
            
/*                using (var stream = File.OpenRead(CurrentPath + "\\cargon.xml"))
                {
                    cd = (CargoDoc)serializer.Deserialize(stream);
                } 


                using (var reader = new StringReader(DeleteNameSpace(cd.ToString())))
                {

                    cd = (CargoDoc)serializer.Deserialize(reader);
                }*/

                /*listBox1.Items.Clear();

                listBox1.Items.Add(cd.Data);
                listBox1.Items.Add(cd.DocID);
                listBox1.Items.Add(cd.Partner);

                foreach (Product p in cd.TotalProducts) {
                    listBox1.Items.Add(p.PName + " - " + p.Quantity);
                }

                foreach (XCode x in cd.XCodes)
                {
                    listBox1.Items.Add(x.ScanCode + " - " + x.Fio);
                } */  
               

                // Read the stream into arrays of 30 characters
                // to add as items in the list box. Repeat until
                // buffer is read.
/*                Char[] read = new Char[30];
                int count = sr.Read(read, 0, 30);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    this.listBox1.Items.Add(str);
                    count = sr.Read(read, 0, 30);
                } */



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
                this.listBox1.Items.Add(message);
            }
            catch (Exception ex)
            {
                this.listBox1.Items.Add(ex.Message);
            }
            finally
            {
                ReceiveStream.Close();
                sr.Close();
            }
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
                    this.listBox1.Items.Add(str);
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
                this.listBox1.Items.Add(message);
            }
            catch (Exception ex)
            {
                this.listBox1.Items.Add(ex.Message);
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

        private void button5_Click(object sender, EventArgs e)
        { 
/*            string p = CurrentPath + @"\2419.xml";
            if ( CargoDoc.LoadFromFile(p , out CargoDocDB ) == true)
            {  */

//            }
//            d.Close();
//            Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (LoadAllDataFromXml())
            {
                DocListForm d = new DocListForm();
                d.Show();
            }
        }

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

        private string DeleteNameSpace(string s) // применять только один раз!
        {
            string begin = s.Substring(0, s.IndexOf(" "));
            return begin + ">" + s.Substring(s.IndexOf(">") + 1);
        }

        private string DeleteNil(string s)
        {
            return s.Replace(@" i:nil=""true""", "");
        }


    }
}
