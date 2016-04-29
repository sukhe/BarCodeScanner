namespace BarCodeScanner
{
    partial class MainForm
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
            this.timer1 = new System.Windows.Forms.Timer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelF3 = new System.Windows.Forms.Label();
            this.labelF1 = new System.Windows.Forms.Label();
            this.buttonF3 = new System.Windows.Forms.Button();
            this.buttonF1 = new System.Windows.Forms.Button();
            this.panelTrigger = new System.Windows.Forms.Panel();
            this.labelTrigger = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelF4 = new System.Windows.Forms.Label();
            this.buttonF4 = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panelF2 = new System.Windows.Forms.Panel();
            this.labelTo = new System.Windows.Forms.Label();
            this.labelFrom = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelF2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panelTrigger.SuspendLayout();
            this.panelF2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelF3);
            this.panel1.Controls.Add(this.labelF1);
            this.panel1.Controls.Add(this.buttonF3);
            this.panel1.Controls.Add(this.buttonF1);
            this.panel1.Controls.Add(this.panelTrigger);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.labelF4);
            this.panel1.Controls.Add(this.buttonF4);
            this.panel1.Location = new System.Drawing.Point(0, 211);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 83);
            // 
            // labelF3
            // 
            this.labelF3.BackColor = System.Drawing.Color.Green;
            this.labelF3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF3.ForeColor = System.Drawing.Color.White;
            this.labelF3.Location = new System.Drawing.Point(122, 36);
            this.labelF3.Name = "labelF3";
            this.labelF3.Size = new System.Drawing.Size(35, 20);
            this.labelF3.Text = "F3";
            this.labelF3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelF1
            // 
            this.labelF1.BackColor = System.Drawing.Color.Red;
            this.labelF1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF1.ForeColor = System.Drawing.Color.White;
            this.labelF1.Location = new System.Drawing.Point(2, 36);
            this.labelF1.Name = "labelF1";
            this.labelF1.Size = new System.Drawing.Size(35, 20);
            this.labelF1.Text = "F1";
            this.labelF1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF3
            // 
            this.buttonF3.Location = new System.Drawing.Point(120, 34);
            this.buttonF3.Name = "buttonF3";
            this.buttonF3.Size = new System.Drawing.Size(120, 24);
            this.buttonF3.TabIndex = 4;
            this.buttonF3.Text = "         Ручн.№док";
            this.buttonF3.Click += new System.EventHandler(this.buttonF3_Click);
            // 
            // buttonF1
            // 
            this.buttonF1.Location = new System.Drawing.Point(0, 34);
            this.buttonF1.Name = "buttonF1";
            this.buttonF1.Size = new System.Drawing.Size(120, 24);
            this.buttonF1.TabIndex = 2;
            this.buttonF1.Text = "           Передать";
            this.buttonF1.Click += new System.EventHandler(this.buttonF1_Click);
            // 
            // panelTrigger
            // 
            this.panelTrigger.BackColor = System.Drawing.Color.Chocolate;
            this.panelTrigger.Controls.Add(this.labelTrigger);
            this.panelTrigger.Location = new System.Drawing.Point(2, 60);
            this.panelTrigger.Name = "panelTrigger";
            this.panelTrigger.Size = new System.Drawing.Size(35, 20);
            // 
            // labelTrigger
            // 
            this.labelTrigger.BackColor = System.Drawing.Color.Black;
            this.labelTrigger.Location = new System.Drawing.Point(5, 6);
            this.labelTrigger.Name = "labelTrigger";
            this.labelTrigger.Size = new System.Drawing.Size(25, 14);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(47, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 16);
            this.label2.Text = "Получить";
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Yellow;
            this.labelF4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF4.Location = new System.Drawing.Point(122, 61);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF4
            // 
            this.buttonF4.Location = new System.Drawing.Point(120, 59);
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.Size = new System.Drawing.Size(120, 24);
            this.buttonF4.TabIndex = 5;
            this.buttonF4.Text = "          Назад";
            this.buttonF4.Click += new System.EventHandler(this.buttonF4_Click);
            // 
            // dataGrid1
            // 
            this.dataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(240, 195);
            this.dataGrid1.TabIndex = 2;
            this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
            this.dataGrid1.Click += new System.EventHandler(this.dataGrid1_Click);
            // 
            // labelInfo
            // 
            this.labelInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.labelInfo.Location = new System.Drawing.Point(0, 195);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(162, 17);
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labelTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.labelTime.Location = new System.Drawing.Point(164, 195);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(75, 17);
            this.labelTime.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(43, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 15);
            this.label8.Text = "Куда";
            // 
            // panelF2
            // 
            this.panelF2.BackColor = System.Drawing.Color.Azure;
            this.panelF2.Controls.Add(this.labelTo);
            this.panelF2.Controls.Add(this.labelFrom);
            this.panelF2.Controls.Add(this.label8);
            this.panelF2.Controls.Add(this.label6);
            this.panelF2.Controls.Add(this.labelF2);
            this.panelF2.Location = new System.Drawing.Point(0, 211);
            this.panelF2.Name = "panelF2";
            this.panelF2.Size = new System.Drawing.Size(240, 33);
            this.panelF2.Click += new System.EventHandler(this.panelF2_Click);
            // 
            // labelTo
            // 
            this.labelTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.labelTo.Location = new System.Drawing.Point(100, 16);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(135, 15);
            // 
            // labelFrom
            // 
            this.labelFrom.ForeColor = System.Drawing.Color.Maroon;
            this.labelFrom.Location = new System.Drawing.Point(100, 2);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(135, 14);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(42, 2);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 14);
            this.label6.Text = "Откуда";
            // 
            // labelF2
            // 
            this.labelF2.BackColor = System.Drawing.Color.Blue;
            this.labelF2.Enabled = false;
            this.labelF2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF2.ForeColor = System.Drawing.Color.White;
            this.labelF2.Location = new System.Drawing.Point(3, 7);
            this.labelF2.Name = "labelF2";
            this.labelF2.Size = new System.Drawing.Size(35, 20);
            this.labelF2.Text = "F2";
            this.labelF2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.panelF2);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Накладные";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panelTrigger.ResumeLayout(false);
            this.panelF2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Button buttonF1;
        private System.Windows.Forms.Panel panelTrigger;
        private System.Windows.Forms.Label labelTrigger;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Button buttonF4;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label labelF3;
        private System.Windows.Forms.Button buttonF3;
        private System.Windows.Forms.Panel panelF2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.Label labelFrom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelF2;
    }
}

