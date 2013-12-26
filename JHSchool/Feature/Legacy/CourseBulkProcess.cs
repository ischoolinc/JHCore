using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using Framework;
using FISCA.Authentication;

namespace SmartSchool.Feature.Course
{
    [AutoRetryOnWebException()]
    public class CourseBulkProcess
    {
        public static System.Xml.XmlElement GetExportDescription()
        {
            return CallNoneRequestService("SmartSchool.Course.BulkProcessJH.GetExportDescription");
        }

        private static XmlElement CallNoneRequestService(string serviceName)
        {
            string strServiceName = serviceName;
            DSResponse rsp = DSAServices.CallService(serviceName, new DSRequest());

            if (rsp.GetContent() == null)
                throw new Exception("服務未回傳任何欄位資訊。(" + strServiceName + ")");

            return rsp.GetContent().BaseElement;
        }

        public static DSResponse GetExportList(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Course.BulkProcessJH.Export", request);
        }

        public static XmlElement GetImportDescription()
        {
            return CallNoneRequestService("SmartSchool.Course.BulkProcessJH.GetImportFieldList");
        }

        public static XmlElement GetFieldValidationRule()
        {
            return CallNoneRequestService("SmartSchool.Course.BulkProcessJH.GetValidateFieldRule");
        }

        public static XmlElement GetPrimaryKeyList()
        {
            return CallNoneRequestService("SmartSchool.Course.BulkProcessJH.GetUniqueFieldData");
        }

        public static XmlElement GetShiftCheckList(params string[] fieldList)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            foreach (string each in fieldList)
                request.AddElement(".", each);

            string sn = "SmartSchool.Course.BulkProcessJH.GetShiftCheckList";
            return DSAServices.CallService(sn, new DSRequest(request)).GetContent().BaseElement;
        }

        public static XmlElement GetCourseTeachers(IEnumerable<string> fieldList)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            foreach (string each in fieldList)
                request.AddElement(".", each);

            string sn = "SmartSchool.Course.BulkProcessJH.GetCourseTeachers";
            return DSAServices.CallService(sn, new DSRequest(request)).GetContent().BaseElement;
        }

        public static void InsertImportCourse(XmlElement data)
        {
            DSAServices.CallService("SmartSchool.Course.BulkProcessJH.InsertImportCourse", new DSRequest(data));
        }

        public static void UpdateImportCourse(XmlElement data)
        {
            DSAServices.CallService("SmartSchool.Course.BulkProcessJH.UpdateImportCourse", new DSRequest(data));
        }
    }
}
