using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool
{
    public class ClassTag : GeneralTag<ClassTagRecord>
    {
        private static ClassTag _instance;

        public static ClassTag Instance
        {
            get
            {
                if (_instance == null) _instance = new ClassTag();
                return _instance;
            }
        }

        protected override string ServiceName
        {
            get { return "SmartSchool.Tag.GetDetailListByClass"; }
        }

        protected override string EntityConditionName
        {
            get { return "ClassID"; }
        }
    }

    public static class ClassTag_ExtendMethods
    {
        /// <summary>
        /// 取得班級類別資料。
        /// </summary>
        public static List<ClassTagRecord> GetTags(this ClassRecord record)
        {
            return ClassTag.Instance[record.ID];
        }

        /// <summary>
        /// 批次同步班級類別資料，並快取。
        /// </summary>
        public static void SyncTagCache(this IEnumerable<ClassRecord> classes)
        {
            ClassTag.Instance.SyncDataBackground(classes.AsKeyList());
        }
    }
}
