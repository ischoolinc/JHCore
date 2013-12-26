using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DocValidate;
using JHSchool.Legacy.ImportSupport.Validators;
using JHSchool.Legacy.ImportSupport;

namespace JHSchool.CourseExtendControls.Ribbon.CourseImportWizardControls
{
    internal class CourseLookup
    {
        private ConditionKeySetCollection _key_set_list;
        private ImportCondition _condition;

        public CourseLookup(ImportCondition condition)
        {
            List<ImportCondition> conditions = new List<ImportCondition>();
            conditions.Add(condition);
            _condition = condition;

            XmlElement xmlRecords = SmartSchool.Feature.Course.CourseBulkProcess.GetPrimaryKeyList();
            _key_set_list = new ConditionKeySetCollection(conditions, xmlRecords, "Record");
        }

        public string GetCourseID(IRowSource rowSource)
        {
            DuplicateInfo dup = _key_set_list.GetDuplicateBy(_condition, rowSource);

            if (dup != null)
                return dup.Record["CourseID"];
            else
                return string.Empty;
        }

        public static string ExtensionName
        {
            get { return "CourseLookup"; }
        }
    }
}
