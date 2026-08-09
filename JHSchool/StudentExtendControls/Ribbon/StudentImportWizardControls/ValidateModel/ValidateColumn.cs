using System;
using System.Collections.Generic;
using System.Text;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class ValidateColumn
    {
        private string _name;
        private byte _index;
        private SheetColumn _column;

        public ValidateColumn(SheetColumn bindColumn, byte index)
        {
            _name = bindColumn.Name;
            _index = index;
            _column = bindColumn;
        }

        /// <summary>
        /// 內部匯入欄位名稱（父親／母親），供驗證規則與 SheetReader 查詢。
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// 資料修正頁／使用者可見欄位名稱，優先使用 Excel 原始標題（家長1／家長2）。
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (_column != null && !string.IsNullOrEmpty(_column.SourceName))
                    return _column.SourceName;
                return _name;
            }
        }

        public byte Index
        {
            get { return _index; }
        }
        public SheetColumn BindingSheetColumn
        {
            get { return _column; }
        }
    }
}
