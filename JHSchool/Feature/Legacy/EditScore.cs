using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class EditScore
    {
        public static void UpdateSemesterSubjectScore(string subjectScoreID, string gradeYear, XmlElement scoreInfo)
        {
            DSRequest dsreq = new DSRequest("<UpdateRequest><SemesterSubjectScore><Field> <GradeYear>" + gradeYear + "</GradeYear> <ScoreInfo>" + scoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + subjectScoreID + "</ID></Condition></SemesterSubjectScore></UpdateRequest>");
            DSAServices.CallService("SmartSchool.Score.UpdateSemesterSubjectScore", dsreq);
        }
        public static void UpdateSemesterEntryScore(string entryScoreID, string gradeYear, XmlElement scoreInfo)
        {
            DSRequest dsreq = new DSRequest("<UpdateRequest><SemesterEntryScore><Field> <GradeYear>" + gradeYear + "</GradeYear><ScoreInfo>" + scoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + entryScoreID + "</ID></Condition></SemesterEntryScore></UpdateRequest>");
            DSAServices.CallService("SmartSchool.Score.UpdateSemesterEntryScore", dsreq);
        }
        public static void UpdateSchoolYearSubjectScore(string subjectScoreID, string gradeYear, XmlElement scoreInfo)
        {
            DSRequest dsreq = new DSRequest("<UpdateRequest><SchoolYearSubjectScore><Field> <GradeYear>" + gradeYear + "</GradeYear> <ScoreInfo>" + scoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + subjectScoreID + "</ID></Condition></SchoolYearSubjectScore></UpdateRequest>");
            DSAServices.CallService("SmartSchool.Score.UpdateSchoolYearSubjectScore", dsreq);
        }
        public static void UpdateSchoolYearEntryScore(string entryScoreID, string gradeYear, XmlElement scoreInfo)
        {
            DSRequest dsreq = new DSRequest("<UpdateRequest><SchoolYearEntryScore><Field> <GradeYear>" + gradeYear + "</GradeYear><ScoreInfo>" + scoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + entryScoreID + "</ID></Condition></SchoolYearEntryScore></UpdateRequest>");
            DSAServices.CallService("SmartSchool.Score.UpdateSchoolYearEntryScore", dsreq);
        }


        public static void UpdateSemesterSubjectScore(params UpdateInfo[] infos)
        {
            string req = "<UpdateRequest>";
            foreach (UpdateInfo info in infos)
            {
                int tryParseInt;
                req += "<SemesterSubjectScore><Field>" + (int.TryParse(info.GradeYear, out tryParseInt) ? ("<GradeYear>" + info.GradeYear + "</GradeYear> ") : "") + "<ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + info.ID + "</ID></Condition></SemesterSubjectScore>";
            }
            req += "</UpdateRequest>";
            DSRequest dsreq = new DSRequest(req);
            DSAServices.CallService("SmartSchool.Score.UpdateSemesterSubjectScore", dsreq);
        }
        public static void UpdateSemesterEntryScore(params UpdateInfo[] infos)
        {
            string req = "<UpdateRequest>";
            foreach (UpdateInfo info in infos)
            {
                int tryParseInt;
                req += "<SemesterEntryScore><Field>" + (int.TryParse(info.GradeYear, out tryParseInt) ? ("<GradeYear>" + info.GradeYear + "</GradeYear> ") : "") + " <ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + info.ID + "</ID></Condition></SemesterEntryScore>";
            }
            req += "</UpdateRequest>";
            DSRequest dsreq = new DSRequest(req);
            DSAServices.CallService("SmartSchool.Score.UpdateSemesterEntryScore", dsreq);
        }
        public static void UpdateSchoolYearSubjectScore(params UpdateInfo[] infos)
        {
            string req = "<UpdateRequest>";
            foreach (UpdateInfo info in infos)
            {
                int tryParseInt;
                req += "<SchoolYearSubjectScore><Field> " + (int.TryParse(info.GradeYear, out tryParseInt) ? ("<GradeYear>" + info.GradeYear + "</GradeYear> ") : "") + " <ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + info.ID + "</ID></Condition></SchoolYearSubjectScore>";
            }
            req += "</UpdateRequest>";
            DSRequest dsreq = new DSRequest(req);
            DSAServices.CallService("SmartSchool.Score.UpdateSchoolYearSubjectScore", dsreq);
        }
        public static void UpdateSchoolYearEntryScore(params UpdateInfo[] infos)
        {
            string req = "<UpdateRequest>";
            foreach (UpdateInfo info in infos)
            {
                int tryParseInt;
                req += "<SchoolYearEntryScore><Field> " + (int.TryParse(info.GradeYear, out tryParseInt) ? ("<GradeYear>" + info.GradeYear + "</GradeYear> ") : "") + " <ScoreInfo>" + info.ScoreInfo.OuterXml + "</ScoreInfo></Field><Condition><ID>" + info.ID + "</ID></Condition></SchoolYearEntryScore>";
            }
            req += "</UpdateRequest>";
            DSRequest dsreq = new DSRequest(req);
            DSAServices.CallService("SmartSchool.Score.UpdateSchoolYearEntryScore", dsreq);
        }



        public class UpdateInfo
        {
            private readonly string _Id, _GradeYear;
            private readonly XmlElement _ScoreInfo;
            public UpdateInfo(string id, string gradeYear, XmlElement scoreInfo)
            {
                _Id = id;
                _GradeYear = gradeYear;
                _ScoreInfo = scoreInfo;
            }
            public string ID { get { return _Id; } }
            public string GradeYear { get { return _GradeYear; } }
            public XmlElement ScoreInfo { get { return _ScoreInfo; } }
        }

        public static void UpdateSemesterMoralScore(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Score.UpdateSemesterMoralScore", dSRequest);
        }
    }
}
