using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel
{
    /// <summary>
    /// 代表匯入匯出的完整描述資訊。
    /// </summary>
    public class BulkDescription
    {
        private XmlElement _description;
        private BulkColumnCollection _columns;
        private List<ElementGenerator> _generators;

        public BulkDescription(XmlElement desc)
        {
            _description = desc;

            _columns = new BulkColumnCollection();
            foreach (XmlElement each in desc.SelectNodes("//Field"))
            {
                BulkColumn column = new BulkColumn(each);

                try
                {
                    _columns.Add(column.FullDisplayText, column);
                }
                catch (ArgumentException ex)
                {
                    throw new XmlParseException("載入欄位資訊錯誤，可能是欄位名稱重覆。(" + column.FullDisplayText + ")", each, ex);
                }
            }

            _generators = new List<ElementGenerator>();
            foreach (XmlNode each in desc.ChildNodes)
            {
                if (each.NodeType == XmlNodeType.Element)
                {
                    _generators.Add(new ElementGenerator(each as XmlElement));
                }
            }
        }

        public BulkColumnCollection Columns
        {
            get { return _columns; }
        }

        public void GenerateInsertRequest(SheetReader reader, BulkColumnCollection acceptColumns, XmlElement output)
        {
            foreach (ElementGenerator each in _generators)
            {
                if (acceptColumns.ContainGroup(each.SourceName))
                {
                    if (each.SourceName == "畢結業證書字號")
                    {
                        XmlElement dn = output.OwnerDocument.CreateElement("DiplomaNumber");
                        XmlElement dnc = output.OwnerDocument.CreateElement("DiplomaNumber");
                        output.AppendChild(dn);
                        dn.AppendChild(dnc);

                        dnc.InnerText = reader.GetValue("畢結業證書字號");
                    }
                    else
                        each.Generate(reader, output);
                }
            }

            //if (acceptColumns.ContainsKey("新生:異動代號") &&
            //    acceptColumns.ContainsKey("新生:異動日期") &&
            //    acceptColumns.ContainsKey("新生:國中校名") &&
            //    acceptColumns.ContainsKey("新生:國中縣市"))
            //{
            //    XmlElement newelm = output.OwnerDocument.CreateElement("EnrollmentSynced");
            //    output.AppendChild(newelm);
            //    newelm.InnerText = "0";
            //}
        }

        public void GenerateUpdateRequest(SheetReader reader, BulkColumnCollection acceptColumns,
            XmlElement output, string identifyName, string shiftName)
        {
            foreach (ElementGenerator each in _generators)
            {
                if (acceptColumns.ContainGroup(each.SourceName) && each.SourceName != identifyName && each.SourceName != shiftName)
                {
                    if (each.SourceName == "畢結業證書字號")
                    {
                        XmlElement dn = output.OwnerDocument.CreateElement("DiplomaNumberRaw");
                        output.AppendChild(dn);

                        dn.InnerText = reader.GetValue("畢結業證書字號");
                    }
                    else
                        each.Generate(reader, output);
                }
                else if (each.SourceName == identifyName)
                {
                    XmlElement condition = output.OwnerDocument.CreateElement("Condition");
                    output.AppendChild(condition);

                    XmlElement identifyElm = output.OwnerDocument.CreateElement(each.TargetName);
                    condition.AppendChild(identifyElm);

                    identifyElm.InnerText = reader.GetValue(identifyName);
                }
            }

            //if (acceptColumns.ContainsKey("新生:異動代號") &&
            //    acceptColumns.ContainsKey("新生:異動日期") &&
            //    acceptColumns.ContainsKey("新生:國中校名") &&
            //    acceptColumns.ContainsKey("新生:國中縣市"))
            //{
            //    XmlElement newelm = output.OwnerDocument.CreateElement("EnrollmentSynced");
            //    output.AppendChild(newelm);
            //    newelm.InnerText = "0";
            //}
        }


    }
}
