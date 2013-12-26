using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;

namespace JHSchool
{
    public class SemesterHistory : CacheManager<List<SemesterHistoryRecord>>
    {
        private static SemesterHistory _Instance = null;
        public static SemesterHistory Instance { get { if (_Instance == null)_Instance = new SemesterHistory(); return _Instance; } }
        private SemesterHistory() { }

        protected override Dictionary<string, List<SemesterHistoryRecord>> GetAllData()
        {
            return new Dictionary<string, List<SemesterHistoryRecord>>();
        }

        protected override Dictionary<string, List<SemesterHistoryRecord>> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, List<SemesterHistoryRecord>> group = new Dictionary<string, List<SemesterHistoryRecord>>();
            foreach (var item in Feature.QueryStudent.GetSemesterHistories(primaryKeys))
            {
                if (!group.ContainsKey(item.RefStudentID))
                    group.Add(item.RefStudentID, new List<SemesterHistoryRecord>());
                group[item.RefStudentID].Add(item);
            }

            Dictionary<string, List<SemesterHistoryRecord>> result = new Dictionary<string, List<SemesterHistoryRecord>>();
            foreach (string primaryKey in primaryKeys)
            {
                result.Add(primaryKey, (group.ContainsKey(primaryKey) ? group[primaryKey] : new List<SemesterHistoryRecord>()));
            }
            return result;
        }
    }

    public static class StudentRecord_ExtendMethods
    {
        /// <summary>
        /// 取得學生學期歷程資料。
        /// </summary>
        /// <param name="studentRec"></param>
        /// <returns></returns>
        public static List<SemesterHistoryRecord> GetSemesterHistories(this StudentRecord studentRec)
        {
            return SemesterHistory.Instance[studentRec.ID];
        }

        /// <summary>
        /// 批次同步學期歷程資料。
        /// </summary>
        public static void SyncSemesterHistoryCache(this IEnumerable<StudentRecord> studentRecs)
        {
            SemesterHistory.Instance.SyncDataBackground(studentRecs.AsKeyList());
        }
    }
}
