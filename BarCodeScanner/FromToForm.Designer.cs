namespace BarCodeScanner
{
    partial class FromToForm
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
            this.labelF4 = new System.Windows.Forms.Label();
            this.buttonF4 = new System.Windows.Forms.Button();
            this.labelF1 = new System.Windows.Forms.Label();
            this.buttonF1 = new System.Windows.Forms.Button();
            this.listBoxOperation = new System.Windows.Forms.ListBox();
            this.labelFrom = new System.Windows.Forms.Label();
            this.listBoxFrom = new System.Windows.Forms.ListBox();
            this.labelTo = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.listBoxTo = new System.Windows.Forms.ListBox();
            this.labelOperation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Yellow;
            this.labelF4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF4.Location = new System.Drawing.Point(122, 272);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonF4
            // 
            this.buttonF4.Location = new System.Drawing.Point(120, 270);
            this.buttonF4.Name = "buttonF4";
            this.buttonF4.Size = new System.Drawing.Size(120, 24);
            this.buttonF4.TabIndex = 5;
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
            // buttonF1
            // 
            this.buttonF1.Location = new System.Drawing.Point(0, 270);
            this.buttonF1.Name = "buttonF1";
            this.buttonF1.Size = new System.Drawing.Size(120, 24);
            this.buttonF1.TabIndex = 4;
            this.buttonF1.Text = "          Дальше";
            this.buttonF1.Click += new System.EventHandler(this.buttonF1_Click);
            // 
            // listBoxOperation
            // 
            this.listBoxOperation.Location = new System.Drawing.Point(3, 23);
            this.listBoxOperation.Name = "listBoxOperation";
            this.listBoxOperation.Size = new System.Drawing.Size(234, 58);
            this.listBoxOperation.TabIndex = 1;
            this.listBoxOperation.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // labelFrom
            // 
            this.labelFrom.Location = new System.Drawing.Point(65, 87);
            this.labelFrom.Name = "labelFrom";
            this.labelFrom.Size = new System.Drawing.Size(169, 16);
            // 
            // listBoxFrom
            // 
            this.listBoxFrom.Enabled = false;
            this.listBoxFrom.Location = new System.Drawing.Point(3, 104);
            this.listBoxFrom.Name = "listBoxFrom";
            this.listBoxFrom.Size = new System.Drawing.Size(234, 72);
            this.listBoxFrom.TabIndex = 2;
            this.listBoxFrom.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // labelTo
            // 
            this.labelTo.Location = new System.Drawing.Point(65, 178);
            this.labelTo.Name = "labelTo";
            this.labelTo.Size = new System.Drawing.Size(169, 16);
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label21.Location = new System.Drawing.Point(3, 87);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(56, 16);
            this.label21.Text = "Откуда";
            // 
            // label31
            // 
            this.label31.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label31.Location = new System.Drawing.Point(3, 178);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(56, 16);
            this.label31.Text = "Куда";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(3, 6);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(68, 16);
            this.label11.Text = "Операция";
            // 
            // listBoxTo
            // 
            this.listBoxTo.Enabled = false;
            this.listBoxTo.Location = new System.Drawing.Point(3, 195);
            this.listBoxTo.Name = "listBoxTo";
            this.listBoxTo.Size = new System.Drawing.Size(234, 72);
            this.listBoxTo.TabIndex = 3;
            this.listBoxTo.SelectedIndexChanged += new System.EventHandler(this.listBox3_SelectedIndexChanged);
            // 
            // labelOperation
            // 
            this.labelOperation.Location = new System.Drawing.Point(77, 6);
            this.labelOperation.Name = "labelOperation";
            this.labelOperation.Size = new System.Drawing.Size(157, 16);
            // 
            // FromToForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.Controls.Add(this.labelOperation);
            this.Controls.Add(this.listBoxTo);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.labelTo);
            this.Controls.Add(this.listBoxFrom);
            this.Controls.Add(this.labelFrom);
            this.Controls.Add(this.listBoxOperation);
            this.Controls.Add(this.labelF1);
            this.Controls.Add(this.buttonF1);
            this.Controls.Add(this.labelF4);
            this.Controls.Add(this.buttonF4);
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "FromToForm";
            this.Text = "Откуда / куда";
            this.Load += new System.EventHandler(this.FromToForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FromToForm_Closing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FromToForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Button buttonF4;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Button buttonF1;
        private System.Windows.Forms.ListBox listBoxOperation;
        private System.Windows.Forms.Label labelFrom;
        private System.Windows.Forms.ListBox listBoxFrom;
        private System.Windows.Forms.Label labelTo;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ListBox listBoxTo;
        private System.Windows.Forms.Label labelOperation;
    }
}