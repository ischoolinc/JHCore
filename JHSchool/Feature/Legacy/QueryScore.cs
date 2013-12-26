using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class QueryScore
    {
        public enum EntryGroup { 行為, 學習 }
        public static DSResponse GetSemesterEntryScore(params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterEntryScore", dsreq);
            return dsrsp;
        }
        public static DSXmlHelper GetSemesterEntryScoreBySemester(bool includePlace, string schoolYear, string semester, EntryGroup group, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "ScoreInfo");
            if (includePlace)
            {
                helper.AddElement("Field", "ClassRating");
                helper.AddElement("Field", "DeptRating");
                helper.AddElement("Field", "YearRating");
            }

            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Condition", "SchoolYear", schoolYear);
            helper.AddElement("Condition", "Semester", semester);
            helper.AddElement("Condition", "EntryGroup", group.ToString());

            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterEntryScore", dsreq);
            return dsrsp.GetContent();
        }
        //跟上面的差別只是把 EntryGroup 拿掉。
        public static DSXmlHelper GetSemesterEntryScoreBySemester(bool includePlace, string schoolYear, string semester, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");

            if (includePlace)
            {
                helper.AddElement("Field", "ClassRating");
                helper.AddElement("Field", "DeptRating");
                helper.AddElement("Field", "YearRating");
            }

            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Condition", "SchoolYear", schoolYear);
            helper.AddElement("Condition", "Semester", semester);

            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterEntryScore", dsreq);
            return dsrsp.GetContent();
        }
        public static DSResponse GetSemesterEntryScoreBySemester(int schoolyear, int semester, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear.ToString());
            helper.AddElement("Condition", "Semester", semester.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterEntryScore", dsreq);
            return dsrsp;
        }
        public static DSResponse GetSemesterEntryScoreByEntryGroup(EntryGroup entry, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "EntryGroup", entry.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterEntryScore", dsreq);
            return dsrsp;
        }

        public static DSResponse GetSemesterSubjectScore(params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterSubjectScore", dsreq);
            return dsrsp;
        }

        public static DSXmlHelper GetSemesterSubjectScoreBySemester(bool includePlace, string schoolYear, string semester, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Field", "GradeYear");
            if (includePlace)
            {
                helper.AddElement("Field", "ClassRating");
                helper.AddElement("Field", "DeptRating");
                helper.AddElement("Field", "YearRating");
            }
            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Condition", "SchoolYear", schoolYear);
            helper.AddElement("Condition", "Semester", semester);

            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterSubjectScore", dsreq);
            return dsrsp.GetContent();
        }
        public static DSResponse GetSemesterSubjectScoreBySemester(int schoolyear, int semester, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear.ToString());
            helper.AddElement("Condition", "Semester", semester.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSemesterSubjectScore", dsreq);
            return dsrsp;
        }
        public static DSResponse GetSchoolYearEntryScore(params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearEntryScore", dsreq);
            return dsrsp;
        }
        public static DSXmlHelper GetSchoolYearEntryScore(bool includePlace, string schoolYear, EntryGroup? group, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSemesterEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");

            if (includePlace)
            {
                helper.AddElement("Field", "ClassRating");
                helper.AddElement("Field", "DeptRating");
                helper.AddElement("Field", "YearRating");
            }

            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Condition", "SchoolYear", schoolYear);

            if (group.HasValue)
                helper.AddElement("Condition", "EntryGroup", group.Value.ToString());

            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");

            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearEntryScore", dsreq);
            return dsrsp.GetContent();
        }

        public static DSResponse GetSchoolYearEntryScoreBySchoolYearGradeYear(int schoolyear, int gradeyear, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear.ToString());
            helper.AddElement("Condition", "GradeYear", gradeyear.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearEntryScore", dsreq);
            return dsrsp;
        }
        public static DSResponse GetSchoolYearEntryScoreBySchoolYear(int schoolyear, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearEntryScore", dsreq);
            return dsrsp;
        }
        public static DSResponse GetSchoolYearEntryScoreByEntryGroup(EntryGroup entry, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearEntryScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "EntryGroup");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "EntryGroup", entry.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearEntryScore", dsreq);
            return dsrsp;
        }
        public static DSResponse GetSchoolYearSubjectScore(params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearSubjectScore", dsreq);
            return dsrsp;
        }
        public static DSXmlHelper GetSchoolYearSubjectScore(bool includePlace, string schoolYear, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");

            if (includePlace)
            {
                helper.AddElement("Field", "ClassRating");
                helper.AddElement("Field", "DeptRating");
                helper.AddElement("Field", "YearRating");
            }

            helper.AddElement("Condition");
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Condition", "SchoolYear", schoolYear);
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");

            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearSubjectScore", dsreq);
            return dsrsp.GetContent();
        }
        public static DSResponse GetSchoolYearSubjectScoreBySchoolYearGradeYear(int schoolyear, int gradeyear, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear.ToString());
            helper.AddElement("Condition", "GradeYear", gradeyear.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearSubjectScore", dsreq);
            return dsrsp;
        }
        public static DSResponse GetSchoolYearSubjectScoreBySchoolYear(int schoolyear, params string[] studentIDList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearSubjectScore");
            DSRequest dsreq = new DSRequest();
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefStudentId");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Field", "GradeYear");
            helper.AddElement("Field", "ScoreInfo");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear.ToString());
            if (studentIDList.Length > 0)
            {
                helper.AddElement("Condition", "StudentIDList");
                foreach (string id in studentIDList)
                {
                    helper.AddElement("Condition/StudentIDList", "ID", id);
                }
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "RefStudentId");
            helper.AddElement("Order", "SchoolYear");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Score.GetSchoolYearSubjectScore", dsreq);
            return dsrsp;
        }
        //public static DSResponse GetClassSemesterMoralScore(DSRequest request)
        //{
        //    return K12.DSAServices.CallService("SmartSchool.Score.GetClassSemesterMoralScore", request);
        //}
        public static DSResponse GetSemesterMoralScore(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Score.GetSemesterMoralScore", request);
        }

        public static DSResponse GetSemesterScore(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Replacing.GetSemesterScoreList", request);
        }

        public static DSResponse GetSchoolYearScore(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Replacing.GetSchoolYearScoreList", request);
        }

        public static DSResponse GetSemesterEntryScoreForPlacing(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Replacing.GetSemesterEntryScoreList", request);
        }

        public static DSResponse GetSchoolYearEntryScoreForPlacing(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.Replacing.GetSchoolYearEntryScore", request);
        }
    }
}
