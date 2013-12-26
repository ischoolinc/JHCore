using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool
{
    public class TeacherTag : GeneralTag<TeacherTagRecord>
    {
        private static TeacherTag _instance;

        public static TeacherTag Instance
        {
            get
            {
                if (_instance == null) _instance = new TeacherTag();
                return _instance;
            }
        }

        protected override string ServiceName
        {
            get { return "SmartSchool.Tag.GetDetailListByTeacher"; }
        }

        protected override string EntityConditionName
        {
            get { return "TeacherID"; }
        }
    }

    public static class TeacherTag_ExtendMethods
    {
        /// <summary>
        /// 取得教師類別資料。
        /// </summary>
        public static List<TeacherTagRecord> GetTags(this TeacherRecord record)
        {
            return TeacherTag.Instance[record.ID];
        }

        /// <summary>
        /// 批次同步教師類別資料，並快取。
        /// </summary>
        public static void SyncTagCache(this IEnumerable<TeacherRecord> teachers)
        {
            TeacherTag.Instance.SyncDataBackground(teachers.AsKeyList());
        }
    }
}
