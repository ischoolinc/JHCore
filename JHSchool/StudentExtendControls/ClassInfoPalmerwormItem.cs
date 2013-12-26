using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar;
using Framework;
using Framework.Security;
using FISCA.DSAUtil;
using JHSchool.Feature.Legacy;

namespace JHSchool.StudentExtendControls
{
    //[FeatureCode("Content0020")]
    internal partial class ClassInfoPalmerwormItem : JHSchool.Legacy.PalmerwormItem
    {
        private bool _isInitialized = false;
        private BackgroundWorker _getDepartmentBackgroundWorker;
        //private BackgroundWorker _getClassBackgroundWorker;
        private BackgroundWorker _getSeatNoBackgroundWorker;

        private string _WaitToGetSeatNo;
        private string _WaitToGetClass;
        //private string test;
        //private AutoResetEvent _getDepartmentWait;

        private DSResponse dsrsp;
        //private Dictionary<string, string> classList;
        private List<int> seatNoList;

        private EnhancedErrorProvider _errors = new EnhancedErrorProvider();

        public ClassInfoPalmerwormItem()
            : base()
        {
            InitializeComponent();
            //_isLoading = true;           
            this.Title = "學生班級資訊";

            //warnings.Icon = Resources.warning;
            //_errors.Icon = Resources.error;
            //_getClassBackgroundWorker = new BackgroundWorker();
            //_getClassBackgroundWorker.DoWork += new DoWorkEventHandler(_getClassBackgroundWorker_DoWork);
            //_getClassBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_getClassBackgroundWorker_RunWorkerCompleted);

            _getSeatNoBackgroundWorker = new BackgroundWorker();
            _getSeatNoBackgroundWorker.DoWork += new DoWorkEventHandler(_getSeatNoBackgroundWorker_DoWork);
            _getSeatNoBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_getSeatNoBackgroundWorker_RunWorkerCompleted);

            //_getDepartmentWait = new AutoResetEvent(true);
            //_getDepartmentBackgroundWorker = new BackgroundWorker();
            //_getDepartmentBackgroundWorker.DoWork += new DoWorkEventHandler(_getDepartmentBackgroundWorker_DoWork);
            //_getDepartmentBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_getDepartmentBackgroundWorker_RunWorkerCompleted);

            //_getDepartmentBackgroundWorker.RunWorkerAsync();
            //_getDepartmentWait.Reset();
        }

        //void _getClassBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    if ("" + e.Result != _WaitToGetClass)
        //    {
        //        GetGradeYearClassList(_WaitToGetClass);
        //    }
        //    else
        //    {
        //        foreach (string key in classList.Keys)
        //        {
        //            cboClass.AddItem(key, classList[key]);
        //        }
        //        //cboClass.SetComboBoxValue(test);

        //    }
        //}

        //void _getClassBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    string gradeyearID = e.Argument as string;
        //    classList = Class.GetClassListByGradeYear(gradeyearID);
        //    e.Result = gradeyearID;
        //}

        private void Initialize()
        {
            warnings.Clear();
            _errors.Clear();

            //年級
            //cboGradeYear.SelectedItem = null;
            //cboGradeYear.Items.Clear();
            //cboGradeYear.AddItem("", "");
            //cboGradeYear.AddItem("1", "一年級");
            //cboGradeYear.AddItem("2", "二年級");
            //cboGradeYear.AddItem("3", "三年級");
            //cboGradeYear.AddItem("4", "四年級");




            if (_isInitialized) return;

            //科別
            // 2008/04/30 黃耀明
            //DSResponse dsrsp = SmartSchool.Feature.Basic.Config.GetDepartment();
            //foreach (XmlNode node in dsrsp.GetContent().GetElements("Department"))
            //{
            //    cboDept.AddItem(node.SelectSingleNode("@ID").InnerText, node.SelectSingleNode("Name").InnerText);
            //}
            foreach (DepartmentRecord dept in Department.Instance.Items)
            {
                cboDept.AddItem(dept.ID, dept.FullName);
            }

            _isInitialized = true;
        }

