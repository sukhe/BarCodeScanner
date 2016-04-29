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
    public partial class XCodeInfoForm : Form
    {
        /// <summary>
        /// Конструктор формы, отображающей полную информацию по штрихкоду
        /// </summary>
        public XCodeInfoForm()
        {
            InitializeComponent();

            label11.Text = MainForm.xcodetable.Rows[XCodeListForm.currentxcoderow].Field<string>(0);
            label12.Text = MainForm.ConvertToDDMMYYhhmmss(MainForm.xcodetable.Rows[XCodeListForm.currentxcoderow].Field<string>(5));
            label13.Text = MainForm.xcodetable.Rows[XCodeListForm.currentxcoderow].Field<string>(2);
            label14.Text = MainForm.ConvertToDDMMYYhhmmss(MainForm.xcodetable.Rows[XCodeListForm.currentxcoderow].Field<string>(3));
            label15.Text = MainForm.xcodetable.Rows[XCodeListForm.currentxcoderow].Field<string>(4);

            if (label14.Text == "")
            {
                label4.Enabled = false;
                label5.Enabled = false;
                label14.Enabled = false;
                label15.Enabled = false;
            }
            else
            {
                label4.Enabled = true;
                label5.Enabled = true;
                label14.Enabled = true;
                label15.Enabled = true;
            };

            label16.Text = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(0);
            label17.Text = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(1);
            
            label18.Text = MainForm.cargodocs[MainForm.currentdocrow].Number.Trim();
            label19.Text = MainForm.ConvertToDDMMYYhhmmss(MainForm.cargodocs[MainForm.currentdocrow].Data.ToString());
            label20.Text = MainForm.cargodocs[MainForm.currentdocrow].Partner.Trim();
        }

        private void buttonF4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void XCodeInfoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F4) 
                buttonF4_Click(this, e);
        }

    }
}