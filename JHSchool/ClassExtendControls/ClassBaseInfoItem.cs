using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation;
using Framework;
using JHSchool.Data;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.ClassExtendControls
{
    [FCode("JHSchool.Class.Detail0010", "�Z�Ű򥻸��")]
    internal partial class ClassBaseInfoItem : FISCA.Presentation.DetailContent
    {
        //�~�ŲM��
        List<string> _gradeYearList = new List<string>();
        //?
        private ErrorProvider epTeacher = new ErrorProvider();
        private ErrorProvider epDisplayOrder = new ErrorProvider();
        private ErrorProvider epGradeYear = new ErrorProvider();
        private ErrorProvider epClassName = new ErrorProvider();
        private PermRecLogProcess prlp;

        private bool _isBGWorkBusy = false;
        private BackgroundWorker _BGWorker;
        private ChangeListener _DataListener { get; set; }
        private JHClassRecord _ClassRecord;
        private List<JHClassRecord> _AllClassRecList;
        private Dictionary<string,string> _TeacherNameDic;

        //?
        private string _NamingRule = "";

        //�غc�l
        public ClassBaseInfoItem()
        {
            InitializeComponent();
            Group = "�Z�Ű򥻸��";
            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtClassName));
            _DataListener.Add(new TextBoxSource(txtSortOrder));
            _DataListener.Add(new TextBoxSource(txtClassNumber));
            _DataListener.Add(new ComboBoxSource(cboGradeYear, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboTeacher, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);
            _TeacherNameDic = new Dictionary<string, string>();            
            prlp = new PermRecLogProcess();
            JHClass.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHClass_AfterChange);
            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            Disposed += new EventHandler(ClassBaseInfoItem_Disposed);
        }

        void ClassBaseInfoItem_Disposed(object sender, EventArgs e)
        {
            JHClass.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(JHClass_AfterChange);
        }

        void JHClass_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHClass_AfterChange), sender, e);
            }
            else
            {
                if (PrimaryKey != "")
                {
                    if (!_BGWorker.IsBusy)
                        _BGWorker.RunWorkerAsync();
                }
            }
        }

        void _DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (!IsValid())
            {
                FISCA.Presentation.Controls.MsgBox.Show("��J��ƥ��q�L���ҡA�Эץ���A���x�s");
                return;
            }

            
            _ClassRecord.NamingRule = _NamingRule;
            _ClassRecord.ClassNumber = txtClassNumber.Text;
            // �~��
            int GrYear;
            if (int.TryParse(cboGradeYear.Text, out GrYear))
                _ClassRecord.GradeYear = GrYear;
            else
                _ClassRecord.GradeYear = null;
            
            // �Z�W�૬
            if (ValidateNamingRule(_NamingRule))
                _ClassRecord.Name = ParseClassName(_NamingRule, GrYear);
            else
            {
                if (ValidClassName(_ClassRecord.ID, txtClassName.Text))
                    _ClassRecord.Name = txtClassName.Text;
                else
                    return;            
            }

            _ClassRecord.RefTeacherID = "";
            // �Юv
            foreach (KeyValuePair<string, string> val in _TeacherNameDic)
                if (val.Value == cboTeacher.Text)
                    _ClassRecord.RefTeacherID = val.Key;
            _ClassRecord.DisplayOrder = txtSortOrder.Text;

            SaveButtonVisible = false;
            CancelButtonVisible = false;
            // Log
            prlp.SetAfterSaveText("�Z�ŦW��", txtClassName.Text);
            prlp.SetAfterSaveText("�Z�ũR�W�W�h", _ClassRecord.NamingRule);
            prlp.SetAfterSaveText("�~��", cboGradeYear.Text);
            prlp.SetAfterSaveText("�Z�ɮv", cboTeacher.Text);
            prlp.SetAfterSaveText("�ƦC�Ǹ�", txtSortOrder.Text);
            prlp.SetAfterSaveText("�Z�Žs��", txtClassNumber.Text);
            prlp.SetActionBy("���y", "�Z�Ű򥻸��");
            prlp.SetAction("�ק�Z�Ű򥻸��");
            prlp.SetDescTitle("�Z�ŦW��:" + _ClassRecord.Name+",");
            prlp.SaveLog("", "", "class", PrimaryKey);
            JHClass.Update(_ClassRecord);
            Class.Instance.SyncDataBackground(PrimaryKey);
           

        }

        protected override void OnCancelButtonClick(EventArgs e)
        {            
            _DataListener.SuspendListen();
            LoadDALDefaultDataToForm();
            _DataListener.Reset();
            _DataListener.ResumeListen();
            SaveButtonVisible = false;
            CancelButtonVisible = false;

        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGWorkBusy)
            {
                _isBGWorkBusy = false;
                _BGWorker.RunWorkerAsync();
                return;
            }            
            BindDataToForm();
        }

        private void LoadDefaultDataToForm()
        {
            // �~��
            LoadGradeYearToForm();

            // �Юv
            LoadTeacherNameToForm();
        }

        private void LoadTeacherNameToForm()
        {
            cboTeacher.Items.Clear();
            List<string> nameList = new List<string>();
            foreach (string name in _TeacherNameDic.Values)
                nameList.Add(name);
            nameList.Sort();

            cboTeacher.Items.AddRange(nameList.ToArray());
        }

        private void LoadGradeYearToForm()
        {
            cboGradeYear.Items.Clear();
            List<string> GradeYearList = new List<string>();
            foreach (JHClassRecord classRec in JHClass.SelectAll())
                if (classRec.GradeYear.HasValue)
                    if (!GradeYearList.Contains(classRec.GradeYear.Value + ""))
                        GradeYearList.Add(classRec.GradeYear.Value + "");
            GradeYearList.Sort(GradeYearSort);
            cboGradeYear.Items.AddRange(GradeYearList.ToArray());
        }

        private int GradeYearSort(string x, string y)
        {
            string xx = x.PadLeft(10, '0');
            string yy = y.PadLeft(10, '0');

            return xx.CompareTo(yy);
        }

        private void BindDataToForm()
        {

            _DataListener.SuspendListen();
            // �w�]��
            LoadDefaultDataToForm();
            LoadDALDefaultDataToForm();

            // Before log
            prlp.SetBeforeSaveText("�Z�ŦW��", txtClassName.Text);
            prlp.SetBeforeSaveText("�~��", cboGradeYear.Text);
            prlp.SetBeforeSaveText("�Z�ɮv", cboTeacher.Text);
            prlp.SetBeforeSaveText("�ƦC�Ǹ�", txtSortOrder.Text);
            prlp.SetBeforeSaveText("�Z�Žs��", txtClassNumber.Text);
            if (_ClassRecord !=null )
                prlp.SetBeforeSaveText("�Z�ũR�W�W�h", _ClassRecord.NamingRule);
            _DataListener.Reset();
            _DataListener.ResumeListen();
            this.Loading = false;
            SaveButtonVisible = false;
            CancelButtonVisible = false;

        }


        // �N DAL ��Ʃ�� Form
        private void LoadDALDefaultDataToForm()
        {
            if (_ClassRecord != null)
            {
                txtSortOrder.Text = _ClassRecord.DisplayOrder;
                txtClassNumber.Text = _ClassRecord.ClassNumber;
                if (_ClassRecord.GradeYear.HasValue)
                    cboGradeYear.Text = _ClassRecord.GradeYear.Value + "";
                else
                    cboGradeYear.Text = "";

                if (_TeacherNameDic.ContainsKey(_ClassRecord.RefTeacherID))
                    cboTeacher.Text = _TeacherNameDic[_ClassRecord.RefTeacherID];
                else
                    cboTeacher.Text = "";

                _NamingRule = _ClassRecord.NamingRule;
                txtClassName.Text = _ClassRecord.Name;
            }
        }


        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _ClassRecord = JHClass.SelectByID(PrimaryKey);
            _AllClassRecList = JHClass.SelectAll();
            
            // �Юv�W�ٯ���
            _TeacherNameDic.Clear();
            foreach (JHTeacherRecord TRec in JHTeacher.SelectAll())
            {
                if (TRec.Status == K12.Data.TeacherRecord.TeacherStatus.�R��)
                    continue;

                if (string.IsNullOrEmpty(TRec.Nickname))
                    _TeacherNameDic.Add(TRec.ID, TRec.Name);
                else
                    _TeacherNameDic.Add(TRec.ID, TRec.Name + "(" + TRec.Nickname + ")");
            }
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {            
            epClassName.Clear();
            epDisplayOrder.Clear();
            epGradeYear.Clear();
            epTeacher.Clear();
            _NamingRule = "";
            this.Loading = true;
            if (_BGWorker.IsBusy)
                _isBGWorkBusy = true;
            else
                _BGWorker.RunWorkerAsync();
        }
        



        private void InitializeGradeYearList()
        {
            List<string> gList = new List<string>();
            foreach (XmlNode node in JHSchool.Feature.Legacy.QueryClass.GetGradeYearList().GetContent().GetElements("GradeYear"))
            {
                string gradeYear = node.SelectSingleNode("GradeYear").InnerText;
                if (!gList.Contains(gradeYear))
                    gList.Add(gradeYear);
            }
            cboGradeYear.Items.AddRange(gList.ToArray());
            _gradeYearList = gList;
        }

 

        protected void cboGradeYear_TextChanged(object sender, EventArgs e)
        {
            if (ValidateNamingRule(_NamingRule))
            {
                int gradeYear = 0;
                if (int.TryParse(cboGradeYear.Text, out gradeYear))
                {
                    string classname = ParseClassName(_NamingRule, gradeYear);
                    classname = classname.Replace("{", "");
                    classname = classname.Replace("}", "");
                    txtClassName.Text = classname;

                    if(_ClassRecord.GradeYear.HasValue)
                        if (gradeYear != _ClassRecord.GradeYear.Value)
                        {
                            SaveButtonVisible = true;
                            CancelButtonVisible = true;
                        }
                        else
                        {
                            SaveButtonVisible = false;
                            CancelButtonVisible = false;
                        }
                }
                else
                    txtClassName.Text = _NamingRule;
            }
        }

        public bool IsValid()
        {
            // �Z�ŦW������
            bool valid = true;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl.Tag != null)
                {
                    if (ctrl.Tag.ToString() == "error")
                        valid = false;
                }
            }
            return valid;
        }

        private void cboTeacher_Validated(object sender, EventArgs e)
        {
            string id = string.Empty;

            foreach (KeyValuePair<string, string> var in _TeacherNameDic)
                if (var.Value == cboTeacher.Text)
                    id = var.Key;



            if (!string.IsNullOrEmpty(cboTeacher.Text) && id == "")
            {
                epTeacher.SetError(cboTeacher, "�d�L���Юv");
                cboTeacher.Tag = "error";
                return;
            }
            else
            {
                epTeacher.Clear();
                cboTeacher.Tag = id;
            }
        }

        private void cboTeacher_Validating(object sender, CancelEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int index = combo.FindStringExact(combo.Text);
            if (index >= 0)
            {
                combo.SelectedItem = combo.Items[index];
            }

        }

        private void txtSortOrder_Validated(object sender, EventArgs e)
        {
            string text = txtSortOrder.Text;
            int i;
            if (!string.IsNullOrEmpty(text) && !int.TryParse(text, out i))
            {
                epDisplayOrder.SetError(txtSortOrder, "�п�J���");
                txtSortOrder.Tag = "error";
                return;
            }
            epDisplayOrder.Clear();
            txtSortOrder.Tag = null;
        }

        private void cboGradeYear_Validated(object sender, EventArgs e)
        {
            string text = cboGradeYear.Text;
            bool hasGradeYear = false;

            if(_gradeYearList.Contains(text))
                    hasGradeYear = true;

            int i;
            if (!string.IsNullOrEmpty(text) && !int.TryParse(text, out i))
            {                
                epGradeYear.SetError(cboGradeYear, "�~�ť������Ʀr");
                cboGradeYear.Tag = "error";
                return;
            }

            if (!string.IsNullOrEmpty(text) && !hasGradeYear)
            {             
                epGradeYear.SetError(cboGradeYear, "�L���~��");
                cboGradeYear.Tag = null;
            }
            else
            {
                epGradeYear.Clear();
                cboGradeYear.Tag = null;
            }
        }




        private bool ValidateNamingRule(string namingRule)
        {
            return namingRule.IndexOf('{') < namingRule.IndexOf('}');
        }

        // �ˬd�Z�ũR�W�W�h
        private string ParseClassName(string namingRule, int gradeYear)
        {
            // ��~�ŬO7,8,9
            if (gradeYear >= 6)
                gradeYear -= 6;

            gradeYear--;
            if (!ValidateNamingRule(namingRule))
                return namingRule;
            string classlist_firstname = "", classlist_lastname = "";
            if (namingRule.Length == 0) return "{" + (gradeYear + 1) + "}";

            string tmp_convert = namingRule;

            // ��X"{"���e��r �é�J classlist_firstname , �ð��h"{"
            if (tmp_convert.IndexOf('{') > 0)
            {
                classlist_firstname = tmp_convert.Substring(0, tmp_convert.IndexOf('{'));
                tmp_convert = tmp_convert.Substring(tmp_convert.IndexOf('{') + 1, tmp_convert.Length - (tmp_convert.IndexOf('{') + 1));
            }
            else tmp_convert = tmp_convert.TrimStart('{');

            // ��X } �����r classlist_lastname , �ð��h"}"
            if (tmp_convert.IndexOf('}') > 0 && tmp_convert.IndexOf('}') < tmp_convert.Length - 1)
            {
                classlist_lastname = tmp_convert.Substring(tmp_convert.IndexOf('}') + 1, tmp_convert.Length - (tmp_convert.IndexOf('}') + 1));
                tmp_convert = tmp_convert.Substring(0, tmp_convert.IndexOf('}'));
            }
            else tmp_convert = tmp_convert.TrimEnd('}');

            // , �s�J array
            string[] listArray = new string[tmp_convert.Split(',').Length];
            listArray = tmp_convert.Split(',');

            // �ˬd�O�_�b�M��d��
            if (gradeYear >= 0 && gradeYear < listArray.Length)
            {
                tmp_convert = classlist_firstname + listArray[gradeYear] + classlist_lastname;
            }
            else
            {
                tmp_convert = classlist_firstname + "{" + (gradeYear + 1) + "}" + classlist_lastname;
            }
            return tmp_convert;
        }

        // �ˬd�Z�ŦW�٬O�_����
        private bool ValidClassName(string classid, string className)
        {            
            if (string.IsNullOrEmpty(className)) return false;
            foreach (JHClassRecord classRec in _AllClassRecList)
            {
                if (classRec.ID != classid && classRec.Name == className)
                    return false;
            }
            return true;
        }

        private void txtClassName_TextChanged(object sender, EventArgs e)
        {
            _DataListener.SuspendListen();
            //if (!_StopEvent)
            //{

                if (string.IsNullOrEmpty(txtClassName.Text))
                {
                    epClassName.SetError(txtClassName, "�Z�ŦW�٤��i�ť�");
                    txtClassName.Tag = "error";
                    _DataListener.Reset();
                    _DataListener.ResumeListen();

                    return;
                }
                if (ValidClassName(PrimaryKey, txtClassName.Text)==false)
                {
                    epClassName.SetError(txtClassName, "�Z�Ť��i�P�䥦�Z�ŭ���");
                    txtClassName.Tag = "error";
                    _DataListener.Reset();
                    _DataListener.ResumeListen();

                    return;
                }
                epClassName.Clear();
                txtClassName.Tag = null;
            //}
            _DataListener.Reset();
            _DataListener.ResumeListen();
            
        }

        private void txtClassNumber_TextChanged(object sender, EventArgs e)
        {
            //_DataListener.SuspendListen();
            //_DataListener.Reset();
            //_DataListener.ResumeListen();
            //txtClassNumber.Tag = null;
        }
        private void txtClassName_Leave(object sender, EventArgs e)
        {
            _DataListener.SuspendListen();
                     
            _NamingRule = txtClassName.Text;

            if (ValidateNamingRule(txtClassName.Text))
            {
                int gradeYear = 0;
                if (int.TryParse(cboGradeYear.Text, out gradeYear))
                {
                    txtClassName.Text = ParseClassName(_NamingRule, gradeYear);
                }
                //_ClassRecord.NamingRule = _NamingRule;
            }
            else
            {
                _ClassRecord.NamingRule = _NamingRule;
            }
            _DataListener.Reset();
            _DataListener.ResumeListen();
            //if (!string.IsNullOrEmpty(_ClassRecord.NamingRule))
            if ((txtClassName.Text != _ClassRecord.Name) || (_NamingRule != _ClassRecord.NamingRule))
            {
                SaveButtonVisible = true;
                CancelButtonVisible = true;
                //txtClassName.Focus();
            }

            if (string.IsNullOrEmpty(_ClassRecord.NamingRule))
            {
                SaveButtonVisible = false;
                CancelButtonVisible = false;
            }
        }

        private void txtClassName_Enter(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_NamingRule))
            {

                _DataListener.SuspendListen();

                if (ValidateNamingRule(_NamingRule))
                {
                    //_StopEvent = true;
                    txtClassName.Text = _NamingRule;
                    //_StopEvent = false;
                }
                else
                    _NamingRule = txtClassName.Text;

                _DataListener.Reset();
                _DataListener.ResumeListen();
            }
        }

        public DetailContent GetContent()
        {
            return new ClassBaseInfoItem();
        }
    }
}
