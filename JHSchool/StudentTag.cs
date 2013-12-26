using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool
{
    public class StudentTag : GeneralTag<StudentTagRecord>
    {
        private static StudentTag _instance;

        public static StudentTag Instance
        {
            get
            {
                if (_instance == null) _instance = new StudentTag();
                return _instance;
            }
        }

        protected override string ServiceName
        {
            get { return "SmartSchool.Tag.GetDetailListByStudent"; }
        }

        protected override string EntityConditionName
        {
            get { return "StudentID"; }
        }
    }

    public static class StudentTag_ExtendMethods
    {
        /// <summary>
        /// 取得學生類別資料。
        /// </summary>
        public static List<StudentTagRecord> GetTags(this StudentRecord student)
        {
            return StudentTag.Instance[student.ID];
        }

        /// <summary>
        /// 批次同步學生類別資料，並快取。
        /// </summary>
        public static void SyncTagCache(this IEnumerable<StudentRecord> students)
        {
            StudentTag.Instance.SyncDataBackground(students.AsKeyList());
        }
    }
}
