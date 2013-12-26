using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public class RemoveScore
    {
        public static void DeleteSemesterEntityScore(params  string[] idlist)
        {
            string req = "<DeleteRequest><SemesterEntryScore>";
            foreach (string id in idlist)
            {
                req += "<ID>" + id + "</ID>";
            }
            req += "</SemesterEntryScore></DeleteRequest>";
            DSRequest dsreq = new DSRequest(req);
            //DSRequest dsreq = new DSRequest("<DeleteRequest><SemesterEntryScore><ID>"+id+"</ID></SemesterEntryScore></DeleteRequest>");
            DSAServices.CallService("SmartSchool.Score.DeleteSemesterEntryScore", dsreq);
        }
        public static void DeleteSemesterSubjectScore(params  string[] idlist)
        {
            string req = "<DeleteRequest><SemesterSubjectScore>";
            foreach (string id in idlist)
            {
                req += "<ID>" + id + "</ID>";
            }
            req += "</SemesterSubjectScore></DeleteRequest>";
            DSRequest dsreq = new DSRequest(req);

            //DSRequest dsreq = new DSRequest("<DeleteRequest><SemesterSubjectScore><ID>" + id + "</ID></SemesterSubjectScore></DeleteRequest>");
            DSAServices.CallService("SmartSchool.Score.DeleteSemesterSubjectScore", dsreq);
        }
        public static void DeleteSchoolYearEntityScore(params  string[] idlist)
        {
            string req = "<DeleteRequest><SchoolYearEntryScore>";
            foreach (string id in idlist)
            {
                req += "<ID>" + id + "</ID>";
            }
            req += "</SchoolYearEntryScore></DeleteRequest>";
            DSRequest dsreq = new DSRequest(req);

            //DSRequest dsreq = new DSRequest("<DeleteRequest><SchoolYearEntryScore><ID>" + id + "</ID></SchoolYearEntryScore></DeleteRequest>");
            DSAServices.CallService("SmartSchool.Score.DeleteSchoolYearEntryScore", dsreq);
        }
        public static void DeleteSchoolYearSubjectScore(params  string[] idlist)
        {
            string req = "<DeleteRequest><SchoolYearSubjectScore>";
            foreach (string id in idlist)
            {
                req += "<ID>" + id + "</ID>";
            }
            req += "</SchoolYearSubjectScore></DeleteRequest>";
            DSRequest dsreq = new DSRequest(req);

            //DSRequest dsreq = new DSRequest("<DeleteRequest><SchoolYearSubjectScore><ID>" + id + "</ID></SchoolYearSubjectScore></DeleteRequest>");
            DSAServices.CallService("SmartSchool.Score.DeleteSchoolYearSubjectScore", dsreq);
        }

        public static void DeleteSemesterMoralScore(DSRequest dSRequest)
        {
            DSAServices.CallService("SmartSchool.Score.DeleteSemesterMoralScore", dSRequest);
        }
    }
}
