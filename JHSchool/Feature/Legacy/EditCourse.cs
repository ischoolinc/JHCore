using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    public static class EditCourse
    {
        [AutoRetryOnWebException()]
        public static void UpdateCourse(DSRequest request)
        {
            DSAServices.CallService("SmartSchool.Course.Update", request);
        }

        [AutoRetryOnWebException()]
        public static void RemoveCourseTeachers(string courseId)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Course");
            helper.AddElement("Course", "CourseID", courseId);

            DSAServices.CallService("SmartSchool.Course.RemoveCourseTeacher", new DSRequest(helper));


        }

        [AutoRetryOnWebException()]
        public static void RemoveCourseTeachers(DSXmlHelper request)
        {
            DSAServices.CallService("SmartSchool.Course.RemoveCourseTeacher", new DSRequest(request));
        }

        public static List<string> AddCourseTeacher(DSXmlHelper request)
        {
            DSResponse rsp=DSAServices.CallService("SmartSchool.Course.AddCourseTeacher", new DSRequest(request));

            List<string> newidlist=new List<string>();
            foreach (var each in rsp.GetContent().GetElements("NewID"))
                newidlist.Add(each.InnerText);

            return newidlist;
        }
    }
}
