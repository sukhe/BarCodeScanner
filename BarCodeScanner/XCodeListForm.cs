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

    /// <summary>
    /// Список отсканированных штрихкодов по заданному типу продукции текущего документа
    /// </summary>
    public partial class XCodeListForm : Form
    {

        /// <summary>
        /// Переменная для хранения текущей строки списка штрихкодов
        /// </summary>
        public static int currentxcoderow;

        /// <summary>
        /// Конструктор для формы со списком штрихкодов
        /// </summary>
        public XCodeListForm()
        {
            InitializeComponent();
            labelProductName.Text = MainForm.cargodocs[MainForm.currentdocrow].Partner.Trim() + " " + MainForm.cargodocs[MainForm.currentdocrow].Number.Trim();
            MainForm.scanmode = ScanMode.BarCod;
            dataGrid1.Focus();
        }

        /// <summary>
        /// Действия при загрузке формы
        /// </summary>
        private void XCodeListForm_Load(object sender, EventArgs e)
        {
            if (MainForm.xcodetable == null)
                MainForm.xcodetable = new DataTable();
            dataGrid1.DataSource = MainForm.xcodetable;
            CreateXCodeTable();
        }

        /// <summary>
        /// Загрузка данных в экранную таблицу списка штрихкодов из соответствующей структуры в памяти
        /// </summary>
        public void ReloadXCodeTable()
        {
            MainForm.xcodetable.Rows.Clear();
            labelProductName.Text = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(1);
            string pid = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(0);
            int i = 0;
            foreach (XCode x in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
            {
                if (pid==x.PID) // отбираем штрихкоды по текущему типу продукции
                {
                    MainForm.xcodetable.Rows.Add(new object[] { x.ScanCode, MainForm.ConvertToDDMMYY(x.Data), x.FIO, x.DData, x.DFIO, x.Data });
                    if (x.DData == "") i++;
                }
            }
            labelQuontity.Text = MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(2) + "/" + i.ToString();
            if (i == 0) labelQuontity.BackColor = Color.White;
            else // определяем, набрано-ли уже нужное кол-во штрихкодов по этому типу продукции и делаем фон соответствующего цвета
                if (i != Convert.ToInt16(MainForm.producttable.Rows[ProductListForm.currentproductrow].Field<string>(2))) 
                     labelQuontity.BackColor = MainForm.partialColor;
                else labelQuontity.BackColor = MainForm.fullColor;
            MainForm.xcodetable.AcceptChanges();
        }

        /// <summary>
        /// Формирование экранной таблицы списка штрихкодов
        /// </summary>
        private void CreateXCodeTable()
        {
            if (MainForm.xcodetable.Columns.Count == 0)
            {
                DataColumn BarCod = MainForm.xcodetable.Columns.Add("Штрихкод", typeof(string));
                MainForm.xcodetable.Columns.Add("Дата", typeof(string));
                MainForm.xcodetable.Columns.Add("ФИО", typeof(string));
                MainForm.xcodetable.Columns.Add("ДатаДел", typeof(string));
                MainForm.xcodetable.Columns.Add("ФИОДел", typeof(string));
                DataColumn FullData = MainForm.xcodetable.Columns.Add("ПолнДат", typeof(string));

                // Set the ID column as the primary key column.
                //MainForm.xcodetable.PrimaryKey = new DataColumn[] { FullData, BarCod };
            }

            ReloadXCodeTable();

            // описываем колонки
            MainForm.xcodelistform.dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            DataGridTextBoxColumnColored col0 = new DataGridTextBoxColumnColored();
            col0.Width = 175;
            col0.MappingName = MainForm.xcodetable.Columns[0].ColumnName;
            col0.HeaderText = MainForm.xcodetable.Columns[0].ColumnName;
            col0.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col0);


            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            col1.Width = 92;
            col1.MappingName = MainForm.xcodetable.Columns[1].ColumnName;
            col1.HeaderText = MainForm.xcodetable.Columns[1].ColumnName;
            col1.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            if (MainForm.xcodetable.Rows.Count > 12) col2.Width = 132;
            else col2.Width = 156;
            col2.MappingName = MainForm.xcodetable.Columns[2].ColumnName;
            col2.HeaderText = MainForm.xcodetable.Columns[2].ColumnName;
            col2.NeedBackgroundXCode += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerXCode(OnBackgroundEventHandlerProductXCode);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();            
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

            dataGrid1.TableStyles.Add(tableStyle);

            MainForm.xcodetable.AcceptChanges();
        }

        /// <summary>
        /// Нажата кнопка удаления штрихкода
        /// </summary>
        private void buttonF1_Click(object sender, EventArgs e)
        {
            int i = dataGrid1.CurrentRowIndex;
            string barcod = MainForm.xcodetable.Rows[i].Field<string>(0);
            string data = MainForm.xcodetable.Rows[i].Field<string>(5);
            string dfio = MainForm.xcodetable.Rows[i].Field<string>(4);
            MainForm.scanmode = ScanMode.Nothing;
            if (MainForm.xcodetable.Rows[i].Field<string>(3) == "") // не пытаемся-ли мы удалить уже удалённый штрихкод?
            {
                if (DialogForm.Dialog("Удалить штрихкод ", barcod, "Удалить?", "        Да", "        Нет") == DialogResult.Retry)
                {
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

                    x.DData = MainForm.ConvertToFullDataTime(System.DateTime.Now.ToString()); // время удаления и фамилия удалившего
                    x.DFIO = Config.userName;

                    if (MainForm.xcodelistform != null && MainForm.xcodelistform.Visible)
                    {
                        MainForm.xcodetable.AcceptChanges();
                        MainForm.xcodelistform.ReloadXCodeTable();
                    }
                    currentxcoderow = dataGrid1.CurrentCell.RowNumber;
                }
            }
            else
            {
                MessageBox.Show("Этот штрихкод уже удалён");
            }
            MainForm.scanmode = ScanMode.BarCod;
        }

        /// <summary>
        /// Выход из формы
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            MainForm.productlistform.ReloadProductTable();
            Close();
        }

        /// <summary>
        /// Обработка нажатия клавиш
        /// </summary>
        private void XCodeListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1))
            {
                buttonF1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonF4_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                if (dataGrid1.VisibleRowCount != 0)
                {
                    MainForm.scanmode = ScanMode.Nothing;
                    XCodeInfoForm xcodeinfo = new XCodeInfoForm();
                    xcodeinfo.ShowDialog();
                    MainForm.scanmode = ScanMode.BarCod;
                }
            }
        }

        /// <summary>
        /// Определяем класс для работы с цветными строками экранной таблицы списка штрихкодов
        /// </summary>
        public class DataGridTextBoxColumnColored : DataGridTextBoxColumn
        {
            /// <summary>
            /// Определим класс аргумента события, делегат и само событие, необходимые для "общения" кода выполняющего прорисовку ячейки, 
            /// с кодом, предоставляющим цвет для этой ячейки. 
            /// </summary>
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

            /// <summary>
            /// Переопределенный метод DataGridTextBoxColumn.Paint(), запрашивающий при помощи события цвет и передающий его 
            /// базовому методу Paint(), в параметре backBrush. 
            /// Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, используя при этом подставленный нами backBrush. 
            /// </summary>
            protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
            {
                NeedBackgroundEventArgs e = new NeedBackgroundEventArgs(source, rowNum, backBrush, foreBrush);
                if (NeedBackgroundXCode != null) NeedBackgroundXCode(this, e);
                base.Paint(g, bounds, source, rowNum, e.BackBrush, e.ForeBrush, alignToRight);
            }
        }

        /// <summary>
        /// Раскрашивание в нужный цвет ячеек экранной таблицы списка штрихкодов
        /// в зависимости от того, удалён штрихкод или нет 
        /// </summary>
        private void OnBackgroundEventHandlerProductXCode(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {
            e.ForeBrush = new SolidBrush(Color.Black);

            if (MainForm.xcodetable.Rows[e.RowNum].Field<string>(3) == "")
                e.BackBrush = new SolidBrush(Color.White);
            else
                e.BackBrush = new SolidBrush(MainForm.partialColor);
        }

        /// <summary>
        /// Если перешли на другой штрихкод - нужно обновить переменную, хранящую номер строки
        /// </summary>
        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            currentxcoderow = dataGrid1.CurrentCell.RowNumber;
        }
    }
}