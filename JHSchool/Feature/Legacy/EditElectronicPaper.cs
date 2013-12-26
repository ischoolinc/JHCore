using System;
using System.Collections.Generic;
using System.Text;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature.Legacy
{
    public static class EditElectronicPaper
    {
        public static string Insert(string name, string schoolYear, string semester, string viewerType, Dictionary<string, string> metadata)
        {
            DSXmlHelper dsreq = new DSXmlHelper("Request");
            dsreq.AddElement("ElectronicPaper");
            dsreq.AddElement("ElectronicPaper", "Name", name);
            dsreq.AddElement("ElectronicPaper", "SchoolYear", schoolYear);
            dsreq.AddElement("ElectronicPaper", "Semester", semester);
            dsreq.AddElement("ElectronicPaper", "ViewerType", viewerType);

            if (metadata != null)
            {
                DSXmlHelper hlpmd = new DSXmlHelper("Metadata");
                foreach (KeyValuePair<string, string> each in metadata)
                {
                    XmlElement item = hlpmd.AddElement("Item");
                    item.SetAttribute("Name", each.Key);
                    item.SetAttribute("Value", each.Value);
                }
                dsreq.AddElement("ElectronicPaper", hlpmd.BaseElement);
            }

            DSResponse dsrsp = DSAServices.CallService("SmartSchool.ElectronicPaper.Insert", new DSRequest(dsreq));
            if (dsrsp.HasContent)
            {
                DSXmlHelper helper = dsrsp.GetContent();
                string newid = helper.GetText("NewID");
                return newid;
            }
            return "";
        }

        public static void UpdatePaperName(string new_name, string id)
        {
            DSXmlHelper dsreq = new DSXmlHelper("Request");
            dsreq.AddElement("ElectronicPaper");
            dsreq.AddElement("ElectronicPaper", "Name", new_name);
            dsreq.AddElement("ElectronicPaper", "Condition");
            dsreq.AddElement("ElectronicPaper/Condition", "ID", id);
            DSAServices.CallService("SmartSchool.ElectronicPaper.Update", new DSRequest(dsreq));
        }

        public static void Delete(string id)
        {
            DSXmlHelper dsreq = new DSXmlHelper("Request");
            dsreq.AddElement("ElectronicPaper");
            dsreq.AddElement("ElectronicPaper", "ID", id);
            DSAServices.CallService("SmartSchool.ElectronicPaper.Delete", new DSRequest(dsreq));
        }

        public static void InsertPaperItem(DSXmlHelper request)
        {
            DSAServices.CallService("SmartSchool.ElectronicPaper.InsertPaperItem", new DSRequest(request));
        }

        public static void DeletePaperItem(params string[] item_ids)
        {
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Paper");
            foreach (string each_id in item_ids)
                helper.AddElement("Paper", "PaperItemID", each_id);
            DSAServices.CallService("SmartSchool.ElectronicPaper.DeletePaperItem", new DSRequest(helper));
        }
    }
}
