using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Feature.Legacy;

namespace JHSchool.Legacy.ImportSupport.Lookups
{
    /// <summary>
    /// 代表課程規則 (GraduationPlan) 查照表。
    /// </summary>
    internal class GPLookup
    {
        private Dictionary<string, string> _items;

        public GPLookup()
        {
            _items = new Dictionary<string, string>();

            XmlElement xmlRecords = QueryGraduationPlan.GetGraduationPlanList().GetContent().BaseElement;
            foreach (XmlElement each in xmlRecords.SelectNodes("GraduationPlan"))
            {
                string name = each.SelectSingleNode("Name").InnerText;
                string id = each.GetAttribute("ID");

                _items.Add(name, id);
            }
        }

        public string GetGraduationPlanID(string name)
        {
            if (_items.ContainsKey(name))
                return _items[name];
            else
                return string.Empty;
        }

        public bool Contains(string name)
        {
            return _items.ContainsKey(name);
        }

        public static string Name
        {
            get { return "GPLookup"; }
        }
    }
}
