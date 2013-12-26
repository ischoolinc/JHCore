using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    internal static class QueryGraduationPlan
    {
        public static DSResponse GetGraduationPlanList()
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            request.AddElement(".", "Field", "<ID/><Name/>", true);
            return DSAServices.CallService("SmartSchool.GraduationPlan.GetDetailList", new DSRequest(request));
        }
    }
}
