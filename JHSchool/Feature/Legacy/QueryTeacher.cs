using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class QueryTeacher
    {
        public static DSResponse GetTeacherList()
        {
            DSXmlHelper helper = new DSXmlHelper("GetAbstractList");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "TeacherName");
            helper.AddElement("Field", "Nickname");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "Status", "一般");
            helper.AddElement("Order");
            helper.AddElement("Order", "TeacherName");
            return DSAServices.CallService("SmartSchool.Teacher.GetAbstractList", new DSRequest(helper));
        }
        public static DSResponse GetTeacherListWithSupervisedByClassInfo(params string[] teacherIdList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "TeacherName");
            helper.AddElement("Field", "Nickname");
            helper.AddElement("Field", "Gender");
            helper.AddElement("Field", "IDNumber");
            helper.AddElement("Field", "ContactPhone");
            helper.AddElement("Field", "Category");
            helper.AddElement("Field", "SupervisedByClassID");
            helper.AddElement("Field", "SupervisedByClassName");
            helper.AddElement("Field", "SupervisedByGradeYear");
            helper.AddElement("Field", "Status");
            helper.AddElement("Condition");
            if ( teacherIdList.Length > 0 )
            {
                helper.AddElement("Condition", "IDList");
                foreach ( string id in teacherIdList )
                {
                    helper.AddElement("Condition/IDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "TeacherName");
            return DSAServices.CallService("SmartSchool.Teacher.GetDetailListWithSupervisedByClassInfo", new DSRequest(helper));
        }

        public static DSResponse GetTeacherDetail(string teacherid)
        {
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", teacherid);
            return DSAServices.CallService("SmartSchool.Teacher.GetDetailList", new DSRequest(helper));
        }

        internal static DSResponse GetStudentListBelong(string teacherid)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "RefTeacherID", teacherid);
            helper.AddElement("Order");
            helper.AddElement("Order", "ClassName");
            helper.AddElement("Order", "SeatNumber");
            return DSAServices.CallService("SmartSchool.Teacher.GetStudentListBelong", new DSRequest(helper));
        }

        internal static DSResponse GetTeachClass(string teacherID)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "TeacherID", teacherID);
            return DSAServices.CallService("SmartSchool.Teacher.GetTeachClass", new DSRequest(helper));
        }

        public static DSResponse GetCourseList(string teacherid)
        {
            DSXmlHelper helper = new DSXmlHelper("SelectRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "TeacherID", teacherid);
            helper.AddElement("Order");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            return DSAServices.CallService("SmartSchool.Teacher.GetCourseList", new DSRequest(helper));
        }

        //測試用
        public static DSResponse GetTeacherDetailTest(params string[] idList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetDetailListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "TeacherName");
            helper.AddElement("Field", "Nickname");
            helper.AddElement("Field", "Gender");
            helper.AddElement("Field", "IDNumber");
            helper.AddElement("Field", "ContactPhone");
            //helper.AddElement("Field", "Email");
            //helper.AddElement("Field", "Photo");
            helper.AddElement("Field", "Category");
            //helper.AddElement("Field", "SmartTeacherLoginName");
            //helper.AddElement("Field", "SmartTeacherPassword");  
            //helper.AddElement("Field", "RemoteAccount");
            helper.AddElement("Field", "Status");
            if ( idList.Length > 0 )
            {
                helper.AddElement("Condition");
                foreach ( string var in idList )
                {
                    helper.AddElement("Condition", "TeacherID", var);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "TeacherName");
            return DSAServices.CallService("SmartSchool.Teacher.GetDetailList", new DSRequest(helper));
        }

        public static bool LoginNameExists(string identity, string loginName)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement(".", "Field", "<ID/><TeacherName/>", true);
            helper.AddElement(".", "Condition", "<SmartTeacherLoginName>" + loginName + "</SmartTeacherLoginName>", true);

            DSXmlHelper response = DSAServices.CallService("SmartSchool.Teacher.GetDetailList", new DSRequest(helper)).GetContent();

            return response.GetElements("Teacher[@ID!='" + identity + "']").Length > 0;
        }

        public static bool NameExists(string identity, string name, string nick)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement(".", "Field", "<ID/><TeacherName/>", true);
            helper.AddElement(".", "Condition", "<TeacherName>" + name + "</TeacherName>", true);
            helper.AddElement("Condition", "Nickname", nick);
            //helper.AddElement("Condition", "Status", "一般");

            DSXmlHelper response = DSAServices.CallService("SmartSchool.Teacher.GetDetailList", new DSRequest(helper)).GetContent();

            if ( string.IsNullOrEmpty(identity) )
                return response.GetElements("Teacher").Length > 0;
            else
                return response.GetElements("Teacher[@ID!='" + identity + "']").Length > 0;
        }
    }
}
