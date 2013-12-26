using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using DevComponents.DotNetBar;
using Framework;
//using SmartSchool.ApplicationLog;
//using SmartSchool.AccessControl;

namespace JHSchool.StudentExtendControls
{
    public partial class TeacherBiasItem : JHSchool.Legacy.PalmerwormItem
    {
        private bool _initFinished;
        private Dictionary<string, string> _deletedRowID;
        private List<string> _addedRows;
        private decimal _tsLimit;
        private Dictionary<string, string> _commentList;
        private const string VALUE_KEY = "【SchoolYear】學年度【Semester】學期-「ColumnText」";
        private const string ADD_ROW_COUNT = "AddRowCount";
        private const string ORI_ROW_COUNT = "RowCount";
        public TeacherBiasItem()
        {
            InitializeComponent();
            Title = "德行分項成績";
        }

        protected override object OnBackgroundWorkerWorking()
        {
            DSResponse d1 = JHSchool.Feature.Legacy.Config.GetMoralDiffItemList();

            decimal tsLimit = 0;
            try
            {
                tsLimit = JHSchool.Feature.Legacy.Config.GetSupervisedRatingRange();
            }
            catch
            {
                tsLimit = 0;
            }

            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "All");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "RefStudentID", RunningID);
            helper.AddElement("Order");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");
            DSResponse d2 = JHSchool.Feature.Legacy.QueryScore.GetSemesterMoralScore(new DSRequest(helper));
            BackGroundInfo info = new BackGroundInfo(d1, d2, tsLimit);

