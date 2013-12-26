namespace JHSchool.Legacy
{
    partial class PalmerwormItemBase
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該公開 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.picWaiting = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).BeginInit();
            this.SuspendLayout();
            // 
            // picWaiting
            // 
            //this.picWaiting.Image = global::SmartSchool.Properties.Resources.loading5;
            this.picWaiting.Location = new System.Drawing.Point(4, 4);
            this.picWaiting.Margin = new System.Windows.Forms.Padding(4);
            this.picWaiting.Name = "picWaiting";
            this.picWaiting.Size = new System.Drawing.Size(32, 32);
            this.picWaiting.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picWaiting.TabIndex = 0;
            this.picWaiting.TabStop = false;
            this.picWaiting.Visible = false;
            // 
            // PalmerwormItemBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.picWaiting);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PalmerwormItemBase";
            this.Size = new System.Drawing.Size(550, 150);
            this.SizeChanged += new System.EventHandler(this.PalmerwormItemBase_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picWaiting;

    }
}
