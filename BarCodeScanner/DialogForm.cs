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
    public partial class DialogForm : Form
    {
        private static DialogResult dr;

        public DialogForm(string text, string title, string button1Text, string button2Text)
        {
            InitializeComponent();
            label1.Text = text;
            this.Text = title;
            buttonRetry.Text = button1Text;
            buttonCancel.Text = button2Text;
        }

        private void buttonRetry_Click(object sender, EventArgs e)
        {
            DialogResult = buttonRetry.DialogResult;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = buttonCancel.DialogResult;
            Close();
        }

        public static DialogResult Dialog(string text, string title, string button1Text, string button2Text)
        {
            DialogForm d = new DialogForm(text, title, button1Text, button2Text);
            dr = d.ShowDialog();
            d.Close();
            return dr;
        }

        private void DialogForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                //                MessageBox.Show("Нажата F1 аппаратно");
                buttonRetry_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                buttonRetry_Click(this, e);
                //                MessageBox.Show("Нажата F2 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                buttonCancel_Click(this, e);
                //                MessageBox.Show("Нажата F3 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonCancel_Click(this, e);
                //                MessageBox.Show("Нажата F4 аппаратно");
            }

        }

    }
}