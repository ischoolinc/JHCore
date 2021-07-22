namespace JHSchool.ClassExtendControls
{
    partial class ClassBaseInfoItem
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
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.txtClassName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.cboDept = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboTeacher = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboGradeYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.txtSortOrder = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.txtClassNumber = new DevComponents.DotNetBar.Controls.TextBoxX();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelX1
            // 
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(18, 20);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(60, 24);
            this.labelX1.TabIndex = 2;
            this.labelX1.Text = "班級名稱";
            // 
            // txtClassName
            // 
            // 
            // 
            // 
            this.txtClassName.Border.Class = "TextBoxBorder";
            this.txtClassName.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtClassName.Location = new System.Drawing.Point(84, 20);
            this.txtClassName.Name = "txtClassName";
            this.txtClassName.Size = new System.Drawing.Size(151, 25);
            this.txtClassName.TabIndex = 1;
            this.txtClassName.TextChanged += new System.EventHandler(this.txtClassName_TextChanged);
            this.txtClassName.Enter += new System.EventHandler(this.txtClassName_Enter);
            this.txtClassName.Leave += new System.EventHandler(this.txtClassName_Leave);
            // 
            // labelX2
            // 
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(18, 140);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(70, 24);
            this.labelX2.TabIndex = 4;
            this.labelX2.Text = "科　　別";
            this.labelX2.Visible = false;
            // 
            // labelX3
            // 
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(347, 20);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(56, 24);
            this.labelX3.TabIndex = 6;
            this.labelX3.Text = "班 導 師";
            // 
            // cboDept
            // 
            this.cboDept.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboDept.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboDept.DisplayMember = "Text";
            this.cboDept.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboDept.DropDownWidth = 150;
            this.cboDept.FormattingEnabled = true;
            this.cboDept.Location = new System.Drawing.Point(94, 139);
            this.cboDept.Name = "cboDept";
            this.cboDept.Size = new System.Drawing.Size(150, 26);
            this.cboDept.TabIndex = 7;
            this.cboDept.Visible = false;
            // 
            // cboTeacher
            // 
            this.cboTeacher.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboTeacher.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cboTeacher.DisplayMember = "FullName";
            this.cboTeacher.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTeacher.FormattingEnabled = true;
            this.cboTeacher.Location = new System.Drawing.Point(399, 19);
            this.cboTeacher.Name = "cboTeacher";
            this.cboTeacher.Size = new System.Drawing.Size(131, 26);
            this.cboTeacher.TabIndex = 3;
            this.cboTeacher.Validating += new System.ComponentModel.CancelEventHandler(this.cboTeacher_Validating);
            this.cboTeacher.Validated += new System.EventHandler(this.cboTeacher_Validated);
            // 
            // cboGradeYear
            // 
            this.cboGradeYear.DisplayMember = "Text";
            this.cboGradeYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboGradeYear.FormattingEnabled = true;
            this.cboGradeYear.Location = new System.Drawing.Point(281, 19);
            this.cboGradeYear.Name = "cboGradeYear";
            this.cboGradeYear.Size = new System.Drawing.Size(60, 26);
            this.cboGradeYear.TabIndex = 2;
            this.cboGradeYear.TextChanged += new System.EventHandler(this.cboGradeYear_TextChanged);
            this.cboGradeYear.Validated += new System.EventHandler(this.cboGradeYear_Validated);
            // 
            // labelX4
            // 
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(244, 20);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(41, 24);
            this.labelX4.TabIndex = 10;
            this.labelX4.Text = "年級";
            // 
            // labelX6
            // 
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.Class = "";
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Location = new System.Drawing.Point(18, 50);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(60, 24);
            this.labelX6.TabIndex = 13;
            this.labelX6.Text = "排列序號";
            // 
            // txtSortOrder
            // 
            // 
            // 
            // 
            this.txtSortOrder.Border.Class = "TextBoxBorder";
            this.txtSortOrder.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtSortOrder.Location = new System.Drawing.Point(84, 50);
            this.txtSortOrder.Name = "txtSortOrder";
            this.txtSortOrder.Size = new System.Drawing.Size(151, 25);
            this.txtSortOrder.TabIndex = 14;
            this.txtSortOrder.WatermarkText = "請輸入整數";
            this.txtSortOrder.Validated += new System.EventHandler(this.txtSortOrder_Validated);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // labelX5
            // 
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(18, 81);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(60, 24);
            this.labelX5.TabIndex = 16;
            this.labelX5.Text = "班級編號";
            // 
            // txtClassNumber
            // 
            // 
            // 
            // 
            this.txtClassNumber.Border.Class = "TextBoxBorder";
            this.txtClassNumber.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.txtClassNumber.Location = new System.Drawing.Point(84, 81);
            this.txtClassNumber.Name = "txtClassNumber";
            this.txtClassNumber.Size = new System.Drawing.Size(151, 25);
            this.txtClassNumber.TabIndex = 15;
            this.txtClassNumber.TextChanged += new System.EventHandler(this.txtClassNumber_TextChanged);
            // 
            // ClassBaseInfoItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelX5);
            this.Controls.Add(this.txtClassNumber);
            this.Controls.Add(this.labelX6);
            this.Controls.Add(this.txtSortOrder);
            this.Controls.Add(this.cboGradeYear);
            this.Controls.Add(this.labelX4);
            this.Controls.Add(this.cboTeacher);
            this.Controls.Add(this.cboDept);
            this.Controls.Add(this.labelX3);
            this.Controls.Add(this.labelX2);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.txtClassName);
            this.Name = "ClassBaseInfoItem";
            this.Size = new System.Drawing.Size(550, 115);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX labelX1;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtClassName;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.LabelX labelX3;
        protected DevComponents.DotNetBar.Controls.ComboBoxEx cboDept;
        protected DevComponents.DotNetBar.Controls.ComboBoxEx cboTeacher;
        protected DevComponents.DotNetBar.Controls.ComboBoxEx cboGradeYear;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.LabelX labelX6;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtSortOrder;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DevComponents.DotNetBar.LabelX labelX5;
        protected DevComponents.DotNetBar.Controls.TextBoxX txtClassNumber;
    }
}
