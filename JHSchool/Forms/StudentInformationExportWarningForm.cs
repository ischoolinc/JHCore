using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using FISCA.Presentation.Controls;

namespace JHSchool.Forms
{
    public partial class StudentInformationExportWarningForm : BaseForm
    {
        public StudentInformationExportWarningForm()
        {
            InitializeComponent();

            labelX1.Text = "請輸入用途資訊，匯出用途將儲存於LOG紀錄中存查使用。";
        }

        // 事由
        public String reason;        

        //送出
        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (textBoxX1.Text == "")
            {
                MsgBox.Show("請輸入事由", "匯出用途", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                reason = textBoxX1.Text;                 
                this.DialogResult =  DialogResult.OK;                                    
            }
        }

        //取消
        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;                                    
        }
    }
}
