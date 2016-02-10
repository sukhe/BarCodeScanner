using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace BarCodeScanner
{
    public partial class FileListForm : Form
    {
        private DataTable filetable;

        public FileListForm()
        {
            InitializeComponent();
            filetable = new DataTable();
            //if (doclist.Contains(CurrentPath + @"doc\" + docnum + ".xml"))
//            documentList1.SelectedDirectory = MainForm.CurrentPath+"doc";
        }



        private void button1_Click(object sender, EventArgs e)
        {
//            MessageBox.Show(documentList1.SelectedDirectory.ToString());
            File.Delete(MainForm.CurrentPath + @"doc\" + filetable.Rows[dataGrid1.CurrentRowIndex].Field<string>(0));
            LoadFileTable(false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F3))
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = @"\FlashDisk\Program Files\Notepad\Notepad.exe";
                processStartInfo.Arguments = MainForm.CurrentPath + @"doc\" + filetable.Rows[dataGrid1.CurrentRowIndex].Field<string>(0);
                try
                {
                    Process.Start(processStartInfo);
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }                
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = @"\Windows\iexplore.exe";
                processStartInfo.Arguments = MainForm.CurrentPath + @"doc\" + filetable.Rows[dataGrid1.CurrentRowIndex].Field<string>(0);
                try
                {
                    Process.Start(processStartInfo);
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }
            }
        }

        private void FileListForm_Load(object sender, EventArgs e)
        {
            dataGrid1.DataSource = filetable;
            LoadFileTable(true);
        }

        private void LoadFileTable(Boolean firstCall)
        {
            MainForm.doclist = Directory.GetFiles(MainForm.CurrentPath + "doc", "*_*_*.xml");
            filetable.Clear();
            if (MainForm.doclist.Length > 0)
            {

                if (filetable.Columns.Count == 0)
                {
                    DataColumn Name = filetable.Columns.Add("Имя файла", typeof(string));
                    filetable.Columns.Add("Размер", typeof(string));
                    filetable.Columns.Add("Дата", typeof(string));

                    // Set the ID column as the primary key column.
                    filetable.PrimaryKey = new DataColumn[] { Name };
                }

                DirectoryInfo di = new DirectoryInfo(MainForm.CurrentPath + "doc");
                FileInfo[] fileinfo = di.GetFiles("*_*_*.xml");

//                filetable.Clear();

                foreach (FileInfo fi in fileinfo)
                {
                    filetable.Rows.Add(new object[] { fi.Name, fi.Length, fi.LastWriteTime.ToString().Substring(0, 8) });
                }
                filetable.AcceptChanges();

                dataGrid1.TableStyles.Clear();
                DataGridTableStyle tableStyle = new DataGridTableStyle();
                DataGridTextBoxColumn col0 = new DataGridTextBoxColumn();
                col0.Width = 235;
                col0.MappingName = filetable.Columns[0].ColumnName;
                col0.HeaderText = filetable.Columns[0].ColumnName;
                tableStyle.GridColumnStyles.Add(col0);

                DataGridTextBoxColumn col1 = new DataGridTextBoxColumn();
                col1.Width = 105;
                col1.MappingName = filetable.Columns[1].ColumnName;
                col1.HeaderText = filetable.Columns[1].ColumnName;
                tableStyle.GridColumnStyles.Add(col1);

                DataGridTextBoxColumn col2 = new DataGridTextBoxColumn();
                col2.Width = 90;
                col2.MappingName = filetable.Columns[2].ColumnName;
                col2.HeaderText = filetable.Columns[2].ColumnName;
                tableStyle.GridColumnStyles.Add(col2);

                dataGrid1.TableStyles.Add(tableStyle);

                filetable.AcceptChanges();
            }
            else
            {
                if (firstCall) MessageBox.Show("Нет загруженных документов");
                else MessageBox.Show("Все документы удалены");
            }
        }

    }
}