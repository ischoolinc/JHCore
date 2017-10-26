using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.Drawing;

namespace JHSchool.Legacy.ImportSupport
{
    public class SheetHelper
    {
        private const int StartRow = 0, StartColumn = 0;

        private Workbook _book;
        private Worksheet _sheet;
        private Dictionary<string, SheetField> _fields;

        public SheetHelper(string sourceFile)
        {
            Initial(sourceFile);
        }

        private void Initial(string sourceFile)
        {
            _book = GetWorkbook(sourceFile);
            _sheet = GetWorksheet(_book);
            _fields = GetFieldList(_sheet);
        }

        public Workbook Book
        {
            get { return _book; }
        }

        public Worksheet Sheet
        {
            get { return _sheet; }
        }

        public int GetFieldIndex(string fieldName)
        {
            if (_fields.ContainsKey(fieldName))
                return _fields[fieldName].AbsoluteIndex;
            else
                return -1;
        }

        public List<string> Fields
        {
            get
            {
                return new List<string>(_fields.Keys);
            }
        }

        public List<string> GetFieldsByColor(Color color)
        {
            List<string> fields = new List<string>();
            foreach (string each in Fields)
            {
                // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，GetStyle()
                //Style fStyle = _sheet.Cells[StartRow, GetFieldIndex(each)].Style;

                Style fStyle = _sheet.Cells[StartRow, GetFieldIndex(each)].GetStyle();

                Color c1 = MatchColor(fStyle.ForegroundColor);
                Color c2 = MatchColor(color);

                if (c1 == c2) fields.Add(each);
            }

            return fields;
        }

        public void SetFieldsStyle(List<string> fields, Style style)
        {
            foreach (string each in fields)
            {

                // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                //_sheet.Cells[StartRow, GetFieldIndex(each)].Style = style;

                _sheet.Cells[StartRow, GetFieldIndex(each)].SetStyle(style);

            }
            
        }

        /// <summary>
        /// 設定儲存格值。
        /// </summary>
        /// <param name="row">zero based.</param>
        /// <param name="column">zero based.</param>
        /// <returns></returns>
        public string GetValue(int row, int column)
        {
            return _sheet.Cells[row, column].StringValue;
        }

        /// <summary>
        /// 設定儲存格值。
        /// </summary>
        /// <param name="row">zero based.</param>
        /// <param name="column">zero based.</param>
        /// <returns></returns>
        public void SetValue(int row, int column, string value)
        {
            _sheet.Cells[row, column].PutValue(value);
        }

        public void SetStyle(int row, int column, Style style)
        {
            // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
            //_sheet.Cells[row, column].Style = style;

            _sheet.Cells[row, column].SetStyle(style);
        }

        /// <summary>
        /// 將所有含有資料的儲存格設成統一的樣式。
        /// </summary>
        /// <param name="style"></param>
        public void SetAllStyle(Style style)
        {
            int maxColumn = _sheet.Cells.MaxDataColumn;
            int maxRow = MaxDataRowIndex;

            Range rng = _sheet.Cells.CreateRange(0, 0, maxRow + 1, maxColumn + 1);

            // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
            //rng.Style = style;

            rng.SetStyle(style);
        }

        public void SetComment(int row, int column, string msg)
        {
            Comment cmm = _sheet.Comments[_sheet.Comments.Add(row, (byte)column)];
            cmm.Note = msg;
        }

        public void ClearComments()
        {
            _sheet.Comments.Clear();
        }

        /// <summary>
        /// 建立一個新的樣式。
        /// </summary>
        /// <returns></returns>
        public Style NewStyle()
        {
            return Book.Styles[Book.Styles.Add()];
        }

        public Color MatchColor(Color color)
        {
            return Book.GetMatchingColor(color);
        }

        public void Save(string fileName)
        {
            _book.Save(fileName);
            Initial(fileName);
        }

        public int FirstDataRowIndex { get { return StartRow + 1; } }

        public int MaxDataRowIndex
        {
            get { return _sheet.Cells.MaxDataRow; }
        }

        public int DataRowCount
        {
            get { return _sheet.Cells.MaxDataRow - StartRow; }
        }

        private static Worksheet GetWorksheet(Workbook book)
        {
            Worksheet sheet = null;

            try
            {
                sheet = book.Worksheets[0];
            }
            catch (Exception e)
            {
                throw new ArgumentException("讀取工作表資訊失敗。", e);
            }
            return sheet;
        }

        private static Workbook GetWorkbook(string sourceFile)
        {
            Workbook book = new Workbook();
            book.Open(sourceFile);

            return book;
        }

        private static Dictionary<string, SheetField> GetFieldList(Worksheet sheet)
        {
            Cell startCell = sheet.Cells[StartRow, StartColumn];
            int maxColumn = sheet.Cells.MaxDataColumn; //最大的資料欄 Index (zero based)。
            Dictionary<string, SheetField> columns = new Dictionary<string, SheetField>();

            for (int i = StartColumn; i <= maxColumn; i++)
            {
                Cell colCell = sheet.Cells[StartRow, i];
                if (colCell.StringValue != null && colCell.StringValue.Trim() != string.Empty)
                {
                    SheetField objField = new SheetField(colCell);

                    if (columns.ContainsKey(objField.FieldName))
                        throw new ArgumentException("重覆的欄位名稱：" + objField.FieldName);

                    columns.Add(objField.FieldName, objField);
                }
            }

            return columns;
        }

        #region InnerClass SheetField
        private class SheetField
        {
            public SheetField(Cell fieldCell)
            {
                _field_name = fieldCell.StringValue;
                _absolute_index = fieldCell.Column;
            }

            private string _field_name;
            public string FieldName
            {
                get { return _field_name; }
            }

            private int _absolute_index;
            /// <summary>
            /// zero based.
            /// </summary>
            public int AbsoluteIndex
            {
                get { return _absolute_index; }
            }
        }
        #endregion
    }
}
