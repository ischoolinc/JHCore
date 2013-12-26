using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Editor
{
    public class SemesterHistoryRecordEditor
    {
        public bool Remove { get; set; }
        internal string RefStudentID { get; set; }
        public int SchoolYear { get; set; }
        public int Semester { get; set; }
        public int GradeYear { get; set; }
        public string ClassName { get; set; }
        public string SeatNo { get; set; }
        public string Teacher { get; set; }
        public string SchoolDayCount { get; set; }
        public StudentRecord Student
        {
            get { return JHSchool.Student.Instance[RefStudentID]; }
        }

        public EditorStatus EditorStatus
        {
            get
            {
                if ( SemesterHistoryRecord == null )
                {
                    if ( !Remove )
                        return EditorStatus.Insert;
                    else
                        return EditorStatus.NoChanged;
                }
                else
                {
                    if ( Remove )
                        return EditorStatus.Delete;
                    if ( SemesterHistoryRecord.ClassName!=ClassName ||
                        SemesterHistoryRecord.GradeYear!=GradeYear||
                        SemesterHistoryRecord.SchoolYear!= SchoolYear||
                        SemesterHistoryRecord.SeatNo!=SeatNo ||
                        SemesterHistoryRecord.Semester!=Semester ||
                        SemesterHistoryRecord.Teacher !=Teacher ||
                        SemesterHistoryRecord.SchoolDayCount != SchoolDayCount
                        )
                    {
                        return EditorStatus.Update;
                    }
                }
                return EditorStatus.NoChanged;
            }
        }

        public void Save()
        {
            if ( EditorStatus != EditorStatus.NoChanged )
                Feature.EditStudent.SaveSemesterHistoryRecordEditor(new SemesterHistoryRecordEditor[] { this });
        }

        internal SemesterHistoryRecord SemesterHistoryRecord { get; set; }
        internal SemesterHistoryRecordEditor(SemesterHistoryRecord info)
        {
            SemesterHistoryRecord = info;
            ClassName = info.ClassName;
            GradeYear = info.GradeYear;
            SchoolYear = info.SchoolYear;
            SeatNo = info.SeatNo;
            Semester = info.Semester;
            RefStudentID = info.RefStudentID;
            Teacher = info.Teacher;
            SchoolDayCount = info.SchoolDayCount;
        }
        internal SemesterHistoryRecordEditor(StudentRecord student)
        {
            RefStudentID = student.ID;
        }
    }
    public static class SemesterHistoryRecordEditor_ExtendFunctions
    {
        public static SemesterHistoryRecordEditor GetEditor(this SemesterHistoryRecord semesterHistoryRecord)
        {
            return new SemesterHistoryRecordEditor(semesterHistoryRecord);
        }
        public static void SaveAllEditors(this IEnumerable<SemesterHistoryRecordEditor> editors)
        {
            Feature.EditStudent.SaveSemesterHistoryRecordEditor(editors);
        }
        public static SemesterHistoryRecordEditor AddSemesterHistory(this StudentRecord studentRec)
        {
            return new SemesterHistoryRecordEditor(studentRec);
        }
    }
}
