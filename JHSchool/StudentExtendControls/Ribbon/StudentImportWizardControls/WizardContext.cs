using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Cells;
using System.IO;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;

namespace JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls
{
    public class WizardContext
    {
        public string SubFieldSeparator
        {
            get { return ":"; }
        }

        public WizardContext()
        {
            AllowExit = true;
        }

        /// <summary>
        /// 取得或設定指示 Wizard 目前狀態是否可以關閉畫面。
        /// </summary>
        public bool AllowExit;

        public ImportMode ImportMode = ImportMode.None;

        /// <summary>
        /// 取得或設定匯入的來源檔案名稱。
        /// </summary>
        public string SourceFileName = string.Empty;

        private BulkDescription _bulk_desc;
        /// <summary>
        /// 取得或設定匯入規格資訊。
        /// </summary>
        public BulkDescription BulkDescription
        {
            get { return _bulk_desc; }
            set { _bulk_desc = value; }
        }

        private SheetReader _source_reader;
        /// <summary>
        /// 取得或設定匯入的資料來源讀取器。
        /// </summary>
        public SheetReader SourceReader
        {
            get { return _source_reader; }
        }

        /// <summary>
        /// 取得來源資料的所有欄位。
        /// </summary>
        public SheetColumnCollection SourceColumns
        {
            get { return SourceReader.Columns; }
        }

        private BulkColumnCollection _accept_columns;
        public BulkColumnCollection AcceptColumns
        {
            get { return _accept_columns; }
        }

        private Workbook _workbook;
        /// <summary>
        /// 取得或設定匯入來源的工作簿。
        /// </summary>
        public Workbook SourceBook
        {
            get { return _workbook; }
        }

        private Worksheet _source_sheet;
        /// <summary>
        /// 取得或設定匯入來源的工作表。
        /// </summary>
        public Worksheet SourceSheet
        {
            get { return _source_sheet; }
        }

        private TipStyle _styles;
        public TipStyle TipStyle
        {
            get { return _styles; }
        }

        /// <summary>
        /// 載入匯入的相關檔案與執行相關初始化動作。
        /// </summary>
        public void RefreshImportSource()
        {
            FileInfo file = new FileInfo(SourceFileName);
            string backupName = Path.Combine(file.DirectoryName, "原始資備份_" + file.Name);

            if (!File.Exists(backupName))
                File.Copy(SourceFileName, backupName);

            Workbook book = new Workbook();
            book.Open(SourceFileName);

            _workbook = book;
            Worksheet sheet;
            try
            {
               // sheet = _workbook.Worksheets["學生資料"];

                // 先改成預設讀取第1個工作表，保持與其他匯入一致。
                sheet = _workbook.Worksheets[0];

            }
            catch (Exception)
            {
                sheet = _workbook.Worksheets[0];
            }
            // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，GetStyle()
            //_styles = new TipStyle(_workbook, sheet.Cells[0, 0].Style); //用第一格來當作樣版。


            _styles = new TipStyle(_workbook, sheet.Cells[0, 0].GetStyle()); //用第一格來當作樣版。


            _source_sheet = sheet;

            _source_reader = new SheetReader();
            _source_reader.BindSheet(sheet, 0, 0);

            //設定資料讀取器的「識別欄」。
            if (!string.IsNullOrEmpty(IdentifyField))
                _source_reader.SetKeyColumn(IdentifyField);

            //取得可匯入欄位與使用者提供的欄位交集。
            _accept_columns = BulkDescription.Columns.GetInstersection(_source_reader.Columns.GetNames());

            //檢查是否有之前驗證過的欄位。
            //將 Accept SheetColumn 與 BulkColumn 建立關係。
            foreach (SheetColumn each in SourceColumns.Values)
            {
                // 2017/8/22 穎驊依據高雄小組專案 [03-05][04+] EXCEL匯入格式可否修正為xlsx也可匯入？ 更改為新版 Aspose.Cells_201402 寫法 ，GetStyle()
                //Style s = each.BindingCell.Style, t = TipStyle.Header;

                Style s = each.BindingCell.GetStyle(), t = TipStyle.Header;

                if (s.ForegroundColor == t.ForegroundColor && s.Font.Color == t.Font.Color)
                    each.UsedValid = true;
                else
                    each.UsedValid = false;

                if (_accept_columns.ContainsKey(each.Name))
                    each.SetBulkColumn(_accept_columns[each.Name]);
            }

            //復原欄位到預設樣式。
            foreach (SheetColumn each in SourceColumns.Values)
                each.SetStyle(TipStyle.Normal);

            //設定「識別欄」樣式。
            if (!string.IsNullOrEmpty(IdentifyField))
                SourceColumns[IdentifyField].SetStyle(TipStyle.Header);

            //設定「檢查欄」樣式。
            if (!string.IsNullOrEmpty(ShiftCheckField))
                SourceColumns[ShiftCheckField].SetStyle(TipStyle.Header);

            //將使用者選擇的欄位設成「欄」樣式。
            foreach (SheetColumn each in SourceColumns.Values)
            {
                if (SelectedFields != null)
                {
                    if (SelectedFields.ContainsKey(each.Name))
                        each.SetStyle(TipStyle.Header);
                }
            }

        }

        private string _identify_field;
        /// <summary>
        /// 取得或設定資料來源的識別欄位名稱。
        /// </summary>
        public string IdentifyField
        {
            get { return _identify_field; }
            set
            {
                _identify_field = value;

                if (string.IsNullOrEmpty(value)) return;

                _source_reader.SetKeyColumn(value);
                SourceColumns[value].SetStyle(TipStyle.Header);
            }
        }

        private string _shift_check_field;
        /// <summary>
        /// 取得或設定資料來源的檢查欄位名稱。
        /// </summary>
        public string ShiftCheckField
        {
            get { return _shift_check_field; }
            set
            {
                _shift_check_field = value;

                if (string.IsNullOrEmpty(_shift_check_field)) return;

                SourceColumns[value].SetStyle(TipStyle.Header);
            }
        }

        private SheetColumnCollection _selected_fields;
        public SheetColumnCollection SelectedFields
        {
            get { return _selected_fields; }
            set { _selected_fields = value; }
        }

        private BulkColumn _empty_shift_check_field = BulkColumn.GetNullField("<不使用>");
        /// <summary>
        /// 取得代表空的「驗證欄位」。
        /// </summary>
        public BulkColumn EmptyShiftCheckField
        {
            get { return _empty_shift_check_field; }
        }

        private bool _valid_complete = false;
        public bool ValidateComplete
        {
            get { return _valid_complete; }
            set { _valid_complete = value; }
        }

        private ValidateColumnCollection _validColumns;
        public ValidateColumnCollection ValidateColumns
        {
            get { return _validColumns; }
            set { _validColumns = value; }
        }
    }
}
