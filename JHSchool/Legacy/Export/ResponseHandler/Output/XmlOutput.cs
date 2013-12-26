using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace JHSchool.Legacy.Export.ResponseHandler.Output
{
    public class XmlOutput:IOutput<XmlDocument>
    {
        private XmlDocument _doc;

        #region IOutput<XmlDocument> жин√

        public void SetSource(ExportTable dataSource)
        {            
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Root");
            doc.AppendChild(root);

            foreach (ExportRow row in dataSource.Rows)
            {
                XmlElement rowElement = doc.CreateElement("Row");
                rowElement.SetAttribute("RowIndex", row.Index.ToString());
                root.AppendChild(rowElement);

                foreach (ExportField field in dataSource.Columns)
                {
                    XmlElement column = doc.CreateElement("Column");
                    rowElement.AppendChild(column);
                    column.SetAttribute("DisplayName", field.DisplayText);
                    column.InnerText = row.Cells[field.ColumnIndex].Value;
                }
            }
            _doc = doc;
        }

        public XmlDocument GetOutput()
        {
            return _doc;
        }

        public void Save(string fileName)
        {
            _doc.Save(fileName);
        }

        #endregion
    }
}
