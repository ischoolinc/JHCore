using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Feature;

namespace JHSchool.Editor
{
    public class ClassRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; private set; }
        public string Name { get; set; }
        public string GradeYear { get; set; }
        public string NamingRule { get; set; }
        internal string RefTeacherID { get; set; }
        internal string RefDepartmentID { get; set; }
        public string RefProgramPlanID { get; set; }
        public string RefScoreCalcRuleID { get; set; }
        
        public string DisplayOrder { get; set; }
        public TeacherRecord Teacher
        {
            get { return JHSchool.Teacher.Instance.Items[RefTeacherID]; }
            set { RefTeacherID = value.ID; }
        }
        public DepartmentRecord Department
        {
            get { return JHSchool.Department.Instance.Items[RefDepartmentID]; }
            set { RefDepartmentID = value.ID; }
        }
        //public List<StudentRecord> Students { get { return Student.Instance.GetClassStudents(this); } }

        internal ClassRecord Class { get; private set; }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Remove)
                    return EditorStatus.Delete;

                if (Class == null)
                {
                    if (Remove) return EditorStatus.NoChanged;

                    return EditorStatus.Insert;
                }
                else
                {
                    if (Class.Name != Name ||
                        Class.GradeYear != GradeYear ||
                        Class.NamingRule != NamingRule ||
                        Class.RefTeacherID != RefTeacherID ||
                        Class.RefDepartmentID != RefDepartmentID ||
                        Class.RefProgramPlanID != RefProgramPlanID ||
                        Class.RefScoreCalcRuleID != RefScoreCalcRuleID ||
                        Class.DisplayOrder != DisplayOrder)
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        public void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
                EditClass.SaveClassRecordEditor(new ClassRecordEditor[] { this });
        }

        internal ClassRecordEditor(ClassRecord record)
        {
            Remove = false;
            Class = record;

            ID = Class.ID;
            Name = Class.Name;
            NamingRule = Class.NamingRule;
            RefTeacherID = Class.RefTeacherID;
            RefDepartmentID = Class.RefDepartmentID;
            RefProgramPlanID = Class.RefProgramPlanID;
            RefScoreCalcRuleID = Class.RefScoreCalcRuleID;
            DisplayOrder = Class.DisplayOrder;
            GradeYear = Class.GradeYear;
        }

        internal ClassRecordEditor()
        {
            Class = null;
            Remove = false;
        }
    }

    public static class ClassRecordEditor_ExtendFunctions
    {
        public static ClassRecordEditor GetEditor(this ClassRecord record)
        {
            return new ClassRecordEditor(record);
        }

        /// <summary>
        /// 儲存在集合中的所有班級資料。
        /// </summary>
        public static void SaveAllEditors(this IEnumerable<ClassRecordEditor> editors)
        {
            EditClass.SaveClassRecordEditor(editors);
        }

        /// <summary>
        /// 新增班級。
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public static ClassRecordEditor AddClass(this Class cla)
        {
            return new ClassRecordEditor();
        }
    }
}
