using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Data
{
    public static class Student_Extensions
    {
        /// <summary>
        /// 取得學生物件的「系統編號」清單。
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        public static List<string> ToKeys(this IEnumerable<JHStudentRecord> students)
        {
            List<string> keys = new List<string>();
            foreach (JHStudentRecord each in students)
                keys.Add(each.ID);
            return keys;
        }

        /// <summary>
        /// 將學生物件轉換成 Dictionary 物件。
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        public static Dictionary<string, JHStudentRecord> ToDictionary(this IEnumerable<JHStudentRecord> students)
        {
            Dictionary<string, JHStudentRecord> dicstuds = new Dictionary<string, JHStudentRecord>();
            foreach (JHStudentRecord each in students)
                dicstuds.Add(each.ID, each);
            return dicstuds;
        }
    }
}
