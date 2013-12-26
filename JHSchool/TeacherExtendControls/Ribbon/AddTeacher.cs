using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace JHSchool.TeacherExtendControls.Ribbon
{
    public partial class AddTeacher : BaseForm
    {
        public AddTeacher()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
                return;

            // 檢查教師名稱，驗證方式，姓名+暱稱 不能重複。
            List<JHSchool.Data.JHTeacherRecord> TRecs = JHSchool.Data.JHTeacher.SelectAll();
            Dictionary<string, JHSchool.Data.JHTeacherRecord> checkStr = new Dictionary<string, JHSchool.Data.JHTeacherRecord>();
            foreach (JHSchool.Data.JHTeacherRecord TRec in TRecs)
                checkStr.Add(TRec.Name + TRec.Nickname, TRec);

            string strName = txtName.Text + txtNickName.Text;

            if (checkStr.ContainsKey(strName))
            {
                if (checkStr[strName].Status == K12.Data.TeacherRecord.TeacherStatus.一般)
                {
                    MsgBox.Show("教師姓名:" + txtName.Text + ",已存在系統內,如果要使用相同姓名請加暱稱.");
                    return;
                }

                // 當刪除狀態，修正刪除教師內的暱稱 與 TeacherID
                if (checkStr[strName].Status == K12.Data.TeacherRecord.TeacherStatus.刪除)
                {
                    JHSchool.Data.JHTeacherRecord delRec = checkStr[strName];
                    delRec.Nickname = delRec.ID;
                    JHSchool.Data.JHTeacher.Update(delRec);
                }
            }

            JHSchool.Data.JHTeacherRecord teacherRec = new JHSchool.Data.JHTeacherRecord();
            teacherRec.Name = txtName.Text;
            teacherRec.Nickname = txtNickName.Text;

            string TeacherID = JHSchool.Data.JHTeacher.Insert(teacherRec);

            Teacher.Instance.SyncDataBackground(TeacherID);

            if (chkInputData.Checked == true)
            {
                if (TeacherID != "")
                {
                    Teacher.Instance.PopupDetailPane(TeacherID);
                    Teacher.Instance.SyncDataBackground(TeacherID);
                }
            }
            PermRecLogProcess prlp = new PermRecLogProcess();
            prlp.SaveLog("學籍.教師", "新增教師", "新增教師,姓名:" + txtName.Text + ",暱稱:" + txtNickName.Text);

            this.Close();
        }
    }
}
