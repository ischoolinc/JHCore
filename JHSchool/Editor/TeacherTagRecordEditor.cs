using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Editor
{
    public class TeacherTagRecordEditor : GeneralTagRecordEditor
    {
        internal override GeneralTagRecord GetEntityTagRecord()
        {
            return TeacherTagRecord;
        }

        internal TeacherTagRecord TeacherTagRecord { get; private set; }

        public override void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
            {
                JHSchool.Feature.EditTeacher.SaveTeacherTagRecordEditor(new TeacherTagRecordEditor[] { this });
            }
        }

        public TeacherTagRecordEditor(TeacherTagRecord record)
        {
            TeacherTagRecord = record;
            RefEntityID = TeacherTagRecord.RefEntityID;
            RefTagID = TeacherTagRecord.RefTagID;
        }

        public TeacherTagRecordEditor(TeacherRecord teacher, TagRecord record)
        {
            TeacherTagRecord = null;
            RefEntityID = teacher.ID;
            RefTagID = record.ID;
        }
    }

    public static class TeacherTagRecordEditor_ExtendMethods
    {
        public static TeacherTagRecordEditor GetEditor(this TeacherTagRecord record)
        {
            return new TeacherTagRecordEditor(record);
        }

        public static TeacherTagRecordEditor AddTag(this TeacherRecord teacher, TagRecord record)
        {
            if (record.Category.ToUpper() != TagCategory.Teacher.ToString().ToUpper())
                throw new ArgumentException("");
            return new TeacherTagRecordEditor(teacher, record);
        }

        public static void SaveAllEditors(this IEnumerable<TeacherTagRecordEditor> editors)
        {
            JHSchool.Feature.EditTeacher.SaveTeacherTagRecordEditor(editors);
        }
    }
}
