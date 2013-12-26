using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [Obsolete("過渡時期的Service")]
    [AutoRetryOnWebException()]
    public class QueryStudent 
    {
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
            helper.AddElement("Field", "PermanentPhone");
            helper.AddElement("Field", "ContactPhone");
            helper.AddElement("Field", "Birthdate");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "RefClassID");
            helper.AddElement("Field", "SeatNo");
            helper.AddElement("Field", "TagPrefix");
            helper.AddElement("Field", "TagName");
            helper.AddElement("Field", "TagID");
            helper.AddElement("Field", "TagColor");
            helper.AddElement("Field", "OverrideDeptName");
            helper.AddElement("Field", "LeaveInfo");
            return helper;
        }

        public static DSResponse SearchStudent(string key, int pageSize, int startPage, bool _SearchInStudentID, bool _SearchInSSN, bool _SearchInName, params string[] _StatusList)
        {
            key = "%" + key.Replace("*", "%") + "%";

            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            foreach (string status in _StatusList)
            {
                helper.AddElement("Condition", "Status", status);
            }
            helper.AddElement("Condition", "Search");
            helper.AddElement("Condition/Search", "Or");
            if (_SearchInStudentID)
                helper.AddElement("Condition/Search/Or", "StudentNumber", key);

            if (_SearchInSSN)
                helper.AddElement("Condition/Search/Or", "IDNumber", key);

            if (_SearchInName)
                helper.AddElement("Condition/Search/Or", "Name", key);

            helper.AddElement("Order");
            helper.AddElement("Order", "SeatNo");
            helper.AddElement("Pagination");
            helper.AddElement("Pagination", "PageSize", pageSize.ToString());
            helper.AddElement("Pagination", "StartPage", startPage.ToString());
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.GetAbstractListWithTag", dsreq);
        }

        public static DSResponse GetExtension(string nameSpace, string[] fields, string[] studentIDs)
        {
            if ( studentIDs.Length == 0 ) return null;
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.SetAttribute(".","Namespace", nameSpace);
            helper.AddElement("Field");
            foreach ( string  field in fields )
                helper.AddElement("Field",field);
            if ( fields.Length == 0 )
                helper.AddElement("Field", "All.Field");
            helper.AddElement("Condition");
            foreach ( string id in studentIDs )
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetExtend", dsreq);
            return dsrsp;
        }

        /// <summary>
        /// 取得詳細資料列表
        /// </summary>
        /// <param name="id">學生編號</param>
        /// <returns></returns>
        public static DSResponse GetDetailList(IEnumerable<string> fields, params string[] list)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            bool hasfield = false;
            foreach (string field in fields)
            {
                helper.AddElement("Field", field);
                hasfield = true;
            }
            if (!hasfield)
                throw new Exception("必須傳入Field");
            helper.AddElement("Condition");
            foreach (string id in list)
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }

        public static DSResponse GetDetailList(IEnumerable<string> fields)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            bool hasfield = false;
            foreach (string field in fields)
            {
                helper.AddElement("Field", field);
                hasfield = true;
            }
            if (!hasfield)
                throw new Exception("必須傳入Field");

            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }

        public static DSResponse GetAbstractList()
        {
            //DSRequest dsreq = new DSRequest(
            //    "<GetStudentListRequest><Fields><ID/><StudentID/><Name/><EnglishName/><Gender/><Nationality/><SSN/><NationalityLocation/><EthnicGroup/><ClassID/><GradeYear/><ClassName/><SeatNo/><Status/></Fields>"+
            //    "<Condition><Status>一般</Status><Status>休學</Status><Status>輟學</Status><Status>刪除</Status></Condition>" +
            //    "<Order><GradeYear/><ClassName/><SeatNo/></Order>"+
            //    "</GetStudentListRequest>");
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            //helper.AddElement("Condition", "Status", "一般");
            //helper.AddElement("Condition", "Status", "休學");
            //helper.AddElement("Condition", "Status", "延修");
            //helper.AddElement("Condition", "Status", "刪除");
            helper.AddElement("Order");
            helper.AddElement("Order", "SeatNo");
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetAbstractListWithTag", dsreq);
            //DSResponse dsrsp = CallService("SmartSchool.Student.GetAbstractListWithClassInfo", dsreq);
            return dsrsp;
        }

        public static DSXmlHelper GetAbstractList(params string[] gradeYear)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement(".", "Field", "<ID/><Name/><RefClassID/><ClassName/><SeatNo/><RefDepartmentID/><DepartmentName/><GradeYear/>", true);
            helper.AddElement("Condition");
            helper.AddElement("Condition", "Status", "一般");

            foreach (string each in gradeYear)
                helper.AddElement("Condition", "GradeYear", each);

            helper.AddElement(".", "Order", "<GradeYear/><RefClassID/><SeatNo/>", true);
            dsreq.SetContent(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetAbstractList", dsreq);
            return dsrsp.GetContent();
        }

        public static DSResponse GetAbstractInfo(params string[] list)
        {
            //string req = "<GetStudentListRequest><Fields><ID/><StudentID/><Name/><EnglishName/><Gender/><Nationality/><SSN/><NationalityLocation/><EthnicGroup/><ClassID/><GradeYear/><ClassName/><SeatNo/><Status/></Fields><Condition>";
            //foreach (string var in list)
            //{
            //    req += "<ID>" + var + "</ID>";
            //}
            //req += "</Condition><Order><GradeYear/><ClassName/><SeatNo/></Order></GetStudentListRequest>";
            DSXmlHelper helper = CreateBriefFieldHelper();
            helper.AddElement("Condition");
            foreach (string var in list)
            {
                helper.AddElement("Condition", "ID", var);
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "SeatNo");

            DSRequest dsreq = new DSRequest(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetAbstractListWithTag", dsreq);
            return dsrsp;
        }

        //public static DSResponse GetDefaultUpdateRecordInfo(params string[] list)
        //{
        //    DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
        //    helper.AddElement("Field");
        //    helper.AddElement("Field", "ID");
        //    helper.AddElement("Field", "Status");
        //    helper.AddElement("Field", "Name");
        //    helper.AddElement("Field", "Birthdate");
        //    helper.AddElement("Field", "Gender");
        //    helper.AddElement("Field", "IDNumber");
        //    helper.AddElement("Field", "StudentNumber");
        //    helper.AddElement("Field", "GradeYear");
        //    helper.AddElement("Field", "Department");
        //    helper.AddElement("Field", "LastADDate");
        //    helper.AddElement("Field", "LastADNumber");
        //    helper.AddElement("Field", "LastUpdateCode");
        //    helper.AddElement("Condition");
        //    foreach (string var in list)
        //    {
        //        helper.AddElement("Condition", "ID", var);
        //    }
        //    helper.AddElement("Order");
        //    helper.AddElement("Order", "GradeYear");
        //    helper.AddElement("Order", "ClassName");
        //    helper.AddElement("Order", "SeatNo");

        //    DSRequest dsreq = new DSRequest(helper);
        //    DSResponse dsrsp = CallService("SmartSchool.Student.GetAbstractListWithClassInfo", dsreq);
        //    return dsrsp;
        //}

        public static DSResponse GetExportList(DSRequest request)
        {
            return DSAServices.CallService("SmartSchool.Student.BulkProcessJH.Export", request);
        }

        public static XmlNode GetClassInfo(string studentid)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetClassRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", studentid);
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetClassInfo", dsreq).GetContent().BaseElement;
        }

        public static DSResponse GetPhoneDetail(string id)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "PermanentPhone");
            helper.AddElement("Field", "ContactPhone");
            helper.AddElement("Field", "OtherPhones");
            helper.AddElement("Field", "SMSPhone");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", id);
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetAbstractList", dsreq);
        }

        public static DSResponse GetParentInfo(params  string[] idlist)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetParentInfoRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in idlist)
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetParentInfo", dsreq);
        }

        public static DSResponse GetMultiParentInfo(params string[] idlist)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetParentInfoRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in idlist)
            {
                helper.AddElement("Condition", "ID", id);
            }
            dsreq.SetContent(helper);
            return DSAServices.CallService("SmartSchool.Student.GetMultiParentInfo", dsreq);
        }

        public static DSResponse GetAddress(string RunningID)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "PermanentAddress");
            helper.AddElement("Field", "MailingAddress");
            helper.AddElement("Field", "OtherAddresses");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", RunningID);
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }

        public static DSResponse GetAddressWithID(params string[] RunningID)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "PermanentAddress");
            helper.AddElement("Field", "MailingAddress");
            helper.AddElement("Field", "OtherAddresses");
            helper.AddElement("Condition");
            foreach (string var in RunningID)
            {
                helper.AddElement("Condition", "ID", var);
            }
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }

        public static DSResponse GetAddressWithPhoto(params string[] RunningID)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "PermanentAddress");
            helper.AddElement("Field", "MailingAddress");
            helper.AddElement("Field", "OtherAddresses");
            helper.AddElement("Field", "FreshmanPhoto");
            helper.AddElement("Condition");
            foreach (string var in RunningID)
            {
                helper.AddElement("Condition", "ID", var);
            }
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
        }

        /// <summary>
        /// 取得學生異動資料清單
        /// </summary>
        /// <param name="RunningID"></param>
        /// <returns></returns>
        public static DSResponse GetUpdateInfoList(string RunningID)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateInfoListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "RefStudentID", RunningID);
            helper.AddElement("Order");
            helper.AddElement("Order", "UpdateDate", "desc");
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetDetailList", dsreq);
        }

        public static DSResponse GetUpdateRecordByStudentIDList(params string[] idList)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateInfoListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string id in idList)
            {
                helper.AddElement("Condition", "RefStudentID", id);
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "UpdateDate", "desc");
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetDetailList", dsreq);
        }

        public static DSResponse GetUpdateRecord(string updateid)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateInfoListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", updateid);
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetDetailList", dsreq);
        }

        public static DSResponse GetUpdateRecord(DSRequest req)
        {
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetDetailList", req);
        }

        public static DSResponse GetUpdateRecordByCode(params string[] updateCodes)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateInfoListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string var in updateCodes)
            {
                helper.AddElement("Condition", "UpdateCode", var);
            }
            helper.AddElement("Condition", "Special");
            helper.AddElement("Condition/Special", "NonADNumber");
            helper.AddElement("Condition/Special", "StudentNotDeleted");
            helper.AddElement("Order");
            helper.AddElement("Order", "UpdateDate");
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetDetailList", dsreq);
        }

        //國中異動不使用異動代號，故使用異動原因及事項
        public static DSResponse GetUpdateRecordByDescription(params string[] updateDescs)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateInfoListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            foreach (string var in updateDescs)
            {
                helper.AddElement("Condition", "UpdateDescription", var);
            }
            helper.AddElement("Order");
            helper.AddElement("Order", "UpdateDate");
            DSRequest dsreq = new DSRequest(helper);
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetDetailList", dsreq);
        }


        /// <summary>
        /// 取得異動名冊學年度列表
        /// </summary>
        /// <returns></returns>
        public static DSResponse GetSchoolYearList()
        {
            DSXmlHelper helper = new DSXmlHelper("GetSchoolYearListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Order");
            helper.AddElement("Order", "SchoolYear", "desc");
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetSchoolYearList", new DSRequest(helper));
        }

        /// <summary>
        /// 取得異動名冊資料
        /// </summary>
        /// <param name="id">名冊編號</param>
        /// <returns>結果</returns>
        public static DSResponse GetUpdateRecordBatch(string id)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateRecordBatchRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", id);
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetBatchDetailList", new DSRequest(helper));
        }

        /// <summary>
        /// 由學年度取得名冊清單
        /// </summary>
        /// <param name="schoolYear">學年度</param>
        /// <returns>結果 DSResponse</returns>
        public static DSResponse GetUpdateRecordBatchBySchoolYear(string schoolYear)
        {
            DSXmlHelper helper = new DSXmlHelper("GetUpdateRecordBatchRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ADNumber");
            helper.AddElement("Field", "Name");
            helper.AddElement("Field", "ID");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolYear);
            helper.AddElement("Order");
            helper.AddElement("Order", "ADDate", "desc");
            helper.AddElement("Order", "ID", "desc");
            return DSAServices.CallService("SmartSchool.Student.UpdateRecord.GetBatchDetailList", new DSRequest(helper));
        }

        public static bool IDNumberExists(string identity, string idNumber)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement(".", "Field", "<ID/><Name/>", true);
            helper.AddElement(".", "Condition", "<IDNumber>" + idNumber + "</IDNumber>", true);
            helper.AddElement("Condition", "Status", "一般");

            DSXmlHelper response = DSAServices.CallService("SmartSchool.Student.GetDetailList", new DSRequest(helper)).GetContent();

            return response.GetElements("Student[@ID!='" + identity + "']").Length > 0;
        }

        public static bool StudentNumberExists(string identity, string studentNumber)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement(".", "Field", "<ID/><Name/>", true);
            helper.AddElement(".", "Condition", "<StudentNumber>" + studentNumber + "</StudentNumber>", true);
            helper.AddElement("Condition", "Status", "一般");

            DSXmlHelper response = DSAServices.CallService("SmartSchool.Student.GetDetailList", new DSRequest(helper)).GetContent();

            return response.GetElements("Student[@ID!='" + identity + "']").Length > 0;
        }

        public static bool LoginIDExists(string loginID, string RunningID)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SALoginName", loginID);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetDetailList", new DSRequest(helper));
            if (dsrsp.GetContent().GetElements("Student").Length == 0)
                return false;
            if (dsrsp.GetContent().GetElement("Student").GetAttribute("ID") == RunningID)
                return false;
            return true;
        }

        public static DSResponse GetStudentExamScore(DSRequest dSRequest)
        {
            return DSAServices.CallService("SmartSchool.Student.GetStudentExamScore", dSRequest);
        }

        public static DSResponse GetStudentMajorCourse(DSRequest dSRequest)
        {
            return DSAServices.CallService("SmartSchool.Student.GetMajorCourse", dSRequest);
        }

        public static DSResponse GetDetailListByClassID(params string[] classId)
        {
            DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "CustodianName");
            helper.AddElement("Field", "SMSPhone");
            helper.AddElement("Field", "MailingAddress");
            helper.AddElement("Condition");

            foreach (string var in classId)
            {
                helper.AddElement("Condition", "RefClassID", var);
            }

            DSRequest dsreq = new DSRequest(helper);
            DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.GetDetailList", dsreq);
            return dsrsp;
        }


    }
}
