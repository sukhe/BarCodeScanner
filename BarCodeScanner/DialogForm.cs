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
    public partial class DialogForm : Form
    {
        /// <summary>
        /// Переменная для хранения результата нажатия кнопок 
        /// </summary>
        private static DialogResult dr;

        /// <summary>
        /// Конструктор для формы с вопросом
        /// </summary>
        /// <param name="text1">Текст на 1-й строке</param>
        /// <param name="text2">Текст на 2-й строке</param>
        /// <param name="title">Заголовок окна</param>
        /// <param name="buttonF1Text">Текст на кнопке F1</param>
        /// <param name="buttonF4Text">Текст на кнопке F4</param>
        public DialogForm(string text1, string text2, string title, string buttonF1Text, string buttonF4Text)
        {
            InitializeComponent();
            labelText1.Text = text1;
            labelText2.Text = text2;
            this.Text = title;
            buttonF1.Text = buttonF1Text;
            buttonF4.Text = buttonF4Text;
        }

        /// <summary>
        /// Форма закрывается, возвращая результат нажатия кнопки F1
        /// </summary>
        private void buttonF1_Click(object sender, EventArgs e)
        {
            DialogResult = buttonF1.DialogResult;
            Close();
        }

        /// <summary>
        /// Форма закрывается, возвращая результат нажатия кнопки F4
        /// </summary>
        private void buttonF4_Click(object sender, EventArgs e)
        {
            DialogResult = buttonF4.DialogResult;
            Close();
        }

        /// <summary>
        /// Форма вызыватся через функцию, чтобы можно было вернуть результат
        /// </summary>
        /// <param name="text1">Текст на 1-й строке</param>
        /// <param name="text2">Текст на 2-й строке</param>
        /// <param name="title">Заголовок окна</param>
        /// <param name="buttonF1Text">Текст на кнопке F1</param>
        /// <param name="buttonF4Text">Текст на кнопке F4</param> 
        /// <returns>Результат нажатия кнопок типа DialogResult: Retry - нажата F1, Cancel - нажата F4</returns>
        public static DialogResult Dialog(string text1, string text2, string title, string buttonF1Text, string buttonF4Text)
        {
            DialogForm d = new DialogForm(text1, text2, title, buttonF1Text, buttonF4Text);
            dr = d.ShowDialog();
            d.Close();
            return dr;
        }

        /// <summary>
        /// Обработка нажатия кнопок
        /// </summary>
        private void DialogForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyCode == System.Windows.Forms.Keys.F1 )
            {
                buttonF1_Click(this, e);
            }
            if ( e.KeyCode == System.Windows.Forms.Keys.F4 )
            {
                buttonF4_Click(this, e);
            }
        }

    }
}