using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using DocValidate;

namespace JHSchool.Legacy.ImportSupport
{
    public class ImportCondition
    {
        public ImportCondition(string name, string uniqueGroup, bool emptySkipValidate)
        {
            _fields = new ImportFieldCollection();
            _name = name;
            _unique_group = uniqueGroup;
            _empty_skip_validate = emptySkipValidate;
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _unique_group;
        public string UniqueGroup
        {
            get { return _unique_group; }
        }

        private bool _empty_skip_validate;
        public bool EmptySkipValidate
        {
            get { return _empty_skip_validate; }
        }

        private ImportFieldCollection _fields;
        /// <summary>
        /// 此條件中所包含的欄位。
        /// </summary>
        public ImportFieldCollection Fields
        {
            get { return _fields; }
        }

        public bool ContainsAnyField(params string[] fields)
        {
            foreach (string each in fields)
            {
                if (Fields.Contains(each))
                    return true;
            }

            return false;
        }

        public bool ContainsAllField(params string[] fields)
        {
            List<string> allfields = new List<string>(fields);

            foreach (ImportField each in Fields)
            {
                if (!allfields.Contains(each.FieldName))
                    return false;
            }
            return true;
        }

        public bool ContainsAllField(IEnumerable<string> fields)
        {
            List<string> allfields = new List<string>(fields);

            foreach (ImportField each in Fields)
            {
                if (!allfields.Contains(each.FieldName))
                    return false;
            }
            return true;
        }

        internal string GetKeyByInternalName(Dictionary<string, string> values)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ImportField each in Fields)
                builder.Append(values[each.InternalName] + ":");

            return builder.ToString();
        }

        /// <summary>
        /// 判斷產生出來的 Key 是否為 Empty。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        internal bool IsEmptyKey(Dictionary<string, string> values)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ImportField each in Fields)
                builder.Append(values[each.InternalName]);

            return string.IsNullOrEmpty(builder.ToString().Trim());
        }

        internal string GetKeyByFieldName(IRowSource rowSource)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ImportField each in Fields)
                builder.Append(rowSource.GetFieldData(each.FieldName) + ":");

            return builder.ToString();

        }

        internal bool IsEmptyKey(IRowSource rowSource)
        {
            StringBuilder builder = new StringBuilder();
            foreach (ImportField each in Fields)
                builder.Append(rowSource.GetFieldData(each.FieldName));

            return string.IsNullOrEmpty(builder.ToString().Trim());
        }

        internal string GetCombineFieldName()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ImportField each in Fields)
            {
                builder.Append(each.FieldName);
                builder.Append("+");
            }
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        public static List<ImportCondition> CreateConditionFromXml(XmlElement fieldData, ImportFieldCollection supportFields)
        {
            List<ImportCondition> conditions = new List<ImportCondition>();
            foreach (XmlElement each in fieldData.SelectNodes("UpdateCondition/Condition"))
            {
                string name = each.GetAttribute("Name");
                string uniqueGroup = each.GetAttribute("UniqueGroup");
                bool emptySkip;

                if (!bool.TryParse(each.GetAttribute("EmptySkipValidate"), out emptySkip))
                    emptySkip = false;

                ImportCondition condition = new ImportCondition(name, uniqueGroup, emptySkip);
                condition._fields = supportFields.GetByUniqueGroup(uniqueGroup);
                conditions.Add(condition);
            }

            return conditions;
        }
    }
}
