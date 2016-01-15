namespace BarCodeScanner
{
    partial class DialogForm
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
            this.buttonRetry = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelText1 = new System.Windows.Forms.Label();
            this.labelF1 = new System.Windows.Forms.Label();
            this.labelF4 = new System.Windows.Forms.Label();
            this.labelText2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonRetry
            // 
            this.buttonRetry.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.buttonRetry.Location = new System.Drawing.Point(3, 173);
            this.buttonRetry.Name = "buttonRetry";
            this.buttonRetry.Size = new System.Drawing.Size(115, 28);
            this.buttonRetry.TabIndex = 0;
            this.buttonRetry.Text = "      Retry";
            this.buttonRetry.Click += new System.EventHandler(this.buttonRetry_Click);
            this.buttonRetry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DialogForm_KeyDown);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(124, 173);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(113, 28);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "      Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            this.buttonCancel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DialogForm_KeyDown);
            // 
            // labelText1
            // 
            this.labelText1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelText1.Location = new System.Drawing.Point(25, 86);
            this.labelText1.Name = "labelText1";
            this.labelText1.Size = new System.Drawing.Size(188, 25);
            this.labelText1.Text = "Text";
            this.labelText1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelF1
            // 
            this.labelF1.BackColor = System.Drawing.Color.Red;
            this.labelF1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF1.ForeColor = System.Drawing.Color.White;
            this.labelF1.Location = new System.Drawing.Point(6, 177);
            this.labelF1.Name = "labelF1";
            this.labelF1.Size = new System.Drawing.Size(35, 20);
            this.labelF1.Text = "F1";
            this.labelF1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelF4
            // 
            this.labelF4.BackColor = System.Drawing.Color.Yellow;
            this.labelF4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelF4.ForeColor = System.Drawing.Color.Black;
            this.labelF4.Location = new System.Drawing.Point(127, 177);
            this.labelF4.Name = "labelF4";
            this.labelF4.Size = new System.Drawing.Size(35, 20);
            this.labelF4.Text = "F4";
            this.labelF4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labelText2
            // 
            this.labelText2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.labelText2.Location = new System.Drawing.Point(25, 123);
            this.labelText2.Name = "labelText2";
            this.labelText2.Size = new System.Drawing.Size(188, 25);
            this.labelText2.Text = "Text2";
            this.labelText2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // DialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(240, 294);
            this.ControlBox = false;
            this.Controls.Add(this.labelText2);
            this.Controls.Add(this.labelF4);
            this.Controls.Add(this.labelF1);
            this.Controls.Add(this.labelText1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonRetry);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "DialogForm";
            this.Text = "DialogForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DialogForm_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonRetry;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelText1;
        private System.Windows.Forms.Label labelF1;
        private System.Windows.Forms.Label labelF4;
        private System.Windows.Forms.Label labelText2;

    }
}