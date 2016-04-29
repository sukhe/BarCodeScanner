namespace BarCodeScanner
{
    partial class XCodeListForm
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
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labelF1 = new System.Windows.Forms.Label();
            this.buttonF1 = new System.Windows.Forms.Button();
            this.labelF4 = new System.Windows.Forms.Label();
            this.buttonF4 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelQuontity = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(240, 249);
            this.dataGrid1.TabIndex = 0;
            this.dataGrid1.CurrentCellChanged += new System.EventHandler(this.dataGrid1_CurrentCellChanged);
            this.dataGrid1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XCodeListForm_KeyDown);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labelF1);
            this.panel3.Controls.Add(this.buttonF1);
            this.panel3.Controls.Add(this.labelF4);
            this.panel3.Controls.Add(this.buttonF4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 270);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(240, 24);
            // 
            // labelF1
            // 
            this.labelF1.BackColor = System.Drawing.Color.Red;
            this.labelF1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF1.ForeColor = System.Drawing.Color.White;
            this.labelF1.Location = new System.Drawing.Point(2, 2);
            this.labelF1.Name = "labelF1";
            this.labelF1.Size = new System.Drawing.Size(35, 20);
            this.labelF1.Text = "F1";
            this.labelF1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF1
            // 
            this.buttonF1.Location = new System.Drawing.Point(0, 0);
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
            this.labelF4.Location = new System.Drawing.Point(122, 2);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF4
            // 
            this.buttonF4.Location = new System.Drawing.Point(120, 0);
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.Size = new System.Drawing.Size(120, 24);
            this.buttonF4.TabIndex = 11;
            this.buttonF4.Text = "          Назад";
            this.buttonF4.Click += new System.EventHandler(this.buttonF4_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelQuontity);
            this.panel1.Controls.Add(this.labelProductName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 21);
            // 
            // labelQuontity
            // 
            this.labelQuontity.Location = new System.Drawing.Point(185, 3);
            this.labelQuontity.Name = "labelQuontity";
            this.labelQuontity.Size = new System.Drawing.Size(51, 16);
            this.labelQuontity.Text = "labelQuontity";
            this.labelQuontity.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelProductName
            // 
            this.labelProductName.Location = new System.Drawing.Point(3, 3);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(160, 16);
            this.labelProductName.Text = "labelProductName";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dataGrid1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 21);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 249);
            // 
            // XCodeListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.MinimizeBox = false;
            this.Name = "XCodeListForm";
            this.Text = "Штрихкоды";
            this.Load += new System.EventHandler(this.XCodeListForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.XCodeListForm_KeyDown);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Button buttonF1;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Button buttonF4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelQuontity;
        private System.Windows.Forms.Panel panel2;
    }
}