namespace BarCodeScanner
{
    partial class DocListForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // dataGrid1
            // 
            this.dataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGrid1.Location = new System.Drawing.Point(0, 0);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.Size = new System.Drawing.Size(240, 149);
            this.dataGrid1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Location = new System.Drawing.Point(0, 157);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(240, 100);
            this.listBox1.TabIndex = 1;
            // 
            // DocListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.dataGrid1);
            this.Menu = this.mainMenu1;
            this.Name = "DocListForm";
            this.Text = "DocListForm";
            this.Deactivate += new System.EventHandler(this.DocListForm_Deactivate);
            this.Load += new System.EventHandler(this.DocListForm_Load);
            this.Closed += new System.EventHandler(this.DocListForm_Closed);
            this.LostFocus += new System.EventHandler(this.DocListForm_LostFocus);
            this.Activated += new System.EventHandler(this.DocListForm_Activated);
            this.GotFocus += new System.EventHandler(this.DocListForm_GotFocus);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DocListForm_Closing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Windows.Forms.ListBox listBox1;

    }
}