using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
//using SmartSchool.ClassRelated.RibbonBars.DeXing;
//using SmartSchool.Customization.Data;
using System.Xml;
using Framework;
//using SmartSchool.ApplicationLog;

namespace JHSchool.StudentExtendControls
{
    public partial class WordCommentPalmerworm : JHSchool.Legacy.PalmerwormItem
    {
        private CommentList _commentList;
        private ReserveFaces _reserveFaces;
        private bool _initialized = false;
        private Dictionary<string, string> _deletedRows;
        private List<string> _addedRows;

        private const string VALUE_KEY = "【SchoolYear】學年度【Semester】學期-「ColumnText」";
        private const string ADD_ROW_COUNT = "AddRowCount";
        private const string ORI_ROW_COUNT = "RowCount";

        public WordCommentPalmerworm()
        {
            InitializeComponent();
            Title = "文字評量";

         
        }

        protected override object OnBackgroundWorkerWorking()
        {
            //抓文字評量對照表
            CommentList commentList = new CommentList();
            List<string> faceList = new List<string>();
            DSXmlHelper wcHelper = JHSchool.Feature.Legacy.Config.GetWordCommentList().GetContent();
            foreach (XmlElement morality in wcHelper.GetElements("Content/Morality"))
            {
                DSXmlHelper moralityHelper = new DSXmlHelper(morality);
                string face = moralityHelper.GetText("@Face");
                if (!faceList.Contains(face)) faceList.Add(face);
                foreach (XmlElement item in moralityHelper.GetElements("Item"))
                {
                    string code = item.GetAttribute("Code");
                    string comment = item.GetAttribute("Comment");
                    if (commentList.Add(face, code, comment) == false)
                        FISCA.Presentation.Controls.MsgBox.Show("文字評量代碼表有誤。");
                }
            }

            //取得學生清單資料
            DSXmlHelper helper = new DSXmlHelper("Request");
            helper.AddElement("Field");
            helper.AddElement("Field", "ID");
            helper.AddElement("Field", "TextScore");
            helper.AddElement("Field", "SchoolYear");
            helper.AddElement("Field", "Semester");
            helper.AddElement("Condition");
            helper.AddElement("Condition", "StudentIDList");
            helper.AddElement("Condition/StudentIDList", "ID", RunningID);
            helper.AddElement("Order");
            helper.AddElement("Order", "SchoolYear");
            helper.AddElement("Order", "Semester");

            DSResponse dsrsp = JHSchool.Feature.Legacy.QueryScore.GetSemesterMoralScore(new DSRequest(helper));
            if (!dsrsp.HasContent)
            {
                FISCA.Presentation.Controls.MsgBox.Show("取得回覆文件錯誤:" + dsrsp.GetFault().Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<object>(new object[] { commentList, faceList });
            }
            helper = dsrsp.GetContent();
            return new List<object>(new object[] { commentList, faceList, helper });
        }

        protected override void OnBackgroundWorkerCompleted(object result)
        {
            _initialized = false;

            _deletedRows = new Dictionary<string, string>();
            _addedRows = new List<string>();
            _reserveFaces = new ReserveFaces();

            List<object> resultObject = result as List<object>;
            _commentList = resultObject[0] as CommentList;
            List<string> faceList = resultObject[1] as List<string>;
            DSXmlHelper helper = null;
            if (resultObject.Count > 2) helper = resultObject[2] as DSXmlHelper;
            else helper = new DSXmlHelper("BOOM");

            dgView.Rows.Clear();
            dgView.Columns.Clear();

            dgView.Columns.Add(colSchoolYear);
            dgView.Columns.Add(colSemester);
            //int columnIndex;
            //DataGridViewColumn col;
            //columnIndex = dataGridViewX1.Columns.Add(colSchoolYear);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 80;

            //columnIndex = dataGridViewX1.Columns.Add(colSemester);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 60;

            //columnIndex = dataGridViewX1.Columns.Add(colTB);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 125;

            //columnIndex = dataGridViewX1.Columns.Add(colTT);
            //col = dataGridViewX1.Columns[columnIndex];
            //col.Width = 100;

            //文字評量 Face Column
            int index = 0;
            foreach (string face in faceList)
            {
                int colIndex = dgView.Columns.Add("colItem" + index, face);
                dgView.Columns[colIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                index++;
            }

            //int index = 0;
            //foreach (XmlElement element in helper.GetElements("DiffItem"))
            //{
            //    string name = element.GetAttribute("Name");
            //    System.Windows.Forms.DataGridViewTextBoxColumn newCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            //    newCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            //    newCol.HeaderText = name;
            //    newCol.MinimumWidth = 45;
            //    newCol.Name = "colItem" + index;
            //    newCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            //    dataGridViewX1.Columns.Add(newCol);
            //    index++;
            //}
            //colTT.DisplayIndex = dataGridViewX1.Columns.Count - 1;

            
            _valueManager.AddValue(ADD_ROW_COUNT, "0");
            //helper = info.ScoreList.GetContent();
            _valueManager.AddValue(ORI_ROW_COUNT, helper.GetElements("SemesterMoralScore").Length.ToString());

            foreach (XmlElement element in helper.GetElements("SemesterMoralScore"))
            {
                int rowIndex = dgView.Rows.Add();
                DataGridViewRow row = dgView.Rows[rowIndex];
                DataGridViewCell cell;
                string schoolYear = element.SelectSingleNode("SchoolYear").InnerText;
                string semester = element.SelectSingleNode("Semester").InnerText;
                string key;

                cell = row.Cells[colSchoolYear.Index];
                cell.Value = schoolYear;
                cell.Tag = schoolYear;
                key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", colSchoolYear.HeaderText);
                _valueManager.AddValue(key, cell.Value.ToString());
                cell.ReadOnly = true;

                cell = row.Cells[colSemester.Index];
                cell.Value = element.SelectSingleNode("Semester").InnerText;
                cell.Tag = element.SelectSingleNode("Semester").InnerText;
                key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", colSemester.HeaderText);
                _valueManager.AddValue(key, cell.Value.ToString());
                cell.ReadOnly = true;

                JHSchool.Legacy.RecordInfo rinfo = new JHSchool.Legacy.RecordInfo();
                rinfo.IsAddedRow = false;
                rinfo.ID = element.GetAttribute("ID");
                row.Tag = rinfo;

                foreach (XmlNode node in element.SelectNodes("TextScore/Morality"))
                {
                    string face = node.SelectSingleNode("@Face").InnerText;

                    //如果 face 不存在對照表中，偷偷保留下來
                    if (!_commentList.ContainsFace(face)) _reserveFaces.Add(rinfo.ID, face, node.InnerText);

                    foreach (DataGridViewColumn column in dgView.Columns)
                    {
                        if (column.HeaderText != face) continue;
                        cell = row.Cells[column.Index];
                        cell.Value = node.InnerText;
                        cell.Tag = node.InnerText;
                        key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", column.HeaderText);
                        _valueManager.AddValue(key, cell.Value.ToString());
                        cell.ReadOnly = false;
                    }
                }
            }
            _initialized = true;
        }

        public override void Save()
        {
            if (!ValidAll())
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料內容有誤, 請先修正後再行儲存!", "內容錯誤", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            //string studentName = "";
            //foreach (BriefStudentData student in SmartSchool.StudentRelated.Student.Instance.SelectionStudents)
            //{
            //    if (student.ID == RunningID)
            //        studentName = student.Name;
            //}

            string studentName = JHSchool.Student.Instance.Items[RunningID].Name;

            StringBuilder isb = new StringBuilder("");
            StringBuilder usb = new StringBuilder("");
            StringBuilder dsb = new StringBuilder("");

            int uc = 0, ic = 0;
            DSXmlHelper uh = new DSXmlHelper("Request");
            DSXmlHelper ih = new DSXmlHelper("Request");

            foreach (DataGridViewRow row in dgView.Rows)
            {
                if (row.IsNewRow) continue;
                JHSchool.Legacy.RecordInfo rinfo = row.Tag as JHSchool.Legacy.RecordInfo;
                if (!rinfo.IsAddedRow)
                {
                    string id = rinfo.ID;
                    bool changed = false;
                    string schoolYear = ""+row.Cells[colSchoolYear.Index].Value;
                    string semester = ""+row.Cells[colSemester.Index].Value;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string columnText = dgView.Columns[cell.ColumnIndex].HeaderText;
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
                        //uh.AddElement("SemesterMoralScore", "SupervisedByDiff", GetValue(row.Cells[colTB.Name]));
                        //uh.AddElement("SemesterMoralScore", "SupervisedByComment", GetValue(row.Cells[colTT.Name]));
                        uh.AddElement("SemesterMoralScore", "TextScore");

                        bool hasRoot = false;
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            DataGridViewColumn column = cell.OwningColumn;
                            if (column.Name.StartsWith("colItem"))
                            {
                                if (!hasRoot)
                                {
                                    uh.AddElement("SemesterMoralScore/TextScore", "Content");
                                    hasRoot = true;
                                }
                                XmlElement element = uh.AddElement("SemesterMoralScore/TextScore/Content", "Morality", GetValue(cell));
                                element.SetAttribute("Face", column.HeaderText);
                            }
                        }

                        foreach (string other_face in _reserveFaces.GetFacesByID(id))
                        {
                            if (!hasRoot)
                            {
                                uh.AddElement("SemesterMoralScore/TextScore", "Content");
                                hasRoot = true;
                            }
                            XmlElement element = uh.AddElement("SemesterMoralScore/TextScore/Content", "Morality", _reserveFaces.GetComment(id, other_face));
                            element.SetAttribute("Face", other_face);
                        }
                    }
                }


                // 處理 insert
                else
                {
                    isb.Append("新增【").Append(studentName).Append("-").Append(GetValue(row.Cells[colSchoolYear.Name]))
                        .Append("學年度").Append(GetValue(row.Cells[colSemester.Name])).Append("學期】資料：\n");

                    //isb.Append("\t").Append(colTB.HeaderText).Append("：").Append(GetValue(row.Cells[colTB.Name])).Append("\n");
                    //isb.Append("\t").Append(colTT.HeaderText).Append("：").Append(GetValue(row.Cells[colTT.Name])).Append("\n");

                    ih.AddElement("SemesterMoralScore");
                    ih.AddElement("SemesterMoralScore", "RefStudentID", RunningID);
                    ih.AddElement("SemesterMoralScore", "SchoolYear", GetValue(row.Cells[colSchoolYear.Index]));
                    ih.AddElement("SemesterMoralScore", "Semester", GetValue(row.Cells[colSemester.Index]));
                    //ih.AddElement("SemesterMoralScore", "SupervisedByDiff", GetValue(row.Cells[colTB.Name]));
                    //ih.AddElement("SemesterMoralScore", "SupervisedByComment", GetValue(row.Cells[colTT.Name]));
                    ih.AddElement("SemesterMoralScore", "TextScore");
                    bool hasRoot = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        DataGridViewColumn column = cell.OwningColumn;
                        if (column.Name.StartsWith("colItem"))
                        {
                            if (!hasRoot)
                            {
                                ih.AddElement("SemesterMoralScore/TextScore", "Content");
                                hasRoot = true;
                            }
                            XmlElement element = ih.AddElement("SemesterMoralScore/TextScore/Content", "Morality", GetValue(cell));
                            element.SetAttribute("Face", column.HeaderText);
                            isb.Append("\t").Append(column.HeaderText).Append("：").Append(GetValue(row.Cells[column.Name])).Append("\n");
                        }
                    }
                    ic++;
                }
            }

