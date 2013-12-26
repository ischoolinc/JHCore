using FISCA.Presentation.Controls;
using SmartSchool.Common;
namespace JHSchool.TeacherExtendControls
{
    partial class TeachCourseItem
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
            this.listView = new SmartSchool.Common.ListViewEX();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.cboSemsFilter = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).BeginInit();
            this.SuspendLayout();
            // 
            // listView
            // 
            // 
            // 
            // 
            this.listView.Border.Class = "ListViewBorder";
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(18, 45);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(512, 247);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "課程名稱";
            this.columnHeader1.Width = 202;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "學分數";
            this.columnHeader2.Width = 72;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "修課人數";
            this.columnHeader3.Width = 88;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "學年度";
            this.columnHeader4.Width = 73;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "學期";
            // 
            // cboSemsFilter
            // 
            this.cboSemsFilter.DisplayMember = "Text";
            this.cboSemsFilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemsFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSemsFilter.FormattingEnabled = true;
            this.cboSemsFilter.ItemHeight = 17;
            this.cboSemsFilter.Location = new System.Drawing.Point(18, 11);
            this.cboSemsFilter.Name = "cboSemsFilter";
            this.cboSemsFilter.Size = new System.Drawing.Size(160, 23);
            this.cboSemsFilter.TabIndex = 2;
            this.cboSemsFilter.SelectedIndexChanged += new System.EventHandler(this.cboSemsFilter_SelectedIndexChanged);
            // 
            // TeachCourseItem
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.cboSemsFilter);
            this.Name = "TeachCourseItem";
            this.Size = new System.Drawing.Size(550, 307);
            this.Controls.SetChildIndex(this.cboSemsFilter, 0);
            this.Controls.SetChildIndex(this.listView, 0);
            this.Controls.SetChildIndex(this.picWaiting, 0);
            ((System.ComponentModel.ISupportInitialize)(this.picWaiting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListViewEX listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemsFilter;
    }
}
