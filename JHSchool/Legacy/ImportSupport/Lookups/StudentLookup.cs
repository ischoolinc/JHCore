using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.Feature.Legacy;

namespace JHSchool.Legacy.ImportSupport.Lookups
{
    public class StudentLookup
    {
        private Dictionary<string, string> _students;

        /// <summary>
        /// 建立學生編號找尋器。
        /// </summary>
        /// <param name="fieldName">允許值：ID、IDNumber、StudentNumber</param>
        public StudentLookup(string fieldName)
        {
            _students = new Dictionary<string, string>();

            XmlElement xmlStudents = StudentBulkProcess.GetPrimaryKeyList();
            foreach (XmlElement each in xmlStudents.SelectNodes("Item"))
            {
                string key = each.GetAttribute(fieldName);
                string value = each.GetAttribute("ID");

                if (string.IsNullOrEmpty(key)) continue;

                _students.Add(key, value);
            }
        }

        public bool Contains(string lookValue)
        {
            return _students.ContainsKey(lookValue.Trim());
        }

        public string GetStudentID(string lookValue)
        {
            return _students[lookValue.Trim()];
        }
    }
}
