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

        private static DialogResult LoginResult;

        public LoginForm()
        {
            InitializeComponent();
        }

        private Boolean TestPassword(string pwd)
        {
            Config.userName = "";

/*            try
            {
                string p = pwd.Substring(0, 6);
                double d = Math.Sqrt(Convert.ToInt64(p) * 171587);
                pwd = Math.Floor(Math.Sqrt(i * 171587)).ToString(); // дополнить спереди нулями
                    //=ЦЕЛОЕ(КОРЕНЬ(A11*171587))
            }
            catch
            {
                return false;
            } */

            foreach (User u in MainForm.settings.Users)
            {
                if (u.Pwd == pwd)
                {
                    Config.userName = u.FIO;
                    break;
                }
            }

            if (Config.userName != "")
            {
//                Config.superuser = false;
                return true;
            }
            else
            { 
                if (pwd == ".1111.")
                {
                    Config.userName = "Superuser";
//                    Config.superuser = true;
                    ServiceForm serviceform = new ServiceForm();
                    serviceform.ShowDialog();
                    serviceform.Close();
//                    return true;
                }
                else
                    if (pwd == "..11..")
                    {
                        MainForm.SoftReset();
                    }
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
/*                    if (Config.userName == "Superuser")
                    {
                        Config.userName = "";
                        if (DialogForm.Dialog("Войти в программу?", "", "Вопрос", "              Да", "          Нет") == DialogResult.Retry)
                        {
                            textBox1.Text = "";
                        }
                        else
                        {
                            LoginResult = DialogResult.Abort;
                            Close();
                        }
                    }
                    else
                    {*/
                        if (DialogForm.Dialog("Неверный пароль", "", "Ошибка!", "           Повторить", "         Выход") == DialogResult.Retry)
                        {
                            textBox1.Text = "";
                        }
                        else
                        {
                            LoginResult = DialogResult.Abort;
                            Close();
                        }
//                    }
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
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonClose_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonClose_Click(this, e);
            }

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonClose_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonClose_Click(this, e);
            }
        }

    }
}