            string ustring = usb.ToString();
            usb = new StringBuilder("");
            usb.Append("修改【").Append(studentName).Append("】資料：\n");
            usb.Append(ustring);

            try
            {
                if (_deletedRows.Count > 0)
                {
                    DSXmlHelper dh = new DSXmlHelper("Request");
                    dh.AddElement("SemesterMoralScore");
                    foreach (string id in _deletedRows.Keys)
                    {
                        dsb.Append("刪除【").Append(studentName).Append("-").Append(_deletedRows[id]).Append("】\n");
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

        #region DataGridView Event
        private void dgView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewColumn column = dgView.Columns[e.ColumnIndex];
            DataGridViewRow row = dgView.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            if (cell.Value == null) cell.Value = string.Empty;
            string schoolYear = "" + row.Cells[colSchoolYear.Index].Value;
            string semester = "" + row.Cells[colSemester.Index].Value;
            string key = VALUE_KEY.Replace("SchoolYear", schoolYear).Replace("Semester", semester).Replace("ColumnText", column.HeaderText);

            if (!_valueManager.GetValues().ContainsKey(key) && _initialized == true)
                _valueManager.AddValue(key, "");

            OnValueChanged(key, "" + cell.Value);
        }

        private void dgView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0 && dgView.SelectedCells.Count > 0)
                dgView.BeginEdit(true);
        }

