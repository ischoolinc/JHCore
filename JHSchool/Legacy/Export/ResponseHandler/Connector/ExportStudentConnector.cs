using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Legacy.Export.RequestHandler.Formater;
using JHSchool.Legacy.Export.RequestHandler.Generator;
using JHSchool.Legacy.Export.RequestHandler;
using JHSchool.Legacy.Export.Util;
using JHSchool.Legacy.Export.RequestHandler.Generator.Condition;
using JHSchool.Legacy.Export.RequestHandler.Generator.Orders;
using JHSchool.Legacy.Export.ResponseHandler.Formater;
using JHSchool.Feature.Legacy;

namespace JHSchool.Legacy.Export.ResponseHandler.Connector
{
    public class ExportStudentConnector : IExportConnector
    {
        //private DSConnection _connection;
        private FieldCollection _selectFields;
        // 一定有 ID
        private FieldCollection _selectFieldsID;

        private List<string> _conditions;

        public ExportStudentConnector()
        {
            _conditions = new List<string>();
        }

        #region IExportConnector 成員

        public void SetSelectedFields(FieldCollection fields)
        {
            _selectFields = fields;
        }

        public void AddCondition(string studentid)
        {
            _conditions.Add(studentid);
        }



        public ExportTable Export()
        {
            // 取得縣市對照表
            XmlElement schoolLocationList = Config.GetSchoolLocationList().GetContent().BaseElement;

            // 取得匯出規則描述
            XmlElement descElement = StudentBulkProcess.GetExportDescription();
            IFieldFormater fieldFormater = new BaseFieldFormater();
            IResponseFormater responseFormater = new ResponseFormater();

            FieldCollection fieldCollection = fieldFormater.Format(descElement);
            ExportFieldCollection exportFields = responseFormater.Format(descElement);



            fieldCollection = FieldUtil.Match(fieldCollection, _selectFields);
            exportFields = FieldUtil.Match(exportFields, _selectFields);

            //// 有選狀態時加入
            //if (_selectFields.FindByDisplayText("狀態") != null)
            //{
            //    fieldCollection.Add(_selectFields.FindByDisplayText("狀態"));
            //    ExportField ex = new ExportField();
            //    ex.DisplayText = "狀態";
            //    ex.RequestName = "StudentStatus";
            //    ex.ColumnIndex = exportFields.Length;
            //    ex.DataType = "";
            //    ex.XPath = "";
            //    exportFields.Add(ex);

            //}


            IRequestGenerator reqGenerator = new ExportStudentRequestGenerator();
            
            _selectFieldsID = new FieldCollection();
            foreach (Field fd in _selectFields)
                _selectFieldsID.Add(fd);

            if (_selectFieldsID.Find("StudentID") == null)
            {
                Field fd1 = new Field();
                fd1.FieldName = "StudentID";
                fd1.DisplayText = "學生系統編號";
                _selectFieldsID.Add(fd1);
            }
            reqGenerator.SetSelectedFields(_selectFieldsID);

            // 預設找-1, 不然會傳回所有學生
            ICondition condition = new BaseCondition("ID", "-1");
            reqGenerator.AddCondition(condition);
            foreach (string id in _conditions)
            {
                ICondition condition2 = new BaseCondition("ID", id);
                reqGenerator.AddCondition(condition2);
            }
            
            reqGenerator.AddOrder(new Order("GradeYear"));
            reqGenerator.AddOrder(new Order("Department"));
            reqGenerator.AddOrder(new Order("RefClassID"));
            reqGenerator.AddOrder(new Order("SeatNo"));

            DSRequest request = reqGenerator.Generate();
            DSResponse response = QueryStudent.GetExportList(request);

            ExportTable table = new ExportTable();

                       
            
            foreach (ExportField field in exportFields)
                table.AddColumn(field);

            //// 取得學生狀態
            //Dictionary<string, string> StudStatusDic = new Dictionary<string, string>();
            //foreach (JHSchool.Data.JHStudentRecord stud in JHSchool.Data.JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.SelectedSource ))
            //    StudStatusDic.Add(stud.ID, stud.Status.ToString());            

            foreach (XmlElement record in response.GetContent().GetElements("Student"))
            {
               
                ExportRow row = table.AddRow();
                foreach (ExportField column in table.Columns)
                {
                    int columnIndex = column.ColumnIndex;
                    ExportCell cell = row.Cells[columnIndex];

                    XmlNode cellNode = record.SelectSingleNode(column.XPath);

                    //if(column.DisplayText !="狀態")
                    //    cellNode = record.SelectSingleNode(column.XPath);
                    // CustodianOtherInfo/CustodianOtherInfo[1]/EducationDegree[1]

                    #region 這段程式是處理匯入/匯出程式不一致問題
                    if (column.XPath.StartsWith("CustodianOtherInfo/Custodian"))
                    {
                        if (cellNode == null)
                        {
                            string x = column.XPath.Replace("CustodianOtherInfo/Custodian", "CustodianOtherInfo/CustodianOtherInfo");
                            cellNode = record.SelectSingleNode(x);
                            if (cellNode == null)
                            {
                                x = column.XPath.Replace("CustodianOtherInfo/CustodianOtherInfo", "CustodianOtherInfo/Custodian");
                                cellNode = record.SelectSingleNode(x);
                            }
                        }
                    }
                    if (column.XPath.StartsWith("FatherOtherInfo/Father"))
                    {
                        if (cellNode == null)
                        {
                            string x = column.XPath.Replace("FatherOtherInfo/Father", "FatherOtherInfo/FatherOtherInfo");
                            cellNode = record.SelectSingleNode(x);
                            if (cellNode == null)
                            {
                                x = column.XPath.Replace("FatherOtherInfo/FatherOtherInfo", "FatherOtherInfo/Father");
                                cellNode = record.SelectSingleNode(x);
                            }
                        }
                    }
                    if (column.XPath.StartsWith("MotherOtherInfo/Mother"))
                    {
                        if (cellNode == null)
                        {
                            string x = column.XPath.Replace("MotherOtherInfo/Mother", "MotherOtherInfo/MotherOtherInfo");
                            cellNode = record.SelectSingleNode(x);
                            if (cellNode == null)
                            {
                                x = column.XPath.Replace("MotherOtherInfo/MotherOtherInfo", "MotherOtherInfo/Mother");
                                cellNode = record.SelectSingleNode(x);
                            }
                        }
                    }
                    #endregion

                    if (cellNode != null)
                    {
                        if (column.FieldName == "GraduateSchoolLocationCode")
                            cell.Value = GetCounty(schoolLocationList, cellNode.InnerText);
                        else if (column.FieldName == "DeptName") //處理科別繼承問題。
                        {
                            //這個欄位的資料一定會被回傳，因為設定了 Mandatory 屬性。
                            XmlNode selfDept = record.SelectSingleNode("SelfDeptName");
                            if (string.IsNullOrEmpty(selfDept.InnerText))
                                cell.Value = cellNode.InnerText;
                            else
                                cell.Value = selfDept.InnerText;
                        }
                        else if (column.FieldName == "Status")
                        { 
                            cell.Value =GetStudStatusStr(cellNode.InnerText );
                        }
                        else
                            cell.Value = cellNode.InnerText;
                    }

                    //if (column.DisplayText == "狀態")//record.SelectSingleNode("StudentID")!=null )
                    //{
                    //    // 學生狀態
                    //    if (StudStatusDic.ContainsKey(record.SelectSingleNode("StudentID").InnerText))
                    //        cell.Value = StudStatusDic[record.SelectSingleNode("StudentID").InnerText];
                    //}

                }
            }
            return table;
        }

        
        // 取得學生狀態名稱
        private string GetStudStatusStr(string code)
        {
            string retValue = string.Empty;

            if (code == "1")
                retValue = "一般";

            if (code == "4")
                retValue = "休學";

            if (code == "8")
                retValue = "輟學";

            if (code == "16")
                retValue = "畢業或離校";

            if (code == "256")
                retValue = "刪除";

            return retValue;
        
        }

        private string GetCounty(XmlElement list, string code)
        {
            foreach (XmlNode node in list.SelectNodes("Location"))
            {
                XmlElement element = (XmlElement)node;
                if (element.GetAttribute("Code") == code)
                    return element.InnerText;
            }
            return string.Empty;
        }

        #endregion
    }
}
