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
        }

        private void DocListForm_Closing(object sender, CancelEventArgs e)
        {
            Tag = "Closing";
        }

        private void DocListForm_Load(object sender, EventArgs e)
        {

            // показываем таблицу со списком уже имеющихся у нас документов
            DataTable table = new DataTable();
            dataGrid1.DataSource = table;
            DataTableReader reader = new DataTableReader(GetCustomers(table,dataGrid1));
            table.Load(reader);

            // а вот таперича пробуем загрузить ещё документов

//            if (

        }

        private string colData(string s)
        {

            return s.Substring(8,2)+'.'+s.Substring(5,2)+'.'+s.Substring(2,2);
        }

        private DataTable GetCustomers(DataTable table, DataGrid datagrid)
        {

            DataColumn Number = table.Columns.Add("№ док.", typeof(string));
            table.Columns.Add("Дата", typeof(string));
            table.Columns.Add("Контрагент", typeof(string));

            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { Number };

            foreach (CargoDoc d in MainForm.cargodocs)
            {
                table.Rows.Add(new object[] { d.Number.Trim(), colData(d.Data), d.Partner });
            }

            // ширина колонок
            datagrid.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = table.TableName;

            DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
            col1.Width = 80;
            col1.MappingName = table.Columns[0].ColumnName;
            col1.HeaderText = table.Columns[0].ColumnName;
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
            col2.Width = 90;
            col2.MappingName = table.Columns[1].ColumnName;
            col2.HeaderText = table.Columns[1].ColumnName;
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumn col3 = new DataGridTextBoxColumn();
            col3.Width = 280;
            col3.MappingName = table.Columns[2].ColumnName;
            col3.HeaderText = table.Columns[2].ColumnName;
            tableStyle.GridColumnStyles.Add(col3);

            datagrid.TableStyles.Add(tableStyle);


/*            table.Rows.Add(new object[] { 0, "Mary" });
            table.Rows.Add(new object[] { 1, "Andy" });
            table.Rows.Add(new object[] { 2, "Peter" });*/

            table.AcceptChanges();
            return table;
        }

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
     
    }
}