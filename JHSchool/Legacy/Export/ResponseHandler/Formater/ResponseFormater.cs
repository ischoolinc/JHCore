using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler;
using System.Xml;
using JHSchool.Legacy.Export.ResponseHandler.Converter;
using JHSchool.Legacy.Export.ResponseHandler.DataType;

namespace JHSchool.Legacy.Export.ResponseHandler.Formater
{
    public class ResponseFormater : IResponseFormater
    {

        #region IResponseFormater жин√

        public ExportFieldCollection Format(XmlElement source)
        {
            ExportFieldCollection _collection = new ExportFieldCollection();
            foreach (XmlNode node in source.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;
                XmlElement element = (XmlElement)node;
                if (element.Name == "Field")
                {
                    string name = element.GetAttribute("Name");
                    string displaytext = element.GetAttribute("DisplayText");
                    string converter = element.GetAttribute("Converter");
                    string dataType = element.GetAttribute("DataType");

                    ExportField field = new ExportField();
                    field.FieldName = name;
                    field.DisplayText = displaytext;
                    field.XPath = name;
                    field.RequestName = name;
                    field.Converter = converter;
                    field.DataType = dataType;

                    _collection.Add(field);
                }
                else if (element.Name == "XmlField")
                {
                    XmlNode baseNode = element.SelectSingleNode("Element");
                    if (baseNode == null) continue;

                    string name = element.GetAttribute("Name");
                    //string displaytext = element.GetAttribute("DisplayText");

                    //ExportField field = new ExportField();
                    //field.FieldName = name;
                    //field.DisplayText = displaytext;
                    XmlElement baseElement = (XmlElement)baseNode;
                    List<ExportField> fields = GetFields(baseElement, "", name, name, 1);
                    foreach (ExportField f in fields)
                        _collection.Add(f);
                    //foreach (XmlNode recNode in baseElement.SelectNodes("Element"))
                    //{
                    //    XmlElement recElement = (XmlElement)recNode;

                    //    int recIndex = 1;
                    //    foreach (XmlElement fieldNode in recElement.SelectNodes("Field"))
                    //    {
                    //        XmlElement fieldElement = (XmlElement)fieldNode;
                    //        ExportField field = new ExportField();
                    //        string baseDisplay = baseElement.GetAttribute("DisplayText");
                    //        string recDisplay = recElement.GetAttribute("DisplayText");
                    //        string fieldDisplay = fieldElement.GetAttribute("DisplayText");
                    //        string display = baseDisplay + ":" + recDisplay + (string.IsNullOrEmpty(recDisplay) ? "" : ":") + fieldDisplay;
                    //        field.DisplayText = display;

                    //        string baseName = baseElement.GetAttribute("Name");
                    //        string recName = recElement.GetAttribute("Name");
                    //        string fieldName = fieldElement.GetAttribute("Name");
                    //        string xpath = baseName + "/" + recName + "[" + recIndex + "]/" + fieldName;
                    //        field.XPath = xpath;

                    //        field.FieldName = element.GetAttribute("Name");
                    //        recIndex++;
                    //    }
                    //}
                }
            }
            return _collection;
        }

        #endregion

        private List<ExportField> GetFields(XmlElement element, string currentDisplay, string currentXPath, string requestName, int currentRecordIndex)
        {
            List<ExportField> fields = new List<ExportField>();
            string m_name = element.GetAttribute("Name");
            string m_display = element.GetAttribute("DisplayText");
            string appendString = "";
            if (!currentDisplay.EndsWith(":") && currentDisplay != "")
                appendString = ":";

            m_display = currentDisplay + appendString + m_display;
            if (element.SelectNodes("Field").Count == 0)
            {
                int i = 1;
                foreach (XmlElement childElement in element.SelectNodes("Element"))
                {
                    List<ExportField> childFields = GetFields(childElement, m_display, currentXPath + "/" + m_name + "[" + currentRecordIndex + "]", requestName, i);
                    foreach (ExportField cf in childFields)
                    {
                        fields.Add(cf);
                    }
                }
            }


            foreach (XmlElement fieldNode in element.SelectNodes("Field"))
            {
                XmlElement fieldElement = (XmlElement)fieldNode;
                ExportField field = new ExportField();

                string fieldDisplay = fieldElement.GetAttribute("DisplayText");
                string converter = fieldElement.GetAttribute("Converter");
                string dataType = fieldElement.GetAttribute("DataType");

                string astring = "";
                if (!m_display.EndsWith(":") && m_display != "")
                    astring = ":";

                string display = m_display + astring + fieldDisplay;
                field.DisplayText = display;

                string fieldName = fieldElement.GetAttribute("Name");
                int fieldNameCount = 1;
                foreach (ExportField f in fields)
                {
                    if (f.FieldName == fieldName)
                        fieldNameCount++;
                }
                string xpath = currentXPath + "/" + m_name + "[" + currentRecordIndex + "]/" + fieldName + "[" + fieldNameCount + "]";
                field.XPath = xpath;

                field.FieldName = fieldName;
                field.RequestName = requestName;
                field.Converter = converter;
                field.DataType = dataType;

                fields.Add(field);
            }
            return fields;
        }
    }
}
