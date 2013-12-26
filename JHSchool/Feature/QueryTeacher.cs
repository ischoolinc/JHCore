using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature
{
    [AutoRetryOnWebException()]
    public static class QueryTeacher
    {
        private static DSXmlHelper CreateBriefFieldHelper()
        {
            DSXmlHelper helper = new DSXmlHelper("GetTeacherListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "TeacherName");
            helper.AddElement("Field", "Nickname");
            helper.AddElement("Field", "Status");
            helper.AddElement("Field", "Gender");
            helper.AddElement("Field", "IDNumber");
            helper.AddElement("Field", "Category");
            helper.AddElement("Field", "ContactPhone");
            helper.AddElement("Field", "TALoginName");
            return helper;
        }

        public static List<TeacherRecord> GetAllTeachers()
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            helper.AddElement("Order");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Teacher.GetDetailList", dsreq);
            List<TeacherRecord> result = new List<TeacherRecord>();
            foreach (XmlElement var in dsrsp.GetContent().GetElements("Teacher"))
            {
                result.Add(new TeacherRecord(var));
            }
            return result;
        }

        public static List<TeacherRecord> GetTeachers(IEnumerable<string> primaryKeys)
        {
            bool hasKey = false;
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            foreach ( var key in primaryKeys )
            {
                hasKey = true;
                helper.AddElement("Condition", "ID", key);
            }
            helper.AddElement("Order");
            List<TeacherRecord> result = new List<TeacherRecord>();
            if ( hasKey )
            {
                dsreq.SetContent(helper);
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Teacher.GetDetailList", dsreq);
                foreach ( XmlElement var in dsrsp.GetContent().GetElements("Teacher") )
                {
                    result.Add(new TeacherRecord(var));
                }
            }
            return result;
        }
    }
}
