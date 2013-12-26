using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using System.Xml;
using System.IO;
using Aspose.Cells;
using DocValidate;
using FISCA.DSAUtil;
using System.Diagnostics;
using FISCA.Presentation.Controls;
using JHSchool.Legacy.ImportSupport;
using JHSchool.CourseExtendControls.Ribbon.CourseImportWizardControls;
using Framework;
using JHSchool.Legacy.ImportSupport.Lookups;
using DevComponents.DotNetBar.Rendering;

namespace JHSchool.CourseExtendControls.Ribbon
{
    public partial class CourseImportWizard : BaseForm
    {
        private WizardContext _context;
        private ImportDataAccess _data_source;

        public CourseImportWizard()
        {
            InitializeComponent();
            #region 設定Wizard會跟著Style跑
            (GlobalManager.Renderer as Office2007Renderer).ColorTableChanged += new EventHandler(CourseImportWizard_ColorTableChanged);
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
            //SmartSchool.Common.SkillSchool.SetConnection("smartschool@dev", "admin", "1234");
            _context = new WizardContext();
            _data_source = new ImportDataAccess();
            _context.DataSource = _data_source;
        }

        void CourseImportWizard_ColorTableChanged(object sender, EventArgs e)
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
            Context.CurrentMode = ImportMode.Insert;
            chkInsert.Checked = true;
        }

        private void lblUpdateDesc_Click(object sender, EventArgs e)
        {
            Context.CurrentMode = ImportMode.Update;
            chkUpdate.Checked = true;
        }

        private void chkInsert_CheckedChanged(object sender, EventArgs e)
        {
            Context.CurrentMode = ImportMode.Insert;
        }

        private void chkUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Context.CurrentMode = ImportMode.Update;
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

