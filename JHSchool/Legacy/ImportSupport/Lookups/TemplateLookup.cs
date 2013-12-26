using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Feature.Legacy;

namespace JHSchool.Legacy.ImportSupport.Lookups
{
    public class TemplateLookup
    {
        private Dictionary<string, string> _lookup_table;

        public TemplateLookup()
        {
            _lookup_table = new Dictionary<string, string>();

            XmlElement xmlRecords = QueryTemplate.GetAbstractList();
            foreach (XmlElement each in xmlRecords.SelectNodes("ExamTemplate"))
            {
                string name = each.SelectSingleNode("TemplateName").InnerText;
                string id = each.GetAttribute("ID");

                _lookup_table.Add(name, id);
            }
        }

        public string GetTemplateID(string name)
        {
            if (_lookup_table.ContainsKey(name))
                return _lookup_table[name];
            else
                return string.Empty;
        }

        public bool Contains(string name)
        {
            return _lookup_table.ContainsKey(name);
        }

        public static string Name
        {
            get { return "TemplateLookup"; }
        }
    }
}
