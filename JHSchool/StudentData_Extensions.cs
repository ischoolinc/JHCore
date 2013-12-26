using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool
{
    public static class StudentData_Extensions
    {
        public static List<K12.Data.StudentRecord> FilterStatus(this IEnumerable<K12.Data.StudentRecord> students, params K12.Data.StudentRecord.StudentStatus[] status)
        {
            List<K12.Data.StudentRecord> newstuds = new List<K12.Data.StudentRecord>();
            foreach (K12.Data.StudentRecord each in students)
            {
                foreach (K12.Data.StudentRecord.StudentStatus stas in status)
                {
                    if (each.Status == stas)
                    {
                        newstuds.Add(each);
                        break;
                    }
                }
            }
            return newstuds;
        }
    }
}