        private void dgView_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgView.EndEdit();
        }

        private void dgView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex == 0) return;
            if (!_initialized) return;
            DataGridViewRow row = dgView.Rows[e.RowIndex - 1];
            foreach (DataGridViewCell cell in row.Cells)
            {
                ValidCell(cell);
            }

            string key = Guid.NewGuid().ToString();
            JHSchool.Legacy.RecordInfo info = new JHSchool.Legacy.RecordInfo();
            info.IsAddedRow = true;
            info.Name = key;
            row.Tag = info;
            _addedRows.Add(key);
            OnValueChanged(ADD_ROW_COUNT, _addedRows.Count.ToString());
        }

        private void dgView_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            JHSchool.Legacy.RecordInfo info = e.Row.Tag as JHSchool.Legacy.RecordInfo;
            if (info == null) return;
            if (info.IsAddedRow)
            {
                _addedRows.Remove(info.Name);
                OnValueChanged(ADD_ROW_COUNT, _addedRows.Count.ToString());
            }
            else
            {
                string schoolYear = ""+ e.Row.Cells[colSchoolYear.Index].Value;
                string semester = ""+e.Row.Cells[colSemester.Index].Value;
                string msg = schoolYear + "學年度" + semester + "學期";
                _deletedRows.Add(info.ID, msg);
                OnValueChanged(ORI_ROW_COUNT, "-1");
            }
        }

        private void dgView_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dgView.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];
            if (row.IsNewRow) return;
            ValidCell(cell);
        }
        #endregion

        private void ValidCell(DataGridViewCell cell)
        {
            cell.ErrorText = string.Empty;
            JHSchool.Legacy.IValidator validator;
            DataGridViewColumn column = dgView.Columns[cell.ColumnIndex];
            DataGridViewRow row = dgView.Rows[cell.RowIndex];
            switch (column.HeaderText)
            {
                case "學年度":
                    validator = new JHSchool.Legacy.SchoolYearValidator();
                    break;
                case "學期":
                    validator = new JHSchool.Legacy.SemesterValidator();
                    break;
                //case "導師加減分":
                //    validator = new TeacherBiasValidator();
                //    validator.Argument = _tsLimit;
                //    break;
                //case "導師評語":
                //    validator = new CommentValidator();
                //    validator.Argument = _commentList;
                //    break;
                default:
                    validator = new WordCommentValidator();
                    validator.Argument = _commentList;
                    break;
            }

            validator.ValidCell = cell;
            if (!validator.IsValid())
                cell.ErrorText = validator.Message;

            JHSchool.Legacy.IColumnValidator cv;
            if (column.HeaderText == "學年度")
            {
                cv = new JHSchool.Legacy.SemesterColumnValidator();
                cv.Argument = dgView.Columns[colSemester.Name];
            }
            else if (column.HeaderText == "學期")
            {
                cv = new JHSchool.Legacy.SemesterColumnValidator();
                cv.Argument = dgView.Columns[colSchoolYear.Name];
            }
            else
            {
                cv = new JHSchool.Legacy.NoneColumnValidator();
            }
            cv.ValidColumn = column;
            cv.IsValid();
        }

        private bool ValidAll()
        {
            foreach (DataGridViewRow row in dgView.Rows)
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

        private string GetValue(DataGridViewCell cell)
        {
            if (cell.Value == null)
                return string.Empty;
            return cell.Value.ToString();
        }
    }

    public class WordCommentValidator : JHSchool.Legacy.AbstractValidator
    {
        public override bool IsValid()
        {
            CommentList list = (CommentList)Argument;
            if (ValidCell.Value == null) return true;
            if (ValidCell.Value.ToString() == string.Empty) return true;

            string face = ValidCell.OwningColumn.HeaderText;
            StringBuilder sb = new StringBuilder();
            string[] vs = ValidCell.Value.ToString().Split(',');
            foreach (string v in vs)
            {
                string comment = list.GetComment(face, v);
                if (!string.IsNullOrEmpty(comment))
                    sb.Append(comment);
                else
                    sb.Append(v);
                sb.Append(",");
            }
            if (sb.ToString().EndsWith(","))
                sb.Remove(sb.Length - 1, 1);

            ValidCell.Value = sb.ToString();

            //string comment = list.GetComment(ValidCell.OwningColumn.HeaderText, ValidCell.Value.ToString());
            //if (!string.IsNullOrEmpty(comment))
            //    ValidCell.Value = comment;

            return true;
        }
    }

    internal class CommentList
    {
        private Dictionary<string, string> list = new Dictionary<string, string>();
        private List<string> faces = new List<string>();
        private string spliter = "_YAOMING_";

        internal bool Add(string face, string code, string comment)
        {
            string key = string.Format("{0}{1}{2}", face, spliter, code);
            if (list.ContainsKey(key)) return false;
            list.Add(key, comment);
            if (!faces.Contains(face)) faces.Add(face);
            return true;
        }

        internal void Add(string face)
        {
            if (!faces.Contains(face)) faces.Add(face);
        }

        internal string GetComment(string face, string code)
        {
            string key = string.Format("{0}{1}{2}", face, spliter, code);
            if (list.ContainsKey(key)) return list[key];
            return "";
        }

        internal bool ContainsFace(string face)
        {
            return faces.Contains(face);
        }
    }

    internal class ReserveFaces
    {
        private Dictionary<string, string> list = new Dictionary<string, string>();
        private Dictionary<string, List<string>> id_faces = new Dictionary<string, List<string>>();
        private string spliter = "_YAOMING_";

        internal bool Add(string id, string face, string comment)
        {
            string key = string.Format("{0}{1}{2}", id, spliter, face);
            if (list.ContainsKey(key)) return false;
            list.Add(key, comment);
            if (!id_faces.ContainsKey(id))
                id_faces.Add(id, new List<string>());
            id_faces[id].Add(face);
            return true;
        }

        internal string GetComment(string id, string face)
        {
            string key = string.Format("{0}{1}{2}", id, spliter, face);
            if (list.ContainsKey(key)) return list[key];
            return "";
        }

        internal List<string> GetFacesByID(string id)
        {
            if (id_faces.ContainsKey(id)) return id_faces[id];
            return new List<string>();
        }
    }
}