using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
//using SmartSchool.Feature.Basic;
using FISCA.Presentation.Controls;
using Framework;

namespace JHSchool.Legacy
{
    internal partial class SMSForm : BaseForm
    {
        private string _number;

        public SMSForm()
        {
            InitializeComponent();
        }

        public void SetNumber(string number)
        {
            _number = number;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            e.Cancel = true;
            this.txtContent.Text = "";
            this.Hide();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您確定要發送此簡訊給『" + _number + "』?", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //SeniorHSBase.Feature.Legacy.Config.SendSMS(_number, this.txtContent.Text);
                this.Close();
            }
        }

        private void SMSForm_Shown(object sender, EventArgs e)
        {
            txtContent.Focus();
        }
    }
}