using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using FISCA.DSAUtil;

namespace JHSchool
{
    public class Department : Framework.CacheManager<DepartmentRecord>
    {
        private static Department _Instance = null;
        public static Department Instance { get { if (_Instance == null)_Instance = new Department(); return _Instance; } }
        private Department()
        {
            
        }

        protected override Dictionary<string, DepartmentRecord> GetAllData()
        {
            Dictionary<string, DepartmentRecord> items = new Dictionary<string, DepartmentRecord>();
            foreach (XmlElement element in JHSchool.Feature.Legacy.Config.GetDepartment().GetContent().GetElements("Department"))
            {
                DepartmentRecord deptRecord = new DepartmentRecord(element);
                items.Add(deptRecord.ID, deptRecord);
            }
            return items;
        }

        protected override Dictionary<string, DepartmentRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, DepartmentRecord> items = new Dictionary<string, DepartmentRecord>();
            foreach (XmlElement element in JHSchool.Feature.Legacy.Config.GetDepartment().GetContent().GetElements("Department"))
            {
                DepartmentRecord deptRecord = new DepartmentRecord(element);
                if (primaryKeys.Contains<string>(deptRecord.ID))
                    items.Add(deptRecord.ID, deptRecord);
            }
            return items;
        }
    }
}
