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
using System.Net.Http;

namespace JHSchool.StudentExtendControls
    
{
    [FCode("JHSchool.Student.Detail0000", "基本資料")]
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

        // 入學照片
        private string _FreshmanPhotoStr = string.Empty;

        // 畢業照片
        private string _GraduatePhotoStr = string.Empty;

        private ChangeListener _DataListener { get; set; }
        PermRecLogProcess prlp;

        public BaseInfoPalmerwormItem()
        {
            InitializeComponent();
            Group = "基本資料";
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

            // 檢查生日

            
            // 檢查性別
            List<string> checkGender = new List<string>();
            checkGender.Add("男");
            checkGender.Add("");
            checkGender.Add("女");

            if (!checkGender.Contains(cboGender.Text))
            {
                _errors.SetError(cboGender, "性別錯誤，請確認資料。");
                return;            
            }

            DateTime dt;

            if (!string.IsNullOrEmpty(txtBirthDate.Text))
            {
                if (!DateTime.TryParse(txtBirthDate.Text, out dt))
                {
                    _errors.SetError(txtBirthDate, "日期錯誤，請確認資料。");
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
                checkID.Add(studRec.SALoginName.ToLower());
                checkSSN.Add(studRec.IDNumber);
            }
            if(!string.IsNullOrEmpty(_StudRec.SALoginName ))
                if (checkID.Contains(_StudRec.SALoginName.ToLower()))
                {
                    if (_defaultLoginID != _StudRec.SALoginName)
                    {
                        _errors.SetError(txtLoginID, "學生登入帳號重覆，請確認資料。");
                        return;
                    }
                }
            if(!string.IsNullOrEmpty (_StudRec.IDNumber))
                if (checkSSN.Contains(_StudRec.IDNumber))
                {
                    if (_defaultIDNumber != _StudRec.IDNumber)
                    {
                        _errors.SetError(txtSSN, "身分證號重覆，請確認資料。");
                        return;
                    }
                }

            JHStudent.Update(_StudRec);
            SetAfterEditLog();

            if (IfNeededSynchronize(prlp))
                SynchronizeGreeningImmediately();

            SaveLog();

            Student.Instance.SyncDataBackground(PrimaryKey);
            _errors.Clear();
            //BindDataToForm();
        }

        public bool IfNeededSynchronize(PermRecLogProcess prlp)
        {
            bool needed = false;
            string oldValue = "";
            string newValue = "";
            foreach (KeyValuePair<string, string> each in prlp.GetBeforeSaveText())
            {
                if (each.Key == "登入帳號") //登入帳號
                {
                    oldValue = each.Value;
                }
            }

            foreach (KeyValuePair<string, string> each in prlp.GetAfterSaveText())
            {
                if (each.Key == "登入帳號") //登入帳號
                {
                    newValue = each.Value;                  
                }
            }

            if(oldValue!=newValue)
                needed = true;
            return needed;
        }
        public async void SynchronizeGreeningImmediately()
        {
            string dsns = FISCA.Authentication.DSAServices.AccessPoint;
            string url = @"https://onecampus-task-yc3uirpz5a-de.a.run.app/greening/sync/" + dsns + "?delaySeconds=600&mode=immediately";

            try
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage rsp = await client.GetAsync(url);
                if (rsp.IsSuccessStatusCode)
                    Console.WriteLine("Greening帳號同步成功。");
                else
                    Console.WriteLine("Greening帳號同步失敗。");
            }
            catch
            {
                Console.WriteLine("Greening帳號同步失敗。");
            }
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
            _StudRec.IDNumber = txtSSN.Text.Trim();
            _StudRec.Name = txtName.Text.Trim();
            _StudRec.Nationality = cboNationality.Text;
            _StudRec.SALoginName = txtLoginID.Text.Trim();
            _StudRec.SAPassword = txtLoginPwd.Text;
            _StudRec.EMail = txtEmail.Text.Trim();
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

        //將畫面清空
        private void ClearFormValue()
        {
            txtEmail.Text=txtBirthDate.Text = txtBirthPlace.Text = txtEngName.Text = txtLoginID.Text = txtName.Text = txtSSN.Text = cboAccountType.Text = cboGender.Text = cboNationality.Text = string.Empty;           
        }
        
        private void BindDataToForm()
        {
            // 主要加當學生被刪除時檢查
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
            prlp.SetBeforeSaveText("姓名", txtName.Text);
            prlp.SetBeforeSaveText("身分證號", txtSSN.Text);
            prlp.SetBeforeSaveText("生日", txtBirthDate.Text);
            prlp.SetBeforeSaveText("性別", cboGender.Text);
            prlp.SetBeforeSaveText("國籍", cboNationality.Text);
            prlp.SetBeforeSaveText("出生地", txtBirthPlace.Text);
            prlp.SetBeforeSaveText("英文姓名", txtEngName.Text);
            prlp.SetBeforeSaveText("登入帳號", txtLoginID.Text);
            prlp.SetBeforeSaveText("帳號類型", cboAccountType.Text);
            prlp.SetBeforeSaveText("電子信箱", txtEmail.Text);
        }

        private void SetAfterEditLog()
        {
            prlp.SetAfterSaveText("姓名", txtName.Text.Trim());
            prlp.SetAfterSaveText("身分證號", txtSSN.Text.Trim());
            prlp.SetAfterSaveText("生日", txtBirthDate.Text);
            prlp.SetAfterSaveText("性別", cboGender.Text);
            prlp.SetAfterSaveText("國籍", cboNationality.Text);
            prlp.SetAfterSaveText("出生地", txtBirthPlace.Text);
            prlp.SetAfterSaveText("英文姓名", txtEngName.Text);
            prlp.SetAfterSaveText("登入帳號", txtLoginID.Text.Trim());
            prlp.SetAfterSaveText("帳號類型", cboAccountType.Text);
            prlp.SetAfterSaveText("電子信箱", txtEmail.Text.Trim());
            prlp.SetActionBy("學籍", "學生基本資料");
            prlp.SetAction("修改學生基本資料");
            prlp.SetDescTitle("姓名:"+_StudRec.Name+",學號:"+_StudRec.StudentNumber +",");
           // prlp.SaveLog("", "", "Student", PrimaryKey);    
        }

        private void SaveLog()
        {
            //prlp.SetActionBy("學籍", "學生基本資料");
            //prlp.SetAction("修改學生基本資料");
            //prlp.SetDescTitle("姓名:" + _StudRec.Name + ",學號:" + _StudRec.StudentNumber + ",");
            prlp.SaveLog("", "", "Student", PrimaryKey);
        }

        private void LoadDALDataToForm()
        {
            //2017/4/19 穎驊新增  尚未載入完畢，避免觸發cboNationality_TextChanged()
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
            //2017/4/19 穎驊新增 記錄原原國籍，作為與新輸入比較使用
            cboNationality_ori = _StudRec.Nationality;
            // 解析
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
            //載入國家列表
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


            


            //this.cboNationality.Items.Add("中華民國");
            //this.cboNationality.Items.Add("中華人民共合國");
            //this.cboNationality.Items.Add("孟加拉");
            //this.cboNationality.Items.Add("緬甸");
            //this.cboNationality.Items.Add("印尼");
            //this.cboNationality.Items.Add("日本");
            //this.cboNationality.Items.Add("韓國");
            //this.cboNationality.Items.Add("馬來西亞");
            //this.cboNationality.Items.Add("菲律賓");
            //this.cboNationality.Items.Add("新加坡");
            //this.cboNationality.Items.Add("泰國");
            //this.cboNationality.Items.Add("越南");
            //this.cboNationality.Items.Add("汶萊");
            //this.cboNationality.Items.Add("澳大利亞");
            //this.cboNationality.Items.Add("紐西蘭");
            //this.cboNationality.Items.Add("埃及");
            //this.cboNationality.Items.Add("南非");
            //this.cboNationality.Items.Add("法國");
            //this.cboNationality.Items.Add("義大利");
            //this.cboNationality.Items.Add("瑞典");
            //this.cboNationality.Items.Add("英國");
            //this.cboNationality.Items.Add("德國");
            //this.cboNationality.Items.Add("加拿大");
            //this.cboNationality.Items.Add("哥斯大黎加");
            //this.cboNationality.Items.Add("瓜地馬拉");
            //this.cboNationality.Items.Add("美國");
            //this.cboNationality.Items.Add("阿根廷");
            //this.cboNationality.Items.Add("巴西");
            //this.cboNationality.Items.Add("哥倫比亞");
            //this.cboNationality.Items.Add("巴拉圭");
            //this.cboNationality.Items.Add("烏拉圭");
            //this.cboNationality.Items.Add("其他");

            cboGender.Items.AddRange(new string[] { "男", "女" });
          



            _isInitialized = true;
        }


        private void buttonItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "所有影像(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
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
            od.Filter = "所有影像(*.jpg,*.jpeg,*.gif,*.png)|*.jpg;*.jpeg;*.gif;*.png;";
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

        //另存照片
        private void buttonItem2_Click(object sender, EventArgs e)
        {
            SavePicture(_FreshmanPhotoStr);
        }

        //另存照片
        private void buttonItem4_Click(object sender, EventArgs e)
        {
            SavePicture(_GraduatePhotoStr);
        }

        private void SavePicture(string imageString)
        {
            if (imageString == string.Empty)
                return;

            SaveFileDialog sd = new SaveFileDialog();
            sd.Filter = "PNG 影像|*.png;";
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
                _errors.SetError(txtBirthDate, "請輸入 yyyy/mm/dd 符合日期格式文字");
        }

        private void txtSSN_Validating(object sender, CancelEventArgs e)
        {
            ValidateIDNumber();
        }

        private void txtLoginID_Validating(object sender, CancelEventArgs e)
        {
            ValidateLoginID();
        }

        // 檢查
        private void ValidateIDNumber()
        {
            _errors.SetError(txtSSN, string.Empty);

            if (string.IsNullOrEmpty(txtSSN.Text.Trim()))
            {
                _errors.SetError(txtSSN, string.Empty);
                return;
            }

            if (QueryStudent.IDNumberExists(PrimaryKey, txtSSN.Text.Trim()))
                _errors.SetError(txtSSN, "身分證號重覆，請確認資料。");           
                
        }

        private void ValidateLoginID()
        {
            _errors.SetError(txtLoginID, string.Empty);

            if (string.IsNullOrEmpty(txtLoginID.Text.Trim()))
            {
                _errors.SetError(txtLoginID, string.Empty);
                return;
            }

            if (QueryStudent.LoginIDExists(txtLoginID.Text.Trim(), PrimaryKey))
                _errors.SetError(txtLoginID, "帳號重覆，請重新選擇。");
        }

        #region 清除照片
        //清除新生照片
        private void buttonItem5_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您確定要清除此學生的照片嗎？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

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

        //清除畢業照片
        private void buttonItem6_Click(object sender, EventArgs e)
        {
            if (FISCA.Presentation.Controls.MsgBox.Show("您確定要清除此學生的照片嗎？", "", MessageBoxButtons.YesNo) == DialogResult.No) return;

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
            //    errorProvider1.SetError(cboNationality, "此國籍名稱，不存在於教務作業>對照/代碼>國籍中英文對照表 的設定中，建議檢察。");                        
            //}

        }

        //2017/4/19 穎驊新增 監聽 國籍欄位 內容改變事件
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

                    errorProvider1.SetError(cboNationality, "此國籍名稱，不存在於教務作業>對照/代碼>國籍中英文對照表 的設定中，建議檢察。");
                }                        
            }            
        }

        private void txtEmail_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
