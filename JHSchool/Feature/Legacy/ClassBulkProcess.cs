using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace SmartSchool.Feature.Class
{
    public class ClassBulkProcess
    {
        [AutoRetryOnWebException()]
        public static XmlElement GetExportDescription()
        {
            return CallNoneRequestService("SmartSchool.Class.BulkProcessJH.GetExportDescription");
        }

        private static XmlElement CallNoneRequestService(string serviceName)
        {
            string strServiceName = serviceName;
            DSResponse rsp = DSAServices.CallService(serviceName, new DSRequest());

            if (rsp.GetContent() == null)
                throw new Exception("服務未回傳任何欄位資訊。(" + strServiceName + ")");

            return rsp.GetContent().BaseElement;
        }

        [AutoRetryOnWebException()]
        public static DSResponse GetExportList(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Class.BulkProcessJH.Export", request);
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetImportFieldList()
        {
            return CallNoneRequestService("SmartSchool.Class.BulkProcessJH.GetImportFieldList");
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetValidateFieldRule()
        {
            return CallNoneRequestService("SmartSchool.Class.BulkProcessJH.GetValidateFieldRule");
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetUniqueFieldData()
        {
            return CallNoneRequestService("SmartSchool.Class.BulkProcessJH.GetUniqueFieldData");
        }

        [AutoRetryOnWebException()]
        public static XmlElement GetShiftCheckList(params string[] fieldList)
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            foreach (string each in fieldList)
                request.AddElement(".", each);

            string sn = "SmartSchool.Class.BulkProcessJH.GetShiftCheckList";
            return DSAServices.CallService(sn, new DSRequest(request)).GetContent().BaseElement;
        }

        public static void InsertImportData(XmlElement data)
        {
            DSAServices.CallService("SmartSchool.Class.BulkProcessJH.InsertImportClass", new DSRequest(data));
        }

        [AutoRetryOnWebException()]
        public static void UpdateImportData(XmlElement data)
        {
            DSAServices.CallService("SmartSchool.Class.BulkProcessJH.UpdateImportClass", new DSRequest(data));
        }
    }
}
