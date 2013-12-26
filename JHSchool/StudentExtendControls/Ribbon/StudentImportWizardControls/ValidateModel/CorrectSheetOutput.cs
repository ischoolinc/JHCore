using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class CorrectSheetOutput : IMessageOutput
    {
        private const string SheetName = "¸ê®Æ­×¥¿­¶";

        private Worksheet _target_sheet;
        private Workbook _output_book;
        private ValidateColumnCollection _columns;
        private SheetReader _reader;
        private TipStyle _styles;
        private int _new_index;

        public CorrectSheetOutput(Workbook outputBook, SheetReader reader, TipStyle styles,
            ValidateColumnCollection columns)
        {
            outputBook.CalculateFormula();
            _output_book = outputBook;
            _reader = reader;
            _reader.ConvertFormulaToValue();
            _styles = styles;
            _new_index = 1;

            try
            {
                _target_sheet = outputBook.Worksheets[SheetName];
                _target_sheet.Cells.ClearContents(0, 0, _target_sheet.Cells.MaxDataRow, _target_sheet.Cells.MaxDataColumn);
                Range rng = _target_sheet.Cells.CreateRange(0, 0, _target_sheet.Cells.MaxRow + 1, _target_sheet.Cells.MaxColumn + 1);
                rng.Style = _styles.Normal;
            }
            catch (Exception)
            {
                int index = outputBook.Worksheets.Add();
                _target_sheet = outputBook.Worksheets[index];
                _target_sheet.Name = SheetName;
            }

            _columns = columns;

            foreach (ValidateColumn each in columns.Values)
            {
                _target_sheet.Cells[0, each.Index].PutValue(each.Name);
                _target_sheet.Cells[0, each.Index].Style = _styles.Normal;
            }

            _target_sheet.ClearComments();
        }

        #region IMessageOutput Members

        public void Output(RowMessage message)
        {
            foreach (ValidateColumn each in _columns.Values)
            {
                Cell cell = _target_sheet.Cells[_new_index, each.Index];
                cell.PutValue(_reader.GetValue(each.Name));
                cell.Style = _styles.Normal;
                _reader.GetCell(each.Name).Formula = "=" + _target_sheet.Name + "!" + cell.Name;
            }

            List<CellMessage> messages = message.GetMessages();
            foreach (CellMessage each in messages)
            {
                int row = _new_index;
                byte column = _columns[each.Column].Index;

                int index = _target_sheet.Comments.Add(row, column);
                Comment objComment = _target_sheet.Comments[index];
                objComment.Note = each.Message;
                objComment.WidthCM = 5;
                objComment.HeightCM = 3;

                switch (each.MessageType)
                {
                    case MessageType.Correct:
                        _target_sheet.Cells[row, column].Style = _styles.Correct;
                        break;
                    case MessageType.Warning:
                        _target_sheet.Cells[row, column].Style = _styles.Warning;
                        break;
                    case MessageType.Error:
                        _target_sheet.Cells[row, column].Style = _styles.Error;
                        break;
                }
            }
            _new_index++;
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
