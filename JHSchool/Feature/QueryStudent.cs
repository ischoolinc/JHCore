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
    public static class QueryStudent
    {
        /// <summary>
        /// 取得詳細資料列表
        /// </summary>
        /// <returns></returns>
        private static DSResponse GetDetailList(IEnumerable<string> fields, params string[] list)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            bool hasfield = false;
            foreach ( string field in fields )
            {
                helper.AddElement("Field", field);
                hasfield = true;
            }
            if ( !hasfield )
                throw new Exception("必須傳入Field");
            helper.AddElement("Condition");
            foreach ( string id in list )
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }
        private static DSXmlHelper CreateBriefFieldHelper()
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "Status");
            helper.AddElement("Field", "SeatNo");
            helper.AddElement("Field", "Name");
            helper.AddElement("Field", "StudentNumber");
            helper.AddElement("Field", "Gender");
            helper.AddElement("Field", "IDNumber");
            helper.AddElement("Field", "Birthdate");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefClassID");
            helper.AddElement("Field", "OverrideDeptID");
            helper.AddElement("Field", "RefGraduationPlanID");
            helper.AddElement("Field", "RefScoreCalcRuleID");
            helper.AddElement("Field", "Nationality");
            //helper.AddElement("Field", "TagPrefix");
            //helper.AddElement("Field", "TagName");
            //helper.AddElement("Field", "TagID");
            //helper.AddElement("Field", "TagColor");
            return helper;
        }

        public static List<StudentRecord> GetAllStudents()
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            helper.AddElement("Order");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetAbstractListWithTag", dsreq);
            List<StudentRecord> result = new List<StudentRecord>();
            foreach ( XmlElement var in dsrsp.GetContent().GetElements("Student") )
            {
                result.Add(new StudentRecord(var));
            }
            return result;
        }
        public static List<StudentRecord> GetStudents(params string[] primaryKeys)
        {
            return GetStudents((IEnumerable<string>)primaryKeys);
        }
        public static List<StudentRecord> GetStudents(IEnumerable<string> primaryKeys)
        {
            bool hasKey = false ;
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            foreach ( string var in primaryKeys )
            {
                helper.AddElement("Condition", "ID", var);
                hasKey = true;
            }
            helper.AddElement("Order");
            dsreq.SetContent(helper);
            if ( hasKey )
            {
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetAbstractListWithTag", dsreq);
                List<StudentRecord> result = new List<StudentRecord>();
                foreach ( XmlElement var in dsrsp.GetContent().GetElements("Student") )
                {
                    result.Add(new StudentRecord(var));
                    System.Diagnostics.Trace.WriteLine("建立StudentRecord{0}", DateTime.Now.ToLongTimeString());
                }
                return result;
            }
            else
            {
                return new List<StudentRecord>();
            }
        }

        public static List<SemesterHistoryRecord> GetSemesterHistories(params string[] primaryKeys)
        {
            return GetSemesterHistories((IEnumerable<string>)primaryKeys);
        }
        public static List<SemesterHistoryRecord> GetSemesterHistories(IEnumerable<string> primaryKeys)
        {
            List<SemesterHistoryRecord> result = new List<SemesterHistoryRecord>();
            DSXmlHelper helper = GetDetailList(new string[] { "ID", "SemesterHistory" }, new List<string>(primaryKeys).ToArray()).GetContent();
            foreach ( XmlElement element in helper.GetElements("Student") )
            {///SemesterHistory/History
                //var.SemesterHistoryList.Add(new SmartSchool.API.StudentExtension.SemesterHistory(element));
                string ID = element.GetAttribute("ID");
                foreach ( XmlElement item in element.SelectNodes("SemesterHistory/History") )
                {
                    result.Add(new SemesterHistoryRecord(ID, item));
                }
            }
            return result;
        }
    }
}
