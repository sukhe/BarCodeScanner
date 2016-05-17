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
using Microsoft.WindowsMobile.Status;

namespace BarCodeScanner
{
    /// <summary>
    /// Сервисная форма для техперсонала
    /// </summary>
    /// <remarks>
    /// Позволяет протестировать состояние сети, доступность серверов, посмотреть логи и т.п.
    /// </remarks>
    public partial class ServiceForm : Form
    {

        /// <summary>
        /// Конструктор для сервисной формы.
        /// </summary>
        public ServiceForm()
        {
            InitializeComponent();

            MainForm.Log("[SF.Enter] Вход в настройки");

            // показываем всякую статистику
            listBox1.Items.Add("Сканер номер " + Config.scannerNumber);
            listBox1.Items.Add("MAC адрес " + Config.scannerMac);
            listBox1.Items.Add("IP адрес " + Config.scannerIp);
            listBox1.Items.Add("Дата и время " + System.DateTime.Now.ToString());
            try
            {
                if (MainForm.PingServer(Config.serverIp))
                    listBox1.Items.Add("Сервер " + Config.serverIp + " пингуется");
                else
                    listBox1.Items.Add("Сервер " + Config.serverIp + " не пингуется");
            }
            catch
            {
                listBox1.Items.Add("Ошибка пинга сервера " + Config.serverIp);
            }

            try
            {
                listBox1.Items.Add(Test1C());
            }
            catch
            {
                listBox1.Items.Add("1С не отвечает на запросы");
            }

            if ((SystemState.PowerBatteryState & BatteryState.Charging) != BatteryState.Charging)
            {
                listBox1.Items.Add("Заряд батареи " + Battery());

                if ((SystemState.PowerBatteryState & BatteryState.NotPresent) == BatteryState.NotPresent)
                    listBox1.Items.Add("Батарея отстутствует или неисправна");

                if ((SystemState.PowerBatteryState & BatteryState.Critical) == BatteryState.Critical)
                    listBox1.Items.Add("Критическое состояние батареи");
            }

            MainForm.doclist = Directory.GetFiles(MainForm.CurrentPath + "doc", "*_*_*.xml");
            listBox1.Items.Add("Количество документов " + MainForm.doclist.Length.ToString());
        }

        /// <summary>
        /// Показывает заряд аккумулятора в виде процентов
        /// </summary>
        /// <returns>Строка вида "0-20%","41-60%"</returns>         
        private string Battery()
        {
            switch (Microsoft.WindowsMobile.Status.SystemState.PowerBatteryStrength) {
                case BatteryLevel.VeryHigh:
                    return "81-100%";
                case BatteryLevel.High:
                    return "61-80%";
                case BatteryLevel.Medium:
                    return "41-60%";
                case BatteryLevel.Low:
                    return "21-40%";
                default: // VeryLow
                    return "0-20%";
            }
        }

        /// <summary>
        /// Загрузка файла настроек
        /// </summary>
        private void buttonF1_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Вызов формы со списком документов
        /// </summary>
        private void buttonF2_Click(object sender, EventArgs e)
        {
            FileListForm flf = new FileListForm();
            flf.ShowDialog();
            flf.Close();
        }

        /// <summary>
        /// Просмотр лог файла
        /// </summary>
        private void buttonF3_Click(object sender, EventArgs e)
        {
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

        /// <summary>
        /// Выход из сервисного режима
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            MainForm.scanmode = ScanMode.Doc;
            Close();
        }

        /// <summary>
        /// Удаление лог файла
        /// </summary>
        private void buttonCLR_Click(object sender, EventArgs e)
        {
            File.Delete(MainForm.CurrentPath + "log.txt");
            MessageBox.Show("Лог-файл удалён");
        }

        /// <summary>
        /// Принудительная синхронизация времени с сервером
        /// </summary>
        private void buttonDot_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm.SetTime(MainForm.GetTime());
                MessageBox.Show("Время с сервера получено: " + DateTime.Now.ToString());
            }
            catch
            {
                MessageBox.Show("Не удалось получить всемя с сервера");
            }
        }

        /// <summary>
        /// Проверка связи с 1С
        /// </summary>
        private string Test1C()
        {
            if (MainForm.TestConnect1C())
                return "1С отвечает на запросы";
            else return "1С не отвечает";
        }

        /// <summary>
        /// Вызов проверки связи с 1С
        /// </summary>
        private void button0_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Test1C());
        }

        /// <summary>
        /// Обработка нажатия клавиш
        /// </summary>
        private void ServiceForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                buttonF1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                buttonF2_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonF3_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonF4_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Back))
            {
                buttonCLR_Click(this, e);
            }
            if (e.KeyCode.GetHashCode() == 190)
            {
                buttonDot_Click(this, e);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.D0)
            {
                button0_Click(this, e);
            }

        }

    }
}