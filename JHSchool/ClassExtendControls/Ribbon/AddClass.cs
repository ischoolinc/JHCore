using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Editor;

namespace JHSchool.ClassExtendControls.Ribbon
{
    public partial class AddClass : BaseForm
    {
        public AddClass()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool chkHasClassName = false;
            if (txtName.Text.Trim() == "")
                return;

            List<JHSchool.Data.JHClassRecord> AllClassRecs = JHSchool.Data.JHClass.SelectAll();
            foreach (JHSchool.Data.JHClassRecord cr in AllClassRecs)
                if (cr.Name == txtName.Text)
                {
                    MessageBox.Show("班級名稱重複");
                    return;
                }

            PermRecLogProcess prlp = new PermRecLogProcess();
            JHSchool.Data.JHClassRecord classRec = new JHSchool.Data.JHClassRecord();
            classRec.Name = txtName.Text;
            string ClassID = JHSchool.Data.JHClass.Insert(classRec);

            Class.Instance.SyncDataBackground(ClassID);

            if (chkInputData.Checked == true)
            {
                Class.Instance.PopupDetailPane(ClassID);
                Class.Instance.SyncDataBackground(ClassID);
            }

            prlp.SaveLog("學籍.班級", "新增班級", "新增班級,名稱:" + txtName.Text);
            this.Close();

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
