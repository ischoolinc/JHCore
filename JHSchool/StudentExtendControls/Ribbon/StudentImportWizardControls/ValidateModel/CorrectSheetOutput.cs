using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class CorrectSheetOutput : IMessageOutput
    {
        private const string SheetName = "資料修正頁";

        private Worksheet _target_sheet;
        private Workbook _output_book;
        private ValidateColumnCollection _columns;
        private SheetReader _reader;
        private TipStyle _styles;
        private int _new_index;

        public CorrectSheetOutput(Workbook outputBook, SheetReader reader, TipStyle styles,
            ValidateColumnCollection columns)
        {
            if (outputBook == null)
                throw new ArgumentNullException("outputBook");
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (styles == null)
                throw new ArgumentNullException("styles");
            if (columns == null)
                throw new ArgumentNullException("columns");

            outputBook.CalculateFormula();
            _output_book = outputBook;
            _reader = reader;
            _reader.ConvertFormulaToValue();
            _styles = styles;
            _new_index = 1;
            _columns = columns;

            _target_sheet = FindWorksheet(outputBook, SheetName);
            if (_target_sheet == null)
            {
                int index = outputBook.Worksheets.Add();
                _target_sheet = outputBook.Worksheets[index];
                _target_sheet.Name = SheetName;
            }
            else
            {
                ClearWorksheetSafely(_target_sheet);
            }

            foreach (ValidateColumn each in columns.Values)
            {
                _target_sheet.Cells[0, each.Index].PutValue(each.DisplayName);
                _target_sheet.Cells[0, each.Index].SetStyle(_styles.Normal);
            }

            _target_sheet.ClearComments();
        }

        private static Worksheet FindWorksheet(Workbook book, string sheetName)
        {
            try
            {
                // Some Aspose versions return null when worksheet is missing.
                Worksheet sheet = book.Worksheets[sheetName];
                return sheet;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void ClearWorksheetSafely(Worksheet sheet)
        {
            int maxDataRow = sheet.Cells.MaxDataRow;
            int maxDataColumn = sheet.Cells.MaxDataColumn;

            // Empty sheet may report MaxDataRow/MaxDataColumn as -1.
            if (maxDataRow >= 0 && maxDataColumn >= 0)
                sheet.Cells.ClearContents(0, 0, maxDataRow, maxDataColumn);

            int maxRow = sheet.Cells.MaxRow;
            int maxColumn = sheet.Cells.MaxColumn;
            if (maxRow < 0)
                maxRow = 0;
            if (maxColumn < 0)
                maxColumn = 0;

            Range rng = sheet.Cells.CreateRange(0, 0, maxRow + 1, maxColumn + 1);
            rng.SetStyle(_styles.Normal);
        }

        #region IMessageOutput Members

        public void Output(RowMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            foreach (ValidateColumn each in _columns.Values)
            {
                Cell cell = _target_sheet.Cells[_new_index, each.Index];
                cell.PutValue(_reader.GetValue(each.Name));
                cell.SetStyle(_styles.Normal);

                Cell sourceCell = _reader.GetCell(each.Name);
                if (sourceCell == null)
                {
                    throw new InvalidOperationException(
                        "無法建立資料修正頁連結：來源工作表找不到欄位「" + each.DisplayName +
                        "」（內部欄位：" + each.Name + "）。");
                }

                sourceCell.Formula = "=" + _target_sheet.Name + "!" + cell.Name;
            }

            List<CellMessage> messages = message.GetMessages();
            foreach (CellMessage each in messages)
            {
                ValidateColumn validateColumn;
                if (!_columns.TryResolveColumn(each.Column, out validateColumn) || validateColumn == null)
                {
                    // Parent mapping errors must be reported; other missing columns are skipped.
                    if (IsParentRelatedField(each.Column))
                    {
                        throw new InvalidOperationException(
                            "無法輸出驗證訊息到資料修正頁：找不到驗證欄位對應。" +
                            " 訊息欄位＝「" + each.Column + "」。" +
                            " 請確認該欄位已加入驗證欄集合，且家長1／家長2 與父親／母親對應一致。");
                    }
                    continue;
                }

                int row = _new_index;
                byte column = validateColumn.Index;

                int index = _target_sheet.Comments.Add(row, column);
                Comment objComment = _target_sheet.Comments[index];
                objComment.Note = each.Message;
                objComment.WidthCM = 5;
                objComment.HeightCM = 3;

                switch (each.MessageType)
                {
                    case MessageType.Correct:
                        _target_sheet.Cells[row, column].SetStyle(_styles.Correct);
                        break;
                    case MessageType.Warning:
                        _target_sheet.Cells[row, column].SetStyle(_styles.Warning);
                        break;
                    case MessageType.Error:
                        _target_sheet.Cells[row, column].SetStyle(_styles.Error);
                        break;
                }
            }
            _new_index++;
        }

        private static bool IsParentRelatedField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return false;

            return fieldName.StartsWith("家長1") ||
                fieldName.StartsWith("家長2") ||
                fieldName.StartsWith("父親") ||
                fieldName.StartsWith("母親");
        }

        #endregion

        public void OutputComplete()
        {
            if (_new_index <= 1)
            {
                try
                {
                    _output_book.Worksheets.RemoveAt(SheetName);
                }
                catch (Exception) { }
            }
        }
    }
}