        #region IPalmerwormItem 成員

        protected override object OnBackgroundWorkerWorking()
        {
            //if (RunningID != null)
            //    return QueryStudent.GetClassInfo(RunningID);            
            // return null;
            return "";
        }

        //void _getDepartmentBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    foreach (XmlNode node in dsrsp.GetContent().GetElements("Department"))
        //    {
        //        cboProgram.AddItem(node.SelectSingleNode("Name").InnerText, node.SelectSingleNode("Name").InnerText);
        //    }
        //}

        //void _getDepartmentBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    dsrsp = SmartSchool.Feature.Basic.Config.GetDepartment();
        //    //_getDepartmentWait.Set();
        //}

        protected override void OnBackgroundWorkerCompleted(object result)
        {
            Initialize();
            //            XmlNode classInfoNode = result as XmlNode;
            //            bool hasClass = classInfoNode.SelectSingleNode("GradeYear") != null;

            bool hasClass = Student.Instance.Items[RunningID].Class != null;
            string selectedValue;

            //載入年級
            //string gradeYear = hasClass ? classInfoNode.SelectSingleNode("GradeYear").InnerText : "";
            string gradeYear = hasClass ? Student.Instance.Items[RunningID].Class.GradeYear : "";
            //cboGradeYear.SetComboBoxValue(gradeYear);
            cboGradeYear.Text = gradeYear;
            //_valueManager.AddValue("GradeYear", gradeYear); //年級不存在資料庫，只當過濾班級用。

            //載入班級
            //string classID = hasClass ? classInfoNode.SelectSingleNode("RefClassID").InnerText : "";
            string classID = hasClass ? Student.Instance.Items[RunningID].RefClassID : "";
            cboClass.SetComboBoxValue(classID);
            _valueManager.AddValue("RefClassID", classID, "班級");

            TipClassSelection(); //提示班級選擇方式。

            /* 
            //載入科別 
            //2008/04/30 黃耀明
            cboDept.SelectedIndex = -1;
            cboDept.WatermarkText = string.Empty;
            string overrideDeptId = hasClass ? classInfoNode.SelectSingleNode("OverrideDeptID").InnerText : "";
            string inhertDeptName = hasClass ? classInfoNode.SelectSingleNode("InhertDeptName").InnerText : "";
            if (!string.IsNullOrEmpty(overrideDeptId))
                cboDept.SetComboBoxValue(overrideDeptId);
            else
                cboDept.WatermarkText = inhertDeptName;

            selectedValue = cboDept.GetValue();
            _valueManager.AddValue("Department", selectedValue, "科別");
            */

            //載入學號
            //string studentID = hasClass ? classInfoNode.SelectSingleNode("StudentNumber").InnerText : "";
            string studentID = hasClass ? Student.Instance.Items[RunningID].StudentNumber : "";
            this.txtStudentID.Text = studentID;
            _valueManager.AddValue("StudentNumber", studentID, "學號");

            //載入座號
            //string seatNo = hasClass ? classInfoNode.SelectSingleNode("SeatNo").InnerText : "";
            string seatNo = hasClass ? Student.Instance.Items[RunningID].SeatNo : "";
            //SetSeatNo(seatNo);
            cboSeatNo.Text = seatNo;
            _valueManager.AddValue("SeatNo", seatNo, "座號");
        }