            helper = JHSchool.Feature.Legacy.Config.GetMoralCommentCodeList().GetContent();
            foreach (XmlElement element in helper.GetElements("Morality"))
            {
                string code = element.GetAttribute("Code");
                string comment = element.GetAttribute("Comment");
                info.CommonList.Add(code, comment);
            }
            return info;
        }

        protected override void OnBackgroundWorkerCompleted(object result)
        {
            _initFinished = false;
            _deletedRowID = new Dictionary<string, string>();
            dataGridViewX1.Rows.Clear();
            dataGridViewX1.Columns.Clear();

            int columnIndex;
            //DataGridViewColumn col;
            columnIndex = dataGridViewX1.Columns.Add(colSchoolYear);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 80;

            columnIndex = dataGridViewX1.Columns.Add(colSemester);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 60;

            columnIndex = dataGridViewX1.Columns.Add(colTB);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 125;

            columnIndex = dataGridViewX1.Columns.Add(colTT);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 100;

            BackGroundInfo info = result as BackGroundInfo;
            _tsLimit = info.Limit;
            _commentList = info.CommonList;

            DSXmlHelper helper = info.MoralList.GetContent();
            int index = 0;
            foreach (XmlElement element in helper.GetElements("DiffItem"))
            {
                string name = element.GetAttribute("Name");
                System.Windows.Forms.DataGridViewTextBoxColumn newCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
                newCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
                newCol.HeaderText = name;
                newCol.MinimumWidth = 45;
                newCol.Name = "colItem" + index;
                newCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
                dataGridViewX1.Columns.Add(newCol);
                index++;
            }
            colTT.DisplayIndex = dataGridViewX1.Columns.Count - 1;

            _addedRows = new List<string>();
            _valueManager.AddValue(ADD_ROW_COUNT, "0");
            helper = info.ScoreList.GetContent();
            _valueManager.AddValue(ORI_ROW_COUNT, helper.GetElements("SemesterMoralScore").Length.ToString());

            foreach (XmlElement element in helper.GetElements("SemesterMoralScore"))
            {
                int rowIndex = dataGridViewX1.Rows.Add();
                DataGridViewRow row = dataGridViewX1.Rows[rowIndex];
                DataGridViewCell c;
                string schoolYear = element.SelectSingleNode("SchoolYear").InnerText;
                string semester = element.SelectSingleNode("Semester").InnerText;
                string key;

                c = row.Cells[colSchoolYear.Name];
                c.Value = schoolYear;
                c.Tag = schoolYear;
                key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", colSchoolYear.HeaderText);
                _valueManager.AddValue(key, c.Value.ToString());
                c.ReadOnly = true;

                c = row.Cells[colSemester.Name];
                c.Value = element.SelectSingleNode("Semester").InnerText;
                c.Tag = element.SelectSingleNode("Semester").InnerText;
                key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", colSemester.HeaderText);
                _valueManager.AddValue(key, c.Value.ToString());
                c.ReadOnly = true;

                c = row.Cells[colTB.Name];
                c.Value = element.SelectSingleNode("SupervisedByDiff").InnerText;
                c.Tag = element.SelectSingleNode("SupervisedByDiff").InnerText;
                key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", colTB.HeaderText);
                _valueManager.AddValue(key, c.Value.ToString());
                c.ReadOnly = false;

                c = row.Cells[colTT.Name];
                c.Value = element.SelectSingleNode("SupervisedByComment").InnerText;
                c.Tag = element.SelectSingleNode("SupervisedByComment").InnerText;
                key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", colTT.HeaderText);
                _valueManager.AddValue(key, c.Value.ToString());
                c.ReadOnly = false;

                RecordInfo rinfo = new RecordInfo();
                rinfo.IsAddedRow = false;
                rinfo.ID = element.GetAttribute("ID");
                row.Tag = rinfo;

                foreach (XmlNode node in element.SelectNodes("OtherDiffList/OtherDiff"))
                {
                    string name = node.SelectSingleNode("@Name").InnerText;
                    foreach (DataGridViewColumn column in dataGridViewX1.Columns)
                    {
                        if (column.HeaderText != name) continue;
                        c = row.Cells[column.Name];
                        c.Value = node.InnerText;
                        c.Tag = node.InnerText;
                        key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", column.HeaderText);
                        _valueManager.AddValue(key, c.Value.ToString());
                        c.ReadOnly = false;
                    }
                }
            }
            _initFinished = true;
        }

        public override void Save()
        {
            if (!ValidAll())
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料內容有誤, 請先修正後再行儲存!", "內容錯誤", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            

            
            string studentName = "";
            foreach (StudentRecord student in Student.Instance.SelectedList)
            {
                if (student.ID == RunningID)
                    studentName = student.Name;
            }

            StringBuilder isb = new StringBuilder("");
            StringBuilder usb = new StringBuilder("");
            StringBuilder dsb = new StringBuilder("");

            int uc = 0, ic = 0;
            DSXmlHelper uh = new DSXmlHelper("Request");
            DSXmlHelper ih = new DSXmlHelper("Request");

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                RecordInfo rinfo = row.Tag as RecordInfo;
                if (!rinfo.IsAddedRow)
                {
                    string id = rinfo.ID;
                    bool changed = false;
                    string schoolYear = row.Cells[colSchoolYear.Name].Value.ToString();
                    string semester = row.Cells[colSemester.Name].Value.ToString();
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string columnText = dataGridViewX1.Columns[cell.ColumnIndex].HeaderText;
                        string key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", columnText);
                        string newValue = GetValue(cell);
                        if (!_valueManager.GetValues().ContainsKey(key))
                        {
                            if (newValue != "")
                            {
                                usb.Append(key).Append("：「").Append(newValue).Append("」\n");
                                changed = true;
                            }
                        }
                        else if (_valueManager.IsDirtyItem(key))
                        {
                            string oldValue = _valueManager.GetOldValue(key);
                            usb.Append(key).Append("：由「").Append(oldValue).Append("」變更值為「").Append(newValue).Append("」\n");
                            changed = true;
                        }
                    }
                    if (changed)
                    {
                        uc++;
                        uh.AddElement("SemesterMoralScore");
                        uh.AddElement("SemesterMoralScore", "Condition");
                        uh.AddElement("SemesterMoralScore/Condition", "ID", id);
                        uh.AddElement("SemesterMoralScore", "SupervisedByDiff", GetValue(row.Cells[colTB.Name]));
                        uh.AddElement("SemesterMoralScore", "SupervisedByComment", GetValue(row.Cells[colTT.Name]));
                        uh.AddElement("SemesterMoralScore", "OtherDiff");

                        bool hasRoot = false;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            DataGridViewColumn column = dataGridViewX1.Columns[cell.ColumnIndex];
                            if (column.Name.StartsWith("colItem"))
                            {
                                if (!hasRoot)
                                {
                                    uh.AddElement("SemesterMoralScore/OtherDiff", "OtherDiffList");
                                    hasRoot = true;
                                }
                                XmlElement element = uh.AddElement("SemesterMoralScore/OtherDiff/OtherDiffList", "OtherDiff", GetValue(cell));
                                element.SetAttribute("Name", column.HeaderText);
                            }
                        }
                    }
                }

                // 處理 insert
                else
                {
                    isb.Append("新增【").Append(studentName).Append("-").Append(GetValue(row.Cells[colSchoolYear.Name]))
                        .Append("學年度").Append(GetValue(row.Cells[colSemester.Name])).Append("學期】資料：\n");

                    isb.Append("\t").Append(colTB.HeaderText).Append("：").Append(GetValue(row.Cells[colTB.Name])).Append("\n");
                    isb.Append("\t").Append(colTT.HeaderText).Append("：").Append(GetValue(row.Cells[colTT.Name])).Append("\n");

                    ih.AddElement("SemesterMoralScore");
                    ih.AddElement("SemesterMoralScore", "RefStudentID", RunningID);
                    ih.AddElement("SemesterMoralScore", "SchoolYear", GetValue(row.Cells[colSchoolYear.Name]));
                    ih.AddElement("SemesterMoralScore", "Semester", GetValue(row.Cells[colSemester.Name]));
                    ih.AddElement("SemesterMoralScore", "SupervisedByDiff", GetValue(row.Cells[colTB.Name]));
                    ih.AddElement("SemesterMoralScore", "SupervisedByComment", GetValue(row.Cells[colTT.Name]));
                    ih.AddElement("SemesterMoralScore", "OtherDiff");
                    bool hasRoot = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        DataGridViewColumn column = dataGridViewX1.Columns[cell.ColumnIndex];
                        if (column.Name.StartsWith("colItem"))
                        {
                            if (!hasRoot)
                            {
                                ih.AddElement("SemesterMoralScore/OtherDiff", "OtherDiffList");
                                hasRoot = true;
                            }
                            XmlElement element = ih.AddElement("SemesterMoralScore/OtherDiff/OtherDiffList", "OtherDiff", GetValue(cell));
                            element.SetAttribute("Name", column.HeaderText);
                            isb.Append("\t").Append(column.HeaderText).Append("：").Append(GetValue(row.Cells[column.Name])).Append("\n");
                        }
                    }
                    ic++;
                }
            }

            try
            {
                if (_deletedRowID.Count > 0)
                {
                    DSXmlHelper dh = new DSXmlHelper("Request");
                    dh.AddElement("SemesterMoralScore");
                    foreach (string id in _deletedRowID.Keys)
                    {
                        dsb.Append("刪除【").Append(studentName).Append("-").Append(_deletedRowID[id]).Append("】\n");
                        dh.AddElement("SemesterMoralScore", "ID", id);
                    }
                    JHSchool.Feature.Legacy.RemoveScore.DeleteSemesterMoralScore(new DSRequest(dh));
                    //CurrentUser.Instance.AppLog.Write(EntityType.Student, EntityAction.Delete, RunningID, dsb.ToString(), Title, dh.GetRawXml());
                }
                if (ic > 0)
                {
                    JHSchool.Feature.Legacy.AddScore.InsertSemesterMoralScore(new DSRequest(ih));
                    //CurrentUser.Instance.AppLog.Write(EntityType.Student, EntityAction.Insert, RunningID, isb.ToString(), Title, ih.GetRawXml());
                }
                if (uc > 0)
                {
                    
                    JHSchool.Feature.Legacy.EditScore.UpdateSemesterMoralScore(new DSRequest(uh));
                    //CurrentUser.Instance.AppLog.Write(EntityType.Student, EntityAction.Update, RunningID, usb.ToString(), Title, uh.GetRawXml());
                }
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("儲存失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveButtonVisible = false;
            LoadContent(RunningID);
        }

        private void dataGridViewX1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewColumn column = dataGridViewX1.Columns[e.ColumnIndex];
            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            if (row.IsNewRow) return;
            ValidCell(cell);
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewColumn column = dataGridViewX1.Columns[e.ColumnIndex];
            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            if (cell.Value == null) cell.Value = string.Empty;
            string schoolYear = row.Cells[colSchoolYear.Name].Value == null ? string.Empty : row.Cells[colSchoolYear.Name].Value.ToString();
            string semester = row.Cells[colSemester.Name].Value == null ? string.Empty : row.Cells[colSemester.Name].Value.ToString();
            string key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", column.HeaderText);

            if (!_valueManager.GetValues().ContainsKey(key) && _initFinished == true)
                _valueManager.AddValue(key, "");

            OnValueChanged(key, cell.Value.ToString());
        }

        private void dataGridViewX1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == 0) return;
            if (!_initFinished) return;
            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex - 1];
            foreach (DataGridViewCell cell in row.Cells)
            {
                ValidCell(cell);
            }

            string key = Guid.NewGuid().ToString();
            RecordInfo info = new RecordInfo();
            info.IsAddedRow = true;
            info.Name = key;
            row.Tag = info;
            _addedRows.Add(key);
            OnValueChanged(ADD_ROW_COUNT, _addedRows.Count.ToString());
        }

        private void ValidCell(DataGridViewCell cell)
        {
            cell.ErrorText = string.Empty;
            IValidator validator;
            DataGridViewColumn column = dataGridViewX1.Columns[cell.ColumnIndex];
            DataGridViewRow row = dataGridViewX1.Rows[cell.RowIndex];
            switch (column.HeaderText)
            {
                case "學年度":
                    validator = new SchoolYearValidator();
                    break;
                case "學期":
                    validator = new SemesterValidator();
                    break;
                case "導師加減分":
                    validator = new TeacherBiasValidator();
                    validator.Argument = _tsLimit;
                    break;
                case "導師評語":
                    validator = new CommentValidator();
                    validator.Argument = _commentList;
                    break;
                default:
                    validator = new ScoreValidator();
                    break;
            }

            validator.ValidCell = cell;
            if (!validator.IsValid())
                cell.ErrorText = validator.Message;

            IColumnValidator cv;
            if (column.HeaderText == "學年度")
            {
                cv = new SemesterColumnValidator();
                cv.Argument = dataGridViewX1.Columns[colSemester.Name];
            }
            else if (column.HeaderText == "學期")
            {
                cv = new SemesterColumnValidator();
                cv.Argument = dataGridViewX1.Columns[colSchoolYear.Name];
            }
            else
            {
                cv = new NoneColumnValidator();
            }
            cv.ValidColumn = column;
            cv.IsValid();
        }

        private bool ValidAll()
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow) continue;
                if (!string.IsNullOrEmpty(row.ErrorText))
                    return false;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!string.IsNullOrEmpty(cell.ErrorText))
                        return false;
                }
            }
            return true;
        }

        private void dataGridViewX1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            RecordInfo info = e.Row.Tag as RecordInfo;
            if (info == null) return;
            if (info.IsAddedRow)
            {
                _addedRows.Remove(info.Name);
                OnValueChanged(ADD_ROW_COUNT, _addedRows.Count.ToString());
            }
            else
            {
                string schoolYear = e.Row.Cells[colSchoolYear.Name].Value.ToString();
                string semester = e.Row.Cells[colSemester.Name].Value.ToString();
                string msg = schoolYear + "學年度" + semester + "學期";
                _deletedRowID.Add(info.ID, msg);
                OnValueChanged(ORI_ROW_COUNT, "-1");
            }
        }

        private string GetValue(DataGridViewCell cell)
        {
            if (cell.Value == null)
                return string.Empty;
            return cell.Value.ToString();
        }

        private void dataGridViewX1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridViewX1.EndEdit();
        }

        private void dataGridViewX1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dataGridViewX1.SelectedCells.Count > 0)
                dataGridViewX1.BeginEdit(true);
        }
    }

    class BackGroundInfo
    {
        private DSResponse _moralList;
        public DSResponse MoralList
        {
            get { return _moralList; }
        }

        private DSResponse _scoreList;
        public DSResponse ScoreList
        {
            get { return _scoreList; }
        }

        private decimal _limit;
        public decimal Limit
        {
            get { return _limit; }
            set { _limit = value; }
        }

        private Dictionary<string, string> _commonList;
        public Dictionary<string, string> CommonList
        {
            get { return _commonList; }
            set { _commonList = value; }
        }

        public BackGroundInfo(DSResponse moralList, DSResponse scoreList, decimal limit)
        {
            _commonList = new Dictionary<string, string>();
            _limit = limit;
            _moralList = moralList;
            _scoreList = scoreList;
        }
    }

    #region Validators

    public interface IValidator
    {
        object Argument { set;}
        DataGridViewCell ValidCell { set;}
        bool IsValid();
        string Message { get;}
    }

    public abstract class AbstractValidator : IValidator
    {
        private string _message;
        private object _argument;
        private DataGridViewCell _cell;

        #region IValidator 成員

        public object Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }

        public DataGridViewCell ValidCell
        {
            set { _cell = value; }
            get { return _cell; }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        protected virtual void OnInvalid(string errorMessage)
        {
            _message = errorMessage;
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        #endregion
    }

    public class SchoolYearValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            ValidCell.ErrorText = string.Empty;

            if (ValidCell.Value == null)
            {
                OnInvalid("不可空白");
                return false;
            }
            if (ValidCell.Value.ToString() == string.Empty)
            {
                OnInvalid("不可空白");
                return false;
            }

            int s;
            if (!int.TryParse(ValidCell.Value.ToString(), out s))
            {
                OnInvalid("必須為數字");
                return false;
            }

            if (s > Framework.Legacy.GlobalOld.SystemConfig.DefaultSchoolYear)
            {
                OnInvalid("超出目前學年度");
                return false;
            }
            return true;
        }
    }

    public class SemesterValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            if (ValidCell.Value == null)
            {
                OnInvalid("不可空白");
                return false;
            }
            if (ValidCell.Value.ToString() == string.Empty)
            {
                OnInvalid("不可空白");
                return false;
            }
            if (ValidCell.Value.ToString() != "1" && ValidCell.Value.ToString() != "2")
            {
                OnInvalid("只能填入1或2");
                return false;
            }
            return true;
        }
    }

    public class ScoreValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            if (ValidCell.Value == null)
                return true;
            if (ValidCell.Value.ToString() == string.Empty)
                return true;

            decimal d;
            if (!decimal.TryParse(ValidCell.Value.ToString(), out d))
            {
                OnInvalid("必須為數字");
                return false;
            }
            return true;
        }
    }

    public class TeacherBiasValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            if (ValidCell.Value == null)
                return true;
            if (ValidCell.Value.ToString() == string.Empty)
                return true;

            decimal d;
            if (!decimal.TryParse(ValidCell.Value.ToString(), out d))
            {
                OnInvalid("必須為數字");
                return false;
            }

            decimal limit = decimal.Parse(Argument.ToString());
            if (Math.Abs(d) > limit)
            {
                OnInvalid("超出限制範圍");
                return false;
            }
            return true;
        }
    }

    public class CommentValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            Dictionary<string, string> list = (Dictionary<string, string>)Argument;
            if (ValidCell.Value == null) return true;
            if ( ValidCell.Value.ToString() == string.Empty ) return true;
            if ( list == null) return true;

            StringBuilder sb = new StringBuilder();
            string[] vs = ValidCell.Value.ToString().Split(',');
            foreach (string v in vs)
            {
                if (list.ContainsKey(v))
                    sb.Append(list[v]);
                else
                    sb.Append(v);
                sb.Append(",");
            }
            if (sb.ToString().EndsWith(","))
                sb.Remove(sb.Length - 1, 1);

            ValidCell.Value = sb.ToString();
            return true;
        }
    }

    public class AllYouWantValidator : AbstractValidator
    {
        public override bool IsValid()
        {
            return true;
        }
    }

    public interface IColumnValidator
    {
        DataGridViewColumn ValidColumn { set;}
        object Argument { set;}
        bool IsValid();
        string Message { get;}
    }

    public abstract class AbstractColumnValidator : IColumnValidator
    {
        private DataGridViewColumn _validColumn;
        private object _argument;
        private string _message;

        protected virtual void OnInvalid(string message)
        {
            _message = message;
        }

        #region IColumnValidator 成員
        public DataGridViewColumn ValidColumn
        {
            set { _validColumn = value; }
            get { return _validColumn; }
        }

        public object Argument
        {
            set { _argument = value; }
            get { return _argument; }
        }

        public virtual bool IsValid()
        {
            return true;
        }

        public string Message
        {
            get { return _message; }
        }

        #endregion
    }

    public class NoneColumnValidator : AbstractColumnValidator
    {
    }

    public class SemesterColumnValidator : AbstractColumnValidator
    {
        public override bool IsValid()
        {
            DataGridViewColumn sy = ValidColumn;
            DataGridViewColumn sm = Argument as DataGridViewColumn;

            Dictionary<string, string> list = new Dictionary<string, string>();
            foreach (DataGridViewRow row in ValidColumn.DataGridView.Rows)
            {
                if (row.IsNewRow) continue;
                row.ErrorText = string.Empty;
                string y = row.Cells[sy.Index].Value == null ? string.Empty : row.Cells[sy.Index].Value.ToString();
                string m = row.Cells[sm.Index].Value == null ? string.Empty : row.Cells[sm.Index].Value.ToString();
                string key = y + "-" + m;
                bool valid = true;
                if (list.ContainsKey(key))
                {
                    row.ErrorText = "學年度學期與其它資料重覆";
                    valid = false;
                }
                else
                {
                    list.Add(key, null);
                }
            }
            return true;
        }
    }

    #endregion

    class RecordInfo
    {
        private bool _isAddedRow;

        public bool IsAddedRow
        {
            get { return _isAddedRow; }
            set { _isAddedRow = value; }
        }
        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
