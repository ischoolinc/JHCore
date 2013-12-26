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
    }
}