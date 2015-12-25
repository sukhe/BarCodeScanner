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

/*                cargodocTable = new DataTable();
                cargodocDataSet = new DataSet();
//                cargodocDataSet.Tables.Add(cargodocTable);

                DataTableReader reader = GetReader();
                cargodocDataSet.Load(reader, LoadOption.OverwriteChanges, cargodocTable);
//                dataGrid1.DataSource = dataSet1;

            //            PrintColumns(cargodocTable); */


            DataTable table = new DataTable();

            dataGrid1.DataSource = table;


            DataTableReader reader = new DataTableReader(GetCustomers());
            table.Load(reader);

        }

        private DataTable GetCustomers()
        {
            // Create sample Customers table, in order 
            // to demonstrate the behavior of the DataTableReader.
            DataTable table = new DataTable();

            // Create two columns, ID and Name.
            DataColumn Number = table.Columns.Add("№ док-та", typeof(string));
            table.Columns.Add("Дата", typeof(string));
            table.Columns.Add("Контрагент", typeof(string));

            // Set the ID column as the primary key column.
            table.PrimaryKey = new DataColumn[] { Number };

            foreach (CargoDoc d in MainForm.cargodocs)
            {
                table.Rows.Add(new object[] { d.Number, d.Data, d.Partner });
            }

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