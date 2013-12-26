using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Editor
{
    public class StudentTagRecordEditor : GeneralTagRecordEditor
    {
        internal override GeneralTagRecord GetEntityTagRecord()
        {
            return StudentTagRecord;
        }

        internal StudentTagRecord StudentTagRecord { get; private set; }

        public override void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
            {
                JHSchool.Feature.EditStudent.SaveStudentTagRecordEditor(new StudentTagRecordEditor[] { this });
            }
        }

        public StudentTagRecordEditor(StudentTagRecord record)
        {
            StudentTagRecord = record;
            RefEntityID = StudentTagRecord.RefEntityID;
            RefTagID = StudentTagRecord.RefTagID;
        }

        public StudentTagRecordEditor(StudentRecord student, TagRecord record)
        {
            StudentTagRecord = null;
            RefEntityID = student.ID;
            RefTagID = record.ID;
        }
    }

    public static class StudentTagRecordEditor_ExtendMethods
    {
        public static StudentTagRecordEditor GetEditor(this StudentTagRecord record)
        {
            return new StudentTagRecordEditor(record);
        }

        public static StudentTagRecordEditor AddTag(this StudentRecord student, TagRecord record)
        {
            if (record.Category.ToUpper() != TagCategory.Student.ToString().ToUpper())
                throw new ArgumentException("");
            return new StudentTagRecordEditor(student, record);
        }

        public static void SaveAllEditors(this IEnumerable<StudentTagRecordEditor> editors)
        {
            JHSchool.Feature.EditStudent.SaveStudentTagRecordEditor(editors);
        }
    }
}
