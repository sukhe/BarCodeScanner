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

        string number = "";

        public DocNumEnter()
        {
            InitializeComponent();
        }

        private void DocNumEnter_KeyDown(object sender, KeyEventArgs e)
        {
            if (( (e.KeyCode == System.Windows.Forms.Keys.Down) ||
                  (e.KeyCode == System.Windows.Forms.Keys.Right) ||
                  (e.KeyCode == System.Windows.Forms.Keys.Enter) ||
                  (e.KeyCode == System.Windows.Forms.Keys.F1)
                ) && textBox1.Focused )
            {
                number = textBox1.Text;
                dateTimePicker1.Focus();
            }
            if (((e.KeyCode == System.Windows.Forms.Keys.Down) ||
                  (e.KeyCode == System.Windows.Forms.Keys.Right) ||
                  (e.KeyCode == System.Windows.Forms.Keys.Enter) ||
                  (e.KeyCode == System.Windows.Forms.Keys.F1)
                ) && dateTimePicker1.Focused)
            {
                number = textBox1.Text + dateTimePicker1.Text;
            }

            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button2_Click(this, e);
            }

        }

        private void dateTimePicker1_GotFocus(object sender, EventArgs e)
        {
            dateTimePicker1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Focused) dateTimePicker1.Focus();
            if (dateTimePicker1.Focused) Close();
        }
    }
}