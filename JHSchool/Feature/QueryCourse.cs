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
    public static class QueryCourse
    {
        public static List<CourseRecord> GetAllCourses()
        {

            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "CourseName");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "Period");
            helper.AddElement("Field", "Credit");
            //helper.AddElement("Field", "IsRequired");
            //helper.AddElement("Field", "RequiredBy");
            helper.AddElement("Field", "RefExamTemplateID");
            helper.AddElement("Field", "RefClassID");
            helper.AddElement("Field", "Subject");
            helper.AddElement("Field", "Domain");
            helper.AddElement("Field", "ScoreCalcFlag");
            helper.AddElement("Condition");
            helper.AddElement("Order");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Course.GetDetailList", dsreq);
            List<CourseRecord> result = new List<CourseRecord>();
            foreach ( XmlElement var in dsrsp.GetContent().GetElements("Course") )
            {
                result.Add(new CourseRecord(var));
            }
            return result;
        }
        public static List<CourseRecord> GetCourses(params string[] primaryKeys)
        { return GetCourses((IEnumerable<string>)primaryKeys);}
        public static List<CourseRecord> GetCourses(IEnumerable<string> primaryKeys)
        {
            bool hasKey = false;
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "CourseName");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "Period");
            helper.AddElement("Field", "Credit");
            //helper.AddElement("Field", "IsRequired");
            //helper.AddElement("Field", "RequiredBy");
            helper.AddElement("Field", "RefExamTemplateID");
            helper.AddElement("Field", "RefClassID");
            helper.AddElement("Field", "Subject");
            helper.AddElement("Field", "Domain");
            helper.AddElement("Field", "ScoreCalcFlag");
            helper.AddElement("Condition");
            foreach ( var key in primaryKeys )
            {
                helper.AddElement("Condition", "ID", key);
                hasKey = true;
            }
            helper.AddElement("Order");
            List<CourseRecord> result = new List<CourseRecord>();
            if ( hasKey )
            {
                dsreq.SetContent(helper);
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Course.GetDetailList", dsreq);
                foreach ( XmlElement var in dsrsp.GetContent().GetElements("Course") )
                {
                    result.Add(new CourseRecord(var));
                }
            }
            return result;
        }
    }
}
