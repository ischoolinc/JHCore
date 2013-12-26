using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel
{
    public class CommentOutput : IMessageOutput
    {
        private SheetReader _reader;
        private Worksheet _sheet;
        private SheetColumnCollection _columns;
        private TipStyle _styles;

        public CommentOutput(SheetReader reader, TipStyle styles)
        {
            _reader = reader;
            _sheet = reader.Sheet;
            _columns = reader.Columns;
            _styles = styles;

            _sheet.ClearComments(); //清除所有註解。

            ResetSheetStyle();
        }

        private void ResetSheetStyle()
        {
            int startRow = _reader.AbsoluteStartRowIndex;
            byte startColumn = _reader.AbsoluteStartColumnIndex;

            _sheet.Cells.ClearFormats(startRow, startColumn, _sheet.Cells.MaxDataRow, _sheet.Cells.MaxDataColumn);

            Range rng = _sheet.Cells.CreateRange(startRow, startColumn, _sheet.Cells.MaxDataRow, _sheet.Cells.MaxDataColumn + 1);
            rng.Style = _styles.Normal;
        }

        #region IMessageOutput Members

        public void Output(RowMessage message)
        {
            foreach (CellMessage each in message.GetMessages())
            {
                int row = _reader.AbsoluteIndex;
                byte column = 0;
                if(_columns.ContainsKey(each.Column ))
                    column = _columns[each.Column].AbsoluteIndex;

                int index = _sheet.Comments.Add(row, column);
                Comment objComment = _sheet.Comments[index];
                objComment.Note = each.Message;
                objComment.WidthCM = 5;
                objComment.HeightCM = 3;

                switch (each.MessageType)
                {
                    case MessageType.Correct:
                        _sheet.Cells[row, column].Style = _styles.Correct;
                        break;
                    case MessageType.Warning:
                        _sheet.Cells[row, column].Style = _styles.Warning;
                        break;
                    case MessageType.Error:
                        _sheet.Cells[row, column].Style = _styles.Error;
                        break;
                }
            }
        }

        #endregion
    }
}
