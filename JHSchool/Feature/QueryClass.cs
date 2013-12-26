using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature
{
    [AutoRetryOnWebException()]
    public static class QueryClass
    {
        public static List<ClassRecord> GetAllClasses()
        {
            string req = "<GetClassListRequest><Field><ID/><ClassName/><RefTeacherID/><GradeYear/><NamingRule/><RefDepartmentID/><RefGraduationPlanID/><RefScoreCalcRuleID/><DisplayOrder/></Field><Condition>";
            req += "</Condition><Order></Order></GetClassListRequest>";
            List<ClassRecord> result = new List<ClassRecord>();
            foreach ( XmlElement item in DSAServices.CallService("SmartSchool.Class.GetDetailList", new DSRequest(req)).GetContent().GetElements("Class") )
            {
                result.Add(new ClassRecord(item));
            }
            return result;
        }
        public static List<ClassRecord> GetClasses(IEnumerable<string> primaryKeys)
        {
            bool haskey = false;
            string req = "<GetClassListRequest><Field><ID/><ClassName/><RefTeacherID/><GradeYear/><NamingRule/><RefDepartmentID/><RefGraduationPlanID/><RefScoreCalcRuleID/><DisplayOrder/></Field><Condition>";
            foreach ( string key in primaryKeys )
            {
                req += "<ID>" + key + "</ID>";
                haskey = true;
            }
            req += "</Condition><Order></Order></GetClassListRequest>";
            List<ClassRecord> result = new List<ClassRecord>();
            if ( haskey )
            {
                foreach ( XmlElement item in DSAServices.CallService("SmartSchool.Class.GetDetailList", new DSRequest(req)).GetContent().GetElements("Class") )
                {
                    result.Add(new ClassRecord(item));
                }
            }
            return result;
        }
        public static List<ClassRecord> GetClasses(params string[] primaryKeys)
        {
            return GetClasses((IEnumerable<string>)primaryKeys);
        }

        public static DSResponse GetClassDetail(string classid)
        {
            DSXmlHelper helper = new DSXmlHelper("GetClassListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", classid);
            return DSAServices.CallService("SmartSchool.Class.GetDetailList", new DSRequest(helper));
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
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Class.GetAbstractList", dsreq);
        }
    }
}
