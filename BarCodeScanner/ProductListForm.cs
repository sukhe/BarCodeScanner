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
    /// Список продукции текущего документа
    /// </summary>
    /// <remarks>
    /// Показывает тип и количество отгружаемой продукции, а также сколько штрихкодов уже отсканировано.
    /// </remarks>
    public partial class ProductListForm : Form
    {

        public static int currentproductrow;

        public ProductListForm()
        {
            InitializeComponent();
            labelNDoc.Text = "№"+MainForm.cargodocs[MainForm.currentdocrow].Number.Trim() + " / " + MainForm.cargodocs[MainForm.currentdocrow].Partner.Trim();
            MainForm.scanmode = ScanMode.BarCod;
            dataGrid1.Focus();
        }

        private void ProductListForm_Load(object sender, EventArgs e)
        {
            if (MainForm.producttable == null)
                MainForm.producttable = new DataTable();
            dataGrid1.DataSource = MainForm.producttable;
            CreateProductTable();
        }

        /// <summary>
        /// Формирование экранной таблицы списка продукции
        /// </summary>
        private void CreateProductTable()
        {
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

            // описываем колонки
            MainForm.productlistform.dataGrid1.TableStyles.Clear();
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            DataGridTextBoxColumnColored col0 = new DataGridTextBoxColumnColored();
            col0.Width = 80;
            col0.MappingName = MainForm.producttable.Columns[0].ColumnName;
            col0.HeaderText = MainForm.producttable.Columns[0].ColumnName;
            col0.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col0);

            
            DataGridTextBoxColumnColored col1 = new DataGridTextBoxColumnColored();
            if (MainForm.producttable.Rows.Count > 12) 
                col1.Width = 180;
            else 
                col1.Width = 204;
            col1.MappingName = MainForm.producttable.Columns[1].ColumnName;
            col1.HeaderText = MainForm.producttable.Columns[1].ColumnName;
            col1.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col1);

            DataGridTextBoxColumnColored col2 = new DataGridTextBoxColumnColored();
            col2.Width = 76;
            col2.MappingName = MainForm.producttable.Columns[2].ColumnName;
            col2.HeaderText = MainForm.producttable.Columns[2].ColumnName;
            col2.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col2);

            DataGridTextBoxColumnColored col3 = new DataGridTextBoxColumnColored();
            col3.Width = 66;
            col3.MappingName = MainForm.producttable.Columns[3].ColumnName;
            col3.HeaderText = MainForm.producttable.Columns[3].ColumnName;
            col3.NeedBackgroundProduct += new DataGridTextBoxColumnColored.NeedBackgroundEventHandlerProduct(OnBackgroundEventHandlerProduct);
            tableStyle.GridColumnStyles.Add(col3);

            MainForm.productlistform.dataGrid1.TableStyles.Add(tableStyle);

            MainForm.producttable.AcceptChanges();
        }

        /// <summary>
        /// Загрузка данных в экранную таблицу списка продукции из соответствующей структуры в памяти
        /// </summary>
        public void ReloadProductTable()
        {
            string pid; // переменная для типа продукции (product id)
            int need = 0; // необходимое количество продукции по всем типам
            int have = 0; // уже отсканировано продукции по всем типам
            int i;

            MainForm.producttable.Rows.Clear();
            foreach (Product p in MainForm.cargodocs[MainForm.currentdocrow].TotalProducts)
            {
                pid = p.PID;
                i = 0;
                need += Convert.ToInt16(p.Quantity); // p.Quantity - необходимое количество продукции по типу p.PID
                foreach (XCode x in MainForm.cargodocs[MainForm.currentdocrow].XCodes)
                {
                    if (pid == x.PID && x.DData == "") i++; // если штрихкод неудалён - прибавляем его к уже отсканированным
                }
                have += i;
                p.ScannedBar = i.ToString();
                MainForm.producttable.Rows.Add(new object[] { p.PID, p.PName, p.Quantity, p.ScannedBar });

            }
            MainForm.producttable.AcceptChanges();
            MainForm.cargodocs[MainForm.currentdocrow].Quantity = need.ToString();
            MainForm.cargodocs[MainForm.currentdocrow].ScannedBar = have.ToString();
            labelQuontity.Text = need.ToString() + "/" + have.ToString();
            if (have == 0) labelQuontity.BackColor = Color.White; // меняем цвет фона в зависимости от количества нужных/отсканированных штрихкодов
            else if (have != need) labelQuontity.BackColor = MainForm.partialColor;
            else labelQuontity.BackColor = MainForm.fullColor;
            labelQuontity.Refresh();
        }

        /// <summary>
        /// Определяем класс для работы с цветными строками экранной таблицы списка продукции
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
            public delegate void NeedBackgroundEventHandlerProduct(object sender, NeedBackgroundEventArgs e);
            public event NeedBackgroundEventHandlerProduct NeedBackgroundProduct;

            /// <summary>
            /// Переопределенный метод DataGridTextBoxColumn.Paint(), запрашивающий при помощи события цвет и передающий его 
            /// базовому методу Paint(), в параметре backBrush. 
            /// Теперь метод Paint базового класса будет заниматься прорисовкой ячейки, используя при этом подставленный нами backBrush. 
            /// </summary>
            protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
            {
                NeedBackgroundEventArgs e = new NeedBackgroundEventArgs(source, rowNum, backBrush, foreBrush);
                if (NeedBackgroundProduct != null) NeedBackgroundProduct(this, e);
                base.Paint(g, bounds, source, rowNum, e.BackBrush, e.ForeBrush, alignToRight);
            }
        }

        /// <summary>
        /// Раскрашивание в нужный цвет ячеек экранной таблицы списка продукции
        /// в зависимости от количества отсканированных и необходимых штрихкодов этого типа продукции
        /// </summary>
        private void OnBackgroundEventHandlerProduct(object sender, DataGridTextBoxColumnColored.NeedBackgroundEventArgs e)
        {
            e.ForeBrush = new SolidBrush(Color.Black);

            int q = Convert.ToInt16(MainForm.producttable.Rows[e.RowNum][2]);
            int b = Convert.ToInt16(MainForm.producttable.Rows[e.RowNum][3]);

            if ((b != q) && (b != 0))
                e.BackBrush = new SolidBrush(MainForm.partialColor);
            else if (b == q)
                e.BackBrush = new SolidBrush(MainForm.fullColor);
            else
                e.BackBrush = new SolidBrush(Color.White);
        }

        /// <summary>
        /// Если перешли на другой тип продукции - нужно обновить переменную, хранящую номер строки
        /// </summary>
        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            currentproductrow = dataGrid1.CurrentRowIndex;
        }

        /// <summary>
        /// Обработка нажатия клавиш
        /// </summary>
        private void ProductListForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                dataGrid1_Click(sender, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                buttonF4_Click(this, e);
            }
        }

        /// <summary>
        /// Выход из формы со списком продукции
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            MainForm.dataNeedSave = true;
            MainForm.SaveData();
            MainForm.scanmode = ScanMode.Doc;
            Close();
        }

        /// <summary>
        /// Открываем форму со списком штрихкодов по текущему типу продукции
        /// </summary>
        private void dataGrid1_Click(object sender, EventArgs e)
        {
            currentproductrow = dataGrid1.CurrentCell.RowNumber;
            MainForm.xcodelistform = new XCodeListForm();
            MainForm.xcodelistform.ShowDialog();
        }

    }
}