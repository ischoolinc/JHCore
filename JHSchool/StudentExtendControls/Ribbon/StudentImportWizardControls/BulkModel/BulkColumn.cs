using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel
{
    public class BulkColumn
    {
        private string _display_text, _full_display_text;
        private string _group_name, _name, _prefix;
        private bool _shift_checkable, _identifiable, _read_only;
        private bool _is_group_column, _is_required;

        public static BulkColumn GetNullField(string displayText)
        {
            return new BulkColumn(displayText);
        }

        public BulkColumn(string displayText)
        {
            _display_text = displayText;
        }

        public BulkColumn(XmlElement column)
        {
            _shift_checkable = GetBooleanAttribute(column, "ShiftCheckable", false);
            _identifiable = GetBooleanAttribute(column, "Identifiable", false);
            _read_only = GetBooleanAttribute(column, "ReadOnly", false);
            _is_required = GetBooleanAttribute(column, "Required", false);
            _display_text = column.GetAttribute("DisplayText").Trim();
            _name = column.GetAttribute("Name").Trim();
            _full_display_text = GetFullDisplayText(column);
            _full_display_text = _full_display_text.Remove(_full_display_text.Length - 1);

            if (_full_display_text.IndexOf(":") > 0)
            {
                _is_group_column = true;
                _group_name = GetGroupName(column);

                _prefix = _full_display_text.Split(':')[0]; //取得欄位的 Prefix，就「:」前的東西。

                if (string.IsNullOrEmpty(_group_name))
                    _group_name = _prefix;
            }
            else
            {
                _is_group_column = false;
                _group_name = _full_display_text;
            }
        }

        private string GetGroupName(XmlElement column)
        {
            if (column.ParentNode is XmlElement)
            {
                XmlElement parent = column.ParentNode as XmlElement;
                if (parent.LocalName == "XmlField")
                    return parent.GetAttribute("DisplayText");
                else
                    return GetGroupName(parent);
            }
            else
                return string.Empty;
        }

        public bool IsGroupColumn
        {
            get { return _is_group_column; }
        }

        public string GroupName
        {
            get { return _group_name; }
        }

        /// <summary>
        /// 設定預設為 Display Text
        /// </summary>
        public void SetDefaultData1()
        {
            _full_display_text =_name = _group_name = _display_text;
        }

        public string Prefix
        {
            get { return _prefix; }
        }

        public virtual string DisplayText
        {
            get { return _display_text; }
        }

        public string FullDisplayText
        {
            get { return _full_display_text; }
        }

        public bool ShiftCheckable
        {
            get { return _shift_checkable; }
        }

        public bool Identifiable
        {
            get { return _identifiable; }
        }

        public bool IsRequired
        {
            get { return _is_required; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool ReadOnly
        {
            get { return _read_only; }
        }

        private bool GetBooleanAttribute(XmlElement elm, string name, bool defaultValue)
        {
            string value = elm.GetAttribute(name);

            if (string.IsNullOrEmpty(value))
                return defaultValue;
            else
            {
                bool result;
                if (bool.TryParse(value, out result))
                    return result;
                else
                    throw new XmlParseException("屬性值不正確，只允許True、False。", elm);
            }
        }

        private string GetFullDisplayText(XmlElement column)
        {
            if (column == null)
                return string.Empty;

            if (column.LocalName == "Element" || column.LocalName == "Field")
            {
                string displayText = column.GetAttribute("DisplayText");

                if (string.IsNullOrEmpty(displayText))
                    return (GetFullDisplayText(column.ParentNode as XmlElement)).Trim();
                else
                    return (GetFullDisplayText(column.ParentNode as XmlElement) + column.GetAttribute("DisplayText") + ":").Trim();
            }
            else
                return string.Empty;
        }
    }
}
