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
    class Class
    {
        public static List<int> ListSeatNo(string classID)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("ListSeatNoRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "RefClassID", classID);

            dsreq.SetContent(helper);
            DSResponse rsp = DSAServices.CallService("SmartSchool.Class.ListSeatNo", dsreq);

            List<int> list = new List<int>();
            foreach (XmlNode node in rsp.GetContent().GetElements("SeatNo"))
            {
                int no;
                if (int.TryParse(node.InnerText, out no))
                    list.Add(no);
            }
            return list;
        }

        /// <summary>
        /// 依年級取得班級清單
        /// </summary>
        /// <param name="gradeYear">年級</param>
        /// <returns>
        /// key : 班級編號
        /// value : 班級名稱
        /// </returns>
        public static Dictionary<string, string> GetClassListByGradeYear(string gradeYear)
        {
            DSRequest dsreq = new DSRequest();
            DSXmlHelper helper = new DSXmlHelper("GetClassListRequest");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "GradeYear", gradeYear);
            helper.AddElement("Order");
            helper.AddElement("Order", "DisplayOrder", "ASC");
            helper.AddElement("Order", "ID", "ASC");
            dsreq.SetContent(helper);
            DSResponse rsp = DSAServices.CallService("SmartSchool.Class.GetAbstractListByGradeYear", dsreq);

            Dictionary<string, string> classList = new Dictionary<string, string>();
            foreach (XmlNode node in rsp.GetContent().GetElements("Class"))
            {
                classList.Add(node.Attributes["ID"].Value, node.Attributes["ClassName"].Value);
            }
            return classList;
        }
    }
}
