namespace BarCodeScanner
{
    partial class FileListForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.buttonF3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonF2 = new System.Windows.Forms.Button();
            this.labelF1 = new System.Windows.Forms.Label();
            this.buttonF1 = new System.Windows.Forms.Button();
            this.labelF4 = new System.Windows.Forms.Label();
            this.buttonF4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.buttonF3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonF2);
            this.panel1.Controls.Add(this.labelF1);
            this.panel1.Controls.Add(this.buttonF1);
            this.panel1.Controls.Add(this.labelF4);
            this.panel1.Controls.Add(this.buttonF4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 246);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 48);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Green;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(122, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 20);
            this.label2.Text = "F3";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF3
            // 
            this.buttonF3.Location = new System.Drawing.Point(120, 0);
            this.buttonF3.Name = "buttonF3";
            this.buttonF3.Size = new System.Drawing.Size(120, 24);
            this.buttonF3.TabIndex = 18;
            this.buttonF3.Text = "          Содержим.";
            this.buttonF3.Click += new System.EventHandler(this.buttonF3_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Blue;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(2, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 20);
            this.label1.Text = "F2";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF2
            // 
            this.buttonF2.Location = new System.Drawing.Point(0, 0);
            this.buttonF2.Name = "buttonF2";
            this.buttonF2.Size = new System.Drawing.Size(120, 24);
            this.buttonF2.TabIndex = 15;
            this.buttonF2.Text = "          Просмотр";
            this.buttonF2.Click += new System.EventHandler(this.buttonF2_Click);
            // 
            // labelF1
            // 
            this.labelF1.BackColor = System.Drawing.Color.Red;
            this.labelF1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF1.ForeColor = System.Drawing.Color.White;
            this.labelF1.Location = new System.Drawing.Point(2, 25);
            this.labelF1.Name = "labelF1";
            this.labelF1.Size = new System.Drawing.Size(35, 20);
            this.labelF1.Text = "F1";
            this.labelF1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF1
            // 
            this.buttonF1.Location = new System.Drawing.Point(0, 23);
            this.buttonF1.Name = "buttonF1";
            this.buttonF1.Size = new System.Drawing.Size(120, 24);
            this.buttonF1.TabIndex = 12;
            this.buttonF1.Text = "          Удалить";
            this.buttonF1.Click += new System.EventHandler(this.buttonF1_Click);
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Yellow;
            this.labelF4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF4.Location = new System.Drawing.Point(122, 25);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF4
            // 
            this.buttonF4.Location = new System.Drawing.Point(120, 23);
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.Size = new System.Drawing.Size(120, 24);
            this.buttonF4.TabIndex = 11;
            this.buttonF4.Text = "          Назад";
            this.buttonF4.Click += new System.EventHandler(this.buttonF4_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGrid1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 246);
            // 
            // dataGrid1
            // 
            this.dataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(240, 246);
            this.dataGrid1.TabIndex = 0;
            // 
            // FileListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "FileListForm";
            this.Text = "Файлы документов";
            this.Load += new System.EventHandler(this.FileListForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FileListForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Button buttonF1;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Button buttonF4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonF3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonF2;
    }
}