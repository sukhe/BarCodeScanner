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
/*        static DataTable cargodocTable;
        static DataSet cargodocDataSet; */

        public DocListForm()
        {
            InitializeComponent();
        }

        private void DocListForm_Load(object sender, EventArgs e)
        {

            // показываем таблицу со списком уже имеющихся у нас документов
            DataTable table = new DataTable();
            dataGrid1.DataSource = table;
            DataTableReader reader = new DataTableReader(GetCustomers(table,dataGrid1));
            table.Load(reader);

            // а вот таперича пробуем загрузить ещё документов


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
                table.Rows.Add(new object[] { d.Number, d.Data, d.Partner });
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
            col2.Width = 100;
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

        
        private void dataGrid1_GotFocus(object sender, EventArgs e)
        {



/*            cargodocDataSet = new DataSet();
            cargodocTable = new DataTable();

            DataColumn docidColumn = cargodocTable.Columns.Add("DocID", typeof(int));
            DataColumn numberColumn = cargodocTable.Columns.Add("Number", typeof(string));
            DataColumn dataColumn = cargodocTable.Columns.Add("Data", typeof(string));
            DataColumn partnerColumn = cargodocTable.Columns.Add("Partner", typeof(string));

            cargodocTable.PrimaryKey = new DataColumn[] { docidColumn };

            cargodocTable.Columns.Add();

            cargodocTable.Rows.Add(new object[] { 1, "1548", "18.02.2015", "ООО БББ" });
            cargodocTable.Rows.Add(new object[] { 2, "1549", "19.02.2015", "Delta" });
            cargodocTable.Rows.Add(new object[] { 3, "1550", "20.02.2015", "DLkj" });

            cargodocTable.AcceptChanges();

            cargodocDataSet.Tables.Add(cargodocTable);
            dataGrid1.DataSource = cargodocDataSet; */

        }

/*        private static DataTable GetCargoDoc()
        {
//            DataTable table = new DataTable();

            DataColumn docidColumn = cargodocTable.Columns.Add("DocID", typeof(int));
            DataColumn numberColumn = cargodocTable.Columns.Add("Number", typeof(string));
            DataColumn dataColumn = cargodocTable.Columns.Add("Data", typeof(string));
            DataColumn partnerColumn = cargodocTable.Columns.Add("Partner", typeof(string));
            cargodocTable.PrimaryKey = new DataColumn[] { docidColumn };

            cargodocTable.Rows.Add(new object[] { 1, "1548", "18.02.2015", "ООО БББ" });
            cargodocTable.Rows.Add(new object[] { 2, "1549", "19.02.2015", "Delta" });
            cargodocTable.Rows.Add(new object[] { 3, "1550", "20.02.2015", "DLkj" });
            cargodocTable.AcceptChanges();

            cargodocTable.Columns.Add();
            return cargodocTable;
        }

        private static DataTableReader GetReader()
        {
//            DataSet cargodocDataSet = new DataSet();
            cargodocDataSet.Tables.Add(GetCargoDoc());
            return cargodocDataSet.CreateDataReader();
        }
        */
       
    }
}