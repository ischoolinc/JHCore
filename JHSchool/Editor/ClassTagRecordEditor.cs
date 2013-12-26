using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.Editor
{
    public class ClassTagRecordEditor : GeneralTagRecordEditor
    {
        internal override GeneralTagRecord GetEntityTagRecord()
        {
            return ClassTagRecord;
        }

        internal ClassTagRecord ClassTagRecord { get; private set; }

        public override void Save()
        {
            if (EditorStatus != EditorStatus.NoChanged)
            {
                JHSchool.Feature.EditClass.SaveClassTagRecordEditor(new ClassTagRecordEditor[] { this });
            }
        }

        public ClassTagRecordEditor(ClassTagRecord record)
        {
            ClassTagRecord = record;
            RefEntityID = ClassTagRecord.RefEntityID;
            RefTagID = ClassTagRecord.RefTagID;
        }

        public ClassTagRecordEditor(ClassRecord cla, TagRecord record)
        {
            ClassTagRecord = null;
            RefEntityID = cla.ID;
            RefTagID = record.ID;
        }
    }

    public static class ClassTagRecordEditor_ExtendMethods
    {
        public static ClassTagRecordEditor GetEditor(this ClassTagRecord record)
        {
            return new ClassTagRecordEditor(record);
        }

        public static ClassTagRecordEditor AddTag(this ClassRecord cla, TagRecord record)
        {
            if (record.Category.ToUpper() != TagCategory.Class.ToString().ToUpper())
                throw new ArgumentException("");
            return new ClassTagRecordEditor(cla, record);
        }

        public static void SaveAllEditors(this IEnumerable<ClassTagRecordEditor> editors)
        {
            JHSchool.Feature.EditClass.SaveClassTagRecordEditor(editors);
        }
    }
}
