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
            MainForm.scanmode = ScanMode.Nothing;
            dataGrid1.Focus();
        }

        private void ProductListForm_Load(object sender, EventArgs e)
        {
            if (MainForm.producttable == null)
                MainForm.producttable = new DataTable();
            dataGrid1.DataSource = MainForm.producttable;
            GetProducts();
        }

        private void GetProducts()
        {
            //            AddDataToTable();

            if (MainForm.producttable.Columns.Count == 0)
            {

                DataColumn PID = MainForm.producttable.Columns.Add("Код", typeof(string));
                MainForm.producttable.Columns.Add("Название", typeof(string));
                MainForm.producttable.Columns.Add("Надо", typeof(string));
                MainForm.producttable.Columns.Add("Уже", typeof(string));

                // Set the ID column as the primary key column.
                MainForm.producttable.PrimaryKey = new DataColumn[] { PID };
            }

            ReloadProductTable();

            // ширина колонок
            MainForm.productlistform.dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            col1.Width = 90;
            col1.MappingName = MainForm.producttable.Columns[0].ColumnName;
            col1.HeaderText = MainForm.producttable.Columns[0].ColumnName;
            col1.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            col2.Width = 180;
            col2.MappingName = MainForm.producttable.Columns[1].ColumnName;
            col2.HeaderText = MainForm.producttable.Columns[1].ColumnName;
            col2.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            col3.Width = 80;
            col3.MappingName = MainForm.producttable.Columns[2].ColumnName;
            col3.HeaderText = MainForm.producttable.Columns[2].ColumnName;
            col3.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col3);

            DataGridTextBoxColumnColored col4 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            col4.Width = 80;
            col4.MappingName = MainForm.producttable.Columns[3].ColumnName;
            col4.HeaderText = MainForm.producttable.Columns[3].ColumnName;
            col4.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col4);

            // учесть ширину вертикальной прокрутки в ширине колонок

            MainForm.productlistform.dataGrid1.TableStyles.Add(tableStyle);

            MainForm.producttable.AcceptChanges();
        }

        public void ReloadProductTable()
        {
            MainForm.producttable.Rows.Clear();
            foreach (Product p in MainForm.cargodocs[MainForm.currentdocrow].TotalProducts)
            {
                MainForm.producttable.Rows.Add(new object[] { p.PID, p.PName, p.Quantity, p.ScannedBar });
            }
            MainForm.producttable.AcceptChanges();
        }

        // разукрашивание ячеек в нужный цвет
        public class DataGridTextBoxColumnColored : DataGridTextBoxColumn
        {
            //Определим класс аргумента события, делегат и само событие, 
            //необходимые для "общения" кода выполняющего прорисовку ячейки, с кодом, 
            //предоставляющим цвет для этой ячейки. 
            public class NeedBackgroundEventArgs : EventArgs
            {
                private int FRowNum;
                private Brush FBackBrush;
                private Brush FForeBrush;
                private CurrencyManager FSource;

                public int RowNum { get { return FRowNum; } }
                public Brush BackBrush { get { return FBackBrush; } set { FBackBrush = value; } }
                public Brush ForeBrush { get { return FForeBrush; } set { FForeBrush = value; } }
                public CurrencyManager Source { get { return FSource; } }

                public NeedBackgroundEventArgs(CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush)
                {
                    this.FRowNum = rowNum;
                    this.FBackBrush = BackBrush;
                    this.FForeBrush = foreBrush;
                    this.FSource = source;
                }
            }
            public delegate void NeedBackgroundEventHandler(object sender, NeedBackgroundEventArgs e);
            public event NeedBackgroundEventHandler NeedBackground;

            //А вот и переопределенный метод DataGridTextBoxColumn.Paint(), 
            //запрашивающий при помощи события (аргументов) цвет и передающий его 
            //базовому методу Paint(), в параметре backBrush. 
            //Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, 
            //используя при этом подставленный нами backBrush. 
            protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
            {
                NeedBackgroundEventArgs e = new NeedBackgroundEventArgs(source, rowNum, backBrush, foreBrush);
                if (NeedBackground != null) NeedBackground(this, e);
                base.Paint(g, bounds, source, rowNum, e.BackBrush, e.ForeBrush, alignToRight);
            }
        }

        private void OnBackgroundEventHandler(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {

            Color fullColor = new Color();
            Color partialColor = new Color();

            fullColor = Color.FromArgb(255, 127, 127);
            partialColor = Color.FromArgb(127, 255, 127);

            /*            if (e.RowNum == dataGrid1.CurrentRowIndex)
                        {
                            e.BackBrush = new SolidBrush(dataGrid1.SelectionBackColor);
                            e.ForeBrush = new SolidBrush(dataGrid1.SelectionForeColor);
                        }
                        else
                        {*/
            /*                int divVal = e.RowNum / 2;
                            if (divVal * 2 != e.RowNum) e.BackBrush = new SolidBrush(partialColor);
                            else e.BackBrush = new SolidBrush(fullColor);*/

            //e.ForeBrush = new SolidBrush(dataGrid1.ForeColor);
            e.ForeBrush = new SolidBrush(Color.Black);

            string val = MainForm.doctable.Rows[e.RowNum][0].ToString().Trim();
            if (val == "337")
                //                    e.BackBrush = new SolidBrush(Color.LightGreen);
                e.BackBrush = new SolidBrush(fullColor);
            else if (val == "336")
                //                    e.BackBrush = new SolidBrush(Color.Pink);
                e.BackBrush = new SolidBrush(partialColor);
            else
                e.BackBrush = new SolidBrush(dataGrid1.BackColor);

            //                e.ForeBrush = new SolidBrush(dataGrid1.ForeColor); 
            //            } 
        }

    }
}