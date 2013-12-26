using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using Framework;
using Framework.Feature;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public static class Config
    {
        public static DSResponse GetDepartment()
        {
            DSXmlHelper helper = new DSXmlHelper("GetDepartmentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            return DSAServices.CallService("SmartSchool.Config.GetDepartment", new DSRequest(helper));
        }

        public static DSResponse GetNationalityList()
        {
            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetNationalityListRequest");
            helper.AddElement("Fields");
            helper.AddElement("Fields", "All");
            request.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Config.GetNationalityList", request);
        }

        public static DSResponse GetEduDegreeList()
        {
            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetEducationDegreeListRequest");
            helper.AddElement("Fields");
            helper.AddElement("Fields", "All");
            request.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Config.GetEducationDegreeList", request);
        }

        public static DSResponse GetJobList()
        {
            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetJobListRequest");
            helper.AddElement("Fields");
            helper.AddElement("Fields", "All");
            request.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Config.GetJobList", request);
        }

        public static DSResponse GetRelationship()
        {
            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetRelationshipListRequest");
            helper.AddElement("Fields");
            helper.AddElement("Fields", "All");
            request.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Config.GetRelationshipList", request);
        }

        public static List<string> GetCountyList()
        {
            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Fields");
            helper.AddElement("Fields", "All");
            request.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Config.GetCountyTownList", request);

            List<string> countyList = new List<string>();
            foreach (XmlNode node in dsrsp.GetContent().GetElements("Town"))
            {
                string county = node.Attributes["County"].Value;
                if (!countyList.Contains(county))
                    countyList.Add(county);
            }
            return countyList;
        }

        public static DSResponse GetAbsenceList()
        {
            string serviceName = "GetAbsenceList";
            if (DataCacheManager.Get(serviceName) == null)
            {
                DSRequest request = new DSRequest();
                DSXmlHelper helper = new DSXmlHelper("GetAbsenceListRequest");
                helper.AddElement("Field");
                helper.AddElement("Field", "All");
                request.SetContent(helper);
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Others.GetAbsenceList", request);
                DataCacheManager.Add(serviceName, dsrsp);
            }
            return DataCacheManager.Get(serviceName);
        }

        public static XmlElement[] GetTownList(string county)
        {
            DSResponse dsrsp = DataCacheManager.Get("CountyTownBelong");

            if (dsrsp == null)
                return new XmlElement[0];
            else
                return dsrsp.GetContent().GetElements("Town[@County='" + county + "']");
        }

        public static KeyValuePair<string, string> FindTownByZipCode(string zipCode)
        {
            DSResponse dsrsp = DataCacheManager.Get("CountyTownBelong");
            XmlNode node = dsrsp.GetContent().GetElement("Town[@Code='" + zipCode + "']");
            KeyValuePair<string, string> kvp = new KeyValuePair<string, string>();
            if (node != null)
            {
                kvp = new KeyValuePair<string, string>(node.Attributes["County"].Value, node.Attributes["Name"].Value);
            }
            return kvp;
        }

        public static DSResponse GetWordCommentList()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "Content");
            return DSAServices.CallService("SmartSchool.Config.GetWordCommentList", new DSRequest(helper));
        }

        public static DSResponse GetMoralDiffItemList()
        {
            return DSAServices.CallService("SmartSchool.Config.GetMoralDiffItemList", new DSRequest());
        }

        public static decimal GetSupervisedRatingRange()
        {
            DSResponse rsp = DSAServices.CallService("SmartSchool.Config.GetSupervisedRatingRange", new DSRequest());
            XmlElement range = rsp.GetContent().GetElement("RatingRange");

            if (range == null || range.InnerText == string.Empty)
                return 32767;
            else
                return decimal.Parse(range.InnerText);
        }

        public static DSResponse GetMoralCommentCodeList()
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "Content");
            return DSAServices.CallService("SmartSchool.Config.GetMoralCommentCodeList", new DSRequest(helper));
        }


        public const string LIST_PERIODS = "GetPeriodList";
        public const string LIST_PERIODS_NUMBER = "35";
        public static DSResponse GetPeriodList()
        {
            string serviceName = LIST_PERIODS;
            if (DataCacheManager.Get(serviceName) == null)
            {
                DSRequest request = new DSRequest();
                DSXmlHelper helper = new DSXmlHelper("GetPeriodListRequest");
                helper.AddElement("Field");
                helper.AddElement("Field", "All");
                request.SetContent(helper);
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Others.GetPeriodList", request);
                DataCacheManager.Add(serviceName, dsrsp);
            }
            return DataCacheManager.Get(serviceName);
        }

        public static DSResponse GetDisciplineReasonList()
        {
            //string serviceName = "GetDisciplineReasonList";

            // 拿掉快取
            //if (DataCacheManager.Get(serviceName) == null)
            //{

            DSRequest request = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetDisciplineReasonListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            request.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Config.GetDisciplineReasonList", request);
            return dsrsp;

            //DataCacheManager.Add(serviceName, dsrsp);
            //}
            //return DataCacheManager.Get(serviceName);
        }

        public static DSResponse GetScoreEntryList()
        {
            string serviceName = "GetScoreEntryList";
            if (DataCacheManager.Get(serviceName) == null)
            {
                DSRequest request = new DSRequest();
                DSXmlHelper helper = new DSXmlHelper("GetScoreEntryList");
                helper.AddElement("Field");
                helper.AddElement("Field", "All");
                request.SetContent(helper);
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Config.GetScoreEntryList", request);
                DataCacheManager.Add(serviceName, dsrsp);
            }
            return DataCacheManager.Get(serviceName);
        }

        public const string LIST_SCHOOL_LOCATION = "GetSchoolLocationList";
        public static DSResponse GetSchoolLocationList()
        {
            if (DataCacheManager.Get(LIST_SCHOOL_LOCATION) == null)
            {
                DSXmlHelper helper = new DSXmlHelper("GetSchoolLocationListRequest");
                helper.AddElement("Field");
                helper.AddElement("Field", "All");
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Config.GetSchoolLocationList", new DSRequest(helper));
                DataCacheManager.Add(LIST_SCHOOL_LOCATION, dsrsp);
            }
            return DataCacheManager.Get(LIST_SCHOOL_LOCATION);
        }
    }
}
