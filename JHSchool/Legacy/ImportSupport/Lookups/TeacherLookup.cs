using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using JHSchool.Feature.Legacy;

namespace JHSchool.Legacy.ImportSupport.Lookups
{
    public class TeacherLookup
    {
        private Dictionary<string, string> _teachers;
        private Regex _matcher = new Regex(@"^(?<Name>.*[^\(])\s*\(\s*(?<Nickname>.*[^\)])\)?$", RegexOptions.Singleline);

        public TeacherLookup()
        {
            _teachers = new Dictionary<string, string>();

            XmlElement xmlClasses = QueryTeacher.GetTeacherList().GetContent().BaseElement;
            foreach (XmlElement each in xmlClasses.SelectNodes("Teacher"))
            {
                string name = each.SelectSingleNode("TeacherName").InnerText;
                string nick = each.SelectSingleNode("Nickname").InnerText;
                string Id = each.GetAttribute("ID");

                string fullName = string.IsNullOrEmpty(nick) ? name : string.Format("{0}({1})", name, nick);
                _teachers.Add(fullName, Id);
            }
        }

        public string GetTeacherID(string teacherName)
        {
            string fullName;

            if (teacherName.IndexOf('(') >= 0)
            {
                Match m = _matcher.Match(teacherName.Trim());

                string name = m.Groups["Name"].Value.Trim();
                string nickname = m.Groups["Nickname"].Value.Trim();
                fullName = string.IsNullOrEmpty(nickname) ? name : string.Format("{0}({1})", name, nickname);
            }
            else
                fullName = teacherName.Trim();

            if (_teachers.ContainsKey(fullName))
                return _teachers[fullName];
            else
                return string.Empty;
        }

        public bool Contains(string teacherName)
        {
            return GetTeacherID(teacherName) != string.Empty;
        }

        public static string Name
        {
            get { return "TeacherLookup"; }
        }
    }
}
