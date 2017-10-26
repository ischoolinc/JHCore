using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel
{
    public class SheetColumn
    {
        private Cell _binding_cell;
        private BulkColumn _binding_bulk;
        private string _name;

        // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，從使用 byte 改為 int
        //private byte _absolute_index, _relatively_index;

        private int _absolute_index, _relatively_index;

        private bool _is_group_field;
        private bool _used_valid;
        private string _group_name;

        public SheetColumn(Cell bindCell, byte relativelyIndex)
        {
            _name = bindCell.StringValue;
            _absolute_index = bindCell.Column;
            _relatively_index = relativelyIndex;
            _binding_cell = bindCell;

            if (_name.IndexOf(":") > 0)
            {
                _is_group_field = true;
                _group_name = _name.Split(':')[0];
            }
            else
            {
                _is_group_field = false;
                _group_name = _name;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool IsGroupField
        {
            get { return _is_group_field; }
        }

        public string GroupName
        {
            get { return _group_name; }
            private set { _group_name = value; }
        }

        /// <summary>
        /// 取得是否之前曾經驗證過此欄。
        /// </summary>
        public bool UsedValid
        {
            get { return _used_valid; }
            set { _used_valid = value; }
        }


        // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，從使用 byte 改為 int
        /// <summary>
        /// 取的欄位在原始資料中的 Index。
        /// </summary>
        public int AbsoluteIndex
        {
            get
            {
                return _absolute_index;
            }
        }

        // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，從使用 byte 改為 int
        /// <summary>
        /// 取得欄位的順序。
        /// </summary>
        public int RelativelyIndex
        {
            get { return _relatively_index; }
        }

        public Cell BindingCell
        {
            get { return _binding_cell; }
        }

        public void SetStyle(Style style)
        {
            // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
            //_binding_cell.Style = style;

            _binding_cell.SetStyle(style);
        }

        public void SetBulkColumn(BulkColumn column)
        {
            _binding_bulk = column;
            GroupName = column.GroupName;
        }

        public BulkColumn BindingBulkColumn
        {
            get { return _binding_bulk; }
        }
    }
}
