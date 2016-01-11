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
    public partial class ProductListForm : Form
    {
        public ProductListForm()
        {
            InitializeComponent();
            label1.Text = MainForm.cargodocs[MainForm.currentdocrow].Partner.Trim() + " " + MainForm.cargodocs[MainForm.currentdocrow].Number.Trim();
        }
    }
}