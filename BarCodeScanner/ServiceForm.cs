using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace BarCodeScanner
{
    public partial class ServiceForm : Form
    {
        public ServiceForm()
        {
            InitializeComponent();
            MainForm.Log("[SF.Enter] Вход в настройки");
            listBox1.Items.Add("Сканер номер " + Config.scannerNumber);
            listBox1.Items.Add("MAC адрес " + Config.scannerMac);
            listBox1.Items.Add("IP адрес " + Config.scannerIp);
            listBox1.Items.Add("Дата и время " + System.DateTime.Now.ToString());
            try
            {
                if (MainForm.PingServer(Config.serverIp))
                    listBox1.Items.Add("Есть связь с сервером " + Config.serverIp);
                else
                    listBox1.Items.Add("Нет связи с сервером " + Config.serverIp);
            }
            catch
            {
                listBox1.Items.Add("Отсутствует связь с сервером " + Config.serverIp);
            }

            MainForm.doclist = Directory.GetFiles(MainForm.CurrentPath + "doc", "*_*_*.xml");

            listBox1.Items.Add("Количество документов " + MainForm.doclist.Length.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MainForm.DownloadSettings())
            {
                MainForm.Log("[SF.DownloadSettings] Настройки загружены");
                MessageBox.Show("Настройки успешно загружены");
            }
            else
            {
                MainForm.Log("[SF.NotDownloadSettings] Настройки загрузить не удалось");
                MessageBox.Show("Настройки загрузить не удалось");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FileListForm flf = new FileListForm();
            flf.ShowDialog();
            flf.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
/*            listBox1.Items.Clear();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(MainForm.CurrentPath + "log.txt");
            while ((line = file.ReadLine()) != null)
            {
                listBox1.Items.Add(line);
            }
            listBox1.Focus();
            file.Close();*/

            MainForm.LogSave();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = @"\FlashDisk\Program Files\Notepad\Notepad.exe";
            processStartInfo.Arguments = @"\Program Files\barcodescanner\log.txt";
            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            File.Delete(MainForm.CurrentPath + "log.txt");
            MessageBox.Show("Лог-файл удалён");
        }

        private void ServiceForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                button2_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                button3_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Back))
            {
                button5_Click(this, e);
            }

        }

    }
}