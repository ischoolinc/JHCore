using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using DocValidate;
using System.Xml;
using FISCA.DSAUtil;
using System.Windows.Forms;
using JHSchool.Legacy.ImportSupport.Validators;
using IRewriteAPI_JH;

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
            // 判斷匯入來源
            string importType = "教師";
            if (_context.Extensions.ContainsKey("TeacherLookup"))
                importType = "課程"; // 課程匯入
            else if (_context.Extensions.ContainsKey("ClassLookup"))
                importType = "班級"; // 班級匯入


            // 可於此行加上 log 或 debug 用於追蹤
            // 例如: Console.WriteLine($"[DEBUG] 匯入來源: {importType}");
            //int t1 = Environment.TickCount;
            _valid_factory.UpdateUnique = new UpdateUniqueRowValidator(_context, sheet);

            XmlElement xmlRule;

            // 2018/4/17 穎驊註解，因應客服#5944 反應，檢查匯入班級機制，發現其驗證規則為抓取Severice 回傳的xml
            // 經過與恩正、均泰、耀明的討論後，決定將舊的程式碼註解，將其設定存在程式碼中(JH_C_ImportValidatorRule)，直接抓取使用，方便日後維護。
            //XmlElement xmlRule = _context.DataSource.GetValidateFieldRule();

            //2018/12/21 穎驊 完成高雄項目 [10-03][??] 局端夠查詢學校班級有調整”導師”的功能 
            // 有載入高雄自動編班模組的 ， 其匯入規則 載Local 的設定KH版本(班級名稱、班導師 不得空白) 
            IClassBaseInfoItemAPI item = FISCA.InteractionService.DiscoverAPI<IClassBaseInfoItemAPI>();
            if (item != null)
            {
                //讀取XML欄位描述
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Properties.Resources.JH_C_ImportValidatorRule_KH);
                xmlRule = doc.DocumentElement;
            }
            else
            {
                //讀取XML欄位描述
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Properties.Resources.JH_C_ImportValidatorRule);
                xmlRule = doc.DocumentElement;
            }

            if (importType == "課程")
            {
                //讀取XML欄位描述
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Properties.Resources.JH_Course_FieldValidationRule);
                xmlRule = doc.DocumentElement;
            }

            if (importType == "教師")
            {
                //讀取XML欄位描述
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Properties.Resources.JH_T_FieldValidationRule);
                xmlRule = doc.DocumentElement;
            }

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
