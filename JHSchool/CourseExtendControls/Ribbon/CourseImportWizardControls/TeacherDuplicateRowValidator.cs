using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;
using JHSchool.Legacy.ImportSupport;
using JHSchool.Legacy.ImportSupport.Validators;

namespace JHSchool.CourseExtendControls.Ribbon.CourseImportWizardControls
{
    internal class TeacherDuplicateRowValidator : IRowVaildator
    {
        private bool _activate_validator;
        private List<string> _teacher_fields;
        private List<string> _selected_fields;
        private ImportMode _import_mode;
        private ConditionKeySet _course_teachers;
        private ImportDataAccess _data_source;

        public TeacherDuplicateRowValidator(WizardContext context)
        {
            _import_mode = context.CurrentMode;
            _data_source = context.DataSource as ImportDataAccess;
            _course_teachers = new ConditionKeySet(context.IdentifyField);
            _selected_fields = context.SelectedFields;
        }

        #region IRowVaildator 成員

        public void InitFromXMLNode(XmlElement XmlNode)
        {
            _activate_validator = false;
            _teacher_fields = new List<string>();
            foreach (XmlElement each in XmlNode.SelectNodes("ActivatorField"))
            {
                string fieldName = each.InnerText;
                if (_selected_fields.Contains(fieldName))
                {
                    _teacher_fields.Add(fieldName);
                    _activate_validator = true;
                }
            }

            if (!_activate_validator) return;

            if (_import_mode == ImportMode.Update)
            {
                XmlElement records = _data_source.GetCourseTeachers(_course_teachers.Condition.Fields.ToInternalNames());
                foreach (XmlElement each in records.SelectNodes("Record"))
                {
                    Record record = new Record(each);
                    _course_teachers.AddRecord(record);
                }
            }
        }

        public void InitFromXMLString(string XmlString)
        {
            throw new Exception("The method or operation is not implemented.(InitFromXMLString)");
        }

        public string Correct(IRowSource Value)
        {
            throw new Exception("The method or operation is not implemented.(Correct)");
        }

        public string KeyField()
        {
            return "<XmlContent>";
        }

        public string ToString(string Description)
        {
            DSXmlHelper result = new DSXmlHelper("Fields");

            foreach (string each in _teacher_fields)
            {
                XmlElement elmField = result.AddElement("Field");
                elmField.SetAttribute("Name", each);
                elmField.SetAttribute("Description", "一位教師不可擔任單一課程二次(含)以上授課教師。");
            }

            return result.GetRawXml();
        }

        public bool Validate(IRowSource Value)
        {
            if (!_activate_validator) return true;

            if (_import_mode == ImportMode.Insert)
            {
                return IsValid(_teacher_fields, Value);
            }
            else
            {
                if (_course_teachers.Contains(Value))
                    return IsValid(_teacher_fields, _course_teachers.GetRecord(Value), Value);
                else
                    return IsValid(_teacher_fields, Value);
            }
        }

        private static bool IsValid(List<string> fields, IRowSource rowSource)
        {
            Dictionary<string, string> dup = new Dictionary<string, string>();
            foreach (string each in fields)
            {
                var value = rowSource.GetFieldData(each)?.Trim() ?? "";
                if (dup.ContainsKey(value))
                    return false;

                dup.Add(value, each);
            }

            return true;
        }

        private static Dictionary<string, string> _field_map;
        static TeacherDuplicateRowValidator()
        {
            _field_map = new Dictionary<string, string>();
            _field_map.Add("授課教師一", "Teacher1Name");
            _field_map.Add("授課教師二", "Teacher2Name");
            _field_map.Add("授課教師三", "Teacher3Name");
        }

        private static bool IsValid(List<string> fields, Record record, IRowSource rowSource)
        {
            Dictionary<string, string> teachers = new Dictionary<string, string>();
            teachers.Add("授課教師一", string.Empty);
            teachers.Add("授課教師二", string.Empty);
            teachers.Add("授課教師三", string.Empty);

            foreach (string each in new string[] { "授課教師一", "授課教師二", "授課教師三" })
                teachers[each] = record[_field_map[each]];

            foreach (string each in fields)
                teachers[each] = rowSource.GetFieldData(each);

            Dictionary<string, string> dup = new Dictionary<string, string>();
            foreach (string each in teachers.Values)
            {
                if (dup.ContainsKey(each))
                    return false;

                if (!string.IsNullOrEmpty(each))
                    dup.Add(each, each);
            }

            return true;
        }
        #endregion
    }
}