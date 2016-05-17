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
    /// Форма "откуда-куда"
    /// </summary>
    /// <remarks>
    /// Позволяет выбрать тип операции, а также откуда и куда перемещается изделие. 
    /// Такой выбор нужно обязательно сделать перед сканированием штрихкодов
    /// </remarks>
    public partial class FromToForm : Form
    {

        /// <summary>
        /// Конструктор для формы, на которой вводится направление перемещения продукции
        /// </summary>
        public FromToForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Перейти к следующему списку, запомнив выбор в списке текущем
        /// </summary>
        private void buttonF1_Click(object sender, EventArgs e)
        {
            NextList();
        }

        /// <summary>
        /// Закрыть форму без сохранения сделанного выбора
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Обработка нажатия клавиш
        /// </summary>
        private void FromToForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == System.Windows.Forms.Keys.F1 ||
                 e.KeyCode == System.Windows.Forms.Keys.Enter ||
                 e.KeyCode == System.Windows.Forms.Keys.Right)
            {
                buttonF1_Click(this, e);
            }
            if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                buttonF4_Click(this, e);
            }
        }

        /// <summary>
        /// При перемещении по списку операций изменять текст выбранного варианта
        /// </summary>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelOperation.Text = listBoxOperation.SelectedItem.ToString();
        }

        /// <summary>
        /// При перемещении по списку "откуда" изменять текст выбранного варианта
        /// </summary>
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelFrom.Text = listBoxFrom.SelectedItem.ToString();
        }

        /// <summary>
        /// При перемещении по списку "куда" изменять текст выбранного варианта
        /// </summary>
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            labelTo.Text = listBoxTo.SelectedItem.ToString();
        }

        /// <summary>
        /// Загрузить форму и сформировать первый список (список операций) для выбора
        /// </summary>
        private void FromToForm_Load(object sender, EventArgs e)
        {
            foreach (Transfer t in MainForm.settings.Transfers)
            {
                listBoxOperation.Items.Add(t.Name);
            }
            listBoxOperation.Focus();
            if (listBoxOperation.Items.Count == 0)
            {
                MainForm.LogShow("[FTF.Load.1] Ошибка файла настроек: нет доступных операций");
                Close();
            }
            else
                listBoxOperation.SelectedIndex = 0;
        }

        /// <summary>
        /// Выбор значения в текущем списке и переход к следующему списку
        /// </summary>
        /// <remarks>
        /// Переход по спискам - только последовательный. Каждый выбор в текущем списке 
        /// отсекает неподходящие варианты в следующих списках, чтобы кладовщик не мог выбрать бессмысленные варианты. 
        /// 
        /// Пример бессмысленного варианта:
        /// Тип операции - "Приход на склад", откуда - "Контрагент", куда - "Конвейер №2"
        /// 
        /// Пример нормального варианта:
        /// Тип операции - "Перемещение", откуда - "Склад №1", куда - "Склад на ж/д станции"
        /// 
        /// Допустимые операции задаются в 1C и передаются на сканер в файле settings.xml
        /// </remarks>
        private void NextList()
        {
            string[] s;
            if (listBoxOperation.Visible)
            {
                listBoxFrom.Enabled = true; // второй список станет доступен только когда сделан выбор в первом

                s = MainForm.settings.Transfers[listBoxOperation.SelectedIndex].From.Split(','); // заполняем список
                foreach (string t in s)
                {
                    foreach (Location n in MainForm.settings.Locations)
                    {
                        if (n.LID == t)
                        {
                            listBoxFrom.Items.Add(n.Name);
                        }
                    }
                }

                if (listBoxFrom.Items.Count == 0)
                {
                    MainForm.LogShow("[FTF.Load.2] Ошибка файла настроек: нет списка \"Откуда\"");
                    Close();
                }
                else
                {
                    listBoxFrom.Focus();
                    listBoxFrom.SelectedIndex = 0;
                    listBoxOperation.Visible = false; // после того, как появился второй список, первый скрываем
                }
            }
            else if (listBoxFrom.Visible && !listBoxOperation.Visible)
            {
                listBoxTo.Enabled = true;
                s = MainForm.settings.Transfers[listBoxOperation.SelectedIndex].To.Split(',');
                foreach (string t in s)
                {
                    foreach (Location n in MainForm.settings.Locations)
                    {
                        if (n.LID == t)
                        {
                            listBoxTo.Items.Add(n.Name);
                        }
                    }
                }

                if (listBoxTo.Items.Count == 0)
                {
                    MainForm.LogShow("[FTF.Load.3] Ошибка файла настроек: нет списка \"Куда\"");
                    Close();
                }
                else
                {
                    listBoxTo.Focus();
                    listBoxTo.SelectedIndex = 0;
                    listBoxFrom.Visible = false; // показываем список 3, скрываем список номер 2, список 1 уже скрыт
                }
            }
            else if (!listBoxOperation.Visible && !listBoxFrom.Visible) // все три списка пройдены - пора выходить
            {
                this.Tag = "Save";
                Close();
            }
        }

        /// <summary>
        /// Закрыть форму с сохранением сделанного выбора
        /// </summary>
        private void FromToForm_Closing(object sender, CancelEventArgs e)
        {
            if (this.Tag != null) // закрываемся из третьего списка по нажатию F1, Enter или Right
            {
                foreach (Location n in MainForm.settings.Locations)
                {
                    if (n.Name == labelFrom.Text)
                    {
                        Config.transferFrom = labelFrom.Text; // текстовое описание, показываемое на экране
                        Config.transferFromLid = n.LID;     // цифровой код (location id), записываемый в файл
                    }
                }

                foreach (Location n in MainForm.settings.Locations)
                {
                    if (n.Name == labelTo.Text)
                    {
                        Config.transferTo = labelTo.Text;
                        Config.transferToLid = n.LID;
                    }
                }

            }
                
        }

    }
}