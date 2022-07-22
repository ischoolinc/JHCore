using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using System.Xml;
using System.IO;
using ProgramProcess = System.Diagnostics.Process;
using DocValidate;
using FISCA.DSAUtil;
using DevComponents.DotNetBar.Rendering;
using FISCA.Presentation.Controls;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.SheetModel;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.ValidateModel;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls.BulkModel;
using JHSchool.Feature.Legacy;
using JHSchool.Legacy.ImportSupport.Validators;
using JHSchool.StudentExtendControls.Ribbon.StudentImportWizardControls;
using System.Xml.Linq;

namespace JHSchool.StudentExtendControls.Ribbon
{
    public partial class StudentImportWizard : BaseForm
    {
        private WizardContext _context;

        public StudentImportWizard()
        {
            InitializeComponent();

            #region 設定Wizard會跟著Style跑
            (GlobalManager.Renderer as Office2007Renderer).ColorTableChanged += new EventHandler(StudentImportWizard_ColorTableChanged);
            //this.wizard1.FooterStyle.ApplyStyle(( GlobalManager.Renderer as Office2007Renderer ).ColorTable.GetClass(ElementStyleClassKeys.RibbonFileMenuBottomContainerKey));
            this.ImportWizard.HeaderStyle.ApplyStyle((GlobalManager.Renderer as Office2007Renderer).ColorTable.GetClass(ElementStyleClassKeys.RibbonFileMenuBottomContainerKey));
            this.ImportWizard.FooterStyle.BackColorGradientAngle = -90;
            this.ImportWizard.FooterStyle.BackColorGradientType = eGradientType.Linear;
            this.ImportWizard.FooterStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.ImportWizard.FooterStyle.BackColor2 = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.End;
            this.ImportWizard.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.ImportWizard.BackgroundImage = null;
            for (int i = 0; i < 5; i++)
            {
                (this.ImportWizard.Controls[1].Controls[i] as ButtonX).ColorTable = eButtonColor.OrangeWithBackground;
            }
            (this.ImportWizard.Controls[0].Controls[1] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TitleText;
            (this.ImportWizard.Controls[0].Controls[2] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TitleText;
            this.ImportWizard.FooterStyle.BackgroundImage = null;
            #endregion

            _context = new WizardContext();
        }

        void StudentImportWizard_ColorTableChanged(object sender, EventArgs e)
        {
            this.ImportWizard.HeaderStyle.ApplyStyle((GlobalManager.Renderer as Office2007Renderer).ColorTable.GetClass(ElementStyleClassKeys.RibbonFileMenuBottomContainerKey));
            this.ImportWizard.FooterStyle.BackColorGradientAngle = -90;
            this.ImportWizard.FooterStyle.BackColorGradientType = eGradientType.Linear;
            this.ImportWizard.FooterStyle.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.ImportWizard.FooterStyle.BackColor2 = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.End;
            this.ImportWizard.BackColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TopBackground.Start;
            this.ImportWizard.BackgroundImage = null;
            for (int i = 0; i < 5; i++)
            {
                (this.ImportWizard.Controls[1].Controls[i] as ButtonX).ColorTable = eButtonColor.OrangeWithBackground;
            }
            (this.ImportWizard.Controls[0].Controls[1] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.MouseOver.TitleText;
            (this.ImportWizard.Controls[0].Controls[2] as System.Windows.Forms.Label).ForeColor = (GlobalManager.Renderer as Office2007Renderer).ColorTable.RibbonBar.Default.TitleText;
            this.ImportWizard.FooterStyle.BackgroundImage = null;
        }

        private WizardContext Context
        {
            get { return _context; }
        }

        #region Select File and Action Page
        private void lblInserDesc_Click(object sender, EventArgs e)
        {
            Context.ImportMode = ImportMode.Insert;
            chkInsert.Checked = true;
        }

        private void lblUpdateDesc_Click(object sender, EventArgs e)
        {
            Context.ImportMode = ImportMode.Update;
            chkUpdate.Checked = true;
        }

        private void chkInsert_CheckedChanged(object sender, EventArgs e)
        {
            Context.ImportMode = ImportMode.Insert;
        }

        private void chkUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Context.ImportMode = ImportMode.Update;
        }

        private void txtSourceFile_TextChanged(object sender, EventArgs e)
        {
            Context.SourceFileName = txtSourceFile.Text;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            DialogResult dr = SelectSourceFileDialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                txtSourceFile.Text = SelectSourceFileDialog.FileName;
            }
        }

        private void wpSelectFileAndAction_NextButtonClick(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;

                Context.SourceFileName = txtSourceFile.Text;
                if (string.IsNullOrEmpty(Context.SourceFileName))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您必須選擇匯入來源檔案。");
                    return;
                }

                if (!File.Exists(Context.SourceFileName))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您指定的來源檔案並不存在。");
                    return;
                }

                if (Context.ImportMode == ImportMode.None)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您必須決定一種匯入方式。");
                    return;
                }

                lblCollectMsg.Text = "讀取匯入規格描述資訊…";
                pProgram.Visible = true;
                pUser.Visible = false;
                Application.DoEvents();

                XmlElement bulk = StudentBulkProcess.GetBulkDescription();
                Context.BulkDescription = new BulkDescription(bulk);

                Context.RefreshImportSource();
                Context.IdentifyField = string.Empty;
                Context.ShiftCheckField = string.Empty;
                
                //// 加入學生狀態與原住民族別欄位可以勾選
                //BulkColumn BC1 = new BulkColumn("狀態");
                //BC1.SetDefaultData1();
                //BulkColumn BC2 = new BulkColumn("原住民族別");
                //BC2.SetDefaultData1();

                //Context.AcceptColumns.Add(BC1.DisplayText, BC1);
                //Context.AcceptColumns.Add(BC2.DisplayText, BC2);
                //Context.BulkDescription.Columns.Add(BC1.DisplayText, BC1);
                //Context.BulkDescription.Columns.Add(BC2.DisplayText, BC2);

