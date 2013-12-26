using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;
using System.Windows.Forms;
using JHSchool.Legacy.ImportSupport.Validators;

namespace JHSchool.Legacy.ImportSupport
{
    public class ValidateHelper
    {
        private const int ProgressStep = 5;

        private CellCommentManager _comments;
        private DocumentValidate _validator;
        private CommentValidatorFactory _valid_factory;
        private WizardContext _context;
        private SheetRowSource _sheetSource;

        public ValidateHelper(WizardContext context, IValidatorFactory factory)
        {
            _context = context;
            _validator = new DocumentValidate();
            _valid_factory = new CommentValidatorFactory(context);
            _comments = new CellCommentManager();

            if (factory != null)
            {
                _validator.FieldValidatorList.AddValidatorFactory(factory);
                _validator.RowValidatorList.AddValidatorFactory(factory);
            }

            _validator.FieldValidatorList.AddValidatorFactory(_valid_factory);
            _validator.RowValidatorList.AddValidatorFactory(_valid_factory);
        }

        public CellCommentManager Validate(SheetHelper sheet)
        {
            //int t1 = Environment.TickCount;
            _valid_factory.UpdateUnique = new UpdateUniqueRowValidator(_context, sheet);

            XmlElement xmlRule = _context.DataSource.GetValidateFieldRule();
            _validator.InitFromXMLNode(xmlRule);

            //Console.WriteLine("初始化 Validator時間：{0}", Environment.TickCount - t1);

            _validator.ErrorCaptured += new DocumentValidate.ErrorCapturedEventHandler(validator_ErrorCaptured);
            _validator.AutoCorrect += new DocumentValidate.AutoCorrectEventHandler(validator_AutoCorrect);

            _sheetSource = new SheetRowSource(sheet, _context);
            int progress = 0, firstRow = sheet.FirstDataRowIndex, maxRow = sheet.MaxDataRowIndex;
            for (int rowIndex = firstRow; rowIndex <= maxRow; rowIndex++)
            {
                _sheetSource.BindRow(rowIndex);
                _validator.ValidateRow(_sheetSource);

                Application.DoEvents();

                //回報進度。
                if (((++progress) % ProgressStep) == 0)
                {
                    if (ProgressChanged != null)
                    {
                        int percentage = progress * 100 / (maxRow - firstRow);
                        ProgressUserState userState = new ProgressUserState();
                        ProgressChanged(this, new ProgressChangedEventArgs(percentage, userState));

                        if (userState.Cancel) return _comments;
                    }
                }
            }
            ProgressChanged(this, new ProgressChangedEventArgs(100, new ProgressUserState()));

            _validator.ErrorCaptured -= new DocumentValidate.ErrorCapturedEventHandler(validator_ErrorCaptured);
            _validator.AutoCorrect -= new DocumentValidate.AutoCorrectEventHandler(validator_AutoCorrect);

            if (_context.CurrentMode == ImportMode.Update)
            {
                CellCommentManager comments = _valid_factory.UpdateUnique.CheckUpdateResult();
                _comments.MergeFrom(comments);
            }

            //foreach (CellComment each in _comments)
            //    Console.WriteLine(string.Format("{0}:{1} {2} Msg:{3}",
            //        each.RowIndex, each.ColumnIndex, each.BestComment, each.BestComment.Comment));

            return _comments;
        }

        private void validator_AutoCorrect(string FieldName, string OldValue, string NewValue, IRowSource RowSource)
        {
            SheetHelper sheet = _sheetSource.Sheet;
            SheetRowSource source = _sheetSource;

            _comments.WriteCorrect(source.CurrentRowIndex, sheet.GetFieldIndex(FieldName), OldValue, NewValue);

            Console.WriteLine(string.Format("Correct：{0}", FieldName));
        }

        private void validator_ErrorCaptured(string FieldName, string ErrorType, string Description, IRowSource RowSource)
        {
            SheetHelper sheet = _sheetSource.Sheet;
            SheetRowSource source = _sheetSource;

            if (FieldName == "<XmlContent>")
            {
                XmlElement errorInfo = DSXmlHelper.LoadXml(Description);
                foreach (XmlElement each in errorInfo.SelectNodes("Field"))
                {
                    string fieldName = each.GetAttribute("Name");
                    string message = each.GetAttribute("Description");
                    WriteComment(ErrorType, source.CurrentRowIndex, sheet.GetFieldIndex(fieldName), message);
                }
            }
            else
                WriteComment(ErrorType, source.CurrentRowIndex, sheet.GetFieldIndex(FieldName), Description);
        }

        private void WriteComment(string type, int rowIndex, int columnIndex, string msg)
        {
            if (type.ToUpper() == "Error".ToUpper())
                _comments.WriteError(rowIndex, columnIndex, msg);
            else
                _comments.WriteWarning(rowIndex, columnIndex, msg);
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public class ProgressUserState
        {
            public ProgressUserState()
            {
                _cancel = false;
            }

            private bool _cancel;

            public bool Cancel
            {
                get { return _cancel; }
                set { _cancel = value; }
            }

        }
    }
}
