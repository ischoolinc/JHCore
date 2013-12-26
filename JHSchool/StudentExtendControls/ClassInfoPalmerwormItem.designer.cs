using DevComponents.DotNetBar.Controls;
namespace JHSchool.StudentExtendControls
{
    partial class ClassInfoPalmerwormItem
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
            this.components = new System.ComponentModel.Container();
            this.txtStudentID = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label41 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.cboClass = new JHSchool.Legacy.ComboBoxAdv();
            this.cboSeatNo = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.warnings = new System.Windows.Forms.ErrorProvider(this.components);
            this.cboDept = new JHSchool.Legacy.ComboBoxAdv();
            this.lblDept = new System.Windows.Forms.Label();
            this.cboGradeYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem4 = new DevComponents.Editors.ComboItem();
            this.comboItem1 = new DevComponents.Editors.ComboItem();
            this.comboItem2 = new DevComponents.Editors.ComboItem();
            this.comboItem3 = new DevComponents.Editors.ComboItem();
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warnings)).BeginInit();
            this.SuspendLayout();
            // 
            // txtStudentID
            // 
            // 
            // 
            // 
            this.txtStudentID.Border.Class = "TextBoxBorder";
            this.txtStudentID.Location = new System.Drawing.Point(85, 50);
            this.txtStudentID.Margin = new System.Windows.Forms.Padding(4);
            this.txtStudentID.Name = "txtStudentID";
            this.txtStudentID.Size = new System.Drawing.Size(119, 25);
            this.txtStudentID.TabIndex = 4;
            this.txtStudentID.Validating += new System.ComponentModel.CancelEventHandler(this.txtStudentID_Validating);
            this.txtStudentID.TextChanged += new System.EventHandler(this.txtStudentID_TextChanged);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.BackColor = System.Drawing.Color.Transparent;
            this.label41.ForeColor = System.Drawing.Color.Black;
            this.label41.Location = new System.Drawing.Point(30, 54);
            this.label41.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(47, 17);
            this.label41.TabIndex = 197;
            this.label41.Text = "學　號";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.BackColor = System.Drawing.Color.Transparent;
            this.label37.ForeColor = System.Drawing.Color.Black;
            this.label37.Location = new System.Drawing.Point(406, 12);
            this.label37.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(47, 17);
            this.label37.TabIndex = 199;
            this.label37.Text = "座　號";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.BackColor = System.Drawing.Color.Transparent;
            this.label38.ForeColor = System.Drawing.Color.Black;
            this.label38.Location = new System.Drawing.Point(218, 12);
            this.label38.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(47, 17);
            this.label38.TabIndex = 196;
            this.label38.Text = "班　級";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.BackColor = System.Drawing.Color.Transparent;
            this.label40.ForeColor = System.Drawing.Color.Black;
            this.label40.Location = new System.Drawing.Point(30, 12);
            this.label40.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(47, 17);
            this.label40.TabIndex = 198;
            this.label40.Text = "年　級";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label40.Click += new System.EventHandler(this.label40_Click);
            // 
            // cboClass
            // 
            this.cboClass.DisplayMember = "Text";
            this.cboClass.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboClass.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboClass.FormattingEnabled = true;
            this.cboClass.ItemHeight = 16;
            this.cboClass.Location = new System.Drawing.Point(274, 9);
            this.cboClass.Margin = new System.Windows.Forms.Padding(4);
            this.cboClass.Name = "cboClass";
            this.cboClass.Size = new System.Drawing.Size(119, 22);
            this.cboClass.TabIndex = 2;
            this.cboClass.Validating += new System.ComponentModel.CancelEventHandler(this.cboClass_Validating);
            this.cboClass.SelectedIndexChanged += new System.EventHandler(this.cboClass_SelectedIndexChanged_1);
            // 
            // cboSeatNo
            // 
            this.cboSeatNo.DisplayMember = "Text";
            this.cboSeatNo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSeatNo.FormattingEnabled = true;
            this.cboSeatNo.ItemHeight = 19;
            this.cboSeatNo.Location = new System.Drawing.Point(461, 8);
            this.cboSeatNo.Margin = new System.Windows.Forms.Padding(4);
            this.cboSeatNo.Name = "cboSeatNo";
            this.cboSeatNo.Size = new System.Drawing.Size(76, 25);
            this.cboSeatNo.TabIndex = 3;
            this.cboSeatNo.SelectedIndexChanged += new System.EventHandler(this.cboSeatNo_SelectedIndexChanged_1);
            this.cboSeatNo.TextChanged += new System.EventHandler(this.cboSeatNo_TextChanged);
            // 
            // warnings
            // 
            this.warnings.ContainerControl = this;
            // 
            // cboDept
            // 
            this.cboDept.DisplayMember = "Text";
            this.cboDept.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboDept.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDept.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.cboDept.ForeColor = System.Drawing.SystemColors.WindowText;
            this.cboDept.FormattingEnabled = true;
            this.cboDept.ItemHeight = 16;
            this.cboDept.Location = new System.Drawing.Point(274, 54);
            this.cboDept.Margin = new System.Windows.Forms.Padding(4);
            this.cboDept.Name = "cboDept";
            this.cboDept.Size = new System.Drawing.Size(263, 22);
            this.cboDept.TabIndex = 200;
            this.cboDept.Visible = false;
            this.cboDept.SelectedIndexChanged += new System.EventHandler(this.cboDept_SelectedIndexChanged);
            // 
            // lblDept
            // 
            this.lblDept.AutoSize = true;
            this.lblDept.BackColor = System.Drawing.Color.Transparent;
            this.lblDept.ForeColor = System.Drawing.Color.Black;
            this.lblDept.Location = new System.Drawing.Point(218, 57);
            this.lblDept.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDept.Name = "lblDept";
            this.lblDept.Size = new System.Drawing.Size(47, 17);
            this.lblDept.TabIndex = 201;
            this.lblDept.Text = "科　別";
            this.lblDept.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDept.Visible = false;
            // 
            // cboGradeYear
            // 
            this.cboGradeYear.DisplayMember = "Text";
            this.cboGradeYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboGradeYear.FormattingEnabled = true;
            this.cboGradeYear.ItemHeight = 19;
            this.cboGradeYear.Items.AddRange(new object[] {
            this.comboItem4,
            this.comboItem1,
            this.comboItem2,
            this.comboItem3});
            this.cboGradeYear.Location = new System.Drawing.Point(85, 8);
            this.cboGradeYear.Name = "cboGradeYear";
            this.cboGradeYear.Size = new System.Drawing.Size(121, 25);
            this.cboGradeYear.TabIndex = 202;
            this.cboGradeYear.TextChanged += new System.EventHandler(this.cboGradeYear_SelectedIndexChanged);
            // 
            // comboItem1
            // 
            this.comboItem1.Text = "7";
            // 
            // comboItem2
            // 
            this.comboItem2.Text = "8";
            // 
            // comboItem3
            // 
            this.comboItem3.Text = "9";
            // 
            // ClassInfoPalmerwormItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cboGradeYear);
            this.Controls.Add(this.cboDept);
            this.Controls.Add(this.lblDept);
            this.Controls.Add(this.txtStudentID);
            this.Controls.Add(this.cboClass);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.cboSeatNo);
            this.Controls.Add(this.label37);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "ClassInfoPalmerwormItem";
            this.Size = new System.Drawing.Size(550, 87);
            this.Load += new System.EventHandler(this.ClassInfoPalmerwormItem_Load);
            this.Controls.SetChildIndex(this.label37, 0);
            this.Controls.SetChildIndex(this.cboSeatNo, 0);
            this.Controls.SetChildIndex(this.label38, 0);
            this.Controls.SetChildIndex(this.label40, 0);
            this.Controls.SetChildIndex(this.label41, 0);
            this.Controls.SetChildIndex(this.cboClass, 0);
            this.Controls.SetChildIndex(this.txtStudentID, 0);
            this.Controls.SetChildIndex(this.picWaiting, 0);
            this.Controls.SetChildIndex(this.lblDept, 0);
            this.Controls.SetChildIndex(this.cboDept, 0);
            this.Controls.SetChildIndex(this.cboGradeYear, 0);
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warnings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal DevComponents.DotNetBar.Controls.TextBoxX txtStudentID;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.Label label40;
        private JHSchool.Legacy.ComboBoxAdv cboClass;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSeatNo;
        private System.Windows.Forms.ErrorProvider warnings;
        private JHSchool.Legacy.ComboBoxAdv cboDept;
        private System.Windows.Forms.Label lblDept;
        private ComboBoxEx cboGradeYear;
        private DevComponents.Editors.ComboItem comboItem1;
        private DevComponents.Editors.ComboItem comboItem2;
        private DevComponents.Editors.ComboItem comboItem3;
        private DevComponents.Editors.ComboItem comboItem4;
    }
}
