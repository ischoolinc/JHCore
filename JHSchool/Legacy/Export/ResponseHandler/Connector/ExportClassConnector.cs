using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.Legacy.Export.RequestHandler;
using System.Xml;
using JHSchool.Legacy.Export.RequestHandler.Formater;
using JHSchool.Legacy.Export.ResponseHandler.Formater;
using JHSchool.Legacy.Export.Util;
using JHSchool.Legacy.Export.RequestHandler.Generator;
using JHSchool.Legacy.Export.RequestHandler.Generator.Condition;
using FISCA.DSAUtil;
using SmartSchool.Feature.Class;

namespace JHSchool.Legacy.Export.ResponseHandler.Connector
{
    public class ExportClassConnector : IExportConnector
    {
         private FieldCollection _selectFields;
        private List<string> _conditions;

        public ExportClassConnector()
        {
            _conditions = new List<string>();
        }

        public void SetSelectedFields(FieldCollection fields)
        {
            _selectFields = fields;
        }

        public void AddCondition(string classid)
        {
            _conditions.Add(classid);
        }



        public ExportTable Export()
        {
            // 取得匯出規則描述
            XmlElement descElement = ClassBulkProcess.GetExportDescription();
            IFieldFormater fieldFormater = new BaseFieldFormater();
            IResponseFormater responseFormater = new ResponseFormater();

            FieldCollection fieldCollection = fieldFormater.Format(descElement);
            ExportFieldCollection exportFields = responseFormater.Format(descElement);

            fieldCollection = FieldUtil.Match(fieldCollection, _selectFields);
            exportFields = FieldUtil.Match(exportFields, _selectFields);

            IRequestGenerator reqGenerator = new ExportStudentRequestGenerator();
            reqGenerator.SetSelectedFields(_selectFields);

            ICondition condition = new BaseCondition("ID", "-1");
            reqGenerator.AddCondition(condition);
            foreach (string id in _conditions)
            {
                ICondition condition2 = new BaseCondition("ID", id);
                reqGenerator.AddCondition(condition2);
            }

            DSRequest request = reqGenerator.Generate();
            DSResponse response = ClassBulkProcess.GetExportList(request);

            ExportTable table = new ExportTable();
            foreach (ExportField field in exportFields)
            {
                table.AddColumn(field);
            }

            foreach (XmlElement record in response.GetContent().GetElements("Class"))
            {
                ExportRow row = table.AddRow();
                foreach (ExportField column in table.Columns)
                {
                    int columnIndex = column.ColumnIndex;
                    ExportCell cell = row.Cells[columnIndex];
                    XmlNode cellNode = record.SelectSingleNode(column.XPath);
                    if (cellNode != null)
                        cell.Value = cellNode.InnerText;
                }
            }
            return table;
        }
    }
}
