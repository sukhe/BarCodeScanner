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
    public partial class XCodeListForm : Form
    {
        private Color fullColor;
        private Color partialColor;

        public XCodeListForm()
        {
            InitializeComponent();
            label1.Text = MainForm.cargodocs[MainForm.currentdocrow].Partner.Trim() + " " + MainForm.cargodocs[MainForm.currentdocrow].Number.Trim();
            MainForm.scanmode = ScanMode.BarCod;

            fullColor = new Color();
            partialColor = new Color();
            partialColor = Color.FromArgb(255, 127, 127);
            fullColor = Color.FromArgb(127, 255, 127);

            dataGrid1.Focus();
        }

        private void XCodeListForm_Load(object sender, EventArgs e)
        {
            if (MainForm.xcodetable == null)
                MainForm.xcodetable = new DataTable();
            dataGrid1.DataSource = MainForm.xcodetable;
            GetXCodes();
        }

/*
        private string GetNameFromPID(string s)
        {
            string name = "";
            foreach (Product p in MainForm.cargodocs[MainForm.currentdocrow].TotalProducts)
            {
                if (p.PID == s) 
                { 
                    name = p.PName;
                    break;
                }
            }
            return name;
        } */

        public void ReloadXCodeTable()
        {
            MainForm.xcodetable.Rows.Clear();
            label1.Text = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(1);
            string pid = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(0);

            foreach (XCode x in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
            {
                if (pid==x.PID)
                {
                    MainForm.xcodetable.Rows.Add(new object[] { x.ScanCode, MainForm.colData(x.Data), x.Fio, x.DData, x.DFio, x.Data });
                }
            }
            MainForm.xcodetable.AcceptChanges();
        }

        private void GetXCodes()
        {
            //            AddDataToTable();

            if (MainForm.xcodetable.Columns.Count == 0)
            {

//                DataColumn PID = MainForm.producttable.Columns.Add("Код", typeof(string));
                DataColumn BarCod = MainForm.xcodetable.Columns.Add("Штрихкод", typeof(string));
                MainForm.xcodetable.Columns.Add("Дата", typeof(string));
                MainForm.xcodetable.Columns.Add("ФИО", typeof(string));
                MainForm.xcodetable.Columns.Add("ДатаДел", typeof(string));
                MainForm.xcodetable.Columns.Add("ФИОДел", typeof(string));
                DataColumn FullData = MainForm.xcodetable.Columns.Add("ПолнДат", typeof(string));

                // Set the ID column as the primary key column.
                MainForm.xcodetable.PrimaryKey = new DataColumn[] { FullData, BarCod };
            }

            ReloadXCodeTable();

            // ширина колонок
            MainForm.xcodelistform.dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            DataGridTextBoxColumnColored col0 = new DataGridTextBoxColumnColored();
//            DataGridTextBoxColumn col0 = new DataGridTextBoxColumn();
            col0.Width = 175;
            col0.MappingName = MainForm.xcodetable.Columns[0].ColumnName;
            col0.HeaderText = MainForm.xcodetable.Columns[0].ColumnName;
            col0.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col0);


            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            //DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            //            if (MainForm.producttable.Rows.Count > 9) col2.Width = 236;
            //            else col2.Width = 260;
            col1.Width = 95; //204
            col1.MappingName = MainForm.xcodetable.Columns[1].ColumnName;
            col1.HeaderText = MainForm.xcodetable.Columns[1].ColumnName;
            col1.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            //DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            if (MainForm.xcodetable.Rows.Count > 12) col2.Width = 132;
            else col2.Width = 156;
            col2.MappingName = MainForm.xcodetable.Columns[2].ColumnName;
            col2.HeaderText = MainForm.xcodetable.Columns[2].ColumnName;
            col2.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();            
            //DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            col3.Width = 0;
            col3.MappingName = MainForm.xcodetable.Columns[3].ColumnName;
            col3.HeaderText = MainForm.xcodetable.Columns[3].ColumnName;
            col3.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col3);

            DataGridTextBoxColumn col4 = new DataGridTextBoxColumn();
            col4.Width = 0;
            col4.MappingName = MainForm.xcodetable.Columns[4].ColumnName;
            col4.HeaderText = MainForm.xcodetable.Columns[4].ColumnName;
            tableStyle.GridColumnStyles.Add(col4);

            DataGridTextBoxColumn col5 = new DataGridTextBoxColumn();
            col5.Width = 0;
            col5.MappingName = MainForm.xcodetable.Columns[5].ColumnName;
            col5.HeaderText = MainForm.xcodetable.Columns[5].ColumnName;
            tableStyle.GridColumnStyles.Add(col5);

            //DataGridTextBoxColumnColored col4 = new DataGridTextBoxColumnColored();
            /*DataGridTextBoxColumn col4 = new DataGridTextBoxColumn();
            col4.Width = 71;
            col4.MappingName = MainForm.producttable.Columns[3].ColumnName;
            col4.HeaderText = MainForm.producttable.Columns[3].ColumnName;
            //col4.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col4); */

            // учесть ширину вертикальной прокрутки в ширине колонок

            //MainForm.xcodelistform.
                dataGrid1.TableStyles.Add(tableStyle);

            MainForm.xcodetable.AcceptChanges();
        }

        private void button1_Click(object sender, EventArgs e)
        {
//            MessageBox.Show("Удаление штрих-кода ещё не готово");
            int i = dataGrid1.CurrentRowIndex;
            string barcod = MainForm.xcodetable.Rows[i].Field<string>(0);
            string data = MainForm.xcodetable.Rows[i].Field<string>(5);
            MainForm.scanmode = ScanMode.Nothing;
            if (DialogForm.Dialog("Удалить штрих-код ",barcod, "Удалить?", "        Да", "        Нет") == DialogResult.Retry)
            {
//                MainForm.xcodetable.Rows[i][3] = "1";
//                MainForm.xcodetable.Rows[i][4] = Config.userName;

                XCode x = new XCode();

                int j = 0;
                foreach (XCode z in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
                {
                    if (z.Data == data && z.ScanCode == barcod)
                    {
                        x = MainForm.cargodocs[MainForm.currentdocrow].XCodes[j];
                        break;
                    }
                    j++;
                }

//                x = MainForm.cargodocs[MainForm.currentdocrow].XCodes[i];


                x.DData = MainForm.nowcolData(System.DateTime.Now.ToString());
                x.DFio = Config.userName;
                x.Fio = System.DateTime.Now.ToShortTimeString();

/*                x.Data = MainForm.cargodocs[MainForm.currentdocrow].XCodes[i].Data;
                x.Fio = MainForm.cargodocs[MainForm.currentdocrow].XCodes[i].Fio;
                x.DData = MainForm.nowcolData(System.DateTime.Now.ToString());;
                x.DFio = Config.userName;
                x.PID = MainForm.cargodocs[MainForm.currentdocrow].XCodes[i].PID;
//                x.PID = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(0);
                x.ScanCode = MainForm.cargodocs[MainForm.currentdocrow].XCodes[i].ScanCode;
                x.ScanFrom = "";
                x.ScanTo = "";
                x.ScannerID = MainForm.cargodocs[MainForm.currentdocrow].XCodes[i].ScannerID;*/

//                var xl = new List<XCode>();
//                xl.AddRange(MainForm.cargodocs[MainForm.currentdocrow].XCodes);
//                xl.Add(x);
/*                j = 0;
                foreach (XCode z in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
                {
                    if (z.Data == data && z.ScanCode == barcod)
                    { */
//                        MainForm.cargodocs[MainForm.currentdocrow].XCodes[j] = x;
/*                        break;
                    }
                    j++;
                } */

//                MainForm.cargodocs[MainForm.currentdocrow].XCodes[i] = x;

                if (MainForm.xcodelistform != null && MainForm.xcodelistform.Visible)
                {
                    MainForm.xcodetable.AcceptChanges();
                    MainForm.xcodelistform.ReloadXCodeTable();
                }
//                else productlistform.ReloadProductTable();


            }
            MainForm.scanmode = ScanMode.BarCod;
        }

        private void button4_Click(object sender, EventArgs e)
        {

            MainForm.productlistform.ReloadProductTable();
            Close();
        }

        private void XCodeListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }

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
            public delegate void NeedBackgroundEventHandlerXCode(object sender, NeedBackgroundEventArgs e);
            public event NeedBackgroundEventHandlerXCode NeedBackgroundXCode;

            //А вот и переопределенный метод DataGridTextBoxColumn.Paint(), 
            //запрашивающий при помощи события (аргументов) цвет и передающий его 
            //базовому методу Paint(), в параметре backBrush. 
            //Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, 
            //используя при этом подставленный нами backBrush. 
            protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
            {
                NeedBackgroundEventArgs e = new NeedBackgroundEventArgs(source, rowNum, backBrush, foreBrush);
                if (NeedBackgroundXCode != null) NeedBackgroundXCode(this, e);
                base.Paint(g, bounds, source, rowNum, e.BackBrush, e.ForeBrush, alignToRight);
            }
        }

        private void OnBackgroundEventHandlerProductXCode(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {
            e.ForeBrush = new SolidBrush(Color.Black);

            if (MainForm.xcodetable.Rows[e.RowNum].Field<string>(3) == "")
                e.BackBrush = new SolidBrush(Color.White);
            else
                e.BackBrush = new SolidBrush(partialColor);
        }
    }
}