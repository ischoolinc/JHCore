namespace StudentChangeStatus_KH
{
    partial class sendMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblMsg = new DevComponents.DotNetBar.LabelX();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnY = new DevComponents.DotNetBar.ButtonX();
            this.btnN = new DevComponents.DotNetBar.ButtonX();
            this.lbUrl = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblMsg.BackgroundStyle.Class = "";
            this.lblMsg.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblMsg.Location = new System.Drawing.Point(12, 12);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(422, 75);
            this.lblMsg.TabIndex = 0;
            this.lblMsg.Text = "請問是否將 測試學生 由 一般 調整成 畢業或離校，\r\n按下「是」確認後，不需函報教育局，\r\n僅由局端線上審核。";
            this.lblMsg.TextAlignment = System.Drawing.StringAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(12, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "備註：";
            // 
            // txtMsg
            // 
            this.txtMsg.Location = new System.Drawing.Point(60, 94);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(374, 22);
            this.txtMsg.TabIndex = 2;
            // 
            // btnY
            // 
            this.btnY.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnY.AutoSize = true;
            this.btnY.BackColor = System.Drawing.Color.Transparent;
            this.btnY.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnY.Location = new System.Drawing.Point(273, 134);
            this.btnY.Name = "btnY";
            this.btnY.Size = new System.Drawing.Size(75, 25);
            this.btnY.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnY.TabIndex = 3;
            this.btnY.Text = "是";
            this.btnY.Click += new System.EventHandler(this.btnY_Click);
            // 
            // btnN
            // 
            this.btnN.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnN.AutoSize = true;
            this.btnN.BackColor = System.Drawing.Color.Transparent;
            this.btnN.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnN.Location = new System.Drawing.Point(357, 134);
            this.btnN.Name = "btnN";
            this.btnN.Size = new System.Drawing.Size(75, 25);
            this.btnN.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnN.TabIndex = 4;
            this.btnN.Text = "否";
            this.btnN.Click += new System.EventHandler(this.btnN_Click);
            // 
            // lbUrl
            // 
            this.lbUrl.AutoSize = true;
            this.lbUrl.BackColor = System.Drawing.Color.Transparent;
            this.lbUrl.Location = new System.Drawing.Point(15, 138);
            this.lbUrl.Name = "lbUrl";
            this.lbUrl.Size = new System.Drawing.Size(242, 17);
            this.lbUrl.TabIndex = 5;
            this.lbUrl.TabStop = true;
            this.lbUrl.Text = "轉入生自動編班與調班系統紀錄審查作業";
            this.lbUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbUrl_LinkClicked);
            // 
            // sendMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 171);
            this.Controls.Add(this.lbUrl);
            this.Controls.Add(this.btnN);
            this.Controls.Add(this.btnY);
            this.Controls.Add(this.txtMsg);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblMsg);
            this.DoubleBuffered = true;
            this.Name = "sendMessage";
            this.Text = "訊息";
            this.Load += new System.EventHandler(this.sendMessage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lblMsg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMsg;
        private DevComponents.DotNetBar.ButtonX btnY;
        private DevComponents.DotNetBar.ButtonX btnN;
        private System.Windows.Forms.LinkLabel lbUrl;
    }
}