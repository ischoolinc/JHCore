using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;

namespace JHSchool.Legacy.ImportSupport
{
    public class ImportField
    {
        /// <summary>
        /// 用於 Null。
        /// </summary>
        /// <param name="fieldName"></param>
        public ImportField(string fieldName)
        {
            _field_name = fieldName;
        }

        public ImportField(XmlElement fieldData)
        {
            _field_name = fieldData.GetAttribute("Name");
            _internal_name = fieldData.GetAttribute("InternalName");
            _is_primary_key = ParseBoolean(fieldData.GetAttribute("PrimaryKey"), false);
            _shift_checkable = ParseBoolean(fieldData.GetAttribute("ShiftCheckable"), false);
            _is_insert_required = ParseBoolean(fieldData.GetAttribute("InsertRequired"), false);
            _unique_group = fieldData.GetAttribute("UniqueGroup");
            _basic_field = ParseBoolean(fieldData.GetAttribute("BasicField"), true);
        }

        private bool ParseBoolean(string boolString, bool defaultValue)
        {
            bool result;

            if (bool.TryParse(boolString, out result))
                return result;
            else
                return defaultValue;
        }

        private string _field_name;
        /// <summary>
        /// 顯示於 Excel 或是畫面上用的名稱(不可重複)。
        /// </summary>
        public string FieldName
        {
            get { return _field_name; }
        }

        /// <summary>
        /// 用於程式處理的名稱(例如用於 DBHelper)。
        /// </summary>
        private string _internal_name;
        public string InternalName
        {
            get { return _internal_name; }
        }

        private bool _is_primary_key;
        public bool PrimaryKey
        {
            get { return _is_primary_key; }
        }

        private bool _shift_checkable;
        public bool ShiftCheckable
        {
            get { return _shift_checkable; }
        }

        private bool _is_insert_required;
        public bool InsertRequired
        {
            get { return _is_insert_required; }
        }

        private string _unique_group;
        /// <summary>
        /// 取得欄位的 Unique 名稱。
        /// </summary>
        public string UniqueGroup
        {
            get { return _unique_group; }
        }

        /// <summary>
        /// 取得欄位是否為 Unique。
        /// </summary>
        public bool IsUniqueField
        {
            get { return UniqueGroup != string.Empty; }
        }

        private bool _basic_field;
        public bool IsBasicField
        {
            get { return _basic_field; }
        }

    }

    public class ImportFieldCollection : IEnumerable<ImportField>
    {
        private Dictionary<string, ImportField> _fields;

        public ImportFieldCollection()
        {
            _fields = new Dictionary<string, ImportField>();
        }

        public void AddField(ImportField field)
        {
            _fields.Add(field.FieldName, field);
        }

        public bool Contains(string fieldName)
        {
            return _fields.ContainsKey(fieldName);
        }

        public ImportField this[string fieldName]
        {
            get
            {
                return _fields[fieldName];
            }
        }

        /// <summary>
        /// 回傳與特定欄位的交集。
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public ImportFieldCollection Intersect(IEnumerable<string> fields)
        {
            ImportFieldCollection objFields = new ImportFieldCollection();
            foreach (string each in fields)
            {
                if (Contains(each))
                    objFields.AddField(this[each]);
            }
            return objFields;
        }

        public ImportFieldCollection GetByNamePrefix(string prefix)
        {
            ImportFieldCollection fields = new ImportFieldCollection();
            foreach (ImportField each in this)
            {
                if (each.FieldName.StartsWith(prefix))
                    fields.AddField(each);
            }

            return fields;
        }

        public ImportFieldCollection GetByUniqueGroup(string name)
        {
            ImportFieldCollection fields = new ImportFieldCollection();
            foreach (ImportField each in this)
            {
                if (each.UniqueGroup == name)
                    fields.AddField(each);
            }

            return fields;
        }

        public ImportFieldCollection GetByNameList(params string[] names)
        {
            ImportFieldCollection fields = new ImportFieldCollection();
            foreach (string each in names)
            {
                if (Contains(each))
                    fields.AddField(this[each]);
            }

            return fields;
        }

        /// <summary>
        /// 轉換成可列舉的名稱清單。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> ToNames()
        {
            return _fields.Keys;
        }

        public IEnumerable<string> ToInternalNames()
        {
            foreach (ImportField each in this)
            {
                yield return each.InternalName;
            }
        }

        public static ImportFieldCollection CreateFieldsFromXml(XmlElement fieldData)
        {
            ImportFieldCollection fields = new ImportFieldCollection();
            foreach (XmlElement each in fieldData.SelectNodes("Field"))
                fields.AddField(new ImportField(each));

            return fields;
        }

        #region IEnumerable<FieldDescription> 成員

        public IEnumerator<ImportField> GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _fields.Values.GetEnumerator();
        }

        #endregion
    }
}
