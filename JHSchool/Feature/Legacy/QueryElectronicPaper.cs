using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    [AutoRetryOnWebException()]
    public static class QueryElectronicPaper
    {
        public static DSResponse GetPaperItemByViewer(string viewer_type, string viewer_id)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("ID");
            helper.AddElement("Format");
            //helper.AddElement("Content");
            helper.AddElement("PaperID");
            helper.AddElement("PaperName");
            helper.AddElement("Timestamp");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ViewerType", viewer_type);
            helper.AddElement("Condition", "ViewerID", viewer_id);
            return DSAServices.CallService("SmartSchool.ElectronicPaper.GetPaperItem", new DSRequest(helper));
        }

        public static DSResponse GetPaperItemContentById(string id)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Content");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "ID", id);
            return DSAServices.CallService("SmartSchool.ElectronicPaper.GetPaperItem", new DSRequest(helper));
        }

        public static DSResponse GetDetailList(string schoolyear, string semester)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "SchoolYear", schoolyear);
            helper.AddElement("Condition", "Semester", semester);
            return DSAServices.CallService("SmartSchool.ElectronicPaper.GetDetailList", new DSRequest(helper));
        }
    }
}
