using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [Obsolete("過渡時期的Service")]
    class AddScore
    {
        public static void InsertSemesterSubjectScore(string refStudentId, string schoolYear, string semester, string gradeYear, XmlElement ScoreInfo)
        {
            DSRequest dsreq = new DSRequest("<InsertRequest><SemesterSubjectScore><RefStudentId>" + refStudentId + "</RefStudentId><SchoolYear>" + schoolYear + "</SchoolYear><Semester>" + semester + "</Semester><GradeYear>" + gradeYear + "</GradeYear><ScoreInfo>" + ScoreInfo.OuterXml + "</ScoreInfo></SemesterSubjectScore></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSemesterSubjectScore", dsreq);
        }
        public static void InsertSemesterEntryScore(string refStudentId, string schoolYear, string semester, string gradeYear, string entryGroup, XmlElement ScoreInfo)
        {
            DSRequest dsreq = new DSRequest("<InsertRequest><SemesterEntryScore><RefStudentId>" + refStudentId + "</RefStudentId><SchoolYear>" + schoolYear + "</SchoolYear><Semester>" + semester + "</Semester><GradeYear>" + gradeYear + "</GradeYear><EntryGroup>" + entryGroup + "</EntryGroup><ScoreInfo>" + ScoreInfo.OuterXml + "</ScoreInfo></SemesterEntryScore></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSemesterEntryScore", dsreq);
        }

        public static void InsertSchoolYearSubjectScore(string refStudentId, string schoolYear, string gradeYear, XmlElement ScoreInfo)
        {
            DSRequest dsreq = new DSRequest("<InsertRequest><SchoolYearSubjectScore><RefStudentId>" + refStudentId + "</RefStudentId><SchoolYear>" + schoolYear + "</SchoolYear><GradeYear>" + gradeYear + "</GradeYear><ScoreInfo>" + ScoreInfo.OuterXml + "</ScoreInfo></SchoolYearSubjectScore></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSchoolYearSubjectScore", dsreq);
        }
        public static void InsertSchoolYearEntryScore(string refStudentId, string schoolYear, string gradeYear, string entryGroup, XmlElement ScoreInfo)
        {
            DSRequest dsreq = new DSRequest("<InsertRequest><SchoolYearEntryScore><RefStudentId>" + refStudentId + "</RefStudentId><SchoolYear>" + schoolYear + "</SchoolYear><GradeYear>" + gradeYear + "</GradeYear><EntryGroup>" + entryGroup + "</EntryGroup><ScoreInfo>" + ScoreInfo.OuterXml + "</ScoreInfo></SchoolYearEntryScore></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSchoolYearEntryScore", dsreq);
        }



        public static void InsertSemesterSubjectScore(params InsertInfo[] items)
        {
            string req = "<InsertRequest>";
            foreach (InsertInfo info in items)
            {
                req += "<SemesterSubjectScore><RefStudentId>" + info.RefStudentId + "</RefStudentId><SchoolYear>" + info.SchoolYear + "</SchoolYear><Semester>" + info.Semester + "</Semester><GradeYear>" + info.GradeYear + "</GradeYear><ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></SemesterSubjectScore>";
            }
            req += "</InsertRequest>";
            DSRequest dsreq = new DSRequest(req);
            //DSRequest dsreq = new DSRequest("<InsertRequest></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSemesterSubjectScore", dsreq);
        }
        public static void InsertSemesterEntryScore(params InsertInfo[] items)
        {
            string req = "<InsertRequest>";
            foreach (InsertInfo info in items)
            {
                req += "<SemesterEntryScore><RefStudentId>" + info.RefStudentId + "</RefStudentId><SchoolYear>" + info.SchoolYear + "</SchoolYear><Semester>" + info.Semester + "</Semester><GradeYear>" + info.GradeYear + "</GradeYear><EntryGroup>" + info.EntryGroup + "</EntryGroup><ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></SemesterEntryScore>";
            }
            req += "</InsertRequest>";
            DSRequest dsreq = new DSRequest(req);
            //DSRequest dsreq = new DSRequest("<InsertRequest></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSemesterEntryScore", dsreq);
        }

        public static void InsertSchoolYearSubjectScore(params InsertInfo[] items)
        {
            string req = "<InsertRequest>";
            foreach (InsertInfo info in items)
            {
                req += "<SchoolYearSubjectScore><RefStudentId>" + info.RefStudentId + "</RefStudentId><SchoolYear>" + info.SchoolYear + "</SchoolYear><GradeYear>" + info.GradeYear + "</GradeYear><ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></SchoolYearSubjectScore>";
            }
            req += "</InsertRequest>";
            DSRequest dsreq = new DSRequest(req);
            //DSRequest dsreq = new DSRequest("<InsertRequest></InsertRequest>");
            DSAServices.CallService("SmartSchool.Score.InsertSchoolYearSubjectScore", dsreq);
        }
        public static void InsertSchoolYearEntryScore(params InsertInfo[] items)
        {
            string req = "<InsertRequest>";
            foreach (InsertInfo info in items)
            {
                req += "<SchoolYearEntryScore><RefStudentId>" + info.RefStudentId + "</RefStudentId><SchoolYear>" + info.SchoolYear + "</SchoolYear><GradeYear>" + info.GradeYear + "</GradeYear><EntryGroup>" + info.EntryGroup + "</EntryGroup><ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></SchoolYearEntryScore>";
            }
            req += "</InsertRequest>";
            DSRequest dsreq = new DSRequest(req);
            DSAServices.CallService("SmartSchool.Score.InsertSchoolYearEntryScore", dsreq);
        }

        public class InsertInfo
        {
            private readonly string _refStudentId, _GradeYear, _SchoolYear, _Semester, _EntryGroup;
            private readonly XmlElement _ScoreInfo;
            public InsertInfo(string refStudentId, string schoolYear, string semester, string gradeYear, string entryGroup, XmlElement scoreInfo)
            {
                _refStudentId = refStudentId;
                _GradeYear = gradeYear;
                _SchoolYear = schoolYear;
                _Semester = semester;
                _EntryGroup = entryGroup;
                _ScoreInfo = scoreInfo;
            }
            public string RefStudentId { get { return _refStudentId; } }
            public string GradeYear { get { return _GradeYear; } }
            public string SchoolYear { get { return _SchoolYear; } }
            public string Semester { get { return _Semester; } }
            public string EntryGroup { get { return _EntryGroup; } }
            public XmlElement ScoreInfo { get { return _ScoreInfo; } }
        }

        public static void InsertSemesterMoralScore(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Score.InsertSemesterMoralScore", dSRequest);
        }
    }
}
