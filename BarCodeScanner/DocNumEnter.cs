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
    public partial class DocNumEnter : Form
    {
//        Boolean flag;

        public DocNumEnter()
        {
            InitializeComponent();
//            flag = false;
        }

        private void DocNumEnter_KeyDown(object sender, KeyEventArgs e)
        {
            
            if ((e.KeyCode == System.Windows.Forms.Keys.F1) &&
                 dateTimePicker1.Focused)
            {
                this.Owner.Tag = textBox1.Text.Trim() + " " + dateTimePicker1.Value.Year.ToString() +
                                 MainForm.AddZeroIfNeed(dateTimePicker1.Value.Month) + MainForm.AddZeroIfNeed(dateTimePicker1.Value.Day);
                Close();
            }

            if (( 
                  (e.KeyCode == System.Windows.Forms.Keys.Enter) ||
                  (e.KeyCode == System.Windows.Forms.Keys.F1)
                ) && textBox1.Focused )
            {
                dateTimePicker1.Focus();
//                flag = true;
            }

/*            if ((e.KeyCode != System.Windows.Forms.Keys.F1) && (flag))
            {
                flag = false;
            }

            if ((
                  (e.KeyCode == System.Windows.Forms.Keys.F1)
                ) && (!flag) && dateTimePicker1.Focused)*/

            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                this.Controls.Remove(dateTimePicker1);
                dateTimePicker1.Dispose();
                button2_Click(this, e);
            }

        }

        private void dateTimePicker1_GotFocus(object sender, EventArgs e)
        {
            button1.Text = "           Принять";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Owner.Tag = "";
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Focused) dateTimePicker1.Focus();
        }

        private void dateTimePicker1_DropDown(object sender, System.EventArgs e)
        {
            MessageBox.Show("Hello!");
        }

    }
}