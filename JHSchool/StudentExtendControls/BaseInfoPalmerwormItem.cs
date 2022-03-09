using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;
using JHSchool.Data;
using JHSchool.Feature.Legacy;
using JHSchool.Legacy;
using FCode = Framework.Security.FeatureCodeAttribute;
using K12.EduAdminDataMapping;
using FISCA.UDT;

namespace JHSchool.StudentExtendControls
    
{
    [FCode("JHSchool.Student.Detail0000", "�򥻸��")]
    internal partial class BaseInfoPalmerwormItem : FISCA.Presentation.DetailContent
    {
        private bool _isInitialized = false;
        private EnhancedErrorProvider _errors = new EnhancedErrorProvider();
        private bool _isBGBusy = false;
        private BackgroundWorker _BGWorker;
        private JHStudentRecord _StudRec;
        private string _defaultLoginID = string.Empty;
        private string _defaultIDNumber = string.Empty;

        private bool load_completed = false;
        private string cboNationality_ori = "";

        // �J�ǷӤ�
        private string _FreshmanPhotoStr = string.Empty;

        // ���~�Ӥ�
        private string _GraduatePhotoStr = string.Empty;

        private ChangeListener _DataListener { get; set; }
        PermRecLogProcess prlp;

        public BaseInfoPalmerwormItem()
        {
            InitializeComponent();
            Group = "�򥻸��";
            _DataListener = new ChangeListener();
            _DataListener.Add(new TextBoxSource(txtName));
            _DataListener.Add(new TextBoxSource(txtSSN));
            _DataListener.Add(new TextBoxSource(txtBirthDate));
            _DataListener.Add(new TextBoxSource(txtBirthPlace));
            _DataListener.Add(new TextBoxSource(txtEngName));
            _DataListener.Add(new TextBoxSource(txtLoginID));
            _DataListener.Add(new TextBoxSource(txtLoginPwd));
            _DataListener.Add(new TextBoxSource(txtEmail));
            _DataListener.Add(new ComboBoxSource(cboGender, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboNationality, ComboBoxSource.ListenAttribute.Text));
            _DataListener.Add(new ComboBoxSource(cboAccountType, ComboBoxSource.ListenAttribute.Text));
            _DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(_DataListener_StatusChanged);

            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            prlp = new PermRecLogProcess();
            Initialize();
            JHStudent.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterChange);
            JHStudent.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterDelete);
            Disposed += new EventHandler(BaseInfoPalmerwormItem_Disposed); 
        }

        void JHStudent_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            Student.Instance.SyncAllBackground();
        }

        void BaseInfoPalmerwormItem_Disposed(object sender, EventArgs e)
        {
            JHStudent.AfterChange -= new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterChange);
            JHStudent.AfterDelete -= new EventHandler<K12.Data.DataChangedEventArgs>(JHStudent_AfterDelete);
        }

        
        void JHStudent_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHStudent_AfterChange), sender, e);
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
            if (Framework.User.Acl[GetType()].Editable)
                SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            else
                SaveButtonVisible = false;
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_isBGBusy)
            {
                _isBGBusy = false;
                _BGWorker.RunWorkerAsync();
                return;
            }
            BindDataToForm();
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {

            SetFormDataToDALRec();

            // �ˬd�ͤ�

            
            // �ˬd�ʧO
            List<string> checkGender = new List<string>();
            checkGender.Add("�k");
            checkGender.Add("");
            checkGender.Add("�k");

            if (!checkGender.Contains(cboGender.Text))
            {
                _errors.SetError(cboGender, "�ʧO���~�A�нT�{��ơC");
                return;            
            }

            DateTime dt;

            if (!string.IsNullOrEmpty(txtBirthDate.Text))
            {
                if (!DateTime.TryParse(txtBirthDate.Text, out dt))
                {
                    _errors.SetError(txtBirthDate, "������~�A�нT�{��ơC");
                    return;
                }
            }
            else
            {
                _StudRec.Birthday = null;            
            }

            List<string> checkID = new List<string>();
            List<string> checkSSN = new List<string>();


            foreach (JHStudentRecord studRec in JHStudent.SelectAll())
            {
                checkID.Add(studRec.SALoginName);
                checkSSN.Add(studRec.IDNumber);
            }
            if(!string.IsNullOrEmpty(_StudRec.SALoginName ))
                if (checkID.Contains(_StudRec.SALoginName))
                {
                    if (_defaultLoginID != _StudRec.SALoginName)
                    {
                        _errors.SetError(txtLoginID, "�ǥ͵n�J�b�����СA�нT�{��ơC");
                        return;
                    }
                }
            if(!string.IsNullOrEmpty (_StudRec.IDNumber))
                if (checkSSN.Contains(_StudRec.IDNumber))
                {
                    if (_defaultIDNumber != _StudRec.IDNumber)
                    {
                        _errors.SetError(txtSSN, "�����Ҹ����СA�нT�{��ơC");
                        return;
                    }
                }

            JHStudent.Update(_StudRec);
            SetAfterEditLog();
            Student.Instance.SyncDataBackground(PrimaryKey);
            _errors.Clear();
            //BindDataToForm();
        }

        private void SetFormDataToDALRec()
        {
            _StudRec.AccountType = cboAccountType.Text;

            DateTime dt;
            if (DateTime.TryParse(txtBirthDate.Text, out dt))
                _StudRec.Birthday = dt;

            _StudRec.BirthPlace = txtBirthPlace.Text;
            _StudRec.EnglishName = txtEngName.Text;
            _StudRec.Gender = cboGender.Text;
            _StudRec.IDNumber = txtSSN.Text;
            _StudRec.Name = txtName.Text;
            _StudRec.Nationality = cboNationality.Text;
            _StudRec.SALoginName = txtLoginID.Text;
            _StudRec.SAPassword = txtLoginPwd.Text;
            _StudRec.EMail = txtEmail.Text;
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            _DataListener.SuspendListen();
            _errors.Clear();
            ClearFormValue();
            LoadDALDataToForm();
            _DataListener.Reset();
            _DataListener.ResumeListen();
            SaveButtonVisible = false;
            CancelButtonVisible = false;
        }

        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get Photo
            _FreshmanPhotoStr = _GraduatePhotoStr = string.Empty;
            _FreshmanPhotoStr = K12.Data.Photo.SelectFreshmanPhoto(PrimaryKey);
            _GraduatePhotoStr = K12.Data.Photo.SelectGraduatePhoto(PrimaryKey);

            // studentRec
            _StudRec = JHStudent.SelectByID(PrimaryKey);
        }


        
        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            _errors.Clear();
            this.Loading = true;
            if (_BGWorker.IsBusy)
                _isBGBusy = true;
            else
                _BGWorker.RunWorkerAsync();
        }

        //�N�e���M��
        private void ClearFormValue()
        {
            txtEmail.Text=txtBirthDate.Text = txtBirthPlace.Text = txtEngName.Text = txtLoginID.Text = txtName.Text = txtSSN.Text = cboAccountType.Text = cboGender.Text = cboNationality.Text = string.Empty;           
        }
        
        private void BindDataToForm()
        {
            // �D�n�[��ǥͳQ�R�����ˬd
            if (_StudRec != null)
            {
                _DataListener.SuspendListen();
                ClearFormValue();
                LoadDALDataToForm();
                SetBeforeEditLog();

                // get checkDefault
                _defaultIDNumber = _StudRec.IDNumber;
                _defaultLoginID = _StudRec.SALoginName;
                this.Loading = false;
                SaveButtonVisible = false;
                CancelButtonVisible = false;
                _DataListener.Reset();
                _DataListener.ResumeListen();
            }
        }

        private void SetBeforeEditLog()
        {
            prlp.SetBeforeSaveText("�m�W", txtName.Text);
            prlp.SetBeforeSaveText("�����Ҹ�", txtSSN.Text);
            prlp.SetBeforeSaveText("�ͤ�", txtBirthDate.Text);
            prlp.SetBeforeSaveText("�ʧO", cboGender.Text);
            prlp.SetBeforeSaveText("���y", cboNationality.Text);
            prlp.SetBeforeSaveText("�X�ͦa", txtBirthPlace.Text);
            prlp.SetBeforeSaveText("�^��m�W", txtEngName.Text);
            prlp.SetBeforeSaveText("�n�J�b��", txtLoginID.Text);
            prlp.SetBeforeSaveText("�b������", cboAccountType.Text);
            prlp.SetBeforeSaveText("�q�l�H�c", txtEmail.Text);
        }

        private void SetAfterEditLog()
        {
            prlp.SetAfterSaveText("�m�W", txtName.Text);
            prlp.SetAfterSaveText("�����Ҹ�", txtSSN.Text);
            prlp.SetAfterSaveText("�ͤ�", txtBirthDate.Text);
            prlp.SetAfterSaveText("�ʧO", cboGender.Text);
            prlp.SetAfterSaveText("���y", cboNationality.Text);
            prlp.SetAfterSaveText("�X�ͦa", txtBirthPlace.Text);
            prlp.SetAfterSaveText("�^��m�W", txtEngName.Text);
            prlp.SetAfterSaveText("�n�J�b��", txtLoginID.Text);
            prlp.SetAfterSaveText("�b������", cboAccountType.Text);
            prlp.SetAfterSaveText("�q�l�H�c", txtEmail.Text);
            prlp.SetActionBy("���y", "�ǥͰ򥻸��");
            prlp.SetAction("�ק�ǥͰ򥻸��");
            prlp.SetDescTitle("�m�W:"+_StudRec.Name+",�Ǹ�:"+_StudRec.StudentNumber +",");
            prlp.SaveLog("", "", "Student", PrimaryKey);
            
        }

        private void LoadDALDataToForm()
        {
            //2017/4/19 �o�~�s�W  �|�����J�����A�קKĲ�ocboNationality_TextChanged()
            load_completed = false;

            if(_StudRec.Birthday.HasValue )
                txtBirthDate.Text = _StudRec.Birthday.Value.ToShortDateString();
            txtBirthPlace.Text = _StudRec.BirthPlace;
            txtEngName.Text = _StudRec.EnglishName;
            txtLoginID.Text = _StudRec.SALoginName;
            txtLoginPwd.Text = _StudRec.SAPassword;
            txtName.Text = _StudRec.Name;
            txtSSN.Text = _StudRec.IDNumber;
            cboAccountType.Text = _StudRec.AccountType;
            cboGender.Text = _StudRec.Gender;
            cboNationality.Text = _StudRec.Nationality;
            txtEmail.Text = _StudRec.EMail;
            //2017/4/19 �o�~�s�W �O�������y�A�@���P�s��J����ϥ�
            cboNationality_ori = _StudRec.Nationality;
            // �ѪR
            try
            {
                             
                pic1.Image = Photo.ConvertFromBase64Encoding(_FreshmanPhotoStr, pic1.Width, pic1.Height);
            }
            catch (Exception)
            {
                pic1.Image = pic1.InitialImage;
            }

            try
            {                
                pic2.Image = Photo.ConvertFromBase64Encoding(_GraduatePhotoStr, pic2.Width, pic2.Height);
            }
            catch (Exception)
            {
                pic2.Image = pic2.InitialImage;
            }

            load_completed = true;
        }

        public DetailContent GetContent()
        {
            return new BaseInfoPalmerwormItem();
        }

        private void Initialize()
        {
            if (_isInitialized) return;
            //���J��a�C��
            try
            {
                List<string> dataList = new List<string>();
                foreach (string item in Utility.GetNationalityMappingDict().Keys)
                    dataList.Add(item);
                cboNationality.Items.AddRange(dataList.ToArray());
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }


            


            //this.cboNationality.Items.Add("���إ���");
            //this.cboNationality.Items.Add("���ؤH���@�X��");
            //this.cboNationality.Items.Add("�s�[��");
            //this.cboNationality.Items.Add("�q�l");
            //this.cboNationality.Items.Add("�L��");
            //this.cboNationality.Items.Add("�饻");
            //this.cboNationality.Items.Add("����");
            //this.cboNationality.Items.Add("���Ӧ��");
            //this.cboNationality.Items.Add("��߻�");
            //this.cboNationality.Items.Add("�s�[�Y");
            //this.cboNationality.Items.Add("����");
            //this.cboNationality.Items.Add("�V�n");
            //this.cboNationality.Items.Add("�Z��");
            //this.cboNationality.Items.Add("�D�j�Q��");
            //this.cboNationality.Items.Add("�æ���");
            //this.cboNationality.Items.Add("�J��");
            //this.cboNationality.Items.Add("�n�D");
            //this.cboNationality.Items.Add("�k��");
            //this.cboNationality.Items.Add("�q�j�Q");
            //this.cboNationality.Items.Add("���");
            //this.cboNationality.Items.Add("�^��");
            //this.cboNationality.Items.Add("�w��");
            //this.cboNationality.Items.Add("�[���j");
            //this.cboNationality.Items.Add("�����j���[");
            //this.cboNationality.Items.Add("�ʦa����");
            //this.cboNationality.Items.Add("����");
            //this.cboNationality.Items.Add("���ڧ�");
            //this.cboNationality.Items.Add("�ڦ�");
            //this.cboNationality.Items.Add("���ۤ��");
            //this.cboNationality.Items.Add("�کԦc");
            //this.cboNationality.Items.Add("�Q�Ԧc");
            //this.cboNationality.Items.Add("��L");

            cboGender.Items.AddRange(new string[] { "�k", "�k" });
          



            _isInitialized = true;
        }


        private void buttonItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "�Ҧ��v��(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(od.FileName, FileMode.Open);
                    Bitmap orgBmp = new Bitmap(fs);
                    fs.Close();

                    Bitmap newBmp = new Bitmap(orgBmp, pic1.Size);
                    pic1.Image = newBmp;

                    _FreshmanPhotoStr = ToBase64String(Photo.Resize(new Bitmap(orgBmp)));
                    K12.Data.Photo.UpdateFreshmanPhoto(_FreshmanPhotoStr, PrimaryKey);                    
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "�Ҧ��v��(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
            if (od.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = null;
                try
                {
                    fs = new FileStream(od.FileName, FileMode.Open);
                    Bitmap orgBmp = new Bitmap(fs);
                    fs.Close();

                    Bitmap newBmp = new Bitmap(orgBmp, pic2.Size);
                    pic2.Image = newBmp;

                    _GraduatePhotoStr = ToBase64String(Photo.Resize(new Bitmap(orgBmp)));

                    K12.Data.Photo.UpdateGraduatePhoto(_GraduatePhotoStr, PrimaryKey);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }

        private static string ToBase64String(Bitmap newBmp)
        {
            MemoryStream ms = new MemoryStream();
            newBmp.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, (int)ms.Length);
            ms.Close();

            return Convert.ToBase64String(bytes);
        }

        //�t�s�Ӥ�
        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_FreshmanPhotoStr);
        }

        //�t�s�Ӥ�
        private void buttonItem4_Click(object sender, EventArgs e)
        {
            SavePicture(_GraduatePhotoStr);
        }

        private void SavePicture(string imageString)
        {
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG �v��|*.png;";
            sd.FileName = txtSSN.Text + ".png";

            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(sd.FileName, FileMode.Create);
                    byte[] imageData = Convert.FromBase64String(imageString);
                    fs.Write(imageData, 0, imageData.Length);
                    fs.Close();
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                }
            }
        }


        private void txtBirthDate_Validated_1(object sender, EventArgs e)
        {
            _errors.SetError(txtBirthDate, string.Empty);

            if (!txtBirthDate.IsValid)
                _errors.SetError(txtBirthDate, "�п�J yyyy/mm/dd �ŦX����榡��r");
        }

        private void txtSSN_Validating(object sender, CancelEventArgs e)
        {
            ValidateIDNumber();
        }

        private void txtLoginID_Validating(object sender, CancelEventArgs e)
        {
            ValidateLoginID();
        }

        // �ˬd
        private void ValidateIDNumber()
        {
            _errors.SetError(txtSSN, string.Empty);

            if (string.IsNullOrEmpty(txtSSN.Text))
            {
                _errors.SetError(txtSSN, string.Empty);
                return;
            }

            if (QueryStudent.IDNumberExists(PrimaryKey, txtSSN.Text))
                _errors.SetError(txtSSN, "�����Ҹ����СA�нT�{��ơC");           
                
        }

        private void ValidateLoginID()
        {
            _errors.SetError(txtLoginID, string.Empty);

            if (string.IsNullOrEmpty(txtLoginID.Text))
            {
                _errors.SetError(txtLoginID, string.Empty);
                return;
            }

            if (QueryStudent.LoginIDExists(txtLoginID.Text, PrimaryKey))
                _errors.SetError(txtLoginID, "�b�����СA�Э��s��ܡC");
        }

        #region �M���Ӥ�
        //�M���s�ͷӤ�
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("�z�T�w�n�M�����ǥͪ��Ӥ��ܡH", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _FreshmanPhotoStr = string.Empty;
                pic1.Image = pic1.InitialImage;
                K12.Data.Photo.UpdateFreshmanPhoto("", PrimaryKey);
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }

        //�M�����~�Ӥ�
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("�z�T�w�n�M�����ǥͪ��Ӥ��ܡH", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

            try
            {
                _GraduatePhotoStr = string.Empty;
                pic2.Image = pic2.InitialImage;
                K12.Data.Photo.UpdateGraduatePhoto("",PrimaryKey );
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
            }
        }
        #endregion

        private void txtBirthDate_TextChanged(object sender, EventArgs e)
        {
            _errors.SetError(txtBirthDate, string.Empty);
        }

        private void cboGender_TextChanged(object sender, EventArgs e)
        {
            _errors.SetError(cboGender, string.Empty);
        }

        private void cboNationality_Validating(object sender, CancelEventArgs e)
        {

            //List<DAO.UDT_NationalityMapping> nation_list = new List<DAO.UDT_NationalityMapping>();
            //AccessHelper accessHelper = new AccessHelper();
            //nation_list = accessHelper.Select<DAO.UDT_NationalityMapping>();

            //List<string> nation_name_list = new List<string>();

            //foreach (DAO.UDT_NationalityMapping item in nation_list)
            //{
            //    nation_name_list.Add(item.Name);                        
            //}


            //errorProvider1.SetError(cboNationality,string.Empty);

            //if (!nation_name_list.Contains(cboNationality.Text) && cboNationality.Text!="")
            //{
            //    errorProvider1.Icon = SystemIcons.Warning;
            //    errorProvider1.SetError(cboNationality, "�����y�W�١A���s�b��аȧ@�~>���/�N�X>���y���^���Ӫ� ���]�w���A��ĳ�˹�C");                        
            //}

        }

        //2017/4/19 �o�~�s�W ��ť ���y��� ���e���ܨƥ�
        private void cboNationality_TextChanged(object sender, EventArgs e)
        {           
            errorProvider1.SetError(cboNationality, string.Empty);
            if (load_completed && cboNationality.Text !=cboNationality_ori) 
            {
                List<string> nation_name_list = new List<string>();
                List<DAO.UDT_NationalityMapping> nation_list = new List<DAO.UDT_NationalityMapping>();
                AccessHelper accessHelper = new AccessHelper();
                nation_list = accessHelper.Select<DAO.UDT_NationalityMapping>();

                foreach (DAO.UDT_NationalityMapping item in nation_list)
                {
                    nation_name_list.Add(item.Name);
                }

                if (!nation_name_list.Contains(cboNationality.Text) && cboNationality.Text != "")
                {
                    
                    //errorProvider1.Icon = new Icon(SystemIcons.Warning, 8 ,8);

                    errorProvider1.SetError(cboNationality, "�����y�W�١A���s�b��аȧ@�~>���/�N�X>���y���^���Ӫ� ���]�w���A��ĳ�˹�C");
                }                        
            }            
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
