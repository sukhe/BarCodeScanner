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
            if (pwd != "111111") return false;
            else return true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 6)
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

    }
}