                Context.SourceFile = txtSourceFile.Text;
                if (string.IsNullOrEmpty(Context.SourceFile))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您必須選擇匯入來源檔案。");
                    return;
                }

                if (!File.Exists(Context.SourceFile))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您指定的來源檔案並不存在。");
                    return;
                }

                if (Context.CurrentMode == ImportMode.None)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您必須決定一種匯入方式。");
                    return;
                }

                lblCollectMsg.Text = "讀取匯入規格描述資訊…";
                pProgram.Visible = true;
                pUser.Visible = false;
                Application.DoEvents();

                XmlElement fieldData = Context.DataSource.GetImportFieldList();
                Context.SupportFields = ImportFieldCollection.CreateFieldsFromXml(fieldData);
                Context.UpdateConditions = ImportCondition.CreateConditionFromXml(fieldData, Context.SupportFields);

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
                //TODO 檢查來源資料中是否所有必要欄位都有了。
                //如果沒有包含所有必要欄位，必須回到上一頁。
                //Insert 與 Update 的必要欄位不相同。

                if (Context.CurrentMode == ImportMode.Insert)
                    e.NewPage = wpSelectField;
                else
                    e.NewPage = wpCollectKeyInfo;
            }
        }

        /// <summary>
        /// 用於 ShiftCheck 的空白狀況。
        /// </summary>
        private ImportField EmptyShiftField = new ImportField("<不使用>");

        private void wpCollectKeyInfo_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            try
            {
                if (e.PageChangeSource == eWizardPageChangeSource.BackButton)
                    return;

                cboIdField.SelectedIndex = -1;
                cboIdField.Items.Clear();
                cboIdField.DisplayMember = "Name";

                cboValidateField.SelectedItem = null;
                cboValidateField.Items.Clear();
                cboValidateField.DisplayMember = "FieldName";
                cboValidateField.Items.Add(EmptyShiftField);
                cboValidateField.SelectedIndex = 0;

                SheetHelper sheet = new SheetHelper(Context.SourceFile);
                IEnumerable<string> fields = sheet.Fields;
                ImportFieldCollection acceptFields = Context.SupportFields.Intersect(fields);
                List<ImportCondition> conditions = Context.UpdateConditions;

                foreach (ImportCondition each in conditions)
                {
                    if (each.ContainsAllField(fields))
                        cboIdField.Items.Add(each);
                }

                foreach (ImportField each in acceptFields)
                {
                    if (each.ShiftCheckable)
                        cboValidateField.Items.Add(each);
                }

                if (cboIdField.Items.Count <= 0)
                    ctlerrors.SetError(cboIdField, "在來源資料中沒有任何可以當識別欄的欄位。");
                else
                    ctlerrors.SetError(cboIdField, string.Empty);

                wpCollectKeyInfo.NextButtonEnabled = eWizardButtonState.Auto;
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                //CurrentUser.ReportError(ex);
                wpCollectKeyInfo.NextButtonEnabled = eWizardButtonState.False;
            }

        }

        private void wpCollectKeyInfo_NextButtonClick(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            if (cboIdField.SelectedIndex == -1)
            {
                FISCA.Presentation.Controls.MsgBox.Show("您必須要選擇識別欄位。");
                return;
            }

            ImportCondition condition = cboIdField.SelectedItem as ImportCondition;
            if (condition != null)
                Context.IdentifyField = condition;

            ImportField field = cboValidateField.SelectedItem as ImportField;
            if (field == EmptyShiftField)
                Context.ShiftCheckField = null;
            else
                Context.ShiftCheckField = field;

            if (Context.ShiftCheckField != null)
            {
                if (Context.IdentifyField.ContainsAnyField(Context.ShiftCheckField.FieldName))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("「識別欄」與「驗證欄」必須是不同的欄位。");
                    return;
                }
            }

            e.Cancel = false;
        }

        #endregion

        #region Select Field Page

        private List<SheetField> _sheet_fields;
        private bool _block_behavior = false;

        private void wpSelectField_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            try
            {
                if (e.PageChangeSource == eWizardPageChangeSource.BackButton)
                    return;

                //讀取所有工作表上的欄位資訊。
                SheetHelper sheet = new SheetHelper(Context.SourceFile);
                IEnumerable<string> strSheetFields = sheet.Fields;

                //檢查所有必要欄位是否都存在，如果有缺少，就關閉精靈的下一步。
                if (!RequiredFieldAllAvailable(strSheetFields))
                    return;

                //取得工作表欄位中有被支援的欄位。
                _sheet_fields = GetSupportFields(strSheetFields);

                //把前次選過的欄位打勾。
                TipStyles tip = new TipStyles(sheet);
                List<string> usedFields = sheet.GetFieldsByColor(tip.Header.ForegroundColor);
                SelectPreviousUsedFields(usedFields, _sheet_fields);

                //檢查 Insert、Update 模式中需要限制的欄位狀況。
                CheckModeDifference();

                chkHideSome.Checked = false;

                //顯示欄位到 ListView 上。
                ShowFieldsToListView();

                wpSelectField.NextButtonEnabled = eWizardButtonState.Auto;
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                //CurrentUser.ReportError(ex);
                wpSelectField.NextButtonEnabled = eWizardButtonState.False;
            }
        }

        /// <summary>
        /// 把前次選過的欄位打勾。
        /// </summary>
        /// <param name="usedFields">之前有選擇過的欄位清單。</param>
        /// <param name="availableFields">現在所有可用的欄位清單。</param>
        private void SelectPreviousUsedFields(List<string> usedFields, List<SheetField> availableFields)
        {
            if (usedFields.Count <= 0) return;

            foreach (SheetField each in availableFields)
            {
                if (each.BindField == null) continue;

                if (usedFields.Contains(each.BindField.FieldName))
                    each.Checked = true;
            }
        }

        /// <summary>
        /// 檢查 Insert、Update 模式中需要限制的欄位狀況。
        /// </summary>
        private void CheckModeDifference()
        {
            if (Context.CurrentMode == ImportMode.Insert)
            {
                foreach (ListViewItem eachField in _sheet_fields)
                {
                    SheetField field = eachField as SheetField;
                    if (field == null) continue;
                    if (field.BindField == null) continue;

                    if (field.BindField.PrimaryKey)
                    {
                        field.Checked = false;
                        field.Lock("此欄位無法匯入，是由系統自動產生", Color.Silver);
                    }

                    if (field.BindField.InsertRequired)
                    {
                        field.Checked = true;
                        field.Lock("此欄位為新增匯入的必要欄位。", Color.Blue);
                    }
                }
            }
            else if (Context.CurrentMode == ImportMode.Update) //檢查唯讀欄位不可匯入。
            {
                //驗證欄位、識別欄不可匯入。
                //檢查唯讀欄位(Update)。
                foreach (ListViewItem eachField in _sheet_fields)
                {
                    SheetField field = eachField as SheetField;
                    if (field == null) continue;
                    if (field.BindField == null) continue;

                    if (field.BindField == Context.ShiftCheckField)
                    {
                        field.Checked = false;
                        field.Lock("驗證欄位無法匯入。", Color.Silver);
                    }

                    if (Context.IdentifyField.ContainsAnyField(field.BindField.FieldName))
                    {
                        field.Checked = false;
                        field.Lock("識別欄位無法匯入。", Color.Silver);
                    }

                    if (field.BindField.PrimaryKey)
                    {
                        field.Checked = false;
                        field.Lock("此欄位為特殊系統編號欄，無法匯入。", Color.Silver);
                    }
                }
            }
        }

        /// <summary>
        /// 檢查來源欄位中有哪些欄位是被系統支援匯入的。
        /// </summary>
        /// <param name="strSheetFields"></param>
        private List<SheetField> GetSupportFields(IEnumerable<string> strSheetFields)
        {
            List<SheetField> fields = new List<SheetField>();
            foreach (string eachField in strSheetFields)
            {
                ImportFieldCollection objFields = Context.SupportFields;

                SheetField objSheetField;

                if (objFields.Contains(eachField))
                    objSheetField = new SheetField(objFields[eachField]);
                else
                    objSheetField = new SheetField(eachField);

                //檢查欄位是否允許匯入。
                if (objSheetField.BindField == null)
                    objSheetField.Lock("系統不支援此欄位匯入。", Color.Silver);

                fields.Add(objSheetField);
            }

            return fields;
        }

        /// <summary>
        /// 檢查所有必要欄位是否都存在，如果有缺少，就關閉精靈的下一步。
        /// </summary>
        /// <param name="strSheetFields"></param>
        private bool RequiredFieldAllAvailable(IEnumerable<string> strSheetFields)
        {
            if (Context.CurrentMode == ImportMode.Insert)
            {
                List<string> lstSourceFields = new List<string>(strSheetFields);
                foreach (ImportField eachField in Context.SupportFields)
                {
                    if (eachField.InsertRequired)
                    {
                        if (!lstSourceFields.Contains(eachField.FieldName))
                        {
                            FISCA.Presentation.Controls.MsgBox.Show(string.Format("您提供的來源欄位中缺少了必要欄位：{0}。", eachField.FieldName));
                            wpSelectField.NextButtonEnabled = eWizardButtonState.False;
                            return false;
                        }
                    }
                }
                wpSelectField.NextButtonEnabled = eWizardButtonState.Auto;
            }
            return true;
        }

        private void lvSourceFieldList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_block_behavior) return;

            SheetField field = lvSourceFieldList.Items[e.Index] as SheetField;

            if (field == null) return;

            if (field.Locked)
                e.NewValue = e.CurrentValue;
        }

        private void lvSourceFieldList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_block_behavior) return;

            //處理群組欄位問題。
            SheetField field = e.Item as SheetField;

            if (field.BindField == null) return;

            string prefix = field.Text.Split(':')[0].Trim();

            foreach (ListViewItem each in lvSourceFieldList.Items)
            {
                if (each.Text.StartsWith(prefix))
                    each.Checked = field.Checked;
            }
        }

        private void chkHideSome_CheckedChanged(object sender, EventArgs e)
        {
            ShowFieldsToListView();
        }

        private void ShowFieldsToListView()
        {
            _block_behavior = true;
            lvSourceFieldList.Items.Clear();
            foreach (SheetField each in _sheet_fields)
            {
                if (chkHideSome.Checked)
                {
                    if (each.BindField == null)
                        lvSourceFieldList.Items.Add(each);
                }

                if (each.BindField != null)
                    lvSourceFieldList.Items.Add(each);
            }
            _block_behavior = false;
        }

        private void wpSelectField_NextButtonClick(object sender, CancelEventArgs e)
        {
            Context.SelectedFields.Clear();
            foreach (SheetField each in lvSourceFieldList.Items)
            {
                if (each.Checked)
                    Context.SelectedFields.Add(each.Text);
            }

            if (Context.SelectedFields.Count <= 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("您必須選擇要匯入的欄位。選擇欄位時，請在名稱前的方格中打勾。");
                e.Cancel = true;
            }

            List<string> fields = Context.SelectedFields;

            if (fields.Contains("所屬班級"))
            {
                if (Context.Extensions.ContainsKey(ClassLookup.Name))
                    Context.Extensions.Remove(ClassLookup.Name);

                Context.Extensions.Add(ClassLookup.Name, new ClassLookup());
            }

            if (fields.Contains("授課教師一") || fields.Contains("授課教師二") || fields.Contains("授課教師三"))
            {
                if (Context.Extensions.ContainsKey(TeacherLookup.Name))
                    Context.Extensions.Remove(TeacherLookup.Name);

                Context.Extensions.Add(TeacherLookup.Name, new TeacherLookup());
            }

            if (fields.Contains("評量設定"))
            {
                if (Context.Extensions.ContainsKey(TemplateLookup.Name))
                    Context.Extensions.Remove(TemplateLookup.Name);

                Context.Extensions.Add(TemplateLookup.Name, new TemplateLookup());
            }
        }

        private class SheetField : ListViewItem
        {
            private ImportField _field;
            private bool _locked;

            public SheetField(string fieldName)
            {
                Text = string.Format("{0}", fieldName);
            }

            public SheetField(ImportField field)
            {
                _field = field;
                Text = field.FieldName;
            }

            public ImportField BindField
            {
                get { return _field; }
            }

            public bool Locked
            {
                get { return _locked; }
                private set
                {
                    _locked = value;
                }
            }

            public void Lock(string toolTipText, Color itemColor)
            {
                ToolTipText = toolTipText;
                ForeColor = itemColor;
                Locked = true;
            }

        }
        #endregion

        #region Validation Page

        private bool _cancel_validate;

        private List<CellComment> corrects, errors, warnings;
        private CellCommentManager cellManager;

        private void lnkCancelValid_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _cancel_validate = true;
        }

        private void wizardPage2_AfterPageDisplayed(object sender, WizardPageChangeEventArgs e)
        {
            wpValidation.NextButtonEnabled = eWizardButtonState.False;
            pgValidProgress.Value = 0;
            lblCorrectCount.Text = "0";
            lblErrorCount.Text = "0";
            lblWarningCount.Text = "0";
            lnkCancelValid.Visible = false;
            ProgressMessage("");
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                lblCorrectCount.Text = "0";
                lblErrorCount.Text = "0";
                lblWarningCount.Text = "0";

                //int t1 = Environment.TickCount;

                ProgressMessage("載入資料檢查規則…");
                CourseRowValidatorFactory crv = new CourseRowValidatorFactory(Context);
                ValidateHelper validator = new ValidateHelper(Context, crv);
                SheetHelper sheet = new SheetHelper(Context.SourceFile);
                TipStyles styles = new TipStyles(sheet);

                //Console.WriteLine("載入驗證規則時間：{0}", Environment.TickCount - t1);

                validator.ProgressChanged += new ProgressChangedEventHandler(Validator_ProgressChanged);
                pgValidProgress.Value = 0;

                //t1 = Environment.TickCount;
                ProgressMessage("驗證資料中…");
                lnkCancelValid.Visible = true;
                _cancel_validate = false;
                cellManager = validator.Validate(sheet);
                lnkCancelValid.Visible = false;

                //Console.WriteLine("驗證時間：{0}", Environment.TickCount - t1);

                validator.ProgressChanged -= new ProgressChangedEventHandler(Validator_ProgressChanged);

                if (_cancel_validate)
                {
                    wpValidation.NextButtonEnabled = eWizardButtonState.False;
                    ProgressMessage("資料驗證已由使用者取消…");
                    return;
                }
                else
                    wpValidation.NextButtonEnabled = eWizardButtonState.True;

                //t1 = Environment.TickCount;
                SummaryValidateInfo(cellManager);
                //Console.WriteLine("Summary 時間：{0}", Environment.TickCount - t1);

                //t1 = Environment.TickCount;
                sheet.ClearComments();
                sheet.SetAllStyle(styles.Default);
                foreach (CellComment each in cellManager)
                {
                    CommentItem item = each.BestComment;
                    int row, column;
                    row = each.RowIndex;
                    column = each.ColumnIndex;

                    if (item is CorrectComment)
                    {
                        sheet.SetComment(row, column, item.Comment);
                        sheet.SetStyle(row, column, styles.Correct);
                        sheet.SetValue(row, column, (item as CorrectComment).NewValue);
                    }

                    if (item is ErrorComment)
                    {
                        sheet.SetComment(row, column, item.Comment);
                        sheet.SetStyle(row, column, styles.Error);
                    }

                    if (item is WarningComment)
                    {
                        sheet.SetComment(row, column, item.Comment);
                        sheet.SetStyle(row, column, styles.Warning);
                    }
                }
                //Console.WriteLine("Output Errors 時間：{0}", Environment.TickCount - t1);

                sheet.SetFieldsStyle(Context.SelectedFields, styles.Header);
                sheet.Save(Context.SourceFile);

            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                wpValidation.NextButtonEnabled = eWizardButtonState.False;
            }
        }

        private void SummaryValidateInfo(CellCommentManager cmmManager)
        {
            corrects = new List<CellComment>();
            errors = new List<CellComment>();
            warnings = new List<CellComment>();

            foreach (CellComment each in cmmManager)
            {
                if (each.BestComment is CorrectComment)
                    corrects.Add(each);

                if (each.BestComment is ErrorComment)
                    errors.Add(each);

                if (each.BestComment is WarningComment)
                    warnings.Add(each);
            }

            lblCorrectCount.Text = corrects.Count.ToString();
            lblErrorCount.Text = errors.Count.ToString();
            lblWarningCount.Text = warnings.Count.ToString();

            if (corrects.Count > 0 || errors.Count > 0)
            {
                ProgressMessage("發現錯誤資料…");
                wpValidation.NextButtonEnabled = eWizardButtonState.False;
            }
            else
            {
                ProgressMessage("未發現錯誤資料…");
                wpValidation.NextButtonEnabled = eWizardButtonState.True;
            }
        }

        private void Validator_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgValidProgress.Value = e.ProgressPercentage;

            ValidateHelper.ProgressUserState user = e.UserState as ValidateHelper.ProgressUserState;
            user.Cancel = _cancel_validate;
        }

        private void ProgressMessage(string msg)
        {
            lblValidMsg.Text = msg;
            Application.DoEvents();
        }

        private void btnViewResult_Click(object sender, EventArgs e)
        {
            try
            {
                wpValidation.NextButtonEnabled = eWizardButtonState.False;
                Process.Start(Context.SourceFile);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        private void backup_Click(object sender, EventArgs e)
        {
            FileInfo objFile = new FileInfo(Context.SourceFile);
            string dir = objFile.DirectoryName;
            string file = objFile.Name.Replace(".xls", "_備份.xls");

            BackupDialog.InitialDirectory = dir;
            BackupDialog.FileName = file;

            DialogResult dr = BackupDialog.ShowDialog();
            if (dr == DialogResult.OK)
                File.Copy(Context.SourceFile, Path.Combine(dir, file), true);
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

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                pgImport.Minimum = 0;
                pgImport.Maximum = 100;
                pgImport.Value = 0;
                btnImport.Enabled = false;

                ImportMessage("產生匯入資料…");

                XmlElement output = DSXmlHelper.LoadXml("<ImportCourse/>");
                SheetHelper sheet = new SheetHelper(Context.SourceFile);
                SheetRowSource sheetSource = new SheetRowSource(sheet, Context);
                int ProgressStep = 10;
                int progress = 0, firstRow = sheet.FirstDataRowIndex, maxRow = sheet.MaxDataRowIndex;

                for (int rowIndex = firstRow; rowIndex <= maxRow; rowIndex++)
                {
                    sheetSource.BindRow(rowIndex);

                    XmlElement record = CreateChild(output, "Course");
                    if (Context.CurrentMode == ImportMode.Insert)
                        GenerateInsertXml(sheetSource, record);
                    else
                        GenerateUpdateXml(sheetSource, record);

                    //回報進度。
                    if (((++progress) % ProgressStep) == 0)
                    {
                        int percentage = progress * 100 / (maxRow - firstRow);
                        pgImport.Value = percentage;
                    }
                }

                if (Context.SelectedFields.Contains("所屬班級"))
                    ConvertClassField(output);

                if (Context.SelectedFields.Contains("評量設定"))
                    ConvertTemplateField(output);

                pgImport.Value = 100;

                ImportMessage("上傳資料到主機，請稍後…");
                //GeneralActionLog log = new GeneralActionLog();
                //log.Source = "匯入課程基本資料";
                //log.Diagnostics = output.OuterXml;

                if (Context.CurrentMode == ImportMode.Insert)
                {
                    if (UpdateCourseRequired())
                        Context.DataSource.InsertImportData(output);

                    //log.ActionName = "新增匯入";
                    //log.Description = "新增匯入 " + sheet.DataRowCount + " 筆課程資料。";
                }
                else
                {
                    if (UpdateCourseRequired())
                        Context.DataSource.UpdateImportData(output);

                    //log.ActionName = "更新匯入";
                    //log.Description = "更新匯入 " + sheet.DataRowCount + " 筆課程資料。";
                }

                //CurrentUser.Instance.AppLog.Write(log);

                UpdateTeacherInformation(sheet);

                ImportMessage("匯入完成。");
                wpImport.FinishButtonEnabled = eWizardButtonState.True;
                wpImport.BackButtonEnabled = eWizardButtonState.False;

                //SmartSchool.CourseRelated.CourseEntity.Instance.InvokeAfterCourseInsert();
                //SmartSchool.Broadcaster.Events.Items["課程/新增"].Invoke();
                //SmartSchool.TeacherRelated.Teacher tea = SmartSchool.TeacherRelated.Teacher.Instance;
                //tea.Reflash();

                Course.Instance.SyncAllBackground();
                JHSchool.Data.JHCourse.RemoveAll();
                JHSchool.Data.JHCourse.SelectAll();
            }
            catch (Exception ex)
            {
                //CurrentUser user = CurrentUser.Instance;
                //BugReporter.ReportException(user.SystemName, user.SystemVersion, ex, false);

                btnImport.Enabled = true;

                ImportMessage("上傳資料失敗");

                ExceptionReport report = new ExceptionReport();
                TextViewer.ViewXml(report.Transform(ex));

                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        private void GenerateInsertXml(SheetRowSource sheetSource, XmlElement record)
        {
            foreach (string eachField in Context.SelectedFields)
            {
                ImportField field = Context.SupportFields[eachField];
                XmlElement xmlField = CreateChild(record, field.InternalName);
                xmlField.InnerText = sheetSource.GetFieldData(eachField);
            }
        }

        private void GenerateUpdateXml(SheetRowSource sheetSource, XmlElement record)
        {
            foreach (string eachField in Context.SelectedFields)
            {
                ImportField field = Context.SupportFields[eachField];
                XmlElement xmlField = CreateChild(record, field.InternalName);
                xmlField.InnerText = sheetSource.GetFieldData(eachField);
            }

            XmlElement condition = CreateChild(record, "Condition");
            foreach (ImportField each in Context.IdentifyField.Fields)
            {
                XmlElement xmlField = CreateChild(condition, each.InternalName);
                xmlField.InnerText = sheetSource.GetFieldData(each.FieldName);
            }
        }

        private void ConvertClassField(XmlElement xmlData)
        {
            ClassLookup lookup = Context.Extensions[ClassLookup.Name] as ClassLookup;

            foreach (XmlElement each in xmlData.SelectNodes("Course"))
            {
                string className = each.SelectSingleNode("ClassName").InnerText;
                XmlElement classNode = CreateChild(each, "ClassID");
                classNode.InnerText = lookup.GetClassID(className);
            }
        }

        private void ConvertTemplateField(XmlElement output)
        {
            TemplateLookup lookup = Context.Extensions[TemplateLookup.Name] as TemplateLookup;

            foreach (XmlElement each in output.SelectNodes("Course"))
            {
                string name = each.SelectSingleNode("ExamTemplate").InnerText;
                XmlElement node = CreateChild(each, "ExamTemplateID");
                node.InnerText = lookup.GetTemplateID(name);
            }
        }

        private void UpdateTeacherInformation(SheetHelper sheet)
        {
            Dictionary<string, string> selectedFields = new Dictionary<string, string>();

            foreach (string each in Context.SelectedFields)
                selectedFields.Add(each, each);

            if (!UpdateRequired(selectedFields)) return;
            bool doAddRequired = false;

            ImportCondition condition = GetIdentifyField();
            CourseLookup lookup = new CourseLookup(condition);
            TeacherLookup tlookup = Context.Extensions[TeacherLookup.Name] as TeacherLookup;
            SheetRowSource sheetSource = new SheetRowSource(sheet, Context);

            XmlElement removeReq = DSXmlHelper.LoadXml("<Request/>");
            XmlElement addReq = DSXmlHelper.LoadXml("<Request/>");

            int firstRow = sheet.FirstDataRowIndex, maxRow = sheet.MaxDataRowIndex;
            for (int rowIndex = firstRow; rowIndex <= maxRow; rowIndex++)
            {
                sheetSource.BindRow(rowIndex);

                string courseId = lookup.GetCourseID(sheetSource);

                if (string.IsNullOrEmpty(courseId))
                    throw new Exception("資料更新過程中，資料可能被另一使用者更改，匯入資料失敗。");

                if (selectedFields.ContainsKey("授課教師一"))
                {
                    XmlElement removeItem = CreateChild(removeReq, "Course");
                    CreateChild(removeItem, "CourseID").InnerText = courseId;
                    CreateChild(removeItem, "Sequence").InnerText = "1";
                }

                if (selectedFields.ContainsKey("授課教師二"))
                {
                    XmlElement removeItem = CreateChild(removeReq, "Course");
                    CreateChild(removeItem, "CourseID").InnerText = courseId;
                    CreateChild(removeItem, "Sequence").InnerText = "2";
                }

                if (selectedFields.ContainsKey("授課教師三"))
                {
                    XmlElement removeItem = CreateChild(removeReq, "Course");
                    CreateChild(removeItem, "CourseID").InnerText = courseId;
                    CreateChild(removeItem, "Sequence").InnerText = "3";
                }

                if (selectedFields.ContainsKey("授課教師一"))
                {
                    string teacherId = tlookup.GetTeacherID(sheetSource.GetFieldData("授課教師一"));
                    if (!string.IsNullOrEmpty(teacherId))
                    {
                        XmlElement addItem = CreateChild(addReq, "CourseTeacher");
                        CreateChild(addItem, "RefCourseID").InnerText = courseId;
                        CreateChild(addItem, "RefTeacherID").InnerText = teacherId;
                        CreateChild(addItem, "Sequence").InnerText = "1";
                        doAddRequired = true;
                    }
                }

                if (selectedFields.ContainsKey("授課教師二"))
                {
                    string teacherId = tlookup.GetTeacherID(sheetSource.GetFieldData("授課教師二"));
                    if (!string.IsNullOrEmpty(teacherId))
                    {
                        XmlElement addItem = CreateChild(addReq, "CourseTeacher");
                        CreateChild(addItem, "RefCourseID").InnerText = courseId;
                        CreateChild(addItem, "RefTeacherID").InnerText = teacherId;
                        CreateChild(addItem, "Sequence").InnerText = "2";
                        doAddRequired = true;
                    }
                }

                if (selectedFields.ContainsKey("授課教師三"))
                {
                    string teacherId = tlookup.GetTeacherID(sheetSource.GetFieldData("授課教師三"));
                    if (!string.IsNullOrEmpty(teacherId))
                    {
                        XmlElement addItem = CreateChild(addReq, "CourseTeacher");
                        CreateChild(addItem, "RefCourseID").InnerText = courseId;
                        CreateChild(addItem, "RefTeacherID").InnerText = teacherId;
                        CreateChild(addItem, "Sequence").InnerText = "3";
                        doAddRequired = true;
                    }
                }
            }

            ImportDataAccess da = Context.DataSource as ImportDataAccess;
            da.RemoveCourseTeachers(removeReq);

            if (doAddRequired)
                da.AddCourseTeachers(addReq);
        }

        private ImportCondition GetIdentifyField()
        {
            if (Context.IdentifyField != null)
                return Context.IdentifyField;
            else
            {
                foreach (ImportCondition each in Context.UpdateConditions)
                {
                    if (each.UniqueGroup.ToUpper() == "SemesterUnique".ToUpper())
                        return each;
                }
                return Context.UpdateConditions[0]; //如果沒有事到 PrimaryKey 就回傳第一個 UpdateCondition。
            }
        }

        private bool UpdateRequired(Dictionary<string, string> selectedFields)
        {
            bool required;

            required = selectedFields.ContainsKey("授課教師一");
            required |= selectedFields.ContainsKey("授課教師二");
            required |= selectedFields.ContainsKey("授課教師三");

            return required;
        }

        private bool UpdateCourseRequired()
        {
            foreach (string each in Context.SelectedFields)
            {
                ImportField field = Context.SupportFields[each];
                if (field.IsBasicField) return true;
            }

            return false;
        }

        private XmlElement CreateChild(XmlElement parent, string childName)
        {
            XmlElement child = parent.OwnerDocument.CreateElement(childName);
            parent.AppendChild(child);

            return child;
        }

        private void ImportMessage(string msg)
        {
            lblImportProgress.Text = msg;
            Application.DoEvents();
        }
        #endregion

        private void ImportWizard_CancelButtonClick(object sender, CancelEventArgs e)
        {
            Close();
        }

        private void ImportWizard_FinishButtonClick(object sender, CancelEventArgs e)
        {
            Close();
        }

        private class CourseRowValidatorFactory : IValidatorFactory
        {
            private WizardContext _context;

            public CourseRowValidatorFactory(WizardContext context)
            {
                _context = context;
            }

            #region IRowValidatorFactory 成員

            public IRowVaildator CreateRowValidator(string TypeName)
            {
                if (TypeName.ToUpper() == "TeacherDuplicateCheck".ToUpper())
                    return new TeacherDuplicateRowValidator(_context);
                else
                    return null;
            }

            #endregion

            #region IFieldValidatorFactory 成員

            public IFieldValidator CreateFieldValidator(string TypeName)
            {
                return null;
            }

            #endregion
        }
    }
}