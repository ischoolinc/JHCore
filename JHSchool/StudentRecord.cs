using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool
{
    /// <summary>
    /// 學生資訊
    /// </summary>
    public class StudentRecord : IComparable<StudentRecord>
    {
        /// <summary>
        /// 取得系統編號
        /// </summary>
        public string ID { get; internal set; }
        /// <summary>
        /// 取得學生狀態
        /// </summary>
        public string Status { get; private set; }
        /// <summary>
        /// 取得座號
        /// </summary>
        public string SeatNo { get; private set; }
        /// <summary>
        /// 取得學生姓名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 取得學號
        /// </summary>
        public string StudentNumber { get; private set; }
        /// <summary>
        /// 取得性別
        /// </summary>
        public string Gender { get; private set; }
        /// <summary>
        /// 取得身分證號
        /// </summary>
        public string IDNumber { get; private set; }
        /// <summary>
        /// 取得生日
        /// </summary>
        public string Birthday { get; private set; }
        /// <summary>
        /// 取得國籍
        /// </summary>
        public string Nationality { get; private set; }
        /// <summary>
        /// 取得覆蓋後的課程規劃表編號
        /// </summary>
        public string OverrideProgramPlanID { get; private set; }
        /// <summary>
        /// 取得覆蓋後的成績計算規則編號
        /// </summary>
        public string OverrideScoreCalcRuleID { get; private set; }
        /// <summary>
        /// 取得所屬班級的班級編號
        /// </summary>
        internal string RefClassID { get; private set; }
        /// <summary>
        /// 取得所屬班級
        /// </summary>
        public ClassRecord Class
        {
            get
            {
                return JHSchool.Class.Instance.Items[RefClassID];
            }
        }

        internal StudentRecord(JHSchool.Data.JHStudentRecord student)
        {
            ID = student.ID;
            Status = student.Status.ToString();
            SeatNo = "" + student.SeatNo;
            Name = student.Name;
            StudentNumber = student.StudentNumber;
            Gender = student.Gender;
            IDNumber = student.IDNumber;
            Birthday = student.Birthday == null ? "" : student.Birthday.Value.ToString("yyyy/MM/dd");
            OverrideProgramPlanID = student.OverrideProgramPlanID;
            OverrideScoreCalcRuleID = student.OverrideScoreCalcRuleID;
            RefClassID = student.RefClassID;
            Nationality = student.Nationality;
        }

        internal StudentRecord(XmlElement element)
        {
            DSXmlHelper helper = new DSXmlHelper(element);
            ID = helper.GetText("@ID");
            Status = helper.GetText("Status");
            SeatNo = helper.GetText("SeatNo");
            Name = helper.GetText("Name");
            StudentNumber = helper.GetText("StudentNumber");
            Gender = helper.GetText("Gender");
            IDNumber = helper.GetText("IDNumber");
            Birthday = helper.GetText("Birthdate");
            //OverrideDepartmentID = helper.GetText("OverrideDeptID");
            //if (OverrideDepartmentID == "") OverrideDepartmentID = null;
            OverrideProgramPlanID = helper.GetText("RefGraduationPlanID");
            if (OverrideProgramPlanID == "") OverrideProgramPlanID = null;
            OverrideScoreCalcRuleID = helper.GetText("RefScoreCalcRuleID");
            if (OverrideScoreCalcRuleID == "") OverrideScoreCalcRuleID = null;
            RefClassID = helper.GetText("RefClassID");
            Nationality = helper.GetText("Nationality");
        }

        #region IComparable<StudentRecord> 成員

        public static event EventHandler<CompareStudentRecordEventArgs> CompareStudentRecord;

        public int CompareTo(StudentRecord other)
        {
            if (CompareStudentRecord != null)
            {
                CompareStudentRecordEventArgs args = new CompareStudentRecordEventArgs(this, other);
                CompareStudentRecord(null, args);
                return args.Result;
            }
            else
            {
                ClassRecord c1 = this.Class;
                ClassRecord c2 = other.Class;
                if (c1 == c2)
                {
                    int seatNo1 = int.MinValue, seatNo2 = int.MinValue;
                    int.TryParse(this.SeatNo, out seatNo1);
                    int.TryParse(other.SeatNo, out seatNo2);
                    if (seatNo1 == seatNo2)
                        return this.StudentNumber.CompareTo(other.StudentNumber);
                    else
                        return seatNo1.CompareTo(seatNo2);
                }
                else
                {
                    if (c1 == null)
                        return int.MinValue;
                    else if (c2 == null)
                        return int.MaxValue;
                    return c1.CompareTo(c2);
                }
            }
        }

        #endregion
    }
    public class CompareStudentRecordEventArgs : EventArgs
    {
        internal CompareStudentRecordEventArgs(StudentRecord v1, StudentRecord v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public StudentRecord Value1 { get; private set; }
        public StudentRecord Value2 { get; private set; }
        public int Result { get; set; }
    }
}
