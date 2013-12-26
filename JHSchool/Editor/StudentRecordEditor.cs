using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using FISCA.DSAUtil;

namespace JHSchool.Editor
{
    /// <summary>
    /// 編輯學生資料
    /// </summary>
    public class StudentRecordEditor
    {
        /// <summary>
        /// 取得學生系統編號
        /// </summary>
        public string ID { get { return Student == null ? "" : Student.ID; } }
        /// <summary>
        /// 取得或設定學生狀態
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 取得或設定學生座號
        /// </summary>
        public string SeatNo { get; set; }
        /// <summary>
        /// 取得或設定學生姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 取得或設定學號
        /// </summary>
        public string StudentNumber { get; set; }
        /// <summary>
        /// 取得或設定性別
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 取得或設定身分證號
        /// </summary>
        public string IDNumber { get; set; }
        /// <summary>
        /// 取得或設定生日
        /// </summary>
        public string Birthday { get; set; }
        /// <summary>
        /// 取得或設定國籍
        /// </summary>
        public string Nationality { get; set; }
        //internal string OverrideDepartmentID { get; private set; }
        /// <summary>
        /// 取得或設定覆蓋後的課程規劃表編號
        /// </summary>
        public string OverrideProgramPlanID { get; set; }
        /// <summary>
        /// 取得或設定覆蓋後的成績計算規則編號
        /// </summary>
        public string OverrideScoreCalcRuleID { get; set; }
        internal string RefClassID { get; private set; }
        /// <summary>
        /// 取得或設定學生所屬班級
        /// </summary>
        public ClassRecord Class { get { return JHSchool.Class.Instance.Items[RefClassID]; } set { RefClassID = (value == null ? "" : value.ID); } }
        ///// <summary>
        ///// 取得或設定學生所屬科別，變更此值將會同步變更OverriddenDepartment為true
        ///// </summary>
        //public DepartmentRecord Department
        //{
        //    get { return OverrideDepartmentID == null ? (Class == null ? null : Class.Department) : JHSchool.Department.Instance[OverrideDepartmentID]; }
        //    set { OverrideDepartmentID = (value == null ? "" : value.ID); }
        //}
        ///// <summary>
        ///// 取得或設定，指出是否覆蓋學生所屬科別
        ///// </summary>
        //public bool OverriddenDepartment
        //{
        //    get { return OverrideDepartmentID == null; }
        //    set
        //    {
        //        if (value) OverrideDepartmentID = (Department != null ? Department.ID : "");
        //        else OverrideDepartmentID = null;
        //    }
        //}
        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Student == null)
                {
                    return EditorStatus.Insert;
                }
                else
                {
                    if (Student.Birthday != Birthday ||
                        Student.RefClassID != RefClassID ||
                        //Student.OverrideDepartmentID != OverrideDepartmentID ||
                        Student.Gender != Gender ||
                        Student.IDNumber != IDNumber ||
                        Student.Name != Name ||
                        Student.SeatNo != SeatNo ||
                        Student.Status != Status ||
                        Student.StudentNumber != StudentNumber ||
                        Student.OverrideProgramPlanID != OverrideProgramPlanID ||
                        Student.OverrideScoreCalcRuleID != OverrideScoreCalcRuleID ||
                        Student.Nationality != Nationality)
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }
        internal StudentRecord Student
        {
            get;
            private set;
        }
        internal StudentRecordEditor(StudentRecord student)
        {
            Student = student;
            Status = student.Status;
            SeatNo = student.SeatNo;
            Name = student.Name;
            StudentNumber = student.StudentNumber;
            Gender = student.Gender;
            IDNumber = student.IDNumber;
            Birthday = student.Birthday;
            //OverrideDepartmentID = student.OverrideDepartmentID;
            RefClassID = student.RefClassID;
            OverrideProgramPlanID = student.OverrideProgramPlanID;
            OverrideScoreCalcRuleID = student.OverrideScoreCalcRuleID;
            Nationality = student.Nationality;
        }
        
        internal StudentRecordEditor()
        {
            Student = null;
            //OverrideDepartmentID = null;
            OverrideProgramPlanID = null;
            OverrideScoreCalcRuleID = null;
        }
        public void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
                Feature.EditStudent.SaveStudentRecordEditor(new StudentRecordEditor[] { this });
        }
    }
    public static class StudentRecordEditor_ExtendFunctions
    {
        public static StudentRecordEditor GetEditor(this StudentRecord studentRecord)
        {
            return new StudentRecordEditor(studentRecord);
        }
        public static void SaveAllEditors(this IEnumerable<StudentRecordEditor> editors)
        {
            Feature.EditStudent.SaveStudentRecordEditor(editors);
        }
        public static StudentRecordEditor AddStudent(this Student instance)
        {
            return new StudentRecordEditor();
        }
    }
}
