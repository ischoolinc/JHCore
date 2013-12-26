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

namespace JHSchool.StudentExtendControls.Ribbon
{
    public partial class AddStudent : BaseForm
    {
        public AddStudent()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;
            JHSchool.Data.JHStudentRecord studRec = new JHSchool.Data.JHStudentRecord();
            studRec.Name = txtName.Text;
            string StudentID = JHSchool.Data.JHStudent.Insert(studRec);
            PermRecLogProcess prlp = new PermRecLogProcess();
            if (chkInputData.Checked == true)
            {
                if (StudentID != "")
                {
                    Student.Instance.PopupDetailPane(StudentID);
                    Student.Instance.SyncDataBackground(StudentID);
                }
            }
            Student.Instance.SyncDataBackground(StudentID);

            prlp.SaveLog("學籍.學生", "新增學生", "新增學生姓名:" + txtName.Text);
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
