using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool
{
    public class CourseTag : GeneralTag<CourseTagRecord>
    {
        private static CourseTag _instance;

        public static CourseTag Instance
        {
            get
            {
                if (_instance == null) _instance = new CourseTag();
                return _instance;
            }
        }

        protected override string ServiceName
        {
            get { return "SmartSchool.Tag.GetDetailListByCourse"; }
        }

        protected override string EntityConditionName
        {
            get { return "CourseID"; }
        }
    }

    public static class CourseTag_ExtendMethods
    {
        /// <summary>
        /// 取得課程類別資料。
        /// </summary>
        public static List<CourseTagRecord> GetTags(this CourseRecord record)
        {
            return CourseTag.Instance[record.ID];
        }

        /// <summary>
        /// 批次同步課程類別資料，並快取。
        /// </summary>
        public static void SyncTagCache(this IEnumerable<CourseRecord> courses)
        {
            CourseTag.Instance.SyncDataBackground(courses.AsKeyList());
        }
    }
}
