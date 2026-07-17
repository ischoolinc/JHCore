using System;
using System.Collections.Generic;
using System.Text;
using DocValidate;
using System.Collections;
using Aspose.Cells;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel
{
    public class SheetReader
    {
        private Worksheet _sheet;
        private SheetColumnCollection _columns;
        private SheetColumn _key_column;
        private int _data_start_row, _data_end_row, _current_row;
        private byte _data_start_column;

        public SheetReader()
        {
        }

        public void BindSheet(Worksheet sheet, byte startColumn, int startRow)
        {
            _sheet = sheet;
            _data_start_column = startColumn;
            _data_start_row = startRow + 1;
            _data_end_row = Sheet.Cells.MaxDataRow;
            _columns = new SheetColumnCollection();
            RelativelyIndex = -1;

            CreateColumns(sheet, startColumn, startRow);
        }

        /// <summary>
        /// 設定識別欄位，會決定資料的最大筆數。
        /// </summary>
        /// <param name="columnName"></param>
        public void SetKeyColumn(string columnName)
        {
            SheetColumn column = Columns[columnName];
            _data_end_row = Sheet.Cells.MaxDataRowInColumn(column.AbsoluteIndex);
            _key_column = column;
        }

        /// <summary>
        /// 將公式轉換成值。
        /// </summary>
        public void ConvertFormulaToValue()
        {
            foreach (Cell cell in _sheet.Cells)
            {
                if (cell.IsFormula)
                    cell.PutValue(cell.StringValue);
            }
        }

        public SheetColumn KeyColumn
        {
            get { return _key_column; }
        }

        public SheetColumnCollection Columns
        {
            get
            {
                return _columns;
            }
        }

        public byte AbsoluteStartColumnIndex
        {
            get { return _data_start_column; }
        }

        public int AbsoluteStartRowIndex
        {
            get { return _data_start_row; }
        }

        public int AbsoluteIndex
        {
            get
            {
                return RelativelyIndex + _data_start_row;
            }
        }

        public int RelativelyIndex
        {
            get
            {
                return _current_row;
            }
            private set
            {
                _current_row = value;
            }
        }

        public int RowCount
        {
            get
            {
                return _data_end_row - _data_start_row;
            }
        }

        public Worksheet Sheet
        {
            get { return _sheet; }
        }

        public string GetValue(string fieldName)
        {
            SheetColumn column = Columns[fieldName];
            string retVal = "" + _sheet.Cells[AbsoluteIndex, column.AbsoluteIndex].StringValue;

            if (retVal.Contains("\b"))
                retVal = retVal.Replace("\b", "");            

//            return "" + _sheet.Cells[AbsoluteIndex, column.AbsoluteIndex].StringValue;
            return retVal;
        }

        public Cell GetCell(string fieldName)
        {
            SheetColumn column = Columns[fieldName];
            return _sheet.Cells[AbsoluteIndex, column.AbsoluteIndex];
        }

        public void Reset()
        {
            RelativelyIndex = -1;
        }

        public bool MovePrevious()
        {
            if (AbsoluteIndex <= _data_start_row)
                return false;

            RelativelyIndex--;
            return true;
        }

        public bool MoveNext()
        {
            if (AbsoluteIndex >= _data_end_row)
                return false;

            RelativelyIndex++;
            return true;
        }

        public void MoveTo(int relativelyIndex)
        {
            RelativelyIndex = relativelyIndex;
        }

        private void CreateColumns(Worksheet sheet, int startColumn, int startRow)
        {
            Cell startCell = Sheet.Cells[startRow, startColumn];
            for (int i = startColumn; i <= sheet.Cells.MaxDataColumn; i++)
            {
                Cell colCell = Sheet.Cells[startRow, i];
                if (colCell.StringValue != null && colCell.StringValue.Trim() != string.Empty)
                {
                    SheetColumn objCol = new SheetColumn(colCell, (byte)_columns.Count);

                    if (_columns.ContainsKey(objCol.Name))
                        throw new ArgumentException("重覆的欄位名稱：" + objCol.Name);

                    _columns.Add(objCol.Name, objCol);
                }
            }
        }

        /// <summary>
        /// 將 Excel 家長別名欄位（家長1／家長2）正規化為內部欄位名稱（父親／母親）。
        /// 必須在 BindSheet 之後、欄位比對與產生 XML 之前呼叫。
        /// </summary>
        public void NormalizeParentImportFieldNames()
        {
            SheetColumnCollection normalized = new SheetColumnCollection();

            foreach (SheetColumn column in _columns.Values)
            {
                string internalName = NormalizeParentImportFieldName(column.Name);
                column.SetInternalName(internalName);

                if (normalized.ContainsKey(column.Name))
                    throw new ArgumentException("重覆的欄位名稱：" + column.Name);

                normalized.Add(column.Name, column);
            }

            _columns = normalized;
        }

        /// <summary>
        /// Excel 欄位標題 → 內部匯入欄位名稱。
        /// 例如：家長1姓名 → 父親姓名、家長2:學歷 → 母親:學歷。
        /// </summary>
        public static string NormalizeParentImportFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return fieldName;

            if (fieldName.StartsWith("家長1"))
                return "父親" + fieldName.Substring("家長1".Length);

            if (fieldName.StartsWith("家長2"))
                return "母親" + fieldName.Substring("家長2".Length);

            return fieldName;
        }
    }
}