using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Feature;

namespace JHSchool.Editor
{
    public class CourseRecordEditor
    {
        public bool Remove { get; set; }
        public string ID { get; private set; }
        public string Name { get; set; }
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public string Subject { get; set; }
        public string Domain { get; set; }
        public string Period { get; set; }
        public string Credit { get; set; }
        internal string RefClassID { get; set; }
        public string RefAssessmentSetupID { get; set; }
        public string CalculationFlag { get; set; }
        internal CourseRecord Course { get; private set; }

        public ClassRecord Class { get { return JHSchool.Class.Instance[RefClassID]; } set { RefClassID = (value == null ? "" : value.ID); } }

        /// <summary>
        /// 取得修改狀態
        /// </summary>
        public EditorStatus EditorStatus
        {
            get
            {
                if (Remove)
                    return EditorStatus.Delete;

                if (Course == null)
                {
                    if (Remove) return EditorStatus.NoChanged;

                    return EditorStatus.Insert;
                }
                else
                {
                    if (Course.Name != Name ||
                        Course.SchoolYear != SchoolYear ||
                        Course.Semester != Semester ||
                        Course.Subject != Subject ||
                        Course.Domain != Domain ||
                        Course.Period != Period ||
                        Course.Credit != Credit ||
                        Course.RefClassID != RefClassID ||
                        Course.RefAssessmentSetupID != RefAssessmentSetupID ||
                        Course.CalculationFlag != CalculationFlag)
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
                EditCourse.SaveCourseRecordEditor(new CourseRecordEditor[] { this });
        }

        internal CourseRecordEditor(CourseRecord record)
        {
            Remove = false;
            Course = record;
            ID = Course.ID;
            Name = Course.Name;
            SchoolYear = Course.SchoolYear;
            Semester = Course.Semester;
            Subject = Course.Subject;
            Domain = Course.Domain;
            Period = Course.Period;
            Credit = Course.Credit;
            RefClassID = Course.RefClassID;
            RefAssessmentSetupID = Course.RefAssessmentSetupID;
            CalculationFlag = Course.CalculationFlag;
        }

        internal CourseRecordEditor()
        {
            Course = null;
            Remove = false;
        }
    }

    public static class CourseRecordEditor_ExtendFunctions
    {
        public static CourseRecordEditor GetEditor(this CourseRecord record)
        {
            return new CourseRecordEditor(record);
        }

        /// <summary>
        /// 儲存在集合中的所有課程資料。
        /// </summary>
        public static void SaveAllEditors(this IEnumerable<CourseRecordEditor> editors)
        {
            EditCourse.SaveCourseRecordEditor(editors);
        }

        /// <summary>
        /// 新增課程。
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public static CourseRecordEditor AddCourse(this Course course)
        {
            return new CourseRecordEditor();
        }
    }
}
