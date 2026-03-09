using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using FISCA.Authentication;
using FISCA.DSAUtil;
using FISCA.Presentation;
using Framework;
using Framework.Security;
using JHSchool.StudentExtendControls.Ribbon;
using System.Data;
//using Permissions = JHSchool.StudentExtendControls.Permissions;

namespace JHSchool
{
    public class Student : LegacyPresentBase<StudentRecord>
    {
        protected override Dictionary<string, StudentRecord> GetAllData()
        {
            Dictionary<string, StudentRecord> items = new Dictionary<string, StudentRecord>();
            foreach (var item in Feature.QueryStudent.GetAllStudents())
            {
                items.Add(item.ID, item);
            }
            return items;
        }
        protected override Dictionary<string, StudentRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, StudentRecord> items = new Dictionary<string, StudentRecord>();
            foreach (var item in Feature.QueryStudent.GetStudents(primaryKeys))
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        public new void AddDetailBulider(IDetailBulider item)
        {
            DetailContent content = item.GetContent();
            if (content == null) return;

            if (Attribute.IsDefined(content.GetType(), typeof(FeatureCodeAttribute)))
            {
                FeatureCodeAttribute fca = Attribute.GetCustomAttribute(content.GetType(), typeof(FeatureCodeAttribute)) as FeatureCodeAttribute;
                if (fca != null)
                {
                    if (Framework.Legacy.GlobalOld.Acl[content.GetType()].Viewable)
                        base.AddDetailBulider(item);
                }
            }
            else if (Attribute.IsDefined(content.GetType(), typeof(FISCA.Permission.FeatureCodeAttribute)))
                {
                    FISCA.Permission.FeatureCodeAttribute fca = Attribute.GetCustomAttribute(content.GetType(), typeof(FISCA.Permission.FeatureCodeAttribute)) as FISCA.Permission.FeatureCodeAttribute;
                    if (fca != null)
                    {
                        if (FISCA.Permission.UserAcl.Current[fca.Code].Viewable)
                            base.AddDetailBulider(item);
                    }
                }
            else base.AddDetailBulider(item);
        }

