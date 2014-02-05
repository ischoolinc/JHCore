using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using Framework;
using FISCA.Authentication;
using FISCA.Data;
using System.Data;

namespace JHSchool.Feature.Legacy
{
    public class StudentBulkProcess
    {
        public static XmlElement GetBulkDescription()
        {
            //讀取XML欄位描述
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_S_BulkDescription);

            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Student.BulkProcessJH.GetBulkDescription");
        }

        public static XmlElement GetFieldValidationRule()
        {
            //讀取XML欄位描述
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_S_FieldValidationRule);
            foreach (XmlElement elem in doc.SelectNodes("//FieldValidator"))
            {
                if (elem.GetAttribute("Name") == "合法班級")
                {
                    QueryHelper Q = new QueryHelper();
                    DataTable dt = Q.Select("SELECT class_name FROM class WHERE status=1");
                    foreach (DataRow row in dt.Rows)
                    {
                        XmlElement item = doc.CreateElement("Item");
                        item.SetAttribute("Value", row["class_name"].ToString());
                        elem.AppendChild(item);
                    }
                    break;
                }
            }
            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Student.BulkProcessJH.GetFieldValidationRule");
        }

        public static XmlElement GetPrimaryKeyList()
        {
            return CallNoneRequestService("SmartSchool.Student.BulkProcessJH.GetPrimaryKeyList");
        }

        public static XmlElement GetShiftCheckList(string key, string value)
        {
            DSXmlHelper request = new DSXmlHelper("GetShiftCheckList");
            request.AddElement(key);
            request.AddElement(value);

            return DSAServices.CallService("SmartSchool.Student.BulkProcessJH.GetShiftCheckList", new DSRequest(request)).GetContent().BaseElement;
        }

        public static void InsertImportStudent(XmlElement request)
        {
            DSAServices.CallService("SmartSchool.Student.BulkProcessJH.InsertImportStudent", new DSRequest(request));
        }

        public static void UpdateImportStudent(XmlElement request)
        {
            DSAServices.CallService("SmartSchool.Student.BulkProcessJH.UpdateImportStudent", new DSRequest(request));
        }

        ///// <summary>
        ///// 有回傳 Response Xml
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public static XmlElement InsertImportStudentRsp(XmlElement request)
        //{
        //    DSResponse rsp= DSAServices.CallService("SmartSchool.Student.BulkProcessJH.InsertImportStudent", new DSRequest(request));
        //    if(rsp.GetContent ()== null )
        //        throw new Exception("服務未回傳任何欄位資訊。SmartSchool.Student.BulkProcessJH.InsertImportStudent");

        //    return rsp.GetContent().BaseElement;        
        //}
        public static void SyncEnrollmentInfoToUpdateRecord()
        {
            DSAServices.CallService("SmartSchool.Student.BulkProcessJH.SyncEnrollmentInfoToUpdateRecord", new DSRequest());
        }

        private static XmlElement CallNoneRequestService(string serviceName)
        {
            string strServiceName = serviceName;
            DSResponse rsp = DSAServices.CallService(serviceName, new DSRequest());

            if (rsp.GetContent() == null)
                throw new Exception("服務未回傳任何欄位資訊。(" + strServiceName + ")");

            return rsp.GetContent().BaseElement;
        }

        public static XmlElement GetExportDescription()
        {
            //讀取XML欄位描述
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_S_ExportDescription);

            return doc.DocumentElement;
            //return CallNoneRequestService("SmartSchool.Student.BulkProcessJH.GetExportDescription");
        }
    }
}
