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

        public static int currentproductrow;
/*        private Color fullColor;
        private Color partialColor; */

        public ProductListForm()
        {
            InitializeComponent();
            label1.Text = "№"+MainForm.cargodocs[MainForm.currentdocrow].Number.Trim() + " / " + MainForm.cargodocs[MainForm.currentdocrow].Partner.Trim();
            MainForm.scanmode = ScanMode.BarCod;

/*            fullColor = new Color();
            partialColor = new Color();
            partialColor = Color.FromArgb(255, 127, 127);
            fullColor = Color.FromArgb(127, 255, 127); */

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
            DataGridTextBoxColumnColored col0 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col0 = new DataGridTextBoxColumn();
            col0.Width = 80;
            col0.MappingName = MainForm.producttable.Columns[0].ColumnName;
            col0.HeaderText = MainForm.producttable.Columns[0].ColumnName;
            col0.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col0);

            
            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            if (MainForm.producttable.Rows.Count > 12) 
                col1.Width = 180;
            else 
                col1.Width = 204;
            col1.MappingName = MainForm.producttable.Columns[1].ColumnName;
            col1.HeaderText = MainForm.producttable.Columns[1].ColumnName;
            col1.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            col2.Width = 71;
            col2.MappingName = MainForm.producttable.Columns[2].ColumnName;
            col2.HeaderText = MainForm.producttable.Columns[2].ColumnName;
            col2.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();
            //            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            col3.Width = 71;
            col3.MappingName = MainForm.producttable.Columns[3].ColumnName;
            col3.HeaderText = MainForm.producttable.Columns[3].ColumnName;
            col3.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col3);

            // учесть ширину вертикальной прокрутки в ширине колонок

            MainForm.productlistform.dataGrid1.TableStyles.Add(tableStyle);

            MainForm.producttable.AcceptChanges();
        }

        public void ReloadProductTable()
        {
            string pid;
            int i;
            int q = 0;
            int b = 0;
            MainForm.producttable.Rows.Clear();
            foreach (Product p in MainForm.cargodocs[MainForm.currentdocrow].TotalProducts)
            {
                pid = p.PID;
                i = 0;
                q += Convert.ToInt16(p.Quantity);
                foreach (XCode x in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
                {
                    if (pid == x.PID) i++;
                }
                b += i;
                p.ScannedBar = i.ToString();
                MainForm.producttable.Rows.Add(new object[] { p.PID, p.PName, p.Quantity, p.ScannedBar });

//                MainForm.producttable.Rows.Add(new object[] { p.PID });
            }
            MainForm.producttable.AcceptChanges();
            MainForm.cargodocs[MainForm.currentdocrow].Quantity = q.ToString();
            MainForm.cargodocs[MainForm.currentdocrow].ScannedBar = b.ToString();
            label2.Text = q.ToString() + "/" + b.ToString();
            if (b == 0) label2.BackColor = Color.White;
            else if (b < q) label2.BackColor = MainForm.partialColor;
            else label2.BackColor = MainForm.fullColor;
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
            public delegate void NeedBackgroundEventHandlerProduct(object sender, NeedBackgroundEventArgs e);
            public event NeedBackgroundEventHandlerProduct NeedBackgroundProduct;

            //А вот и переопределенный метод DataGridTextBoxColumn.Paint(), 
            //запрашивающий при помощи события (аргументов) цвет и передающий его 
            //базовому методу Paint(), в параметре backBrush. 
            //Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, 
            //используя при этом подставленный нами backBrush. 
            protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
            {
                NeedBackgroundEventArgs e = new NeedBackgroundEventArgs(source, rowNum, backBrush, foreBrush);
                if (NeedBackgroundProduct != null) NeedBackgroundProduct(this, e);
                base.Paint(g, bounds, source, rowNum, e.BackBrush, e.ForeBrush, alignToRight);
            }
        }

        private void OnBackgroundEventHandlerProduct(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {
            e.ForeBrush = new SolidBrush(Color.Black);

            int q = Convert.ToInt16(MainForm.producttable.Rows[e.RowNum][2]);
            int b = Convert.ToInt16(MainForm.producttable.Rows[e.RowNum][3]);

            if ((b < q) && (b != 0))
                e.BackBrush = new SolidBrush(MainForm.partialColor);
            else if (b >= q)
                e.BackBrush = new SolidBrush(MainForm.fullColor);
            else
                e.BackBrush = new SolidBrush(Color.White);
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            currentproductrow = dataGrid1.CurrentRowIndex;
        }

        private void ProductListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                dataGrid1_Click(sender, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            CargoDoc d = new CargoDoc();
            d = MainForm.cargodocs[MainForm.currentdocrow];
            string n = d.Number.Trim();
            string t = MainForm.ConvertToYYYYMMDD(d.Data);
            d.SaveToFile(MainForm.CurrentPath + @"doc\"+ n + "_" + t + "_" + Config.scannerNumber.ToString() + ".xml");
            MainForm.scanmode = ScanMode.Doc;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Удаление штрих-кода ещё не готово");
        }

        private void dataGrid1_Click(object sender, EventArgs e)
        {
//            currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
            currentproductrow = dataGrid1.CurrentCell.RowNumber;
            MainForm.xcodelistform = new XCodeListForm();
            MainForm.xcodelistform.Show();
        }

    }
}