                if (Context.ImportMode == ImportMode.Insert)
                {
                    if (!Context.AcceptColumns.ContainsKey("姓名"))
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("來源資料中缺少必要欄位「姓名」。", Application.ProductName);
                        return;
                    }
                }

                e.Cancel = false;
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
            finally
            {
                pProgram.Visible = false;
                pUser.Visible = true;
            }
        }

        #endregion

        #region Collect Key Info Page
        private void wpCollectKeyInfo_BeforePageDisplayed(object sender, WizardCancelPageChangeEventArgs e)
        {
            if (e.PageChangeSource == eWizardPageChangeSource.NextButton)
            {
                if (Context.ImportMode == ImportMode.Insert)
                    e.NewPage = wpSelectField;
                else
                    e.NewPage = wpCollectKeyInfo;
            }
        }

        private void wpCollectKeyInfo_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            if (e.PageChangeSource == eWizardPageChangeSource.BackButton)
                return;

            cboIdField.SelectedItem = null;
            cboIdField.Items.Clear();
            cboIdField.DisplayMember = "DisplayText";

            cboValidateField.SelectedItem = null;
            cboValidateField.Items.Clear();
            cboValidateField.DisplayMember = "DisplayText";
            cboValidateField.Items.Add(Context.EmptyShiftCheckField);
            cboValidateField.SelectedIndex = 0;

            foreach (BulkColumn each in Context.AcceptColumns.Values)
            {
                if (each.Identifiable)
                    cboIdField.Items.Add(each);

                if (each.ShiftCheckable)
                    cboValidateField.Items.Add(each);
            }

            //檢查是否有提供識別欄位。
            if (cboIdField.Items.Count <= 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("您提供的資料來源中沒有任何可當作識別欄的欄位\n必須提供識別欄位才可使用更新匯入。");
                wpCollectKeyInfo.NextButtonEnabled = eWizardButtonState.False;
            }
            else
                wpCollectKeyInfo.NextButtonEnabled = eWizardButtonState.True;
        }

        private void wpCollectKeyInfo_NextButtonClick(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (cboIdField.SelectedIndex == -1)
            {
                FISCA.Presentation.Controls.MsgBox.Show("您必須要選擇識別欄位。");
                return;
            }

            BulkColumn column = cboIdField.SelectedItem as BulkColumn;
            if (column != null)
                Context.IdentifyField = column.DisplayText;

            column = cboValidateField.SelectedItem as BulkColumn;
            if (column != Context.EmptyShiftCheckField)
                Context.ShiftCheckField = column.DisplayText;

            if (Context.IdentifyField == Context.ShiftCheckField)
            {
                FISCA.Presentation.Controls.MsgBox.Show("「識別欄」與「驗證欄」必須是不同的欄位。");
                return;
            }

            e.Cancel = false;
        }
        #endregion

        #region Select Field Page
        private void wpSelectField_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            if (e.PageChangeSource == eWizardPageChangeSource.BackButton)
                return;

            Dictionary<string, ImportItem> fields = new Dictionary<string, ImportItem>();
            GroupSheetColumn(fields);

            Dictionary<string, BulkColumnCollection> bfields = new Dictionary<string, BulkColumnCollection>();
            GroupBulkColumn(bfields);

            //將欄位顯示於 ListView 中。
            DisplayColumns(fields, bfields);

            //檢查唯讀欄位，此類型欄位不可匯入。
            CheckReadOnlyField();

            //如果是 Insert 時，檢查必要欄位，此類型欄一定要選擇。
            if (Context.ImportMode == ImportMode.Insert)
                CheckRequiredField();
        }

        private void DisplayColumns(Dictionary<string, ImportItem> fields, Dictionary<string, BulkColumnCollection> bfields)
        {
            lvSourceFieldList.Items.Clear();

            //需遮蔽的欄位
            List<string> avoids = new List<string>(new string[] { "帳號類型", "登入密碼" });

            foreach (ImportItem each in fields.Values)
            {
                //遮蔽欄位
                if (avoids.Contains(each.Text)) continue;

                bool hide_column = false;

                if (each.IsGroupColumn)
                {
                    if (bfields.ContainsKey(each.Text))
                        each.CheckAccept(bfields[each.Text]);
                    else
                    { //不在群組欄位中時。
                        hide_column = true;
                    }
                }
                else
                {//非群欄位時。
                    if (!Context.AcceptColumns.ContainsKey(each.Text))
                    { //不在 AcceptColumns 中時。
                        each.Enabled = false;
                        each.ToolTipText = "系統不提供此欄位匯入。";
                    }
                    else if (each.Text == Context.IdentifyField)
                    {//「識別欄」不可以匯入。
                        each.Enabled = false;
                        each.ToolTipText = "識別欄不可以當作匯入欄位。";
                    }
                    else if (each.Text == Context.ShiftCheckField)
                    {//「驗證欄」不可以匯入。
                        each.Enabled = false;
                        each.ToolTipText = "驗證欄不可以當作匯入欄位。";
                    }
                    else
                        each.Enabled = true;
                }

                if (!hide_column)
                    lvSourceFieldList.Items.Add(each);
            }
        }

        private void CheckReadOnlyField()
        {
            foreach (ImportItem each in lvSourceFieldList.Items)
            {
                foreach (SheetColumn column in each.SheetColumns.Values)
                {
                    if (column.BindingBulkColumn != null && column.BindingBulkColumn.ReadOnly)
                    {
                        each.Enabled = false;
                        each.ToolTipText = "唯讀欄位不允許匯入。";
                    }
                }
            }
        }

        private void CheckRequiredField()
        {
            foreach (ImportItem each in lvSourceFieldList.Items)
            {
                foreach (SheetColumn column in each.SheetColumns.Values)
                {
                    if (column.BindingBulkColumn != null && column.BindingBulkColumn.IsRequired)
                    {
                        each.Checked = true;
                        each.Locked = true;
                        each.ToolTipText = "此欄位是必要欄位。";
                    }
                }
            }
        }

        private void GroupBulkColumn(Dictionary<string, BulkColumnCollection> bfields)
        {
            foreach (BulkColumn each in Context.BulkDescription.Columns.Values)
            {
                BulkColumnCollection item = null;

                if (bfields.ContainsKey(each.GroupName))
                    item = bfields[each.GroupName];
                else
                {
                    item = new BulkColumnCollection();
                    bfields.Add(each.GroupName, item);
                }

                item.Add(each.FullDisplayText, each);
            }
        }

        private void GroupSheetColumn(Dictionary<string, ImportItem> fields)
        {
            foreach (SheetColumn each in Context.SourceColumns.Values)
            {
                ImportItem item = null;
                if (fields.ContainsKey(each.GroupName))
                    item = fields[each.GroupName];
                else
                {
                    item = new ImportItem(each.GroupName);
                    fields.Add(each.GroupName, item);
                }

                item.AddSheetColumn(each);
            }
        }

        private void wpSelectField_NextButtonClick(object sender, CancelEventArgs e)
        {
            Context.SelectedFields = new SheetColumnCollection();
            foreach (ImportItem each in lvSourceFieldList.Items)
            {
                if (each.Checked)
                {
                    foreach (SheetColumn column in each.SheetColumns.Values)
                    {
                        Context.SelectedFields.Add(column.Name, column);
                        column.SetStyle(Context.TipStyle.Header);
                    }
                }
            }

            if (Context.SelectedFields.Count <= 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("您必須選擇要匯入的欄位。選擇欄位時，請在名稱前的方格中打勾。");
                e.Cancel = true;
            }
        }

        private void lvSourceFieldList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ImportItem item = lvSourceFieldList.Items[e.Index] as ImportItem;

            if (item != null)
            {
                if (item.Enabled == false)
                    e.NewValue = CheckState.Unchecked;

                if (item.Locked)
                    e.NewValue = e.CurrentValue;
            }
        }

        private void chkSelectAll_CheckedChanged(object sender, System.EventArgs e)
        {
            foreach (ImportItem each in lvSourceFieldList.Items)
            {
                if (each.Enabled == true)
                    each.Checked = chkSelectAll.Checked;
            }
        }

        private class ImportItem : ListViewItem
        {
            private SheetColumnCollection _columns;
            private bool _is_group_column;
            private bool _enabled;
            private bool _locked;

            public ImportItem(string displayName)
            {
                Text = displayName;
                Enabled = false;

                _columns = new SheetColumnCollection();
            }

            public void AddSheetColumn(SheetColumn column)
            {
                if (column.UsedValid)
                    Checked = true;

                _columns.Add(column.Name, column);

                if (_columns.Count > 1)
                    _is_group_column = true;
            }

            public bool IsGroupColumn
            {
                get { return _is_group_column; }
            }

            public void CheckAccept(BulkColumnCollection basis)
            {
                StringBuilder msg = new StringBuilder();
                bool success = true;

                msg.AppendLine("要匯入此欄位，必須下列欄位同時存在：");
                foreach (BulkColumn each in basis.Values)
                {
                    if (!SheetColumns.ContainsKey(each.FullDisplayText))
                    {
                        msg.AppendLine(each.FullDisplayText);
                        success = false;
                    }
                }

                if (success == false)
                {
                    Enabled = false;
                    ToolTipText = msg.ToString();
                }
                else
                    Enabled = true;
            }

            public bool Enabled
            {
                get { return _enabled; }
                set
                {
                    _enabled = value;

                    if (_enabled == false)
                    {
                        ForeColor = Color.Silver;
                        Checked = false; //設成 False 時，Checked 一定是 Unchecked;
                    }
                    else
                        ForeColor = Color.Black;
                }
            }

            public bool Locked
            {
                get { return _locked; }
                set
                {
                    _locked = value;

                    if (_locked)
                        ForeColor = Color.Blue;
                    else
                        ForeColor = Color.Black;
                }
            }

            public SheetColumnCollection SheetColumns
            {
                get { return _columns; }
            }
        }
        #endregion

        #region Validation Page
        private bool _cancel_validate;
        private int _correct_count = 0, _warning_count = 0, _error_count = 0;
        private RowMessage _current_row_message;

        private void lnkCancelValid_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("確定要取消資料檢查？", Application.ProductName, MessageBoxButtons.YesNo) == DialogResult.Yes)
                _cancel_validate = true;
        }

        private void wizardPage2_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            Context.ValidateComplete = false;
            //chkContinue.Checked = false;

            if (e.PageChangeSource == eWizardPageChangeSource.BackButton)
                return;

            pgValidProgress.Value = 0;

            //chkContinue.Visible = false;
            //lblContinueMsg.Visible = false;
            lnkCancelValid.Visible = false;
        }


        //Dictionary<string, string> StudIDByStudNum = new Dictionary<string, string>();
        //Dictionary<string, string> StudIDByStudIDNumber = new Dictionary<string, string>();

        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                bool sepErrors = chkSepErrors.Checked;

                //搜集使用者選擇要匯入的欄位，建立成 ValidateColumn 集合。
                ValidateColumnCollection validColumns = CreateValidateColumns();

                ProgressMessage("載入資料檢查規則…");
                DocumentValidate validator = new DocumentValidate();
                validator.FieldValidatorList.AddValidatorFactory(new FieldValidatorFactory());
                validator.AutoCorrect += new DocumentValidate.AutoCorrectEventHandler(Validator_AutoCorrect);
                validator.ErrorCaptured += new DocumentValidate.ErrorCapturedEventHandler(Validator_ErrorCaptured);
                XmlElement validRule = StudentBulkProcess.GetFieldValidationRule();
                //validator.RowValidatorList.AddValidatorFactory(new RowValidatorFactory(Context.IdentifyField));
                validator.InitFromXMLNode(validRule);

                ProgressMessage("載入識別欄驗證資訊…");
                //XmlElement primaryKeys = StudentBulkProcess.GetPrimaryKeyList();
                ImportRecordCollection importRecords = new ImportRecordCollection();
                //foreach (XmlElement each in primaryKeys.SelectNodes("Item"))
                //    importRecords.Add(new ImportRecord(each));

                //StudIDByStudIDNumber.Clear();
                //StudIDByStudNum.Clear();

                // 讀取所有狀態學生
                foreach (JHSchool.Data.JHStudentRecord studRec in JHSchool.Data.JHStudent.SelectAll())
                {
                //    string str1 = studRec.StudentNumber;
                //    string str2 = studRec.IDNumber;

                //    // 建立用學號對應編號
                //    if (!StudIDByStudNum.ContainsKey(str1))
                //        StudIDByStudNum.Add(str1, studRec.ID);

                //    // 建立用身分證號對應編號
                //    if (!StudIDByStudIDNumber.ContainsKey(str2))
                //        StudIDByStudIDNumber.Add(str2, studRec.ID);

                    importRecords.Add(new ImportRecord(studRec));
                }
                ShiftCheckList checkList = null;
                bool isShiftCheck = false;
                if (Context.ImportMode == ImportMode.Update && !string.IsNullOrEmpty(Context.ShiftCheckField))
                {
                    ProgressMessage("載入驗證欄檢查資訊…");
                    BulkColumn key = Context.BulkDescription.Columns[Context.IdentifyField];
                    BulkColumn shift = Context.BulkDescription.Columns[Context.ShiftCheckField];
                    checkList = new ShiftCheckList(key, shift);
                    checkList.LoadCheckList();
                    isShiftCheck = true;
                }

                ProgressMessage("初始化資料來源…");
                Context.RefreshImportSource();
                SheetRowSource rowSource = new SheetRowSource(Context.SourceReader, validColumns);

                ProgressMessage("初始化錯誤輸出程序…");
                Dictionary<int, RowMessage> rowMessages = new Dictionary<int, RowMessage>();
                CommentOutput msgOutput = new CommentOutput(Context.SourceReader, Context.TipStyle);
                CorrectSheetOutput msgCorrect = null;
                msgCorrect = new CorrectSheetOutput(Context.SourceBook, Context.SourceReader, Context.TipStyle, validColumns);

                InitValidStartScreen();

                ProgressMessage("開始檢查資料…");
                rowSource.Reset();
                while (rowSource.NextRow())
                {
                    _current_row_message = rowSource.CreateRowMessage();

                    //int t1 = Environment.TickCount;
                    validator.ValidateRow(rowSource);
                    //Console.WriteLine(Environment.TickCount - t1);

                    //檢查「驗證欄資料」。
                    CheckShiftColumn(checkList, isShiftCheck, rowSource);

                    rowMessages.Add(_current_row_message.RowIndex, _current_row_message);

                    if (Context.SourceReader.RelativelyIndex % 10 == 0)
                        RefreshValidProgress(Context.SourceReader.RelativelyIndex);

                    if (_cancel_validate)
                        return;
                }
                lnkCancelValid.Visible = false;

                //更新進度 Bar。
                RefreshValidProgress(Context.SourceReader.RelativelyIndex);

                ProgressMessage("檢查識別欄正確性資料…");
                ValidateKey(importRecords, rowMessages);

                //輸出錯誤資訊。
                ProgressMessage("輸出錯誤資訊…");
                pgValidProgress.Minimum = 0;
                pgValidProgress.Maximum = rowMessages.Count;
                pgValidProgress.Value = 0;
                foreach (RowMessage each in rowMessages.Values)
                {
                    if (each.HasMessage)
                    {
                        Context.SourceReader.MoveTo(each.RowIndex);

                        msgOutput.Output(each);

                        if (sepErrors) msgCorrect.Output(each);
                    }

                    if (each.RowIndex % 50 == 0)
                        RefreshValidProgress(each.RowIndex);
                }
                RefreshValidProgress(rowMessages.Count);

                msgCorrect.OutputComplete();

                RefreshCorrectCount();
                RefreshWarningCount();
                RefreshErrorCount();

                Context.ValidateComplete = (_error_count <= 0);

                if (Context.ValidateComplete)
                    ProgressMessage("資料檢查完成，沒有檢查出任何錯誤。");
                else
                    ProgressMessage("資料檢查完成，但有發現資料不正確。");

                validator.AutoCorrect -= new DocumentValidate.AutoCorrectEventHandler(Validator_AutoCorrect);
                validator.ErrorCaptured -= new DocumentValidate.ErrorCapturedEventHandler(Validator_ErrorCaptured);

                Context.SourceBook.Save(Context.SourceFileName);
            }
            catch (Exception ex)
            {
                //CurrentUser user = CurrentUser.Instance;
                //BugReporter.ReportException(user.SystemName, user.SystemVersion, ex, false);

                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                Context.ValidateComplete = false;
            }
            finally
            {
                btnValidate.Enabled = true;
                btnViewResult.Enabled = true;
                chkSepErrors.Enabled = true;
            }
        }

        private void CheckShiftColumn(ShiftCheckList checkList, bool isShiftCheck, SheetRowSource rowSource)
        {
            if (isShiftCheck)
            {
                string key = rowSource.GetFieldData(Context.IdentifyField);

                if (checkList.ContainKey(key))
                {
                    string srcValue = rowSource.GetFieldData(Context.ShiftCheckField);
                    string dbValue = checkList.GetCheckValue(key);

                    if (srcValue.Trim() != dbValue.Trim())
                    {
                        string message = "";
                        if (string.IsNullOrEmpty(dbValue))
                            message = "資料庫中此資料為空值，與此欄不符，請檢查。";
                        else
                            message = "資料庫中值為「" + dbValue + "」，與此欄不符，請檢查。";

                        _current_row_message.ReportMessage(Context.ShiftCheckField, MessageType.Error,
                            message);

                        _error_count++;
                    }
                }
            }
        }

        private void ValidateKey(ImportRecordCollection importRecords, Dictionary<int, RowMessage> rowMessages)
        {
            string id = "學生系統編號", snum = "學號", ssn = "身分證號", login = "登入帳號", StudStatus = "一般"; //StudStatus 預設為一般生

            SheetReader reader = Context.SourceReader;
            reader.Reset();
            if (Context.ImportMode == ImportMode.Insert)
            {
                while (reader.MoveNext())
                {
                    // 預設狀態為一般
                    StudStatus = "一般";
                    if (reader.Columns.ContainsKey("狀態"))
                        StudStatus = reader.GetValue("狀態");

                    if (Context.SelectedFields.ContainsKey(snum))
                    {
                        if (importRecords.ContainStudentNumber(reader.GetValue(snum) + StudStatus))
                        {
                            rowMessages[reader.RelativelyIndex].ReportMessage(snum, MessageType.Error,
                                "此資料已存在於資料庫中，無法重複新增。");

                            _error_count++;
                        }
                    }

                    if (Context.SelectedFields.ContainsKey(ssn))
                    {
                        if (importRecords.ContainIdNumber(reader.GetValue(ssn) + StudStatus))
                        {
                            rowMessages[reader.RelativelyIndex].ReportMessage(ssn, MessageType.Error,
                                "此資料已存在於資料庫中，無法重複新增。");

                            _error_count++;
                        }
                    }

                    if (Context.SelectedFields.ContainsKey(login))
                    {
                        if (importRecords.ContainSALoginName(reader.GetValue(login).ToLower() + StudStatus))
                        {
                            rowMessages[reader.RelativelyIndex].ReportMessage(login, MessageType.Error,
                                "此資料已存在於資料庫中，無法重複新增。");

                            _error_count++;
                        }
                    }
                }
            }
            else
            {
                while (reader.MoveNext())
                {
                    // 預設狀態為一般
                    StudStatus = "一般";

                    if (reader.Columns.ContainsKey("狀態"))
                        StudStatus = reader.GetValue("狀態");

                    if (Context.IdentifyField == id)
                    {
                        if (!importRecords.ContainIdentity(reader.GetValue(id)))
                        {
                            rowMessages[reader.RelativelyIndex].ReportMessage(id, MessageType.Error,
                                "此資料並不存在於資料庫中，無法更新此筆資料。");
                            _error_count++;
                        }
                    }
                    else if (Context.IdentifyField == snum)
                    {
                        if (StudStatus != "一般" && StudStatus != "")
                        {
                            rowMessages[reader.RelativelyIndex].ReportMessage(snum, MessageType.Error,
                                    "若要匯入更新狀態非一般的學生基本資料，請使用學生系統編號作為驗證欄位");
                            _error_count++;
                        }
                        else 
                        {
                            if (!importRecords.ContainStudentNumber(reader.GetValue(snum) + StudStatus))
                            {
                                rowMessages[reader.RelativelyIndex].ReportMessage(snum, MessageType.Error,
                                    "此資料並不存在於資料庫中，無法更新此筆資料，請確認學號是、狀態否輸入錯誤。");
                                _error_count++;
                            }                                                
                        }
                        
                    }
                    else if (Context.IdentifyField == ssn)
                    {
                        if (StudStatus != "一般" && StudStatus != "")
                        {
                            rowMessages[reader.RelativelyIndex].ReportMessage(snum, MessageType.Error,
                                    "若要匯入更新狀態非一般的學生基本資料，請使用學生系統編號作為驗證欄位");
                            _error_count++;
                        }
                        else
                        {
                            if (!importRecords.ContainIdNumber(reader.GetValue(ssn) + StudStatus))
                            {
                                rowMessages[reader.RelativelyIndex].ReportMessage(ssn, MessageType.Error,
                                    "此資料並不存在於資料庫中，無法更新此筆資料，請確認學號是、狀態否輸入錯誤。");
                                _error_count++;
                            }
                        }                        
                    }
                }

                if (_error_count <= 0) //當所有資料都正確時，才進行此檢查。
                {
                    ProgressMessage("檢查資料與識別欄正確性…");
                    reader.Reset(); //深度檢查…
                    while (reader.MoveNext())
                    {
                        // 預設狀態為一般
                        StudStatus = "一般";

                        if (reader.Columns.ContainsKey("狀態"))
                            StudStatus = reader.GetValue("狀態");

                        ImportRecord record = null;
                        string key = reader.GetValue(Context.IdentifyField);

                        if (Context.IdentifyField == id && (importRecords.ContainIdentity(key)))
                            record = importRecords.GetByIdentity(key);
                        else if (Context.IdentifyField == snum)
                        {
                            if (importRecords.ContainStudentNumber(key + StudStatus))
                                record = importRecords.GetByStudentNumber(key+StudStatus);
                        }
                        else if (Context.IdentifyField == ssn)
                        {
                            if (importRecords.ContainIdNumber(key + StudStatus))
                                record = importRecords.GetByIdNumber(key+StudStatus);
                        }
                        if (record == null)
                            continue;

                        //更新回 Object Model，以便判斷資料是否重覆。(StudentNumber)
                        if (Context.SelectedFields.ContainsKey(snum))
                            record.StudentNumber = reader.GetValue(snum);

                        //更新回 Object Model，以便判斷資料是否重覆。(IDNumber)
                        if (Context.SelectedFields.ContainsKey(ssn))
                            record.IDNumber = reader.GetValue(ssn);

                        //更新回 Object Model，以便判斷資料是否重覆。(SALoginName)
                        if (Context.SelectedFields.ContainsKey(login))
                            record.SALoginName = reader.GetValue(login);

                        record.AbsoluteRowIndex = reader.AbsoluteIndex;
                        record.RelativelyRowIndex = reader.RelativelyIndex;
                    }

                    if (Context.IdentifyField != ssn)
                    {
                        List<ImportRecord> dupSSN = importRecords.GetDuplicateIdNumberList();
                        foreach (ImportRecord each in dupSSN)
                        {
                            if (each.RelativelyRowIndex != -1)
                            {
                                rowMessages[each.RelativelyRowIndex].ReportMessage(ssn, MessageType.Error, "更新資料後，會造成此欄位資料重複(可能是與資料庫中資料重覆)。");
                                _error_count++;
                            }
                        }
                    }

                    if (Context.IdentifyField != snum)
                    {
                        List<ImportRecord> dupSNum = importRecords.GetDuplicateStudentNumberList();
                        foreach (ImportRecord each in dupSNum)
                        {
                            if (each.RelativelyRowIndex != -1)
                            {
                                rowMessages[each.RelativelyRowIndex].ReportMessage(snum, MessageType.Error, "更新資料後，會造成此欄位資料重複(可能是與資料庫中資料重覆)。");
                                _error_count++;
                            }
                        }
                    }

                    List<ImportRecord> dupLogin = importRecords.GetDuplicateSALoginNameList();
                    foreach (ImportRecord each in dupLogin)
                    {
                        if (each.RelativelyRowIndex != -1)
                        {
                            rowMessages[each.RelativelyRowIndex].ReportMessage(login, MessageType.Error, "更新資料後，會造成此欄位資料重複(可能是與資料庫中資料重覆)。");
                            _error_count++;
                        }
                    }
                }
            }
        }

        private void ProgressMessage(string msg)
        {
            lblValidMsg.Text = msg;
            Application.DoEvents();
        }

        private void InitValidStartScreen()
        {
            _correct_count = 0;
            RefreshCorrectCount();
            _warning_count = 0;
            RefreshWarningCount();
            _error_count = 0;
            RefreshErrorCount();
            _cancel_validate = false;
            pgValidProgress.Minimum = 0;
            pgValidProgress.Maximum = Context.SourceReader.RowCount;
            pgValidProgress.Value = 0;
            lnkCancelValid.Visible = true;
            btnValidate.Enabled = false;
            btnViewResult.Enabled = false;
            chkSepErrors.Enabled = false;
            Application.DoEvents();
        }

        private ValidateColumnCollection CreateValidateColumns()
        {
            ValidateColumnCollection validColumns = new ValidateColumnCollection();

            if (Context.ImportMode == ImportMode.Update)
            {
                validColumns.Add(Context.IdentifyField,
                    new ValidateColumn(Context.SourceColumns[Context.IdentifyField], (byte)validColumns.Count));

                if (!string.IsNullOrEmpty(Context.ShiftCheckField))
                {
                    validColumns.Add(Context.ShiftCheckField,
                        new ValidateColumn(Context.SourceColumns[Context.ShiftCheckField], (byte)validColumns.Count));
                }
            }

            foreach (SheetColumn each in Context.SelectedFields.Values)
                validColumns.Add(each.Name, new ValidateColumn(each, (byte)validColumns.Count));

            Context.ValidateColumns = validColumns;
            return validColumns;
        }

        private void RefreshValidProgress(int value)
        {
            pgValidProgress.Value = value;
        }

        private void Validator_ErrorCaptured(string FieldName, string ErrorType, string Description, IRowSource RowSource)
        {
            if (FieldName == Context.ShiftCheckField) //如果是驗證欄，就等於不檢查。
                return;

            if (ErrorType.ToUpper() == "WARNING")
            {
                _warning_count++;
                _current_row_message.ReportMessage(FieldName, MessageType.Warning, Description);
            }
            else if (ErrorType.ToUpper() == "ERROR")
            {
                _error_count++;
                _current_row_message.ReportMessage(FieldName, MessageType.Error, Description);
            }

            if (_error_count % 50 == 0)
                RefreshErrorCount();

            if (_warning_count % 50 == 0)
                RefreshWarningCount();
        }

        private void Validator_AutoCorrect(string FieldName, string OldValue, string NewValue, IRowSource RowSource)
        {
            if (FieldName == Context.ShiftCheckField) //如果是驗證欄，就等於不檢查。
                return;

            _correct_count++;

            _current_row_message.ReportMessage(FieldName, MessageType.Correct, "資料由「" + OldValue + "」更正為「" + NewValue + "」。");

            SheetRowSource source = RowSource as SheetRowSource;
            if (source != null)
                source.SetStringValue(FieldName, NewValue);

            if (_correct_count % 50 == 0)
                RefreshCorrectCount();
        }

        private void RefreshErrorCount()
        {
            lblErrorCount.Text = _error_count.ToString();
            Application.DoEvents();
        }

        private void RefreshWarningCount()
        {
            lblWarningCount.Text = _warning_count.ToString();
            Application.DoEvents();
        }

        private void RefreshCorrectCount()
        {
            lblCorrectCount.Text = _correct_count.ToString();
            Application.DoEvents();
        }

        private void btnViewResult_Click(object sender, EventArgs e)
        {
            try
            {
                ProgramProcess.Start(Context.SourceFileName);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        private void lblContinueMsg_Click(object sender, EventArgs e)
        {
            //chkContinue.Checked = true;
        }

        private void chkContinue_CheckedChanged(object sender, EventArgs e)
        {
            if (chkContinue.Checked == true)
                Context.ValidateComplete = true;
            else
                Context.ValidateComplete = false;
        }

        private void wizardPage2_NextButtonClick(object sender, CancelEventArgs e)
        {
            if (!Context.ValidateComplete)
            {
                e.Cancel = true;
                FISCA.Presentation.Controls.MsgBox.Show("請再次進行資料檢查，請確認資料完全正確再進行匯入動作。");
                return;
            }
        }
        #endregion

        #region Import Page
        private void wpImport_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            pgImport.Value = 0;
        }

        private void lnkCancelImport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        // 取得學生狀態代碼
        private string GetStudStatusCode(string str)
        {
            // 預設一般生1
            string retVal = "1";

            if (str == "一般")
                retVal = "1";

            if (str == "休學")
                retVal = "4";

            if (str == "輟學")
                retVal = "8";

            if (str == "畢業或離校")
                retVal = "16";

            if (str == "刪除")
                retVal = "256";

            return retVal;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ImportRecordCollection importRecords = new ImportRecordCollection();

            // 讀取所有狀態學生
            foreach (JHSchool.Data.JHStudentRecord studRec in JHSchool.Data.JHStudent.SelectAll())
            {
                //    string str1 = studRec.StudentNumber;
                //    string str2 = studRec.IDNumber;

                //    // 建立用學號對應編號
                //    if (!StudIDByStudNum.ContainsKey(str1))
                //        StudIDByStudNum.Add(str1, studRec.ID);

                //    // 建立用身分證號對應編號
                //    if (!StudIDByStudIDNumber.ContainsKey(str2))
                //        StudIDByStudIDNumber.Add(str2, studRec.ID);

                importRecords.Add(new ImportRecord(studRec));
            }


            try
            {
                //// 收集學生狀態
                //Dictionary<string, string> StudStatusDict = new Dictionary<string, string>();

                //// 收集新增的學生狀態
                //Dictionary<string, string> NewStudStatusDict = new Dictionary<string, string>();

                XmlElement output = DSXmlHelper.LoadXml("<ImportStudentStudent/>");

                ImportMessage("產生匯入資料…");
                BulkColumnCollection columns = Context.BulkDescription.Columns.GetColumnList(Context.ValidateColumns.GetNames());

                Context.RefreshImportSource();
                Context.SourceBook.CalculateFormula(); //更新畫面上的公式資料。
                SheetReader reader = Context.SourceReader;
                reader.ConvertFormulaToValue();

                BulkDescription bulkdesc = Context.BulkDescription;

                pgImport.Minimum = 0;
                pgImport.Maximum = reader.RowCount;
                pgImport.Value = 0;
                btnImport.Enabled = false;

                reader.Reset();
                while (reader.MoveNext())
                {
                    XmlElement record = output.OwnerDocument.CreateElement("Student");
                                        
                    //// 這段在收集工作表內的狀態，主要用在更新學生狀態使用
                    //if (Context.SourceReader.Columns.ContainsKey("狀態"))
                    //{
                    //    string ID=string.Empty ;
                    //    string Status = Context.SourceReader.GetValue("狀態");
                    //    string Name = string.Empty;

                    //    if (Context.SourceReader.Columns.ContainsKey("姓名"))
                    //        Name = Context.SourceReader.GetValue("姓名");

                    //    // 收集新增
                    //    if (!NewStudStatusDict.ContainsKey(Name))
                    //        NewStudStatusDict.Add(Name, Status);

                    //    if (Context.SourceReader.Columns.ContainsKey("學生系統編號"))
                    //        ID = Context.SourceReader.GetValue("學生系統編號");
                    //    else
                    //    {
                    //        if (Context.SourceReader.Columns.ContainsKey("學號"))
                    //        {
                    //            if(StudIDByStudNum.ContainsKey(Context.SourceReader.GetValue("學號")))
                    //                ID = StudIDByStudNum[Context.SourceReader.GetValue("學號")];
                    //        }
                    //        else
                    //        { 
                    //            if(Context.SourceReader.Columns.ContainsKey("身分證號"))
                    //                if(StudIDByStudIDNumber.ContainsKey(Context.SourceReader.GetValue("身分證號")))
                    //                    ID = StudIDByStudIDNumber[Context.SourceReader.GetValue("身分證號")];
                    //        }
                        
                    //    }   
                        
                    //    if (!StudStatusDict.ContainsKey(ID))
                    //        StudStatusDict.Add(ID, Status);
                    //}


                    if (Context.ImportMode == ImportMode.Insert)
                        bulkdesc.GenerateInsertRequest(Context.SourceReader, columns, record);
                    else
                    //假如是ImportMode.Update 更新模式
                    {

                        string ref_student_id = "";

                        if (Context.IdentifyField == "學生系統編號")
                        {
                            ref_student_id = Context.SourceReader.GetValue("學生系統編號");
                        }
                        if (Context.IdentifyField == "學號")
                        {
                            ref_student_id = importRecords.GetByStudentNumber(Context.SourceReader.GetValue("學號") + "一般").Identity;
                        }
                        if (Context.IdentifyField == "身分證號")
                        {
                            ref_student_id = importRecords.GetByIdNumber(Context.SourceReader.GetValue("身分證號") + "一般").Identity;
                        }

                        bulkdesc.GenerateUpdateRequest(Context.SourceReader, columns, record, Context.IdentifyField, Context.ShiftCheckField, ref_student_id);
                    }
                    

                    output.AppendChild(record);

                    if (reader.RelativelyIndex % 10 == 0)
                        pgImport.Value = reader.RelativelyIndex;
                }
                pgImport.Value = reader.RelativelyIndex;

                //處理班級編號。
                ProcessClassLookup(output);

                //處理畢業證書字號。
                ProcessGraduateInfo(output);

                //output.OwnerDocument.Save(Context.SourceFileName + ".xml");

                //return;

                //計算密碼雜~!
                if (Context.SelectedFields.ContainsKey("登入密碼"))
                    HashPassword(output);

               
               

                ImportMessage("上傳資料到主機，請稍後…");
                //GeneralActionLog log = new GeneralActionLog();
                //log.Source = "匯入學生基本資料";
                //log.Diagnostics = output.OuterXml;

                PermRecLogProcess prlp = new PermRecLogProcess();

                if (Context.ImportMode == ImportMode.Insert)
                {
                    StudentBulkProcess.InsertImportStudent(output);
                    //// 需要回傳新增 StudentID
                    //XmlElement rspXml=StudentBulkProcess.InsertImportStudentRsp(output);


                    //List<string> NewStudIDs = new List<string>();
                    //Dictionary<string, string> UpdateStudStatus = new Dictionary<string, string>();


                    //// 取得新增的 StudentID
                    //if (rspXml != null)
                    //    foreach (XmlElement xm in rspXml.SelectNodes("NewID"))
                    //        NewStudIDs.Add(xm.InnerText);

                    //foreach (JHSchool.Data.JHStudentRecord stud in JHSchool.Data.JHStudent.SelectByIDs(NewStudIDs))
                    //{
                    //    // 用學生姓名比對
                    //    if (NewStudStatusDict.ContainsKey(stud.Name))
                    //       UpdateStudStatus.Add(stud.ID,NewStudStatusDict[stud.Name]);                    
                    //}

                    //// 更新學生狀態
                    //if (UpdateStudStatus.Count > 0)
                    //    UpdateStudStatusByStudID(UpdateStudStatus);

                    //log.ActionName = "新增匯入";
                    //log.Description = "新增匯入 " + reader.RowCount + " 筆學生資料。";
                    prlp.SaveLog("學生.匯入學生基本資料", "批次匯入", "批次新增匯入" + reader.RowCount + "筆學生資料.");
                    

                }
                else
                {
                    // 檢查是否只有更新狀態，如果只有更新狀態就用新的更新學生狀態方式，不呼叫原有方式
                    //bool checkUpdate=true;
                    //if (Context.SelectedFields.ContainsKey("狀態") && Context.SelectedFields.Count == 1)
                    //    checkUpdate = false;

                    //if(checkUpdate)
                        StudentBulkProcess.UpdateImportStudent(output);

                    //// 更新學生狀態
                    //if(StudStatusDict.Count >0)
                    //    UpdateStudStatusByStudID(StudStatusDict);

                    //log.ActionName = "更新匯入";
                    //log.Description = "更新匯入 " + reader.RowCount + " 筆學生資料。";
                    prlp.SaveLog("學生.匯入學生基本資料", "批次匯入", "批次更新匯入" + reader.RowCount + "筆學生資料.");
                }

                //int t1 = Environment.TickCount;

                //if (Context.SelectedFields.ContainsKey("新生:異動代號") &&
                //    Context.SelectedFields.ContainsKey("新生:異動日期") &&
                //    Context.SelectedFields.ContainsKey("新生:國中校名") &&
                //    Context.SelectedFields.ContainsKey("新生:國中縣市"))
                //    StudentBulkProcess.SyncEnrollmentInfoToUpdateRecord();

                //Console.WriteLine("SyncEnrollmentInfoToUpdateRecord:" + Environment.TickCount - t1);

                //CurrentUser.Instance.AppLog.Write(log);

                ImportMessage("匯入完成。");
                wpImport.FinishButtonEnabled = eWizardButtonState.True;
                wpImport.BackButtonEnabled = eWizardButtonState.False;

                Student.Instance.SyncAllBackground();

                // 與 DAL 同步
                JHSchool.Data.JHStudent.RemoveAll();
                JHSchool.Data.JHStudent.SelectAll();

                //SmartSchool.StudentRelated.Student stu = SmartSchool.StudentRelated.Student.Instance;
                //stu.ReloadData();
            }
            
            catch (Exception ex)            
            {
                //Console.Write((ex as DSAServerException).WarpedError.Response);
                ImportMessage("上傳資料失敗");


                XElement errElm = XElement.Parse((ex as DSAServerException).WarpedError.Response);

                if (errElm != null)
                {
                    string msg = errElm.Element("Body").Element("CompleteErrorDetails").LastNode.ToString();
                    FISCA.Presentation.Controls.MsgBox.Show(msg);
                }
                else
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                
                //CurrentUser user = CurrentUser.Instance;
                //BugReporter.ReportException(user.SystemName, user.SystemVersion, ex, false);

                btnImport.Enabled = true;

                
               
            }
        }

        ///// <summary>
        ///// 透過學生編號更新狀態
        ///// </summary>
        ///// <param name="StudStatusName"></param>
        //private void UpdateStudStatusByStudID(Dictionary<string, string> StudStatusName)
        //{
        //    StringBuilder sb = new StringBuilder();           
           
            
        //    List<JHSchool.Data.JHStudentRecord> UpdateList = new List<JHSchool.Data.JHStudentRecord>();
        //    foreach (JHSchool.Data.JHStudentRecord studRec in JHSchool.Data.JHStudent.SelectByIDs(StudStatusName.Keys))
        //    {
        //        if (StudStatusName.ContainsKey(studRec.ID))
        //            if (studRec.Status.ToString() != StudStatusName[studRec.ID])
        //            {
        //                sb.Append("學生:"+studRec.Name +",狀態由「"+studRec.Status.ToString () +"」變更成「"+StudStatusName[studRec.ID]+"」;");
        //                studRec.Status = ConvertJHStudStatus(StudStatusName[studRec.ID]);

        //                UpdateList.Add(studRec);
        //            }
        //    }

        //    // 更新學生狀態寫入 Log
        //    PermRecLogProcess prlp = new PermRecLogProcess();

        //    prlp.SaveLog("學生.匯入學生基本資料", "批次匯入", sb.ToString ());
        //    // 更新
        //    JHSchool.Data.JHStudent.Update(UpdateList);

        //}

        ///// <summary>
        ///// 轉換文字與學生狀態
        ///// </summary>
        ///// <param name="str"></param>
        ///// <returns></returns>
        //private K12.Data.StudentRecord.StudentStatus ConvertJHStudStatus(string str)
        //{
        //    K12.Data.StudentRecord.StudentStatus status = K12.Data.StudentRecord.StudentStatus.一般;

        //    if (str == "一般" || str == "")
        //        status = K12.Data.StudentRecord.StudentStatus.一般;

        //    if (str == "休學")
        //        status = K12.Data.StudentRecord.StudentStatus.休學;

        //    if (str == "刪除")
        //        status = K12.Data.StudentRecord.StudentStatus.刪除;

        //    if (str == "畢業或離校")
        //        status = K12.Data.StudentRecord.StudentStatus.畢業或離校;

        //    if (str == "輟學")
        //        status = K12.Data.StudentRecord.StudentStatus.輟學;

        //    return status;
        //}

        private void ProcessClassLookup(XmlElement output)
        {
            if (Context.SelectedFields.ContainsKey("班級"))
            {
                Dictionary<string, string> classes = GetClassCollection();
                foreach (XmlElement each in output.SelectNodes("Student"))
                {
                    string className = each.SelectSingleNode("ClassName").InnerText.Trim();

                    XmlNode elmClassId = each.OwnerDocument.CreateElement("ClassID");
                    elmClassId = each.AppendChild(elmClassId);

                    if (classes.ContainsKey(className))
                        elmClassId.InnerText = classes[className];
                }
            }
        }

        private void ProcessGraduateInfo(XmlElement output)
        {
            string gradefn = "畢結業證書字號";
            if (Context.SelectedFields.ContainsKey(gradefn))
            {
                if (Context.ImportMode == ImportMode.Update)
                {
                    
                    DiplomaNumberLookup dnlook = new DiplomaNumberLookup();
                    foreach (XmlElement each in output.SelectNodes("Student"))
                    {
                        //string identity = each.SelectSingleNode("Condition/" + dnlook.ExportName).InnerText.Trim();
                        string identity = each.SelectSingleNode("Condition/StudentID").InnerText.Trim();
                        XmlElement diplomaDB = dnlook.GetDiplomaElement(identity);
                        XmlElement diplomaNew = each.SelectSingleNode("DiplomaNumberRaw") as XmlElement;

                        if (diplomaDB.SelectSingleNode("DiplomaNumber") == null)
                        {
                            XmlElement dnNew = each.OwnerDocument.CreateElement("DiplomaNumber");
                            dnNew.InnerText = diplomaNew.InnerText;
                            dnNew = diplomaDB.OwnerDocument.ImportNode(dnNew, true) as XmlElement;
                            diplomaDB.AppendChild(dnNew);
                        }
                        else
                        {
                            XmlElement dnOld = diplomaDB.SelectSingleNode("DiplomaNumber") as XmlElement;
                            dnOld.InnerText = diplomaNew.InnerText;
                        }
                        diplomaDB = each.OwnerDocument.ImportNode(diplomaDB, true) as XmlElement;
                        each.ReplaceChild(diplomaDB, diplomaNew);
                    }
                }
            }
        }

        private void HashPassword(XmlElement output)
        {
            foreach (XmlElement each in output.SelectNodes("Student"))
            {
                if (!string.IsNullOrEmpty(each.SelectSingleNode("Password").InnerText.Trim()))
                    each.SelectSingleNode("Password").InnerText = PasswordHash.Compute(each.SelectSingleNode("Password").InnerText);
            }
        }

        private Dictionary<string, string> GetClassCollection()
        {
            Dictionary<string, string> classes = new Dictionary<string, string>();
            DSXmlHelper rsp = QueryClass.GetClassList().GetContent();

            foreach (XmlElement each in rsp.GetElements("Class"))
            {
                string className = each.SelectSingleNode("ClassName").InnerText.Trim();
                string classId = each.GetAttribute("ID");

                classes.Add(className, classId);
            }

            return classes;
        }

        private void ImportMessage(string msg)
        {
            lblImportProgress.Text = msg;
            Application.DoEvents();
        }
        #endregion

        private void ImportWizard_CancelButtonClick(object sender, CancelEventArgs e)
        {
            if (Context.AllowExit)
                Close();
        }

        private void ImportWizard_FinishButtonClick(object sender, CancelEventArgs e)
        {
            Close();
        }

        private class DiplomaNumberLookup
        {
            private Dictionary<string, XmlElement> _diploma_list;

            public DiplomaNumberLookup()
            {
                _diploma_list = new Dictionary<string, XmlElement>();            

                string[] fields = new string[] {"ID", "DiplomaNumber","Status" };
                DSXmlHelper alldiploma = QueryStudent.GetDetailList(fields).GetContent();

                foreach (XmlElement each in alldiploma.GetElements("Student"))
                {
                    DSXmlHelper hlpeach = new DSXmlHelper(each);
                    string key = hlpeach.GetText("@ID");

                    if (hlpeach.GetText("Status") != "一般") continue;

                    if (string.IsNullOrEmpty(key.Trim())) continue;

                    _diploma_list.Add(key, each.SelectSingleNode("DiplomaNumber") as XmlElement);
                }
            }

            private string getFieldName()
            {
                if (_id_fieldName == "ID")
                    return "@ID";
                else
                    return _id_fieldName;
            }

            public XmlElement GetDiplomaElement(string identity)
            {
                if (_diploma_list.ContainsKey(identity))
                    return _diploma_list[identity];
                else
                    return null;
            }

            private string _id_fieldName;

            private string _export_name;
            public string ExportName
            {
                get { return _export_name; }
            }

        }

        private class FieldValidatorFactory : IFieldValidatorFactory
        {

            #region IFieldValidatorFactory 成員

            public IFieldValidator CreateFieldValidator(string TypeName)
            {
                if (TypeName.ToUpper() == "MixDateCSharp".ToUpper())
                {
                    return new MixDateFieldValidator();
                }
                else
                    return null;
            }

            #endregion
        }
    }
}