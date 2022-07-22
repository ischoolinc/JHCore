using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel
{
    public class ElementGenerator
    {
        private string _sourceName, _targetName;
        private string _full_source_name;
        private List<ElementGenerator> _subGenerator;

        public ElementGenerator(XmlElement elmDesc)
        {
            _sourceName = elmDesc.GetAttribute("DisplayText");
            _targetName = elmDesc.GetAttribute("Name");
            _subGenerator = new List<ElementGenerator>();

            if (elmDesc.LocalName != "XmlField")
            {
                _full_source_name = GetFullDisplayText(elmDesc);
                _full_source_name = _full_source_name.Remove(_full_source_name.Length - 1);
            }
            else
                _full_source_name = _sourceName;

            foreach (XmlNode each in elmDesc.ChildNodes)
            {
                if ((each.NodeType == XmlNodeType.Element) && (each.LocalName == "Element" || each.LocalName == "Field"))
                    _subGenerator.Add(new ElementGenerator(each as XmlElement));
            }
        }

        public string SourceName
        {
            get { return _sourceName; }
        }

        public string TargetName
        {
            get { return _targetName; }
        }

        public void Generate(SheetReader reader, XmlElement output)
        {
            List<string> toTrimColumnsList = new List<string> { "姓名", "身分證號", "登入帳號", "電子信箱", "學號" };

            XmlElement newelm = output.OwnerDocument.CreateElement(TargetName);
            output.AppendChild(newelm);

            if (_subGenerator.Count > 0)
            {
                foreach (ElementGenerator each in _subGenerator)
                    each.Generate(reader, newelm);
            }
            else
            {
                if (_full_source_name == "狀態")
                    newelm.InnerText = GetStudStatusCode(reader.GetValue("狀態"));
                else
                {
                    if (toTrimColumnsList.Contains(_full_source_name))
                        newelm.InnerText = reader.GetValue(_full_source_name).Trim();
                    else
                        newelm.InnerText = reader.GetValue(_full_source_name);
                }
            }
        }

        // 轉換學生狀態代碼
        private string GetStudStatusCode(string str)
        {
            // 預設一般狀態
            string retVal = "1";

            if (str == "一般")
                retVal = "1";
            if (str == "休學")
                retVal = "4";
            if (str == "輟學")
                retVal = "8";
            if (str == "畢業或離校")
                retVal = "16";
            if (str == "刪除")
                retVal = "256";
            return retVal;
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
