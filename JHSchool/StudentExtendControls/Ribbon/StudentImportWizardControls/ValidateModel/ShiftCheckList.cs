using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel;
using JHSchool.Feature.Legacy;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class ShiftCheckList
    {
        private Dictionary<string, string> _checkList;
        private BulkColumn _key, _shift;

        public ShiftCheckList(BulkColumn key, BulkColumn shift)
        {
            _checkList = new Dictionary<string, string>();
            _key = key;
            _shift = shift;
        }

        public void LoadCheckList()
        {
            XmlElement checklist = StudentBulkProcess.GetShiftCheckList(_key.Name, _shift.Name);

            foreach (XmlElement each in checklist.SelectNodes("Student"))
            {
                string key = each.GetAttribute(_key.Name);
                string value = each.GetAttribute(_shift.Name).Trim();

                if (string.IsNullOrEmpty(key)) continue; //沒有 Key 就 Next

                _checkList.Add(key, value);
            }
        }

        public bool ContainKey(string key)
        {
            return _checkList.ContainsKey(key);
        }

        public string GetCheckValue(string key)
        {
            return _checkList[key];
        }
    }
}