        private void _getSeatNoBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string classID = e.Argument as string;
            seatNoList = JHSchool.Feature.Legacy.Class.ListSeatNo(classID);
            e.Result = classID;
        }

        private void _getSeatNoBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ("" + e.Result != _WaitToGetSeatNo)
            {
                GetClassSeatNoList(_WaitToGetSeatNo);
            }
            else
            {
                int maxNo = 0;
                if (seatNoList.Count > 0)
                    maxNo = seatNoList[seatNoList.Count - 1];
                List<int> needSeatNoList = new List<int>();
                for (int i = 1; i <= maxNo + 1; i++)
                {
                    if (!seatNoList.Contains(i))
                        needSeatNoList.Add(i);
                }
                foreach (int sn in needSeatNoList)
                {
                    cboSeatNo.Items.Add(sn);
                }
            }
        }

        private void GetClassSeatNoList(string classID)
        {
            _WaitToGetSeatNo = classID;
            cboSeatNo.SelectedItem = null;
            cboSeatNo.Items.Clear();
            if (!_getSeatNoBackgroundWorker.IsBusy)
            {
                _getSeatNoBackgroundWorker.RunWorkerAsync(classID);
            }
        }

        //private void GetGradeYearClassList(string gradeyearID)
        //{
        //    _WaitToGetClass = gradeyearID;
        //    cboClass.Items.Clear();
        //    if (!_getClassBackgroundWorker.IsBusy)
        //    {
        //        _getClassBackgroundWorker.RunWorkerAsync(gradeyearID);
        //    }
        //}

        public override void Save()
        {
            ValidateStudentNumber(); //在驗一次學生是否重覆，因為在某些狀態不會啟動驗證程式。

            if (_errors.HasError)
            {
                FISCA.Presentation.Controls.MsgBox.Show("輸入資料未通過驗證，請修正後再儲存");
                return;
            }

            //EditStudent.Update(_valueManager.GetRequest("StudentList", "Student", "Field", "Condition", "ID", RunningID));
            DSXmlHelper helper = new DSXmlHelper("StudentList");
            helper.AddElement("Student");
            helper.AddElement("Student", "Field");
            foreach (string key in _valueManager.GetDirtyItems().Keys)
            {
                string elementName = (key == "Department" ? "OverrideDeptID" : key);
                string value = _valueManager.GetDirtyItems()[key];
                helper.AddElement("Student/Field", elementName, value);
            }
            helper.AddElement("Student", "Condition");
            helper.AddElement("Student/Condition", "ID", RunningID);
            EditStudent.Update(new DSRequest(helper));

            //LogUtility.LogChange(_valueManager, RunningID, GetStudentName());

            //Student.Instance.InvokBriefDataChanged(RunningID);
            //SmartSchool.Broadcaster.Events.Items["學生/資料變更"].Invoke(RunningID);
            Student.Instance.SyncDataBackground(RunningID);
            SaveButtonVisible = false;
        }

        private string GetStudentName()
        {
            try
            {
                StudentRecord student = Student.Instance.Items[RunningID];
                return student.Name;
            }
            catch (Exception)
            {
                return "<" + RunningID + ">";
            }
        }

        #endregion

        private void SetComboBoxValue(ComboBox combo, string value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i].ToString() == value)
                {
                    combo.SelectedIndex = i;
                    break;
                }
            }
            if (combo.SelectedIndex == -1)
                combo.Text = "";
        }

        private string GetComboValue(ComboBox combo)
        {
            return ((KeyValuePair<string, string>)combo.SelectedItem).Key;
        }


        private void txtStudentID_TextChanged(object sender, EventArgs e)
        {
            OnValueChanged("StudentNumber", txtStudentID.Text);
        }



        private void cboClass_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            OnValueChanged("RefClassID", cboClass.GetValue());
            //string seatNo = cboSeatNo.Text;
            //SetSeatNo(seatNo);
            cboSeatNo.Text = "";

            //if (cboClass.GetValue() == "")
            //    cboSeatNo.Enabled = false;
            //else
            GetClassSeatNoList(cboClass.GetValue());

            try
            {
                KeyValuePair<string, string> kv = ((KeyValuePair<string, string>)cboClass.SelectedItem);
                DSResponse rsp = QueryClass.GetClass(kv.Key);

                // (string.IsNullOrEmpty(cboDept.Text))
                cboDept.WatermarkText = rsp.GetContent().GetText("Class/Department");
            }
            catch { }
        }

        //private void cboProgram_SelectedIndexChanged_1(object sender, EventArgs e)
        //{
        //    OnValueChanged("StudentDepartment", cboProgram.GetValue());
        //}

        private void cboSeatNo_TextChanged(object sender, EventArgs e)
        {
            int sn;
            if (int.TryParse(cboSeatNo.Text, out sn) || string.IsNullOrEmpty(cboSeatNo.Text))
            {
                OnValueChanged("SeatNo", cboSeatNo.Text);
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("座號必須為數字");
            }
        }

        private void cboDept_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnValueChanged("Department", cboDept.GetValue());
        }

        private void SetSeatNo(string seatNo)
        {

            //載入座號
            bool hasClass = cboClass.GetValue() != "";
            //string seatNo = hasClass ? classInfoNode.SelectSingleNode("SeatNo").InnerText : "";


            cboSeatNo.SelectedItem = null;
            cboSeatNo.Items.Clear();
            if (cboClass.GetValue() != "")
            {
                if (seatNo != "")
                    cboSeatNo.Items.Add(seatNo);
                //List<int> seatNoList = Class.ListSeatNo(cboClass.GetValue());

                int maxNo = 0;
                if (seatNoList.Count > 0)
                    maxNo = seatNoList[seatNoList.Count - 1];
                List<int> needSeatNoList = new List<int>();
                for (int i = 1; i <= maxNo + 1; i++)
                {
                    if (!seatNoList.Contains(i))
                        needSeatNoList.Add(i);
                }
                foreach (int sn in needSeatNoList)
                {
                    cboSeatNo.Items.Add(sn);
                }
                //if (seatNo != "")
                SetComboBoxValue(cboSeatNo, seatNo);
                cboSeatNo.Enabled = true;
            }
            else
            {
                cboSeatNo.Enabled = false;
            }
        }

        private void cboSeatNo_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            OnValueChanged("SeatNo", cboSeatNo.Text);
        }

        private void label40_Click(object sender, EventArgs e)
        {

        }

        private void ClassInfoPalmerwormItem_Load(object sender, EventArgs e)
        {

        }

        private void txtStudentID_Validating(object sender, CancelEventArgs e)
        {
            ValidateStudentNumber();
        }

        private void ValidateStudentNumber()
        {
            if (string.IsNullOrEmpty(txtStudentID.Text))
            {
                _errors.SetError(txtStudentID, string.Empty);
                return; //空白不檢查。
            }

            if (QueryStudent.StudentNumberExists(RunningID, txtStudentID.Text))
                _errors.SetError(txtStudentID, "學號重覆，請確認資料。");
            else
                _errors.SetError(txtStudentID, string.Empty);
        }

        private void TipClassSelection()
        {
            if (string.IsNullOrEmpty(cboClass.GetValue()))
                warnings.SetError(cboClass, "請選擇「年級」後再選擇「班級」。");
            else
                warnings.SetError(cboClass, "");
        }

        private void cboClass_Validating(object sender, CancelEventArgs e)
        {
            TipClassSelection();
        }

        private void cboGradeYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cboClass.SelectedItem = null;
            cboClass.Items.Clear();
            //cboSeatNo.SelectedItem = null;
            cboSeatNo.Items.Clear();
            //cboSeatNo.Text = "";

            foreach (ClassRecord each in Class.Instance.Items)
            {
                if (cboGradeYear.Text == each.GradeYear)
                    cboClass.AddItem(each.ID, each.Name);
            }

            //if (cboGradeYear.GetValue() != "")

            //{
            //Dictionary<string, string> classList = JHSchool.Feature.Legacy.Class.GetClassListByGradeYear(cboGradeYear.GetValue());

            //Dictionary<string, string> classList = JHSchool.Feature.Legacy.Class.GetClassListByGradeYear(cboGradeYear.Text);
            //foreach (string key in classList.Keys)
            //    cboClass.AddItem(key, classList[key]);



            //cboClass.Enabled = false;
            //}
            //else
            //    GetGradeYearClassList(cboGradeYear.GetValue());

            OnValueChanged("GradeYear", cboGradeYear.Text);
            OnValueChanged("RefClassID","");
            foreach(ClassRecord cr in Class.Instance.Items)
                if(cr.Name ==cboClass.Text )
                    OnValueChanged("RefClassID",cr.ID );
            //OnValueChanged("RefClassID", cboClass.GetValue());

        }
    }
}
