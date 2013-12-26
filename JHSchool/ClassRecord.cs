using System;
using System.Collections.Generic;
using System.Text;
using Framework;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool
{
    /// <summary>
    /// 班級資訊
    /// </summary>
    public class ClassRecord : IComparable<ClassRecord>
    {
        /// <summary>
        /// 取得系統編號
        /// </summary>
        public string ID { get; private set; }
        /// <summary>
        /// 取得班級名稱
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 取得年級
        /// </summary>
        public string GradeYear { get; private set; }
        /// <summary>
        /// 取得命名規則
        /// </summary>
        public string NamingRule { get; private set; }
        internal string RefTeacherID { get; private set; }
        internal string RefDepartmentID { get; private set; }
        public string RefProgramPlanID { get; private set; }
        public string RefScoreCalcRuleID { get; private set; }
        /// <summary>
        /// 取得顯示順序
        /// </summary>
        public string DisplayOrder { get; private set; }
        /// <summary>
        /// 取得班導師
        /// </summary>
        public TeacherRecord Teacher { get { return JHSchool.Teacher.Instance.Items[RefTeacherID]; } }
        /// <summary>
        /// 取得科別
        /// </summary>
        public DepartmentRecord Department { get { return JHSchool.Department.Instance.Items[RefDepartmentID]; } }
        /// <summary>
        /// 取得班級學生
        /// </summary>
        public List<StudentRecord> Students { get { return Student.Instance.GetClassStudents(this); } }

        internal ClassRecord(XmlElement element)
        {
            ID = element.GetAttribute("ID");
            DSXmlHelper helper = new DSXmlHelper(element);
            Name = helper.GetText("ClassName");
            GradeYear = helper.GetText("GradeYear");
            NamingRule = helper.GetText("NamingRule");
            RefTeacherID = helper.GetText("RefTeacherID");
            RefDepartmentID = helper.GetText("RefDepartmentID");
            RefProgramPlanID = helper.GetText("RefGraduationPlanID");
            RefScoreCalcRuleID = helper.GetText("RefScoreCalcRuleID");
            DisplayOrder = helper.GetText("DisplayOrder");
        }
        #region IComparable<ClassRecord> 成員

        public static event EventHandler<CompareClassRecordEventArgs> CompareClassRecord;

        public int CompareTo(ClassRecord other)
        {
            if ( CompareClassRecord != null )
            {
                CompareClassRecordEventArgs args = new CompareClassRecordEventArgs(this, other);
                CompareClassRecord(null, args);
                return args.Result;
            }
            else
            {
                int g1 = int.MinValue, g2 = int.MinValue;
                int.TryParse(this.GradeYear.Trim(), out g1);
                int.TryParse(other.GradeYear.Trim(), out g2);
                if ( g1 == g2 )
                {
                    int order1 = int.MinValue, order2 = int.MinValue;
                    int.TryParse(this.DisplayOrder, out order1);
                    int.TryParse(other.DisplayOrder, out order2);
                    // 加這主要目的讓空白 DisplayOrder 排後
                    if (order1 == 0)
                        order1 = int.MaxValue;

                    if (order2 == 0)
                        order2 = int.MaxValue;

                    if ( order1 == order2 )
                    {
                        return Framework.StringComparer.Comparer(this.Name, other.Name);
                    }
                    else
                        return  order1.CompareTo(order2);
                }
                else
                    return g1.CompareTo(g2);
            }
        }

        #endregion
    }
    public class CompareClassRecordEventArgs : EventArgs
    {
        internal CompareClassRecordEventArgs(ClassRecord v1, ClassRecord v2)
        {
            Value1 = v1;
            Value2 = v2;
        }
        public ClassRecord Value1 { get; private set; }
        public ClassRecord Value2 { get; private set; }
        public int Result { get; set; }
    }
}
