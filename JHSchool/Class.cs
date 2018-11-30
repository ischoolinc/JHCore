using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using JHSchool;
using FISCA.Presentation;
using Framework;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using JHSchool.Editor;
using JHSchool.ClassExtendControls.Ribbon;
using Framework.Security;

namespace JHSchool
{
    public class Class : LegacyPresentBase<ClassRecord>
    {
        private static Class _Instance = null;
        public static Class Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Class(K12.Presentation.NLDPanels.Class);
                return _Instance;
            }
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
            else base.AddDetailBulider(item);
        }


        public void SetupPresentation()
        {
            Class.Instance.RibbonBarItems["編輯"].Index = 0;
            Class.Instance.RibbonBarItems["資料統計"].Index = 1;
            Class.Instance.RibbonBarItems["指定"].Index = 2;
            Class.Instance.RibbonBarItems["教務"].Index = 3;
            Class.Instance.RibbonBarItems["學務"].Index = 4;

            #region RibbonBar 班級/編輯
            RibbonBarItem rbItem = Class.Instance.RibbonBarItems["編輯"];
            rbItem["新增"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["新增"].Enable = User.Acl["JHSchool.Class.Ribbon0000"].Executable;
            rbItem["新增"].Image = ClassExtendControls.Ribbon.Resources.btnAddClass;
            rbItem["新增"].Click += delegate
            {
                new JHSchool.ClassExtendControls.Ribbon.AddClass().ShowDialog();
            };

            rbItem["刪除"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["刪除"].Image = ClassExtendControls.Ribbon.Resources.btnDeleteClass;
            rbItem["刪除"].Enable = User.Acl["JHSchool.Class.Ribbon0010"].Executable;
            rbItem["刪除"].Click += delegate
            {
                if (SelectedList.Count == 1)
                {
                    JHSchool.Data.JHClassRecord record = JHSchool.Data.JHClass.SelectByID(SelectedList[0].ID);
                    string msg;
                    // 當有學生
                    if (record.Students.Count > 0)
                        msg = string.Format("確定要刪除「{0}」？班上" + record.Students.Count + "位學生將移到未分年級未分班級.", record.Name);
                    else
                        msg = string.Format("確定要刪除「{0}」？", record.Name);

                    if (FISCA.Presentation.Controls.MsgBox.Show(msg, "刪除班級", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        JHSchool.Data.JHClassRecord classRec = JHSchool.Data.JHClass.SelectByID(record.ID);

                        PermRecLogProcess prlp = new PermRecLogProcess();
                        prlp.SaveLog("學籍.班級", "刪除班級", "刪除班級資料，班級名稱：" + classRec.Name);
                        JHSchool.Data.JHClass.Delete(classRec);
                        Class.Instance.SyncDataBackground(record.ID);
                    }
                    else
                        return;

                }
            };
            #endregion

            #region RibbonBar 學生/匯入匯出

            rbItem = Class.Instance.RibbonBarItems["資料統計"];
            rbItem["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["匯出"].Image = Properties.Resources.Export_Image;
            rbItem["匯出"]["匯出班級基本資料"].Enable = User.Acl["JHSchool.Class.Ribbon0030"].Executable;
            rbItem["匯出"]["匯出班級基本資料"].Click += delegate
            {
                new JHSchool.ClassExtendControls.Ribbon.ClassExportWizard().ShowDialog();
            };

            rbItem["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["匯入"].Image = Properties.Resources.Import_Image;
            rbItem["匯入"]["匯入班級基本資料"].Enable = User.Acl["JHSchool.Class.Ribbon0020"].Executable;
            rbItem["匯入"]["匯入班級基本資料"].Click += delegate
            {
                new JHSchool.ClassExtendControls.Ribbon.ClassImportWizard().ShowDialog();
            };
            #endregion

            //報表,是以常態呈現
            rbItem["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["報表"].Image = Properties.Resources.paste_64;
            rbItem["報表"].SupposeHasChildern = true;

            #region RibbonBar 班級/學務作業
            //rbItem = Class.Instance.RibbonBarItems["學務作業"];
            //rbItem["德行加減分"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["德行加減分"].Image = ClassExtendControls.Ribbon.Resources.btnAdjust_Image;
            //rbItem["德行加減分"].Click += delegate
            //{

            //};

            //rbItem["文字評量"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["文字評量"].Click += delegate
            //{

            //};
            #endregion

            #region RibbonBar 班級/教務作業
            //rbItem = Class.Instance.RibbonBarItems["教務作業"];
            //rbItem["班級升級"].Image = ClassExtendControls.Ribbon.Resources.btnUpgrade_Image;
            //rbItem["班級升級"].Click += delegate
            //{

            //};
            #endregion

            #region RibbonBar 班級/指定
            rbItem = Class.Instance.RibbonBarItems["指定"];
            //rbItem["類別"].Image = InternalExtendControls.Tagging.Resources.ctxTag_Image;
            //rbItem["類別"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //rbItem["類別"].SupposeHasChildern = true;
            //rbItem["類別"].PopupOpen += new EventHandler<PopupOpenEventArgs>(
            //    new TaggingMenu("JHSchool.Class.Ribbon0040", "JHSchool.Class.Ribbon0050").MenuOpen);

            //rbItem = Class.Instance.RibbonBarItems["指定"];
            //rbItem["學生"].Size = RibbonBarButton.MenuButtonSize.Small;
            //rbItem["學生"].Image = ClassExtendControls.Ribbon.Resources.btnAssignStudent_Image;
            //rbItem["學生"].Click += delegate
            //{

            //};

            //rbItem["課程規劃"].Size = RibbonBarButton.MenuButtonSize.Small;
            //rbItem["課程規劃"].Image = ClassExtendControls.Ribbon.Resources.btnPlan_Image;
            //rbItem["課程規劃"].Click += delegate
            //{

            //};

            //rbItem["計算規則"].Size = RibbonBarButton.MenuButtonSize.Small;
            //rbItem["計算規則"].Image = ClassExtendControls.Ribbon.Resources.btnCalcRule_Image;
            //rbItem["計算規則"].Click += delegate
            //{

            //};
            #endregion

            #region RibbonBar 班級/統計報表0
            //rbItem = Class.Instance.RibbonBarItems["統計報表"];
            //rbItem["統計"].Image = ClassExtendControls.Ribbon.Resources.btnStatistics_Image;
            //rbItem["統計"].Click += delegate
            //{

            //};

            //rbItem["報表"].Image = ClassExtendControls.Ribbon.Resources.btnReport_Image;
            //rbItem["報表"].Click += delegate
            //{

            //};
            #endregion

            #region RibbonBar 班級/其它
            //rbItem = Class.Instance.RibbonBarItems["其它"];
            //rbItem["修改歷程"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["修改歷程"].Image = ClassExtendControls.Ribbon.Resources.btnHistory_Image;
            //rbItem["修改歷程"].Click += delegate
            //{

            //};

            //rbItem["問卷"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["問卷"].Image = ClassExtendControls.Ribbon.Resources.btnSurvey_Image;
            //rbItem["問卷"].Click += delegate
            //{

            //};
            #endregion

            //ListPaneField idField = new ListPaneField("ID");
            //idField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            //{
            //    e.Value = e.Key;
            //};

            //#region 排序班級ID
            //idField.CompareValue += delegate(object sender, CompareValueEventArgs e)
            //{
            //    int x, y;

            //    if (!int.TryParse(e.Value1.ToString(), out x))
            //        x = int.MaxValue;

            //    if (!int.TryParse(e.Value2.ToString(), out y))
            //        y = int.MaxValue;

            //    e.Result = x.CompareTo(y);
            //};
            //#endregion
            //this.AddListPaneField(idField);

            ListPaneField gradeYearField = new ListPaneField("年級");
            gradeYearField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].GradeYear;
            };
            this.AddListPaneField(gradeYearField);


            ListPaneField nameField = new ListPaneField("名稱");
            nameField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].Name;
            };

            this.AddListPaneField(nameField);

            ListPaneField classTeacherField = new ListPaneField("班導師");
            classTeacherField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    if (Items[e.Key].Teacher != null)
                    {
                        e.Value = Items[e.Key].Teacher.FullName;
                    }
            };
            this.AddListPaneField(classTeacherField);

            ListPaneField classStudentCountField = new ListPaneField("人數");
            classStudentCountField.CompareValue += new EventHandler<CompareValueEventArgs>(class_CompareValue);
            classStudentCountField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].Students.Count;

            };
            this.AddListPaneField(classStudentCountField);


            ListPaneField classIndexField = new ListPaneField("排列序號");
            classIndexField.CompareValue += new EventHandler<CompareValueEventArgs>(class_CompareValue);
            classIndexField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                {
                    if (Items[e.Key].DisplayOrder != "")
                    {
                        e.Value = Items[e.Key].DisplayOrder;
                    }
                    else
                    {
                        e.Value = "";
                    }
                }

            };
            this.AddListPaneField(classIndexField);

            ListPaneField classNamingRuleField = new ListPaneField("班級命名規則");
            classNamingRuleField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].NamingRule;

            };
            this.AddListPaneField(classNamingRuleField);

            //Class.Instance.AddView(new AllItemView());

            Class.Instance.AddView(new JHSchool.StudentExtendControls.Class_View());
            //Class.Instance.AddView(new JHSchool.ClassExtendControls.SubjectView()); //由類別模組提供

            // 班級基本資料
            //Class.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<JHSchool.ClassExtendControls.ClassBaseInfoItem>());

            //Class.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<ClassExtendControls.ClassBaseInfoItem>());

            Present.NavPaneContexMenu.GetChild("重新整理").Click += delegate { this.SyncAllBackground(); };

            //Class.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<ClassExtendControls.ClassStudentItem>());
            //電子報表(因相關功能未完成先註)
            //Class.Instance.AddDetailBulider(new JHSchool.Legacy.ContentItemBulider<ClassExtendControls.ElectronicPaperPalmerworm>());

            //Class.Instance.AddDetailBulider(new JHSchool.CriticalSection.Test_classInfo());

            #region 註冊權限管理
            Catalog ribbon = RoleAclSource.Instance["班級"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0000", "新增班級"));
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0010", "刪除班級"));
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0020", "匯入班級資料"));
            ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0030", "匯出班級資料"));
            //ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0040", "指定班級類別"));
            //ribbon.Add(new RibbonFeature("JHSchool.Class.Ribbon0050", "管理班級類別清單"));

            Catalog detail = RoleAclSource.Instance["班級"]["資料項目"];
            detail.Add(new DetailItemFeature(typeof(ClassExtendControls.ClassBaseInfoItem)));
            //電子報表(因相關功能未完成先註)
            //detail.Add(new DetailItemFeature(typeof(ClassExtendControls.ElectronicPaperPalmerworm)));
            #endregion

            #region 增加班級搜尋條件鈕

            ConfigData cd = User.Configuration["ClassSearchOptionPreference"];

            SearchClassName = SearchConditionMenu["班級名稱"];
            SearchClassName.AutoCheckOnClick = true;
            SearchClassName.AutoCollapseOnClick = false;
            SearchClassName.Checked = cd.GetBoolean("SearchClassName", true);
            SearchClassName.Click += delegate
            {
                cd.SetBoolean("SearchClassName", SearchClassName.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchClassTeacher = SearchConditionMenu["班級導師"];
            SearchClassTeacher.AutoCheckOnClick = true;
            SearchClassTeacher.AutoCollapseOnClick = false;
            SearchClassTeacher.Checked = cd.GetBoolean("SearchClassTeacher", true);
            SearchClassTeacher.Click += delegate
            {
                cd.SetBoolean("SearchClassTeacher", SearchClassTeacher.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            Present.Search += new EventHandler<SearchEventArgs>(Class_Search);

            #endregion

            //由類別模組提供
            //Present.SetDescriptionPaneBulider(new DescriptionPaneBulider<JHSchool.ClassExtendControls.ClassDescription>());

            MotherForm.AddPanel(K12.Presentation.NLDPanels.Class);
            _Initilized = true;
            FillFilter();

        }

        void class_CompareValue(object sender, CompareValueEventArgs e)
        {
            int x;
            int y;
            if (string.IsNullOrEmpty("" + e.Value1))
                x = 65536;
            else
                int.TryParse("" + e.Value1, out x);

            if (string.IsNullOrEmpty("" + e.Value2))
                y = 65536;
            else
                int.TryParse("" + e.Value2, out y);

            e.Result = x.CompareTo(y);
        }

        private Class(NLDPanel present)
            : base(present)
        {
            this.ItemLoaded += delegate
            {
                lock (_TeacherSupervised)
                {
                    _TeacherSupervised.Clear();
                    foreach (var item in Items)
                    {
                        if (!_TeacherSupervised.ContainsKey(item.RefTeacherID))
                            _TeacherSupervised.Add(item.RefTeacherID, new List<ClassRecord>());
                        _TeacherSupervised[item.RefTeacherID].Add(item);
                    }
                }
            };
            this.ItemUpdated += delegate(object sender, ItemUpdatedEventArgs e)
            {
                lock (_TeacherSupervised)
                {
                    List<string> keys = new List<string>(e.PrimaryKeys);
                    keys.Sort();
                    foreach (var tid in _TeacherSupervised.Keys)
                    {
                        List<ClassRecord> removeItems = new List<ClassRecord>();
                        foreach (var item in _TeacherSupervised[tid])
                        {
                            if (keys.BinarySearch(item.ID) >= 0)
                            {
                                removeItems.Add(item);
                            }
                        }
                        foreach (var item in removeItems)
                        {
                            _TeacherSupervised[tid].Remove(item);
                        }
                    }
                    foreach (var key in e.PrimaryKeys)
                    {
                        var item = Items[key];
                        if (item != null)
                        {
                            if (!_TeacherSupervised.ContainsKey(item.RefTeacherID))
                                _TeacherSupervised.Add(item.RefTeacherID, new List<ClassRecord>());
                            _TeacherSupervised[item.RefTeacherID].Add(item);
                        }
                    }
                }
            };
        }

        #region 班級搜尋主功能

        private MenuButton SearchClassName, SearchClassTeacher; //SearchClassTeacher

        void Class_Search(object sender, SearchEventArgs e)
        {
            try
            {
                List<ClassRecord> classList = new List<ClassRecord>(Class.Instance.Items);
                Dictionary<string, ClassRecord> results = new Dictionary<string, ClassRecord>();
                Regex rx = new Regex(e.Condition, RegexOptions.IgnoreCase);

                if (SearchClassName.Checked)
                {
                    foreach (ClassRecord each in classList)
                    {
                        string name = (each.Name != null) ? each.Name : "";
                        if (rx.Match(name).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                if (SearchClassTeacher.Checked)
                {
                    foreach (ClassRecord each in classList)
                    {
                        string name = (each.Teacher != null) ? each.Teacher.Name : "";
                        if (rx.Match(name).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                e.Result.AddRange(results.Values.AsKeyList());
            }
            catch (Exception) { }
        }

        #endregion


        private bool _Initilized = false;
        private Dictionary<string, List<ClassRecord>> _TeacherSupervised = new Dictionary<string, List<ClassRecord>>();
        public List<ClassRecord> GetTecaherSupervisedClass(TeacherRecord teacher)
        {
            lock (_TeacherSupervised)
            {
                if (_TeacherSupervised.ContainsKey(teacher.ID))
                {
                    return new List<ClassRecord>(_TeacherSupervised[teacher.ID]);
                }
                else
                    return new List<ClassRecord>();
            }
        }

        protected override void FillFilter()
        {
            //資料載入中或資料未載入或畫面沒有設定完成就什麼都不做
            if (!_Initilized || !Loaded) return;

            List<string> primaryKeys = new List<string>();
            foreach (var item in Items)
            {
                primaryKeys.Add(item.ID);
            }
            Present.SetFilteredSource(primaryKeys);
        }

        protected override Dictionary<string, ClassRecord> GetAllData()
        {
            Dictionary<string, ClassRecord> items = new Dictionary<string, ClassRecord>();
            foreach (var item in Feature.QueryClass.GetAllClasses())
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        protected override Dictionary<string, ClassRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, ClassRecord> items = new Dictionary<string, ClassRecord>();
            foreach (var item in Feature.QueryClass.GetClasses(primaryKeys))
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        [Obsolete("程式碼轉移遺留下來的。")]
        internal bool ValidClassName(string classid, string className)
        {
            //_loadingWait.WaitOne();
            if (string.IsNullOrEmpty(className)) return false;
            foreach (ClassRecord classRec in this.Items)
            {
                if (classRec.ID != classid && classRec.Name == className)
                    return false;
            }
            return true;
        }

        [Obsolete("程式碼轉移遺留下來的。")]
        internal bool ValidateNamingRule(string namingRule)
        {
            return namingRule.IndexOf('{') < namingRule.IndexOf('}');
        }

        [Obsolete("程式碼轉移遺留下來的。")]
        internal string ParseClassName(string namingRule, int gradeYear)
        {
            // 當年級是7,8,9
            if (gradeYear >= 6)
                gradeYear -= 6;

            gradeYear--;
            if (!ValidateNamingRule(namingRule))
                return namingRule;
            string classlist_firstname = "", classlist_lastname = "";
            if (namingRule.Length == 0) return "{" + (gradeYear + 1) + "}";

            string tmp_convert = namingRule;

            // 找出"{"之前文字 並放入 classlist_firstname , 並除去"{"
            if (tmp_convert.IndexOf('{') > 0)
            {
                classlist_firstname = tmp_convert.Substring(0, tmp_convert.IndexOf('{'));
                tmp_convert = tmp_convert.Substring(tmp_convert.IndexOf('{') + 1, tmp_convert.Length - (tmp_convert.IndexOf('{') + 1));
            }
            else tmp_convert = tmp_convert.TrimStart('{');

            // 找出 } 之後文字 classlist_lastname , 並除去"}"
            if (tmp_convert.IndexOf('}') > 0 && tmp_convert.IndexOf('}') < tmp_convert.Length - 1)
            {
                classlist_lastname = tmp_convert.Substring(tmp_convert.IndexOf('}') + 1, tmp_convert.Length - (tmp_convert.IndexOf('}') + 1));
                tmp_convert = tmp_convert.Substring(0, tmp_convert.IndexOf('}'));
            }
            else tmp_convert = tmp_convert.TrimEnd('}');

            // , 存入 array
            string[] listArray = new string[tmp_convert.Split(',').Length];
            listArray = tmp_convert.Split(',');

            // 檢查是否在清單範圍
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

        protected override List<string> AsKeyList(List<ClassRecord> list)
        {
            return list.AsKeyList();
        }
    }

    public static class Class_Extends
    {
        /// <summary>
        /// 讀取班級的系統編號轉換成 List。
        /// </summary>
        public static List<string> AsKeyList(this IEnumerable<ClassRecord> classes)
        {
            List<string> keys = new List<string>();
            foreach (ClassRecord each in classes)
                keys.Add(each.ID);

            return keys;
        }
    }
}
