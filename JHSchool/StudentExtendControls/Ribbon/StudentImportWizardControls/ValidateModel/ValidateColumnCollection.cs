using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class ValidateColumnCollection : Dictionary<string, ValidateColumn>
    {
        public string[] GetNames()
        {
            List<string> names = new List<string>(Keys);
            return names.ToArray();
        }

        /// <summary>
        /// 依驗證訊息欄位名稱解析 ValidateColumn。
        /// 支援內部名稱（父親／母親）與 Excel 別名（家長1／家長2）對應。
        /// </summary>
        public bool TryResolveColumn(string columnName, out ValidateColumn column)
        {
            column = null;
            if (string.IsNullOrEmpty(columnName))
                return false;

            if (TryGetValue(columnName, out column) && column != null)
                return true;

            string internalName = SheetReader.NormalizeParentImportFieldName(columnName);
            if (!string.Equals(internalName, columnName, StringComparison.Ordinal) &&
                TryGetValue(internalName, out column) && column != null)
                return true;

            string aliasName = SheetReader.DenormalizeParentImportFieldName(columnName);
            if (!string.Equals(aliasName, columnName, StringComparison.Ordinal) &&
                TryGetValue(aliasName, out column) && column != null)
                return true;

            column = null;
            return false;
        }
    }
}
