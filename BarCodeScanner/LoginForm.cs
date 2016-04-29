using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace BarCodeScanner
{
    public partial class LoginForm : Form
    {

        /// <summary>
        /// Переменная для возврата значения из формы
        /// </summary>
        private static DialogResult LoginResult;

        /// <summary>
        /// Конструктор для формы проверки паролей
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Сравнение введённого пользователем пароля с паролями из конфигурационного файла.
        /// Кроме паролей обрабатывает последовательности клавиш для входа в сервисный режим и для перезагрузки сканера
        /// </summary>
        /// <param name="pwd">Строка из 6 символов, считанных с клавиатуры</param>
        /// <returns>true - найден пользователь, соответствующий введённому паролю, false - не найден</returns>
        private Boolean TestPassword(string pwd)
        {
            Config.userName = "";

            foreach (User u in MainForm.settings.Users) 
            {
                if (u.Pwd == pwd)
                {
                    Config.userName = u.FIO; // нашли, чей пароль
                    break;
                }
            }

            if (Config.userName != "") 
            {
                return true;
            }
            else
            { 
                if (pwd == ".1111.") // вход в сервисный режим
                {
                    ServiceForm serviceform = new ServiceForm();
                    serviceform.ShowDialog();
                    serviceform.Close();
                }
                else
                    if (pwd == "..11..") // перезагрузка сканера
                    {
                        MainForm.SoftReset();
                    }
                return false;
            }

        }

        /// <summary>
        /// Обработчик нажатия клавиш.
        /// Когда нажато 6 клавиш - вызывает функцию проверки пароля. Если пароль неверен - предлагает ввести ещё раз.
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 6)
            {
                if (TestPassword(textBox1.Text))
                { 
                    LoginResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    if (DialogForm.Dialog("Неверный пароль", "", "Ошибка!", "           Повторить", "         Выход") == DialogResult.Retry)
                        {
                            textBox1.Text = "";
                        }
                        else
                        {
                            LoginResult = DialogResult.Abort;
                            Close();
                        }
                }
            }
        }

        /// <summary>
        /// Форму ввода пароля вызываем через функцию, чтобы вернуть в MainForm результат проверки пароля.
        /// </summary>
        /// <returns>DialogResult.OK - пользователь найден, вход в программу разрешён, DialogResult.Abort - производим выход из программы</returns>
        public static DialogResult Dialog()
        {
            MainForm.scanmode = ScanMode.Nothing;
            LoginForm f = new LoginForm();
            f.ShowDialog();
            f.Close();
            return LoginResult;
        }

        /// <summary>
        /// Выход из формы ввода пароля и, соответственно, из программы
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            LoginResult = DialogResult.Abort;
            Close();
        }

        /// <summary>
        /// Обработка нажатия клавиш на форме
        /// </summary>
        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                buttonF4_Click(this, e);
            }
        }

        /// <summary>
        /// Обработка нажатия клавиш в поле ввода
        /// </summary>
/*        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonClose_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonClose_Click(this, e);
            }
        }*/

    }
}