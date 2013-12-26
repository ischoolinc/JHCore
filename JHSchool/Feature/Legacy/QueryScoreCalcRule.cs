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
    internal static class QueryScoreCalcRule
    {
        public static DSResponse GetScoreCalcRuleList()
        {
            DSXmlHelper request = new DSXmlHelper("Request");
            request.AddElement(".", "Field", "<ID/><Name/>", true);
            return DSAServices.CallService("SmartSchool.ScoreCalcRule.GetScoreCalcRule", new DSRequest(request));
        }
    }
}
