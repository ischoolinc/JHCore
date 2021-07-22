using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public static class QueryClass
    {
        /// <summary>
        /// 取得現有年級
        /// </summary>
        /// <returns></returns>
        public static DSResponse GetGradeYearList()
        {
            DSXmlHelper helper = new DSXmlHelper("GetGradeYearRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ClassCount");
            helper.AddElement("Field", "GradeYear");
            //helper.AddElement("Field", "Status");
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Class.GetGradeYearList", dsreq);
        }

        public static DSResponse GetClass(params string[] classIDList)
        {
            string req = "<GetClassListRequest><Field><All></All></Field><Condition>";
            foreach (string id in classIDList)
            {
                req += "<ID>" + id + "</ID>";
            }
            req += "</Condition><Order><GradeYear/><DisplayOrder/></Order></GetClassListRequest>";
            return DSAServices.CallService("SmartSchool.Class.GetDetailList", new DSRequest(req));
        }

        public static DSResponse GetClassList()
        {
            DSXmlHelper helper = new DSXmlHelper("GetGradeYearRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ClassID");
            helper.AddElement("Field", "ClassName");
            helper.AddElement("Order");
            helper.AddElement("Order", "DisplayOrder");
            helper.AddElement("Order", "ClassName");
            helper.AddElement("Order", "ClassNumber");
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Class.GetAbstractList", dsreq);
        }
    }
}
