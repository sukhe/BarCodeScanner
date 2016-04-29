namespace BarCodeScanner
{
    partial class ServiceForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label0 = new System.Windows.Forms.Label();
            this.button0 = new System.Windows.Forms.Button();
            this.labelDot = new System.Windows.Forms.Label();
            this.buttonDot = new System.Windows.Forms.Button();
            this.labelCLR = new System.Windows.Forms.Label();
            this.buttonCLR = new System.Windows.Forms.Button();
            this.labelF3 = new System.Windows.Forms.Label();
            this.labelF2 = new System.Windows.Forms.Label();
            this.labelF1 = new System.Windows.Forms.Label();
            this.buttonF3 = new System.Windows.Forms.Button();
            this.buttonF2 = new System.Windows.Forms.Button();
            this.buttonF1 = new System.Windows.Forms.Button();
            this.labelF4 = new System.Windows.Forms.Label();
            this.buttonF4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label0);
            this.panel1.Controls.Add(this.button0);
            this.panel1.Controls.Add(this.labelDot);
            this.panel1.Controls.Add(this.buttonDot);
            this.panel1.Controls.Add(this.labelCLR);
            this.panel1.Controls.Add(this.buttonCLR);
            this.panel1.Controls.Add(this.labelF3);
            this.panel1.Controls.Add(this.labelF2);
            this.panel1.Controls.Add(this.labelF1);
            this.panel1.Controls.Add(this.buttonF3);
            this.panel1.Controls.Add(this.buttonF2);
            this.panel1.Controls.Add(this.buttonF1);
            this.panel1.Controls.Add(this.labelF4);
            this.panel1.Controls.Add(this.buttonF4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 170);
            // 
            // label0
            // 
            this.label0.BackColor = System.Drawing.Color.Black;
            this.label0.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.label0.ForeColor = System.Drawing.Color.Yellow;
            this.label0.Location = new System.Drawing.Point(3, 123);
            this.label0.Name = "label0";
            this.label0.Size = new System.Drawing.Size(35, 20);
            this.label0.Text = "0";
            this.label0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button0
            // 
            this.button0.Location = new System.Drawing.Point(0, 121);
            this.button0.Name = "button0";
            this.button0.Size = new System.Drawing.Size(240, 24);
            this.button0.TabIndex = 11;
            this.button0.Text = "       Тест связи с 1С";
            this.button0.Click += new System.EventHandler(this.button0_Click);
            // 
            // labelDot
            // 
            this.labelDot.BackColor = System.Drawing.Color.Black;
            this.labelDot.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.labelDot.ForeColor = System.Drawing.Color.White;
            this.labelDot.Location = new System.Drawing.Point(3, 99);
            this.labelDot.Name = "labelDot";
            this.labelDot.Size = new System.Drawing.Size(35, 20);
            this.labelDot.Text = ".";
            this.labelDot.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonDot
            // 
            this.buttonDot.Location = new System.Drawing.Point(0, 97);
            this.buttonDot.Name = "buttonDot";
            this.buttonDot.Size = new System.Drawing.Size(240, 24);
            this.buttonDot.TabIndex = 8;
            this.buttonDot.Text = "       Получить время";
            this.buttonDot.Click += new System.EventHandler(this.buttonDot_Click);
            // 
            // labelCLR
            // 
            this.labelCLR.BackColor = System.Drawing.Color.Black;
            this.labelCLR.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.labelCLR.ForeColor = System.Drawing.Color.Yellow;
            this.labelCLR.Location = new System.Drawing.Point(3, 75);
            this.labelCLR.Name = "labelCLR";
            this.labelCLR.Size = new System.Drawing.Size(35, 20);
            this.labelCLR.Text = "CLR";
            this.labelCLR.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonCLR
            // 
            this.buttonCLR.Location = new System.Drawing.Point(0, 73);
            this.buttonCLR.Name = "buttonCLR";
            this.buttonCLR.Size = new System.Drawing.Size(240, 24);
            this.buttonCLR.TabIndex = 3;
            this.buttonCLR.Text = "      Очистить лог";
            this.buttonCLR.Click += new System.EventHandler(this.buttonCLR_Click);
            // 
            // labelF3
            // 
            this.labelF3.BackColor = System.Drawing.Color.Green;
            this.labelF3.Enabled = false;
            this.labelF3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF3.ForeColor = System.Drawing.Color.White;
            this.labelF3.Location = new System.Drawing.Point(3, 51);
            this.labelF3.Name = "labelF3";
            this.labelF3.Size = new System.Drawing.Size(35, 20);
            this.labelF3.Text = "F3";
            this.labelF3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelF2
            // 
            this.labelF2.BackColor = System.Drawing.Color.Blue;
            this.labelF2.Enabled = false;
            this.labelF2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF2.ForeColor = System.Drawing.Color.White;
            this.labelF2.Location = new System.Drawing.Point(3, 27);
            this.labelF2.Name = "labelF2";
            this.labelF2.Size = new System.Drawing.Size(35, 20);
            this.labelF2.Text = "F2";
            this.labelF2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelF1
            // 
            this.labelF1.BackColor = System.Drawing.Color.Red;
            this.labelF1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF1.ForeColor = System.Drawing.Color.White;
            this.labelF1.Location = new System.Drawing.Point(3, 3);
            this.labelF1.Name = "labelF1";
            this.labelF1.Size = new System.Drawing.Size(35, 20);
            this.labelF1.Text = "F1";
            this.labelF1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF3
            // 
            this.buttonF3.Location = new System.Drawing.Point(0, 49);
            this.buttonF3.Name = "buttonF3";
            this.buttonF3.Size = new System.Drawing.Size(240, 24);
            this.buttonF3.TabIndex = 2;
            this.buttonF3.Text = "        Посмотреть лог";
            this.buttonF3.Click += new System.EventHandler(this.buttonF3_Click);
            // 
            // buttonF2
            // 
            this.buttonF2.Location = new System.Drawing.Point(0, 25);
            this.buttonF2.Name = "buttonF2";
            this.buttonF2.Size = new System.Drawing.Size(240, 24);
            this.buttonF2.TabIndex = 1;
            this.buttonF2.Text = "      Список документов";
            this.buttonF2.Click += new System.EventHandler(this.buttonF2_Click);
            // 
            // buttonF1
            // 
            this.buttonF1.Location = new System.Drawing.Point(0, 1);
            this.buttonF1.Name = "buttonF1";
            this.buttonF1.Size = new System.Drawing.Size(240, 24);
            this.buttonF1.TabIndex = 0;
            this.buttonF1.Text = "           Заново получить настройки";
            this.buttonF1.Click += new System.EventHandler(this.buttonF1_Click);
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Yellow;
            this.labelF4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF4.Location = new System.Drawing.Point(3, 147);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF4
            // 
            this.buttonF4.Location = new System.Drawing.Point(0, 145);
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.Size = new System.Drawing.Size(240, 24);
            this.buttonF4.TabIndex = 4;
            this.buttonF4.Text = "      Назад";
            this.buttonF4.Click += new System.EventHandler(this.buttonF4_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 170);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 124);
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(240, 114);
            this.listBox1.TabIndex = 5;
            // 
            // ServiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "ServiceForm";
            this.Text = "Обслуживание";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ServiceForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelF3;
        private System.Windows.Forms.Label labelF2;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Button buttonF3;
        private System.Windows.Forms.Button buttonF2;
        private System.Windows.Forms.Button buttonF1;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Button buttonF4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label labelCLR;
        private System.Windows.Forms.Button buttonCLR;
        private System.Windows.Forms.Label labelDot;
        private System.Windows.Forms.Button buttonDot;
        private System.Windows.Forms.Button button0;
        private System.Windows.Forms.Label label0;
    }
}