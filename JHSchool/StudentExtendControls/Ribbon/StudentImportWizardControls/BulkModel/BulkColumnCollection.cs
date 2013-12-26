using System;
using System.Collections.Generic;
using System.Text;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel
{
    public class BulkColumnCollection : Dictionary<string, BulkColumn>
    {
        public string[] GetNames()
        {
            List<string> names = new List<string>(Keys);
            return names.ToArray();
        }

        /// <summary>
        /// 取得與 columns 參數交集的欄位資訊。
        /// </summary>
        public BulkColumnCollection GetInstersection(string[] columns)
        {
            BulkColumnCollection cols = new BulkColumnCollection();
            foreach (string each in columns)
            {
                if (ContainsKey(each))
                {
                    BulkColumn column = this[each];
                    cols.Add(column.FullDisplayText, column);
                }
            }

            return cols;
        }

        public BulkColumnCollection GetColumnList(string[] columns)
        {
            return GetInstersection(columns);
        }

        public bool ContainGroup(string groupName)
        {
            foreach (BulkColumn each in Values)
            {
                if (each.GroupName == groupName)
                    return true;
            }

            return false;
        }
    }
}
