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

            _sheet.ClearComments(); //�M���Ҧ����ѡC

            ResetSheetStyle();
        }

        private void ResetSheetStyle()
        {
            int startRow = _reader.AbsoluteStartRowIndex;
            byte startColumn = _reader.AbsoluteStartColumnIndex;

            _sheet.Cells.ClearFormats(startRow, startColumn, _sheet.Cells.MaxDataRow, _sheet.Cells.MaxDataColumn);

            Range rng = _sheet.Cells.CreateRange(startRow, startColumn, _sheet.Cells.MaxDataRow, _sheet.Cells.MaxDataColumn + 1);

            // 2017/8/22 �o�~�̾ڰ����p�ձM�� [03-05][04+] EXCEL�פJ�榡�i�_�ץ���xlsx�]�i�פJ�H ��אּ�s�� Aspose.Cells_201402 �g�k �ASetStyle()
            //rng.Style = _styles.Normal;

            rng.SetStyle(_styles.Normal);
        }

        #region IMessageOutput Members

        public void Output(RowMessage message)
        {
            foreach (CellMessage each in message.GetMessages())
            {
                int row = _reader.AbsoluteIndex;

                // 2017/8/22 �o�~�̾ڰ����p�ձM�� [03-05][04+] EXCEL�פJ�榡�i�_�ץ���xlsx�]�i�פJ�H ��אּ�s�� Aspose.Cells_201402 �g�k �A�q�ϥ� byte �אּ int
                //byte column = 0;
                int column = 0;
                SheetColumn sheetColumn;
                if (_reader.TryResolveColumn(each.Column, out sheetColumn) && sheetColumn != null)
                    column = sheetColumn.AbsoluteIndex;
                else if (_columns.ContainsKey(each.Column))
                    column = _columns[each.Column].AbsoluteIndex;

                int index = _sheet.Comments.Add(row, column);
                Comment objComment = _sheet.Comments[index];
                objComment.Note = each.Message;
                objComment.WidthCM = 5;
                objComment.HeightCM = 3;

                switch (each.MessageType)
                {
                    case MessageType.Correct:
                        // 2017/8/22 �o�~�̾ڰ����p�ձM�� [03-05][04+] EXCEL�פJ�榡�i�_�ץ���xlsx�]�i�פJ�H ��אּ�s�� Aspose.Cells_201402 �g�k �ASetStyle()
                        //_sheet.Cells[row, column].Style = _styles.Correct;
                        _sheet.Cells[row, column].SetStyle(_styles.Correct); 
                        break;
                    case MessageType.Warning:
                        // 2017/8/22 �o�~�̾ڰ����p�ձM�� [03-05][04+] EXCEL�פJ�榡�i�_�ץ���xlsx�]�i�פJ�H ��אּ�s�� Aspose.Cells_201402 �g�k �ASetStyle()
                        //_sheet.Cells[row, column].Style = _styles.Warning;
                        _sheet.Cells[row, column].SetStyle(_styles.Warning);
                        break;
                    case MessageType.Error:
                        // 2017/8/22 �o�~�̾ڰ����p�ձM�� [03-05][04+] EXCEL�פJ�榡�i�_�ץ���xlsx�]�i�פJ�H ��אּ�s�� Aspose.Cells_201402 �g�k �ASetStyle()
                        _sheet.Cells[row, column].SetStyle(_styles.Error);
                        break;
                }
            }
        }

        #endregion
    }
}