        private static Student _Instance = null;
        public static Student Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Student(K12.Presentation.NLDPanels.Student);
                return _Instance;
            }
        }

        private bool _Initilized = false;
        //private bool _UseDefaultContent = true;
        //private List<StudentControls.DetailContent> _DetialContents = new List<K12.StudentControls.DetailContent>();

        private Dictionary<string, List<StudentRecord>> _ClassStudents = new Dictionary<string, List<StudentRecord>>();
        // 反向索引：studentID → classID，加速 ItemUpdated 查詢
        private Dictionary<string, string> _StudentClassMap = new Dictionary<string, string>();

        private Student(NLDPanel present)
            : base(present)
        {
            this.ItemLoaded += delegate
            {
                lock (_ClassStudents)
                {
                    _ClassStudents.Clear();
                    _StudentClassMap.Clear();
                    foreach (var item in this.Items)
                    {
                        if (item.Status == "一般" || item.Status == "輟學")
                        {
                            if (!_ClassStudents.ContainsKey(item.RefClassID))
                                _ClassStudents.Add(item.RefClassID, new List<StudentRecord>());
                            _ClassStudents[item.RefClassID].Add(item);
                            _StudentClassMap[item.ID] = item.RefClassID;
                        }
                    }
                }
            };
            this.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                lock (_ClassStudents)
                {
                    // 使用反向索引快速移除已更新的學生（O(1) per student）
                    foreach (var key in e.PrimaryKeys)
                    {
                        if (_StudentClassMap.TryGetValue(key, out string oldClassId))
                        {
                            if (_ClassStudents.ContainsKey(oldClassId))
                            {
                                _ClassStudents[oldClassId].RemoveAll(s => s.ID == key);
                            }
                            _StudentClassMap.Remove(key);
                        }
                    }
                    // 重新加入已更新的學生
                    foreach (var key in e.PrimaryKeys)
                    {
                        var item = Items[key];
                        if (item != null && (item.Status == "一般" || item.Status == "輟學"))
                        {
                            if (!_ClassStudents.ContainsKey(item.RefClassID))
                                _ClassStudents.Add(item.RefClassID, new List<StudentRecord>());
                            _ClassStudents[item.RefClassID].Add(item);
                            _StudentClassMap[item.ID] = item.RefClassID;
                        }
                    }
                }
            };
        }

        public List<StudentRecord> GetClassStudents(ClassRecord classRec)
        {
            lock (_ClassStudents)
            {
                if (_ClassStudents.ContainsKey(classRec.ID))
                {
                    return new List<StudentRecord>(_ClassStudents[classRec.ID]);
                }
                else
                    return new List<StudentRecord>();
            }
        }

        /// <summary>
        /// 依據學生編號取得學生資料物件。
        /// </summary>
        /// <param name="primaryKeys">要取得資料的學生編號。</param>
        /// <returns></returns>
        public List<StudentRecord> GetStudents(params string[] primaryKeys)
        {
            List<StudentRecord> list = new List<StudentRecord>();
            foreach (string each in primaryKeys)
            {
                StudentRecord record = this[each];
                if (record == null)
                    throw new ArgumentException(string.Format("指定的學生編號不存在：「{0}」", each), "primaryKeys");
                else
                    list.Add(record);
            }

            return list;
        }

        /// <summary>
        /// 新增學生到待處理。
        /// </summary>
        /// <param name="primaryKeys">學生編號清單。</param>
        public void AddToTemporal(List<string> primaryKeys)
        {
            AddToTemp(primaryKeys);
        }

        /// <summary>
        /// 將學生移出待處理。
        /// </summary>
        /// <param name="primaryKeys">學生編號清單。</param>
        public void RemoveFromTemporal(List<string> primaryKeys)
        {
            RemoveFromTemp(primaryKeys);
        }

        public List<string> SelectedKeys
        {
            get
            {
                return SelectedList.AsKeyList();
            }
        }

        public List<string> TemporalKeys
        {
            get
            {
                return TemporaList.AsKeyList();
            }
        }


        List<string> GraduateList;
        Dictionary<string, List<string>> GraduateLeaveInfoU;
        Dictionary<string, List<string>> GraduateLeaveInfoD;
        Dictionary<string, XmlElement> GraduateLeaveInfoAll;

        public void SetupPresentation()
        {
            //CourseSyncAllBackground
            //2013/4/22 - 提供轉學模組更新學生清單
            FISCA.Features.Register("StudentSyncAllBackground", x =>
            {
                this.SyncAllBackground();
            });

            Student.Instance.RibbonBarItems["編輯"].Index = 0;
            Student.Instance.RibbonBarItems["資料統計"].Index = 1;
            Student.Instance.RibbonBarItems["指定"].Index = 2;
            Student.Instance.RibbonBarItems["教務"].Index = 3;
            Student.Instance.RibbonBarItems["學務"].Index = 4;

            #region RibbonBar 學生/編輯
            RibbonBarItem rbItem = Student.Instance.RibbonBarItems["編輯"];
            rbItem["新增"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["新增"].Image = StudentExtendControls.Ribbon.Resources.btnAddStudent_Image;
            rbItem["新增"].Enable = User.Acl["JHSchool.Student.Ribbon0000"].Executable;
            rbItem["新增"].Click += delegate
            {
                IRewriteAPI_JH.IStudentAddStudentAPI item = FISCA.InteractionService.DiscoverAPI<IRewriteAPI_JH.IStudentAddStudentAPI>();
                if (item != null)
                {
                    item.CreateForm().ShowDialog();
                }
                else
                {
                    new JHSchool.StudentExtendControls.Ribbon.AddStudent().ShowDialog();
                }
            };

            //本權限控制來自 - MOD_Tagging (Dylan - 20190828 補功能)
            if (User.Acl["JHSchool.Student.Ribbon04150.Change20130315"].Executable)
            {
                ChangeStatusBatch.Init();//右鍵變更學生狀態
            }

            rbItem["刪除"].Image = StudentExtendControls.Ribbon.Resources.btnDeleteStudent_Image;
            rbItem["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["刪除"].Enable = User.Acl["JHSchool.Student.Ribbon0010"].Executable;
            rbItem["刪除"].Click += delegate
            {
                if (SelectedList.Count == 1)
                {
                    // 2017/11/13 穎驊依據 [06-01][03] 修改學生狀態沒有上傳局端 項目，使用IRewriteAPI_JH 方法， 高雄區 將會 走另一條能夠呈報局端的Form
                    IRewriteAPI_JH.IStudentDeleteStudentAPI item = FISCA.InteractionService.DiscoverAPI<IRewriteAPI_JH.IStudentDeleteStudentAPI>();
                    if (item != null)
                    {
                        item.CreateForm().ShowDialog();
                    }
                    else
                    {
                        PermRecLogProcess prlp = new PermRecLogProcess();
                        JHSchool.Data.JHStudentRecord studRec = JHSchool.Data.JHStudent.SelectByID(SelectedList[0].ID);
                        string msg = string.Format("確定要刪除「{0}」？", studRec.Name);
                        if (FISCA.Presentation.Controls.MsgBox.Show(msg, "刪除學生", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // 檢查刪除狀態是否有同學號或身分證號,空白可刪（使用記憶體快取取代 DB 查詢）
                            HashSet<string> tmpSnumSet = new HashSet<string>();
                            HashSet<string> tmpStudIDNumberSet = new HashSet<string>();
                            foreach (var checkStudRec in Student.Instance.Items)
                                if (checkStudRec.Status == "刪除")
                                {
                                    if (!string.IsNullOrEmpty(checkStudRec.StudentNumber))
                                        tmpSnumSet.Add(checkStudRec.StudentNumber);
                                    if (!string.IsNullOrEmpty(checkStudRec.IDNumber))
                                        tmpStudIDNumberSet.Add(checkStudRec.IDNumber);
                                }

                            if (tmpSnumSet.Contains(studRec.StudentNumber) || tmpStudIDNumberSet.Contains(studRec.IDNumber))
                            {
                                MsgBox.Show("刪除狀態有重複學號或身分證號,請先修改後再刪除!");
                                return;
                            }

                            // 修改學生狀態 delete
                            studRec.Status = K12.Data.StudentRecord.StudentStatus.刪除;
                            JHSchool.Data.JHStudent.Update(studRec);
                            Student.Instance.SyncDataBackground(studRec.ID);
                            prlp.SaveLog("學籍學生", "刪除學生", "刪除學生，姓名:" + studRec.Name + ",學號:" + studRec.StudentNumber);
                        }
                        else
                            return;
                    }                 
                }
            };

            #endregion

            #region RibbonBar 學生/匯入匯出
            rbItem = Student.Instance.RibbonBarItems["資料統計"];

            rbItem["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["匯出"].Image = Properties.Resources.Export_Image;
            rbItem["匯出"]["學籍相關匯出"]["匯出學生基本資料"].Enable = User.Acl["JHSchool.Student.Ribbon0030"].Executable;
            rbItem["匯出"]["學籍相關匯出"]["匯出學生基本資料"].Click += delegate
            {
                //IRewriteAPI_JH.IStudentExportWizardAPI item = FISCA.InteractionService.DiscoverAPI<IRewriteAPI_JH.IStudentExportWizardAPI>();
                //if (item != null)
                //{
                    //item.CreateForm().ShowDialog();
                //}
                //else
                //{
                    new StudentExportWizard().ShowDialog();
                //}
            };

            rbItem["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["匯入"].Image = Properties.Resources.Import_Image;
            rbItem["匯入"]["學籍相關匯入"]["匯入學生基本資料"].Enable = User.Acl["JHSchool.Student.Ribbon0020"].Executable;
            rbItem["匯入"]["學籍相關匯入"]["匯入學生基本資料"].Click += delegate
            {
                IRewriteAPI_JH.IStudentImportWizardAPI item = FISCA.InteractionService.DiscoverAPI<IRewriteAPI_JH.IStudentImportWizardAPI>();
                if (item != null)
                {
                    item.CreateForm().ShowDialog();
                }
                else
                {
                    new StudentImportWizard().ShowDialog();
                }
            
            };
            #endregion

            //報表,是以常態呈現
            rbItem["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["報表"].Image = Properties.Resources.paste_64;
            rbItem["報表"].SupposeHasChildern = true;

            #region RibbonBar 學生/指定
            //由類別模組提供。
            //rbItem = Student.Instance.RibbonBarItems["指定"];
            //rbItem["類別"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //rbItem["類別"].Image = InternalExtendControls.Tagging.Resources.ctxTag_Image;
            //rbItem["類別"].SupposeHasChildern = true;
            //rbItem["類別"].PopupOpen += new EventHandler<PopupOpenEventArgs>(
            //    new TaggingMenu("JHSchool.Student.Ribbon0040", "JHSchool.Student.Ribbon0050").MenuOpen);
            #endregion


            #region List Panel Fields

            ListPaneField classNameField = new ListPaneField("班級");
            classNameField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                {
                    var classRec = Student.Instance.Items[e.Key].Class;
                    e.Value = (classRec == null ? "" : classRec.Name);
                }
            };
            Student.Instance.AddListPaneField(classNameField);

            ListPaneField seatNoField = new ListPaneField("座號");
            seatNoField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                    e.Value = Student.Instance.Items[e.Key].SeatNo;
            };

            seatNoField.CompareValue += delegate(object sender, CompareValueEventArgs e)
            {
                int x, y;

                if (!int.TryParse(e.Value1.ToString(), out x))
                    x = int.MaxValue;

                if (!int.TryParse(e.Value2.ToString(), out y))
                    y = int.MaxValue;

                e.Result = x.CompareTo(y);
            };

            Student.Instance.AddListPaneField(seatNoField);

            ListPaneField nameField = new ListPaneField("姓名");
            nameField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                    e.Value = Items[e.Key].Name;
            };
            Student.Instance.AddListPaneField(nameField);

            ListPaneField genderField = new ListPaneField("性別");
            genderField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                    e.Value = Student.Instance.Items[e.Key].Gender;
            };
            Student.Instance.AddListPaneField(genderField);

            ListPaneField studnumberField = new ListPaneField("學號");
            studnumberField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                    e.Value = Student.Instance.Items[e.Key].StudentNumber;
            };
            Student.Instance.AddListPaneField(studnumberField);

            ListPaneField birthday = new ListPaneField("生日");
            birthday.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                    e.Value = Student.Instance.Items[e.Key].Birthday;
            };
            Student.Instance.AddListPaneField(birthday);

            ListPaneField IDNumber = new ListPaneField("身分證號");
            IDNumber.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Student.Instance.Items[e.Key] != null)
                    e.Value = Student.Instance.Items[e.Key].IDNumber;
            };
            if (User.Acl["Student.Field.身分證號"].Executable)
                Student.Instance.AddListPaneField(IDNumber);

            #endregion

            #region Student Views
            Student.Instance.AddView(new JHSchool.StudentExtendControls.GradeYear_Class_View());
            //Student.Instance.AddView(new JHSchool.StudentExtendControls.CategoryView());
            #endregion

            #region 學生基本資料(20140429)

            IRewriteAPI_JH.IStudentDetailItemAPI itemB = FISCA.InteractionService.DiscoverAPI<IRewriteAPI_JH.IStudentDetailItemAPI>();
            if (itemB != null)
            {
                Student.Instance.AddDetailBulider(itemB.CreateBasicInfo());
            }
            else
            {
                Student.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<JHSchool.StudentExtendControls.BaseInfoPalmerwormItem>());
            }
            #endregion

            //            Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.BaseInfoPalmerwormItem>());
            //            Student.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<JHSchool.StudentExtendControls.SemesterHistoryDetail>());
            //Student.Instance.AddDetailBulider(new DetailBulider<JHSchool.StudentExtendControls.ClassItem>());
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.ClassInfoPalmerwormItem>());
            //文字評量(jenyu)
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.WordCommentPalmerworm>());


            // 電子報表(因相關功能未完成先註)
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.ElectronicPaperPalmerworm>());

            #region Search Conditions
            ConfigData cd = User.Configuration["StudentSearchOptionPreference"];

            SearchName = SearchConditionMenu["姓名"];
            SearchName.AutoCheckOnClick = true;
            SearchName.AutoCollapseOnClick = false;
            SearchName.Checked = cd.GetBoolean("SearchName", true);
            SearchName.Click += delegate
            {
                cd.SetBoolean("SearchName", SearchName.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            // 因用班級 Search 未用到先註 2009/9/30
            //SearchClass = SearchConditionMenu["班級"];
            //SearchClass.AutoCheckOnClick = true;
            //SearchClass.AutoCollapseOnClick = false;
            //SearchClass.Checked = cd.GetBoolean("SearchClass", true);
            //SearchClass.Click += delegate
            //{
            //    cd.SetBoolean("SearchClass", SearchClass.Checked);

            //    BackgroundWorker async = new BackgroundWorker();
            //    async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
            //    async.RunWorkerAsync(cd);
            //};

            SearchStudentNumber = SearchConditionMenu["學號"];
            SearchStudentNumber.AutoCheckOnClick = true;
            SearchStudentNumber.AutoCollapseOnClick = false;
            SearchStudentNumber.Checked = cd.GetBoolean("SearchStudentNumber", true);
            SearchStudentNumber.Click += delegate
            {
                cd.SetBoolean("SearchStudentNumber", SearchStudentNumber.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchStudentIDNumber = SearchConditionMenu["身分證號"];
            SearchStudentIDNumber.AutoCheckOnClick = true;
            SearchStudentIDNumber.AutoCollapseOnClick = false;
            SearchStudentIDNumber.Checked = cd.GetBoolean("SearchStudentIDNumber", true);
            SearchStudentIDNumber.Click += delegate
            {
                cd.SetBoolean("SearchStudentIDNumber", SearchStudentIDNumber.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchStudentParent = SearchConditionMenu["父母監護人"];
            SearchStudentParent.AutoCheckOnClick = true;
            SearchStudentParent.AutoCollapseOnClick = false;
            SearchStudentParent.Checked = cd.GetBoolean("SearchStudentParent", false);
            SearchStudentParent.Click += delegate
            {
                cd.SetBoolean("SearchStudentParent", SearchStudentParent.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchStudentLoginID = SearchConditionMenu["登入帳號"];
            SearchStudentLoginID.AutoCheckOnClick = true;
            SearchStudentLoginID.AutoCollapseOnClick = false;
            SearchStudentLoginID.Checked = cd.GetBoolean("SearchStudentLoginID", false);
            SearchStudentLoginID.Click += delegate
            {
                cd.SetBoolean("SearchStudentLoginID", SearchStudentLoginID.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchEnglishName = SearchConditionMenu["英文姓名"];
            SearchEnglishName.AutoCheckOnClick = true;
            SearchEnglishName.AutoCollapseOnClick = false;
            SearchEnglishName.Checked = cd.GetBoolean("SearchEnglishName", false);
            SearchEnglishName.Click += delegate
            {
                cd.SetBoolean("SearchEnglishName", SearchEnglishName.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            Present.Search += new EventHandler<SearchEventArgs>(Student_Search);
            #endregion

            //缺曠記錄(dylan)
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.AbsencePalmerwormItem>());
            //懲戒資料(dylan)
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.DemeritPalmerwormItem>());
            //獎勵資料(dylan)
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<StudentExtendControls.MeritPalmerwormItem>());
            //測試的先註解了(dylan)
            //Student.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<CriticalSection.Dylan_Test_Item>());

            Present.NavPaneContexMenu.GetChild("重新整理").Click += delegate { this.SyncAllBackground(); };

            Present.Picture = Properties.Resources.StudentIcon;

            //由類別模組提供
            //Present.SetDescriptionPaneBulider(new DescriptionPaneBulider<StudentExtendControls.StudentDescription>());

            //Student.Instance.RequiredDescription += delegate(object sender, RequiredDescriptionEventArgs e)
            //{
            //    if (string.IsNullOrEmpty(e.PrimaryKey)) return;

            //    var studentRec = Student.Instance.Items[e.PrimaryKey];
            //    e.Result = studentRec.GetDescription();
            //};

            //var defaultContentBulider = new DetailBulider<StudentControls.DetailContent>();
            //defaultContentBulider.ContentBulided += delegate(object sender, ContentBulidedEventArgs<K12.StudentControls.DetailContent> e)
            //{
            //    _DetialContents.Add(e.Content);
            //    e.Content.Visible = UseDefaultDetailContent;
            //    e.Content.Disposed += delegate(object sender2, EventArgs e2)
            //    {
            //        _DetialContents.Remove((StudentControls.DetailContent)sender2);
            //    };
            //};
            //this.AddDetailBulider(defaultContentBulider);

            #region 權限管理
            Catalog ribbon = RoleAclSource.Instance["學生"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0000", "新增學生"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0010", "刪除學生"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0020", "匯入學籍資料"));
            ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0030", "匯出學籍資料"));
            //ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0040", "指定學生類別"));
            //ribbon.Add(new RibbonFeature("JHSchool.Student.Ribbon0050", "管理學生類別清單"));

            Catalog detail = RoleAclSource.Instance["學生"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(StudentExtendControls.BaseInfoPalmerwormItem)));
            //detail.Add(new DetailItemFeature(typeof(JHSchool.StudentExtendControls.SemesterHistoryDetail)));
            //電子報表(因相關功能未完成先註)
            //detail.Add(new DetailItemFeature(typeof(StudentExtendControls.ElectronicPaperPalmerworm)));

            //Student.Field.身分證號
            ribbon = RoleAclSource.Instance["學生"]["清單欄位"];
            ribbon.Add(new RibbonFeature("Student.Field.身分證號", "身分證號"));
            #endregion

            //#region 待刪除畢業及離校
            //GraduateList = new List<string>();
            //GraduateLeaveInfoU = getAllStudGraduateIDs("畢業", "學年度畢業");
            //GraduateLeaveInfoD = getAllStudGraduateIDs("修業", "學年度修業");
            //GraduateLeaveInfoAll = getAllStudGraduateIDByStatus("畢業或離校");

            //foreach (string str in GraduateLeaveInfoU.Keys)
            //    if (!GraduateList.Contains(str))
            //        GraduateList.Add(str);

            //foreach (string str in GraduateLeaveInfoD.Keys)
            //    if (!GraduateList.Contains(str))
            //        GraduateList.Add(str);

            //GraduateList.Add("離校(非畢修業)");

            //GraduateList.Sort();
            //#endregion

            foreach (string each in AllStatus)
                CreateFilterItem(each);


            //#region 待刪除畢業及離校

            //JHSchool.Data.JHStudent.AfterUpdate += delegate
            //{
            //    GraduateLeaveInfoAll = null;
            //    GraduateLeaveInfoD = null;
            //    GraduateLeaveInfoU = null;
            //    GraduateList = null;
            //    GraduateList = new List<string>();
            //    GraduateLeaveInfoU = getAllStudGraduateIDs("畢業", "學年度畢業");
            //    GraduateLeaveInfoD = getAllStudGraduateIDs("修業", "學年度修業");
            //    GraduateLeaveInfoAll = getAllStudGraduateIDByStatus("畢業或離校");
            //    foreach (string str in GraduateLeaveInfoU.Keys)
            //        if (!GraduateList.Contains(str))
            //            GraduateList.Add(str);

            //    foreach (string str in GraduateLeaveInfoD.Keys)
            //        if (!GraduateList.Contains(str))
            //            GraduateList.Add(str);

            //    GraduateList.Add("離校(非畢修業)");

            //    GraduateList.Sort();

            //    FillFilter();
            //};
            //#endregion

            bool havedef = false;
            foreach (string each in AllStatus)
                havedef |= FilterMenu[each].Checked;

            if (!havedef)
                FilterMenu["一般"].Checked = true;

            UseFilter = true;

            MotherForm.AddPanel(K12.Presentation.NLDPanels.Student);
            _Initilized = true;
            FillFilter();
        }

        ConfigData preference = User.Configuration["DefaultFiltersPreference"];

        private void CreateFilterItem(string name)
        {
            //ConfigData preference = User.Configuration["DefaultFiltersPreference"];

            FilterMenu[name].AutoCheckOnClick = true;
            FilterMenu[name].AutoCollapseOnClick = false;
            FilterMenu[name].Checked = preference.GetBoolean(name, false);
            FilterMenu[name].Click += delegate
            {
                FillFilter();
                preference.SetBoolean(name, FilterMenu[name].Checked);

                //#region 待刪除畢業及離校
                //if (name == "畢業或離校")
                //{
                //    if (FilterMenu[name].Checked)
                //    {
                //        preference.SetBoolean(name, false);
                //        foreach (MenuButton item in FilterMenu[name].Items)
                //        {
                //            preference.SetBoolean(item.Text, false);
                //        }
                //    }
                //    else
                //    {
                //        preference.SetBoolean(name, true);
                //        foreach (MenuButton item in FilterMenu[name].Items)
                //        {
                //            preference.SetBoolean(item.Text, true);
                //        }
                //    }
                //}
                //#endregion
                preference.SaveAsync();
            };



            //#region 待刪除畢業及離校
            //// 新加入畢業或離校 內 畢業及修業
            //if (name == "畢業或離校")
            //{


            //    //string[] SubName1s = new string[] { "畢業", "修業" ,"離校(非畢修業)"};
            //    if (GraduateList.Count > 0)
            //        foreach (string subname1 in GraduateList)
            //        {
            //            MenuButton mb = FilterMenu[name][subname1];
            //            mb.AutoCheckOnClick = true;
            //            mb.AutoCollapseOnClick = false;
            //            mb.Checked = preference.GetBoolean(subname1, false);
            //            mb.Click += delegate
            //            {
            //                FillFilter();
            //                preference.SetBoolean(mb.Text, mb.Checked);
            //                preference.SaveAsync();
            //            };
            //        }
            //    preference.SetBoolean(name, FilterMenu[name].Checked);
            //    preference.SaveAsync();
            //}
            //#endregion
        }

        private MenuButton SearchStudentNumber, SearchName, SearchStudentIDNumber, SearchStudentParent;
        private MenuButton SearchStudentLoginID, SearchEnglishName;

        private SearchEventArgs SearEvArgs = null;
        //private List<SearchParentEntity> StudParentList = new List<SearchParentEntity>();
        private void Student_Search(object sender, SearchEventArgs e)
        {
            SearEvArgs = e;
            Campus.Windows.BlockMessage.Display("資料搜尋中,請稍候....", new Campus.Windows.ProcessInvoker(ProcessSearch));

        }

        private void ProcessSearch(Campus.Windows.MessageArgs args)
        {
            try
            {
                string condition = SearEvArgs.Condition.Replace("'", "''").ToLower();
                string likeCondition = "%" + condition + "%";
                List<string> conditions = new List<string>();

                if (SearchName.Checked)
                    conditions.Add(string.Format("lower(name) like '{0}'", likeCondition));
                if (SearchStudentNumber.Checked)
                    conditions.Add(string.Format("lower(student_number) like '{0}'", likeCondition));
                if (SearchStudentIDNumber.Checked)
                    conditions.Add(string.Format("lower(id_number) like '{0}'", likeCondition));
                if (SearchStudentLoginID.Checked)
                    conditions.Add(string.Format("lower(sa_login_name) like '{0}'", likeCondition));
                if (SearchEnglishName.Checked)
                    conditions.Add(string.Format("lower(english_name) like '{0}'", likeCondition));
                if (SearchStudentParent.Checked)
                {
                    conditions.Add(string.Format("lower(father_name) like '{0}'", likeCondition));
                    conditions.Add(string.Format("lower(mother_name) like '{0}'", likeCondition));
                    conditions.Add(string.Format("lower(custodian_name) like '{0}'", likeCondition));
                }

                List<string> results = new List<string>();
                if (conditions.Count > 0)
                {
                    string sql = "select id from student where " + string.Join(" or ", conditions.ToArray());
                    FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
                    System.Data.DataTable dr = _queryHelper.Select(sql);
                    foreach (System.Data.DataRow row in dr.Rows)
                    {
                        string id = "" + row[0];
                        if (!results.Contains(id))
                            results.Add(id);
                    }
                }

                FISCA.Presentation.MotherForm.SetStatusBarMessage("共搜尋到：" + results.Count + "名學生");

                SearEvArgs.Result.AddRange(results);
            }
            catch (Exception) { }

        }

        //public bool UseDefaultDetailContent
        //{
        //    get { return _UseDefaultContent; }
        //    set
        //    {
        //        _UseDefaultContent = value;
        //        foreach ( var item in _DetialContents )
        //        {
        //            item.Visible = value;
        //        }
        //    }
        //}



        protected override void FillFilter()
        {
            //畫面沒有設定完成就什麼都不做
            if (!_Initilized || !Loaded) return;

            List<string> filters = new List<string>();

            foreach (string each in AllStatus)
            {
                if (FilterMenu[each].Checked)
                {
                    filters.Add(each);
                }
            }

            HashSet<string> primaryKeysSet = new HashSet<string>();
            foreach (var item in Items.GetStatusStudents(filters.ToArray()))
            {
                primaryKeysSet.Add(item.ID);
            }

            Present.SetFilteredSource(primaryKeysSet.ToList());
        }


        //#region 待刪除畢業及離校
        ////主要是傳入畢業資格(畢業或修業)呼叫 service，回傳學生狀態是畢業或離的校離校學年與 StudentID List.
        //private Dictionary<string, List<string>> getAllStudGraduateIDs(string ReasonName, string returnAddName)
        //{
        //    Dictionary<string, List<string>> data = new Dictionary<string, List<string>>();
        //    DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
        //    helper.AddElement("Field");
        //    helper.AddElement("Field", "ID");
        //    helper.AddElement("Field", "LeaveInfo");
        //    helper.AddElement("Condition");
        //    // 只取學生狀態是畢業或離校
        //    helper.AddElement("Condition", "Status", "畢業或離校");
        //    string nullName = "無" + returnAddName;

        //    DSXmlHelper rsp = DSAServices.CallService("SmartSchool.Student.GetDetailList", new DSRequest(helper.BaseElement)).GetContent();
        //    foreach (XmlElement xm in rsp.GetElements("Student"))
        //    {
        //        XmlElement element = xm.SelectSingleNode("LeaveInfo/LeaveInfo") as XmlElement;
        //        if (element != null)
        //        {
        //            if (element.GetAttribute("Reason") == ReasonName)
        //            {
        //                string SchoolYear = element.GetAttribute("SchoolYear");
        //                string ID = xm.GetAttribute("ID");
        //                // 是否有學年度
        //                if (string.IsNullOrEmpty(SchoolYear))
        //                {
        //                    if (data.ContainsKey(nullName))
        //                        data[nullName].Add(ID);
        //                    else
        //                    {
        //                        // 加一個空的
        //                        List<string> strS = new List<string>();
        //                        strS.Add(ID);
        //                        data.Add(nullName, strS);
        //                    }

        //                }
        //                else
        //                {
        //                    SchoolYear += returnAddName;
        //                    if (data.ContainsKey(SchoolYear))
        //                    {
        //                        data[SchoolYear].Add(ID);
        //                    }
        //                    else
        //                    {
        //                        List<string> strS = new List<string>();
        //                        strS.Add(ID);
        //                        data.Add(SchoolYear, strS);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return data;
        //}
        //#endregion


        //#region 待刪除畢業及離校
        //// 取得特定狀態學生身上畢業資訊
        //private Dictionary<string, XmlElement> getAllStudGraduateIDByStatus(string Status)
        //{
        //    Dictionary<string, XmlElement> data = new Dictionary<string, XmlElement>();
        //    DSXmlHelper helper = new DSXmlHelper("GetStudentListRequest");
        //    helper.AddElement("Field");
        //    helper.AddElement("Field", "ID");
        //    helper.AddElement("Field", "LeaveInfo");
        //    helper.AddElement("Condition");
        //    // 只取學生狀態是畢業或離校
        //    helper.AddElement("Condition", "Status", Status);


        //    DSXmlHelper rsp = DSAServices.CallService("SmartSchool.Student.GetDetailList", new DSRequest(helper.BaseElement)).GetContent();
        //    foreach (XmlElement xm in rsp.GetElements("Student"))
        //    {
        //        XmlElement element = xm.SelectSingleNode("LeaveInfo/LeaveInfo") as XmlElement;
        //        data.Add(xm.GetAttribute("ID"), element);
        //    }
        //    return data;
        //}
        //#region

        private string[] AllStatus
        {
            get { return new string[] { "一般", "休學", "輟學", "畢業或離校", "刪除" }; }
        }

    }
    public static class StudentExtend
    {
        /// <summary>
        /// 取得指定狀態的學生
        /// </summary>
        /// <param name="students"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static List<StudentRecord> GetStatusStudents(this IEnumerable<StudentRecord> students, params string[] status)
        {
            HashSet<string> statusSet = new HashSet<string>(status);
            List<StudentRecord> result = new List<StudentRecord>();
            foreach (var item in students)
            {
                if (statusSet.Contains(item.Status))
                    result.Add(item);
            }
            return result;
        }
        /// <summary>
        /// 取得在校學生(狀態為一般 延修 輟學)
        /// </summary>
        /// <param name="students"></param>
        /// <returns></returns>
        public static List<StudentRecord> GetInSchoolStudents(this IEnumerable<StudentRecord> students)
        {
            return students.GetStatusStudents("一般", "延修", "輟學");
        }
        /// <summary>
        /// 取得學生的簡短描述
        /// </summary>
        /// <param name="student">學生</param>
        /// <returns>簡述</returns>
        public static string GetDescription(this StudentRecord student)
        {
            return (student.Class == null ? "未分班級" : (student.Class.Name + (student.SeatNo == "" ? "" : "(" + student.SeatNo + "號)"))) + " " + student.Name;
        }

        /// <summary>
        /// 讀取學生的系統編號轉換成 List。
        /// </summary>
        public static List<string> AsKeyList(this IEnumerable<StudentRecord> students)
        {
            List<string> keys = new List<string>();
            foreach (StudentRecord each in students)
                keys.Add(each.ID);

            return keys;
        }
    }
}
