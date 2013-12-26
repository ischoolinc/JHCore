using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Feature.Legacy;

namespace JHSchool.Legacy.ImportSupport.Lookups
{
    public class ClassLookup
    {
        private Dictionary<string, string> _classes;

        public ClassLookup()
        {
            _classes = new Dictionary<string, string>();

            XmlElement xmlClasses = QueryClass.GetClassList().GetContent().BaseElement;
            foreach (XmlElement each in xmlClasses.SelectNodes("Class"))
            {
                string className = each.SelectSingleNode("ClassName").InnerText;
                string classId = each.GetAttribute("ID");

                _classes.Add(className, classId);
            }
        }

        public string GetClassID(string name)
        {
            if (_classes.ContainsKey(name))
                return _classes[name];
            else
                return string.Empty;
        }

        public bool Contains(string name)
        {
            return _classes.ContainsKey(name);
        }

        public static string Name
        {
            get { return "ClassLookup"; }
        }
    }
}
