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
            SheetColumn column = ResolveColumn(fieldName);
            string retVal = "" + _sheet.Cells[AbsoluteIndex, column.AbsoluteIndex].StringValue;

            if (retVal.Contains("\b"))
                retVal = retVal.Replace("\b", "");            
            
//            return "" + _sheet.Cells[AbsoluteIndex, column.AbsoluteIndex].StringValue;
            return retVal;
        }

        public Cell GetCell(string fieldName)
        {
            SheetColumn column = ResolveColumn(fieldName);
            return _sheet.Cells[AbsoluteIndex, column.AbsoluteIndex];
        }

        /// <summary>
        /// 以內部名稱或家長1／家長2 別名解析欄位。找不到時拋出明確例外。
        /// </summary>
        public SheetColumn ResolveColumn(string fieldName)
        {
            SheetColumn column;
            if (TryResolveColumn(fieldName, out column))
                return column;

            throw new ArgumentException(
                "來源資料中不包含此欄位。(" + fieldName + ")。" +
                "若為家長欄位，請確認 Excel 標題與內部欄位（家長1↔父親、家長2↔母親）對應正確。");
        }

        /// <summary>
        /// 嘗試以內部名稱或家長1／家長2 別名解析欄位。
        /// </summary>
        public bool TryResolveColumn(string fieldName, out SheetColumn column)
        {
            column = null;
            if (string.IsNullOrEmpty(fieldName) || _columns == null)
                return false;

            if (_columns.ContainsKey(fieldName))
            {
                column = _columns[fieldName];
                return column != null;
            }

            string internalName = NormalizeParentImportFieldName(fieldName);
            if (internalName != fieldName && _columns.ContainsKey(internalName))
            {
                column = _columns[internalName];
                return column != null;
            }

            string aliasName = DenormalizeParentImportFieldName(fieldName);
            if (aliasName != fieldName && _columns.ContainsKey(aliasName))
            {
                column = _columns[aliasName];
                return column != null;
            }

            return false;
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
        /// 同一內部欄位若同時存在新舊 Excel 標題，優先保留家長1／家長2，略過對應的父親／母親舊欄。
        /// </summary>
        public void NormalizeParentImportFieldNames()
        {
            SheetColumnCollection normalized = new SheetColumnCollection();
            Dictionary<string, bool> preferredInternalNames = new Dictionary<string, bool>();

            // Pass 1：蒐集由家長1／家長2 Excel 欄位對應出的內部欄位名稱。
            foreach (SheetColumn column in _columns.Values)
            {
                string sourceName = column.Name;
                if (IsPreferredParentSourceName(sourceName))
                {
                    string preferredInternalName = NormalizeParentImportFieldName(sourceName);
                    if (!preferredInternalNames.ContainsKey(preferredInternalName))
                        preferredInternalNames.Add(preferredInternalName, true);
                }
            }

            // Pass 2：正規化欄位；舊父親／母親欄位僅在沒有對應家長1／家長2 時保留。
            foreach (SheetColumn column in _columns.Values)
            {
                string sourceName = column.Name;
                string internalName = NormalizeParentImportFieldName(sourceName);

                if (IsLegacyParentSourceName(sourceName) &&
                    preferredInternalNames.ContainsKey(internalName))
                    continue;

                column.SetInternalName(internalName);

                if (normalized.ContainsKey(column.Name))
                    throw new ArgumentException("重覆的欄位名稱：" + column.Name);

                normalized.Add(column.Name, column);
            }

            _columns = normalized;
        }

        private static bool IsPreferredParentSourceName(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
                return false;

            return sourceName.StartsWith("家長1") || sourceName.StartsWith("家長2");
        }

        private static bool IsLegacyParentSourceName(string sourceName)
        {
            if (string.IsNullOrEmpty(sourceName))
                return false;

            return sourceName.StartsWith("父親") || sourceName.StartsWith("母親");
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

        /// <summary>
        /// 內部匯入欄位名稱 → Excel／UI 家長別名。
        /// 例如：父親姓名 → 家長1姓名、母親:學歷 → 家長2:學歷。
        /// </summary>
        public static string DenormalizeParentImportFieldName(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return fieldName;

            if (fieldName.StartsWith("父親"))
                return "家長1" + fieldName.Substring("父親".Length);

            if (fieldName.StartsWith("母親"))
                return "家長2" + fieldName.Substring("母親".Length);

            return fieldName;
        }
    }
}