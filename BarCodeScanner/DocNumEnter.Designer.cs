namespace BarCodeScanner
{
    partial class DocNumEnter
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс  следует удалить; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonF1 = new System.Windows.Forms.Button();
            this.buttonF4 = new System.Windows.Forms.Button();
            this.labelF1 = new System.Windows.Forms.Label();
            this.labelF4 = new System.Windows.Forms.Label();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(64, 69);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(61, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 20);
            this.label1.Text = "Номер накладной";
            // 
            // buttonF1
            // 
            this.buttonF1.Location = new System.Drawing.Point(0, 270);
            this.buttonF1.Name = "buttonF1";
            this.buttonF1.Size = new System.Drawing.Size(120, 24);
            this.buttonF1.TabIndex = 2;
            this.buttonF1.Text = "           Дальше";
            this.buttonF1.Click += new System.EventHandler(this.buttonF1_Click);
            // 
            // buttonF4
            // 
            this.buttonF4.Location = new System.Drawing.Point(120, 270);
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.Size = new System.Drawing.Size(120, 24);
            this.buttonF4.TabIndex = 3;
            this.buttonF4.Text = "         Назад";
            this.buttonF4.Click += new System.EventHandler(this.buttonF4_Click);
            // 
            // labelF1
            // 
            this.labelF1.BackColor = System.Drawing.Color.Red;
            this.labelF1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF1.ForeColor = System.Drawing.Color.White;
            this.labelF1.Location = new System.Drawing.Point(2, 272);
            this.labelF1.Name = "labelF1";
            this.labelF1.Size = new System.Drawing.Size(35, 20);
            this.labelF1.Text = "F1";
            this.labelF1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Yellow;
            this.labelF4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF4.Location = new System.Drawing.Point(123, 272);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(46, 151);
            this.dateTimePicker1.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            this.dateTimePicker1.MinDate = new System.DateTime(2015, 1, 1, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(138, 22);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.GotFocus += new System.EventHandler(this.dateTimePicker1_GotFocus);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(64, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 20);
            this.label4.Text = "Дата документа";
            // 
            // DocNumEnter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.labelF4);
            this.Controls.Add(this.labelF1);
            this.Controls.Add(this.buttonF4);
            this.Controls.Add(this.buttonF1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "DocNumEnter";
            this.Text = "Ввод номера документа";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DocNumEnter_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonF1;
        private System.Windows.Forms.Button buttonF4;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label4;
    }
}