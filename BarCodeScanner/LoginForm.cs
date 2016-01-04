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
//        string CurrentPath;

        private static DialogResult LoginResult;

        public LoginForm()
        {
            InitializeComponent();
        }

/*        private Boolean LoadSettings()
        {
            Boolean ls = false;
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            Settings set = new Settings();
//            using (TextReader reader = new StringReader(set.ToString()))
            try
            {
                //using (var stream = File.OpenRead(@"D:\WORK\CASIO\ConnectTo1C\2418.xml"))
                using (var stream = File.OpenRead(CurrentPath+@"\settings.xml"))
                {
                    set = (Settings)serializer.Deserialize(stream);
                    ls = true;
                    Config.scannerNumber = "1";

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки настроек " + ex.Message.ToString() + " - " + ex.InnerException.Message.ToString()); 
            }
            return ls;
        }*/

        private Boolean TestPassword(string pwd)
        {
            Config.userName = "Вася";

            switch (pwd) {
                case "111111":
                    return true;
                case ".1111.":
                    Config.userName = "Олег";
                    Config.superuser = true;
                    return true;
                default:
                    return false;
            }
        }

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
                    if (DialogForm.Dialog("Неверный пароль", "Ошибка!", "Повторить", "Выход") == DialogResult.Retry)
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

        public static DialogResult Dialog()
        {
            LoginForm f = new LoginForm();
            f.ShowDialog();
            f.Close();
            return LoginResult;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            LoginResult = DialogResult.Abort;
            Close();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
/*            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                //                MessageBox.Show("Нажата F1 аппаратно");
                buttonRetry_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                buttonRetry_Click(this, e);
                //                MessageBox.Show("Нажата F2 аппаратно");
            } */
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonClose_Click(this, e);
                //                MessageBox.Show("Нажата F3 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonClose_Click(this, e);
                //                MessageBox.Show("Нажата F4 аппаратно");
            }

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonClose_Click(this, e);
                //                MessageBox.Show("Нажата F3 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonClose_Click(this, e);
                //                MessageBox.Show("Нажата F4 аппаратно");
            }
        }

    }
}