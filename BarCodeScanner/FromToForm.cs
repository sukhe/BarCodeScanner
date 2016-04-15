using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BarCodeScanner
{
    public partial class FromToForm : Form
    {
        private List<ListBox> list = new List<ListBox>();

        public FromToForm()
        {
            InitializeComponent();
            list.Add(listBox1);
            list.Add(listBox2);
            list.Add(listBox3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NextList();
        }

        private void FromToForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                NextList();
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                NextList();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label12.Text = listBox1.SelectedItem.ToString();
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label22.Text = listBox2.SelectedItem.ToString();
        }

        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label32.Text = listBox3.SelectedItem.ToString();
        }

        private void FromToForm_Load(object sender, EventArgs e)
        {
            foreach (Transfer t in MainForm.settings.Transfers)
            {
                listBox1.Items.Add(t.Name);
            }
            listBox1.Focus();
            if (listBox1.Items.Count == 0)
            {
                MainForm.LogShow("[FTF.Load.1] Ошибка файла настроек: нет доступных операций");
                Close();
            }
            else
                listBox1.SelectedIndex = 0;
        }

        private void NextList()
        {
            string[] s;
            if (listBox1.Focused)
            {
                listBox2.Enabled = true;

                s = MainForm.settings.Transfers[listBox1.SelectedIndex].From.Split(',');
                foreach (string t in s)
                {
                    foreach (Location n in MainForm.settings.Locations)
                    {
                        if (n.LID == t)
                        {
                            listBox2.Items.Add(n.Name);
                        }
                    }
                }

                if (listBox2.Items.Count == 0)
                {
                    MainForm.LogShow("[FTF.Load.2] Ошибка файла настроек: нет списка \"Откуда\"");
                    Close();
                }
                else
                {
                    listBox2.Focus();
                    listBox2.SelectedIndex = 0;
                    listBox1.Visible = false;
                }
            }
            else if (listBox2.Focused)
            {
                listBox3.Enabled = true;
/*                button1.Visible = false;
                label5.Visible = false;*/

                s = MainForm.settings.Transfers[listBox1.SelectedIndex].To.Split(',');
                foreach (string t in s)
                {
                    foreach (Location n in MainForm.settings.Locations)
                    {
                        if (n.LID == t)
                        {
                            listBox3.Items.Add(n.Name);
                        }
                    }
                }

                if (listBox3.Items.Count == 0)
                {
                    MainForm.LogShow("[FTF.Load.3] Ошибка файла настроек: нет списка \"Куда\"");
                    Close();
                }
                else
                {
                    listBox3.Focus();
                    listBox3.SelectedIndex = 0;
                    listBox2.Visible = false;
                }
            }
            else if (listBox3.Focused) 
                Close();
        }

        private void FromToForm_Closing(object sender, CancelEventArgs e)
        {

            if (listBox2.Visible == false) // прошли уже все три списка
            {
                //Config.transfer = label12.Text;
                foreach (Location n in MainForm.settings.Locations)
                {
                    if (n.Name == label22.Text)
                    {
                        Config.transferFrom = label22.Text;
                        Config.transferFromLid = n.LID;
                    }
                }

                foreach (Location n in MainForm.settings.Locations)
                {
                    if (n.Name == label32.Text)
                    {
                        Config.transferTo = label32.Text;
                        Config.transferToLid = n.LID;
                    }
                }

            }
                
        }

    }
}