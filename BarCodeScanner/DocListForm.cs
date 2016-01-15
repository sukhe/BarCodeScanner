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
    public partial class DocListForm : Form
    {

        public DocListForm()
        {
            InitializeComponent();
            MainForm.scanmode = ScanMode.Doc;
            dataGrid1.Focus();
        }

        private void DocListForm_Closing(object sender, CancelEventArgs e)
        {
            Tag = "Closing";
        }

        private void DocListForm_Load(object sender, EventArgs e)
        {
            if (MainForm.doctable == null)
                MainForm.doctable = new DataTable();
            dataGrid1.DataSource = MainForm.doctable;
            GetCustomers();
        }

        public void ReloadDocTable()
        {
//            DataTable table = new DataTable();
//            dataGrid1.DataSource = table;
//            DataTableReader reader = new DataTableReader(GetCustomers(table, dataGrid1));
//            MainForm.doctable.Clear();

//            MainForm.doctable.Load(MainForm.docreader);

//            MainForm.doclistform.dataGrid1.Refresh();
//            MainForm.doclistform.dataGrid1.Update();

//            GetCustomers();

            MainForm.doctable.Rows.Clear();

            foreach (CargoDoc d in MainForm.cargodocs)
            {
                MainForm.doctable.Rows.Add(new object[] { d.Number.Trim(), colData(d.Data), d.Partner });
            }

            MainForm.doctable.AcceptChanges();

        }

        private string colData(string s)
        {
            string ss;
            try
            {
                ss = s.Substring(8, 2) + '.' + s.Substring(5, 2) + '.' + s.Substring(2, 2);
            }
            catch
            {
                ss = "01.01.01";
            }
            return ss;
        }

/*        private void AddDataToTable()
        {
            MainForm.doctable.Rows.Clear();

            DataColumn Number = MainForm.doctable.Columns.Add("№ док.", typeof(string));
            MainForm.doctable.Columns.Add("Дата", typeof(string));
            MainForm.doctable.Columns.Add("Контрагент", typeof(string));

            // Set the ID column as the primary key column.
            MainForm.doctable.PrimaryKey = new DataColumn[] { Number };

            foreach (CargoDoc d in MainForm.cargodocs)
            {
                MainForm.doctable.Rows.Add(new object[] { d.Number.Trim(), colData(d.Data), d.Partner });
            }

            MainForm.doctable.AcceptChanges();

        } */

//        private DataTable GetCustomers()
//            (DataTable table, DataGrid datagrid)
        private void GetCustomers()
        {
//            AddDataToTable();

            if (MainForm.doctable.Columns.Count == 0)
            {

                DataColumn Number = MainForm.doctable.Columns.Add("№ док.", typeof(string));
                MainForm.doctable.Columns.Add("Дата", typeof(string));
                MainForm.doctable.Columns.Add("Контрагент", typeof(string));

                // Set the ID column as the primary key column.
                MainForm.doctable.PrimaryKey = new DataColumn[] { Number };
            }

            ReloadDocTable();
            
            // ширина колонок
            MainForm.doclistform.dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
//            tableStyle.MappingName = MainForm.doctable.TableName;

/*            vColumn.MappingName = "Name";
            vColumn.HeaderText = "Наименование";*/

            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
//            DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            col1.Width = 80;
            col1.MappingName = MainForm.doctable.Columns[0].ColumnName;
            col1.HeaderText = MainForm.doctable.Columns[0].ColumnName;
            col1.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
//            DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            col2.Width = 90;
            col2.MappingName = MainForm.doctable.Columns[1].ColumnName;
            col2.HeaderText = MainForm.doctable.Columns[1].ColumnName;
            col2.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();
//            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
//            if (MainForm.cargodocs.Count > 9) col3.Width = 236;
//            else 
            col3.Width = 260;
            col3.MappingName = MainForm.doctable.Columns[2].ColumnName;
            col3.HeaderText = MainForm.doctable.Columns[2].ColumnName;
            col3.NeedBackground += new DataGridTextBoxColumnColored.NeedBackgroundEventHandler(OnBackgroundEventHandler);
            tableStyle.GridColumnStyles.Add(col3);

            MainForm.doclistform.dataGrid1.TableStyles.Add(tableStyle);


/*            table.Rows.Add(new object[] { 0, "Mary" });
            table.Rows.Add(new object[] { 1, "Andy" });
            table.Rows.Add(new object[] { 2, "Peter" });*/

            MainForm.doctable.AcceptChanges();
//            return MainForm.doctable;
        }

        # region Buttons/Events
        private void DocListForm_Closed(object sender, EventArgs e)
        {
            Tag = "Closed";
        }

        private void DocListForm_GotFocus(object sender, EventArgs e)
        {
            Tag = "GotFocus";
        }

        private void DocListForm_Activated(object sender, EventArgs e)
        {
            Tag = "Activated";
        }

        private void DocListForm_Deactivate(object sender, EventArgs e)
        {
            Tag = "Deactivate";
        }

        private void DocListForm_LostFocus(object sender, EventArgs e)
        {
            Tag = "LostFocus";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void DocListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                //                MessageBox.Show("Нажата F1 аппаратно");
//                var z = sender;
//                var ee = e;
/*                MainForm.productlistform = new ProductListForm();
                MainForm.productlistform.Show();*/
                dataGrid1_Click(sender, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                //                MessageBox.Show("Нажата F1 аппаратно");
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F2))
            {
                button2_Click(this, e);
                //                MessageBox.Show("Нажата F2 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                button3_Click(this, e);
                //                MessageBox.Show("Нажата F3 аппаратно");
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
                //                MessageBox.Show("Нажата F4 аппаратно");
            }
        }

        private void DocListForm_Paint(object sender, PaintEventArgs e)
        {
            Tag = "Paint";
        }

        private void dataGrid1_Paint(object sender, PaintEventArgs e)
        {
      //      dataGrid1.
        }

#endregion
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

            partialColor = Color.FromArgb(255, 127, 127);
            fullColor = Color.FromArgb(127, 255, 127);

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

        private void dataGrid1_Click(object sender, EventArgs e)
        {
//            var z = sender;
//            var ee = e;
            MainForm.productlistform = new ProductListForm();
            MainForm.productlistform.Show();
        }

        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            MainForm.currentdoccol = dataGrid1.CurrentCell.ColumnNumber;
            MainForm.currentdocrow = dataGrid1.CurrentCell.RowNumber;
        }

/*        private void dataGrid1_Click(object sender, EventArgs e)
        {
            var z = sender;
            var ee = e;
            //
        } */
    
    }
}