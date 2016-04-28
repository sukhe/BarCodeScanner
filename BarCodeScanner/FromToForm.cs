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
    public partial class FromToForm : Form
    {
//        private List<ListBox> list = new List<ListBox>();

        /// <summary>
        /// Конструктор для формы, на которой вводится направление перемещения продукции
        /// </summary>
        public FromToForm()
        {
            InitializeComponent();
/*            list.Add(listBox1); // список типов операций (отрузка, приход, перемещение)
            list.Add(listBox2); // список "откуда"
            list.Add(listBox3); // список "куда"*/
        }

        /// <summary>
        /// Перейти к следующему списку, запомнив выбор в списке текущем
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            NextList();
        }

        /// <summary>
        /// Закрыть форму без сохранения сделанного выбора
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Обработка нажатия клавиш
        /// </summary>
        private void FromToForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == System.Windows.Forms.Keys.F1) ||
                (e.KeyCode == System.Windows.Forms.Keys.Enter) ||
                (e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                button1_Click(this, e);
            }
            if ((e.KeyCode == System.Windows.Forms.Keys.F4))
            {
                button4_Click(this, e);
            }
/*            if ((e.KeyCode == System.Windows.Forms.Keys.Enter))
            {
                NextList();
            } 
            if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                NextList();
            } */
        }

        /// <summary>
        /// При перемещении по списку операций изменять текст выбранного варианта
        /// </summary>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label12.Text = listBox1.SelectedItem.ToString();
        }

        /// <summary>
        /// При перемещении по списку "откуда" изменять текст выбранного варианта
        /// </summary>
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label22.Text = listBox2.SelectedItem.ToString();
        }

        /// <summary>
        /// При перемещении по списку "куда" изменять текст выбранного варианта
        /// </summary>
        private void listBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            label32.Text = listBox3.SelectedItem.ToString();
        }

        /// <summary>
        /// Загрузить форму и сформировать первый список для выбора
        /// </summary>
        private void FromToForm_Load(object sender, EventArgs e)
        {
            foreach (Transfer t in MainForm.settings.Transfers)
            {
                listBox1.Items.Add(t.Name);
            }
            listBox1.Focus();
            if (listBox1.Items.Count == 0)
            {
                MainForm.LogShow("[FTF.Load.1] Ошибка файла настроек: нет доступных операций");
                Close();
            }
            else
                listBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Выбор текущего значения и переход к следующему списку
        /// </summary>
        /// <remarks>
        /// Переход по спискам - только последовательный. Каждый выбор в текущем списке 
        /// отсекает неподходящие варианты в следующих списках, 
        /// чтобы кладовщик не мог выбрать бессмысленные варианты. 
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
            if (listBox1.Focused)
            {
                listBox2.Enabled = true; // второй список станет доступен только когда сделан выбор в первом

                s = MainForm.settings.Transfers[listBox1.SelectedIndex].From.Split(','); // заполняем список
                foreach (string t in s)
                {
                    foreach (Location n in MainForm.settings.Locations)
                    {
                        if (n.LID == t)
                        {
                            listBox2.Items.Add(n.Name);
                        }
                    }
                }

                if (listBox2.Items.Count == 0)
                {
                    MainForm.LogShow("[FTF.Load.2] Ошибка файла настроек: нет списка \"Откуда\"");
                    Close();
                }
                else
                {
                    listBox2.Focus();
                    listBox2.SelectedIndex = 0;
                    listBox1.Visible = false; // после того, как появился второй список, первый скрываем
                }
            }
            else if (listBox2.Focused)
            {
                listBox3.Enabled = true;
                s = MainForm.settings.Transfers[listBox1.SelectedIndex].To.Split(',');
                foreach (string t in s)
                {
                    foreach (Location n in MainForm.settings.Locations)
                    {
                        if (n.LID == t)
                        {
                            listBox3.Items.Add(n.Name);
                        }
                    }
                }

                if (listBox3.Items.Count == 0)
                {
                    MainForm.LogShow("[FTF.Load.3] Ошибка файла настроек: нет списка \"Куда\"");
                    Close();
                }
                else
                {
                    listBox3.Focus();
                    listBox3.SelectedIndex = 0;
                    listBox2.Visible = false;
                }
            }
            else if (listBox3.Focused) // все три списка пройдены - пора выходить
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
            // закрываемся из третьего списка по нажатию F1, Enter или Right
            if (listBox2.Visible == false && listBox1.Visible == false && this.Tag=="Save") 
            {
                foreach (Location n in MainForm.settings.Locations)
                {
                    if (n.Name == label22.Text)
                    {
                        Config.transferFrom = label22.Text;
                        Config.transferFromLid = n.LID;
                    }
                }

                foreach (Location n in MainForm.settings.Locations)
                {
                    if (n.Name == label32.Text)
                    {
                        Config.transferTo = label32.Text;
                        Config.transferToLid = n.LID;
                    }
                }

            }
                
        }

    }
}