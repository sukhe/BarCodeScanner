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
    /// Форма для ручного ввода номера и даты отгрузочного документа
    /// </summary>
    /// <remarks>
    /// Основной способ ввода реквизитов документа - сканирование штрихкода с бумажной копии. 
    /// Эта форма - резервный вариант, когда нет бумажного документа. Хотя такого быть и не должно.
    /// </remarks>
    public partial class DocNumEnter : Form
    {

        /// <summary>
        /// Конструктор для формы, на которой вручную вводится номер загружаемого документа
        /// </summary>
        public DocNumEnter()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработка нажатия клавиш
        /// </summary>
        private void DocNumEnter_KeyDown(object sender, KeyEventArgs e)
        {
            
            if ((
                e.KeyCode == System.Windows.Forms.Keys.F1 ||
                e.KeyCode == System.Windows.Forms.Keys.Enter
                ) && dateTimePicker1.Focused
                )
            {   
                buttonF1_Click(this, e);
            }

            if (( 
                e.KeyCode == System.Windows.Forms.Keys.F1 ||
                e.KeyCode == System.Windows.Forms.Keys.Enter
                ) && textBox1.Focused 
                )
            {
                dateTimePicker1.Focus();
            }

            if (e.KeyCode == System.Windows.Forms.Keys.F4)
            {
                buttonF4_Click(this, e);
            }

        }

        /// <summary>
        /// При получении фокуса полем ввода данных - меняем подпись у 1-й кнопки
        /// </summary>
        private void dateTimePicker1_GotFocus(object sender, EventArgs e)
        {
            buttonF1.Text = "           Принять";
        }

        /// <summary>
        /// Выходим из формы без сохранения введённых данных
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            this.Owner.Tag = "";
            this.Controls.Remove(dateTimePicker1);
            dateTimePicker1.Dispose();
            Close();
        }

        /// <summary>
        /// Делаем ход дальше - на следующее поле ввода
        /// </summary>
        private void buttonF1_Click(object sender, EventArgs e)
        {
            if (buttonF1.Text != "           Принять") dateTimePicker1.Focus();
            else
            {   // введённые данные записываем в Tag родительской формы
                this.Owner.Tag = textBox1.Text.Trim() + " " + dateTimePicker1.Value.Year.ToString() +
                                 MainForm.AddZeroIfNeed(dateTimePicker1.Value.Month) +
                                 MainForm.AddZeroIfNeed(dateTimePicker1.Value.Day);
                Close();
            };
        }

    }
}