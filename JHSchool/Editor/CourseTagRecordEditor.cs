using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Editor
{
    public class CourseTagRecordEditor : GeneralTagRecordEditor
    {
        internal override GeneralTagRecord GetEntityTagRecord()
        {
            return CourseTagRecord;
        }

        internal CourseTagRecord CourseTagRecord { get; private set; }

        public override void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
            {
                JHSchool.Feature.EditCourse.SaveCourseTagRecordEditor(new CourseTagRecordEditor[] { this });
            }
        }

        public CourseTagRecordEditor(CourseTagRecord record)
        {
            CourseTagRecord = record;
            RefEntityID = CourseTagRecord.RefEntityID;
            RefTagID = CourseTagRecord.RefTagID;
        }

        public CourseTagRecordEditor(CourseRecord course, TagRecord record)
        {
            CourseTagRecord = null;
            RefEntityID = course.ID;
            RefTagID = record.ID;
        }
    }

    public static class CourseTagRecordEditor_ExtendMethods
    {
        public static CourseTagRecordEditor GetEditor(this CourseTagRecord record)
        {
            return new CourseTagRecordEditor(record);
        }

        public static CourseTagRecordEditor AddTag(this CourseRecord course, TagRecord record)
        {
            if (record.Category.ToUpper() != TagCategory.Course.ToString().ToUpper())
                throw new ArgumentException("");
            return new CourseTagRecordEditor(course, record);
        }

        public static void SaveAllEditors(this IEnumerable<CourseTagRecordEditor> editors)
        {
            JHSchool.Feature.EditCourse.SaveCourseTagRecordEditor(editors);
        }
    }
}
