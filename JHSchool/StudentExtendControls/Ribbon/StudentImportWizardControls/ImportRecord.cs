using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls
{
    public class ImportRecord
    {
        private string _identity;
        private int _absolute_row_index = -1;
        private int _relatively_row_index = -1;
        private string _id_number;
        private string _student_number;
        private string _login_name;
        // 學生狀態
        private string _Status;

        public ImportRecord(JHSchool.Data.JHStudentRecord record )  //XmlElement record)
        {
            //_identity = record.GetAttribute("ID");
            //_id_number = record.GetAttribute("IDNumber");
            //_student_number = record.GetAttribute("StudentNumber");
            //_login_name = record.GetAttribute("SALoginName");

            // 學生編號
            _identity = record.ID;
            // 身分證號
            _id_number = record.IDNumber;
            // 學號
            _student_number = record.StudentNumber;
            // 登入帳號
            _login_name = record.SALoginName;

            // 學生狀態
            _Status = record.Status.ToString();

        }

        public string Identity
        {
            get { return _identity; }
        }

        public string IDNumber
        {
            get { return _id_number; }
            set { _id_number = value; }
        }

        public string StudentNumber
        {
            get { return _student_number; }
            set { _student_number = value; }
        }

        /// <summary>
        /// 讀取學生狀態
        /// </summary>
        public string Status
        {
            get { return _Status; }  
        }

        public string SALoginName
        {
            get { return _login_name; }
            set { _login_name = value; }
        }

        public int AbsoluteRowIndex
        {
            get { return _absolute_row_index; }
            set { _absolute_row_index = value; }
        }

        public int RelativelyRowIndex
        {
            get { return _relatively_row_index; }
            set { _relatively_row_index = value; }
        }
    }
}
