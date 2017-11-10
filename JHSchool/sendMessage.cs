using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StudentChangeStatus_KH
{
    public partial class sendMessage : FISCA.Presentation.Controls.BaseForm
    {                
        private string _studName="";
        private string _StudStatus="";
        private string _NewStudStatus="";
        private bool _chkSendSpec = false;
        private string _Message = "";

        public sendMessage(string StudName, string StudStatus, string NewStudStatus, bool chkSendSpec)
        {
            InitializeComponent();
            _studName = StudName;
            _StudStatus = StudStatus;
            _NewStudStatus = NewStudStatus;
            _chkSendSpec = chkSendSpec;
        }

        public string GetMessage()
        {
            return _Message;
        }

        private void btnN_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        private void btnY_Click(object sender, EventArgs e)
        {
            bool pass = true;
            // 檢查當特殊狀態，
            if (_chkSendSpec)
                if (string.IsNullOrWhiteSpace(txtMsg.Text))
                {
                    pass = false;
                    FISCA.Presentation.Controls.MsgBox.Show("備註必填欄位。");
                }
            if(pass)
            {
                _Message = txtMsg.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }            
        }

        private void sendMessage_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.MinimumSize = this.Size;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("請問是否將 " + _studName + " 由" + _StudStatus + " 調整成 " + _NewStudStatus + "，");

            if (_chkSendSpec) // 特殊狀態變更
                sb.AppendLine("按下「是」確認後，不需函報教育局，僅由局端線上審核。");
            else
                sb.AppendLine("按下「是」確認後，局端會留狀態變更紀錄。");

            lblMsg.Text = sb.ToString();
        }

        private void lbUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url = "http://163.32.129.93/%E9%AB%98%E9%9B%84%E5%B8%82%E5%85%AC%E7%A7%81%E7%AB%8B%E5%9C%8B%E6%B0%91%E4%B8%AD%E5%AD%B8%E8%BD%89%E5%85%A5%E7%94%9F%E8%87%AA%E5%8B%95%E7%B7%A8%E7%8F%AD%E8%88%87%E8%AA%BF%E7%8F%AD%E7%B3%BB%E7%B5%B1%E7%B4%80%E9%8C%84%E5%AF%A9%E6%9F%A5%E4%BD%9C%E6%A5%AD.pdf";
            ProcessStartInfo info = new ProcessStartInfo(url);
            Process.Start(info);
        }
    }
}
