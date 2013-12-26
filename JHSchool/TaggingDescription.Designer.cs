namespace JHSchool
{
    partial class TaggingDescription
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if ( disposing && ( components != null ) )
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
            this.TableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.DescriptionPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.TagPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.TableLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.TableLayout.BackColor = System.Drawing.Color.Transparent;
            this.TableLayout.ColumnCount = 1;
            this.TableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayout.Controls.Add(this.DescriptionPanel, 0, 0);
            this.TableLayout.Controls.Add(this.TagPanel, 0, 1);
            this.TableLayout.Location = new System.Drawing.Point(0, 0);
            this.TableLayout.Margin = new System.Windows.Forms.Padding(2);
            this.TableLayout.Name = "tableLayoutPanel1";
            this.TableLayout.RowCount = 2;
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 53F));
            this.TableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 47F));
            this.TableLayout.Size = new System.Drawing.Size(375, 55);
            this.TableLayout.TabIndex = 1;
            // 
            // DescriptionPanel
            // 
            this.DescriptionPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionPanel.Location = new System.Drawing.Point(1, 1);
            this.DescriptionPanel.Margin = new System.Windows.Forms.Padding(1);
            this.DescriptionPanel.Name = "DescriptionPanel";
            this.DescriptionPanel.Size = new System.Drawing.Size(373, 27);
            this.DescriptionPanel.TabIndex = 185;
            // 
            // TagPanel
            // 
            this.TagPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TagPanel.AutoSize = true;
            this.TagPanel.Location = new System.Drawing.Point(187, 42);
            this.TagPanel.Margin = new System.Windows.Forms.Padding(2);
            this.TagPanel.Name = "TagPanel";
            this.TagPanel.Size = new System.Drawing.Size(0, 0);
            this.TagPanel.TabIndex = 186;
            // 
            // TaggingDescription
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.Controls.Add(this.TableLayout);
            this.Name = "TaggingDescription";
            this.Size = new System.Drawing.Size(377, 57);
            this.TableLayout.ResumeLayout(false);
            this.TableLayout.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel TagPanel;
        protected System.Windows.Forms.TableLayoutPanel TableLayout;
        protected System.Windows.Forms.FlowLayoutPanel DescriptionPanel;

    }
}
