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

                // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                //rng.Style = _styles.Normal;

                rng.SetStyle(_styles.Normal);
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

                // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                //_target_sheet.Cells[0, each.Index].Style = _styles.Normal;

                _target_sheet.Cells[0, each.Index].SetStyle(_styles.Normal);
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

                // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                //cell.Style = _styles.Normal;

                cell.SetStyle(_styles.Normal);

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

                        // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                        //_target_sheet.Cells[row, column].Style = _styles.Correct;

                        _target_sheet.Cells[row, column].SetStyle(_styles.Correct);
                        break;
                    case MessageType.Warning:
                        // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                        //_target_sheet.Cells[row, column].Style = _styles.Warning;

                        _target_sheet.Cells[row, column].SetStyle(_styles.Warning);
                        break;
                    case MessageType.Error:
                        // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，SetStyle()
                        //_target_sheet.Cells[row, column].Style = _styles.Error;

                        _target_sheet.Cells[row, column].SetStyle(_styles.Error);
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
