using System;
using System.Collections.Generic;
using System.Text;
using FISCA.Presentation;
using System.ComponentModel;
using System.Threading;
using Framework;
using JHSchool.Editor;
using JHSchool.CourseExtendControls;
using System.Windows.Forms;
using JHSchool.CourseExtendControls.Ribbon;
using Framework.Security;
using System.Text.RegularExpressions;

namespace JHSchool
{
    public class Course : LegacyPresentBase<CourseRecord>
    {
        private static Course _Instance = null;
        public static Course Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Course(K12.Presentation.NLDPanels.Course);
                return _Instance;
            }
        }

        private SemesterInfo FiltedSemester = new SemesterInfo() { SchoolYear = Framework.Int.Parse(School.DefaultSchoolYear), Semester = Framework.Int.Parse(School.DefaultSemester) };

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
            Course.Instance.RibbonBarItems["編輯"].Index = 0;
            Course.Instance.RibbonBarItems["資料統計"].Index = 1;
            Course.Instance.RibbonBarItems["指定"].Index = 2;
            Course.Instance.RibbonBarItems["教務"].Index = 3;

            //CourseSyncAllBackground
            FISCA.Features.Register("CourseSyncAllBackground", x =>
            {
                this.SyncAllBackground();
            });

            RibbonBarItem rbItema = Course.Instance.RibbonBarItems["編輯"];

            #region RibbonBar 學生/匯入匯出
            RibbonBarItem rbItem = Course.Instance.RibbonBarItems["資料統計"];

            rbItem["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["匯出"].Image = Properties.Resources.Export_Image;
            rbItem["匯出"]["匯出課程基本資料"].Enable = User.Acl["JHSchool.Course.Ribbon0030"].Executable;
            rbItem["匯出"]["匯出課程基本資料"].Click += delegate
            {
                new JHSchool.CourseExtendControls.Ribbon.CourseExportWizard().ShowDialog();
            };

            rbItem["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["匯入"].Image = Properties.Resources.Import_Image;
            rbItem["匯入"]["匯入課程基本資料"].Enable = User.Acl["JHSchool.Course.Ribbon0020"].Executable;
            rbItem["匯入"]["匯入課程基本資料"].Click += delegate
            {
                new JHSchool.CourseExtendControls.Ribbon.CourseImportWizard().ShowDialog();
                Course.Instance.SyncAllBackground();
            };
            #endregion

            //報表,是以常態呈現
            rbItem["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            rbItem["報表"].Image = Properties.Resources.paste_64;
            rbItem["報表"].SupposeHasChildern = true;

            #region RibbonBar 課程/指定
            //目的是先建立按鈕順序(新增 / 刪除 / 類別)
            //by dylan
            RibbonBarItem rbItemx1 = Course.Instance.RibbonBarItems["編輯"];
            RibbonBarButton rbButton = rbItemx1["新增"];
            rbButton.Size = RibbonBarButton.MenuButtonSize.Large;
            rbButton = rbItemx1["刪除"];
            rbButton.Size = RibbonBarButton.MenuButtonSize.Large;

            rbItem = Course.Instance.RibbonBarItems["指定"];
            //由類別模組提供
            //rbItem["類別"].Image = InternalExtendControls.Tagging.Resources.ctxTag_Image;
            //rbItem["類別"].Size = RibbonBarButton.MenuButtonSize.Medium;
            //rbItem["類別"].SupposeHasChildern = true;
            //rbItem["類別"].PopupOpen += new EventHandler<PopupOpenEventArgs>(
            //    new TaggingMenu("JHSchool.Course.Ribbon0040", "JHSchool.Course.Ribbon0050").MenuOpen);
            #endregion

            #region RibbonBar 課程/統計報表
            //rbItem = Course.Instance.RibbonBarItems["統計報表"];
            //rbItem["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["報表"].Image = CourseExtendControls.Ribbon.Resources.btnReport_Image;
            //rbItem["報表"].Click += delegate
            //{

            //};
            #endregion

            #region RibbonBar 課程/其它
            //rbItem = Course.Instance.RibbonBarItems["其它"];
            //rbItem["修改歷程"].Size = RibbonBarButton.MenuButtonSize.Large;
            //rbItem["修改歷程"].Image = CourseExtendControls.Ribbon.Resources.btnHistory_Image;
            //rbItem["修改歷程"].Click += delegate
            //{

            //};
            #endregion

            #region 註冊權限管理
            Catalog ribbon = RoleAclSource.Instance["課程"]["功能按鈕"];
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0000", "新增課程資料"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0010", "刪除課程資料"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0020", "匯入課程資料"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0030", "匯出課程資料"));
            //ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0040", "指定課程類別"));
            //ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0050", "管理課程類別清單"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0060", "分組上課"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0070", "成績輸入"));
            ribbon.Add(new RibbonFeature("JHSchool.Course.Ribbon0080", "成績計算"));

            Catalog detail = RoleAclSource.Instance["課程"]["資料項目"];

            Catalog report = RoleAclSource.Instance["課程"]["報表"];
            report.Add(new ReportFeature("JHSchool.Course.Report0000", "學生修課清單"));
            #endregion

            //ListPaneField idField = new ListPaneField("ID");
            //idField.GetVariable += delegate(object sender, GetVariableEventArgs e)
            //{
            //    e.Value = e.Key;
            //};
            //idField.CompareValue += delegate(object sender, CompareValueEventArgs e)
            //{
            //    int x, y;

            //    if (!int.TryParse(e.Value1.ToString(), out x))
            //        x = int.MaxValue;

            //    if (!int.TryParse(e.Value2.ToString(), out y))
            //        y = int.MaxValue;

            //    e.Result = x.CompareTo(y);
            //};
            //this.AddListPaneField(idField);


            #region Search Conditions
            ConfigData cd = User.Configuration["CourseSearchOptionPreference"];

            SearchName = SearchConditionMenu["課程名稱"];
            SearchName.AutoCheckOnClick = true;
            SearchName.AutoCollapseOnClick = false;
            SearchName.Checked = cd.GetBoolean("SearchName", true);
            SearchName.Click += delegate
            {
                cd.SetBoolean("SearchName", SearchName.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate (object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchDomain = SearchConditionMenu["領域"];
            SearchDomain.AutoCheckOnClick = true;
            SearchDomain.AutoCollapseOnClick = false;
            SearchDomain.Checked = cd.GetBoolean("SearchDomain", true);
            SearchDomain.Click += delegate
            {
                cd.SetBoolean("SearchDomain", SearchDomain.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate (object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            SearchSubject = SearchConditionMenu["科目"];
            SearchSubject.AutoCheckOnClick = true;
            SearchSubject.AutoCollapseOnClick = false;
            SearchSubject.Checked = cd.GetBoolean("SearchSubject", true);
            SearchSubject.Click += delegate
            {
                cd.SetBoolean("SearchSubject", SearchSubject.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate (object sender, DoWorkEventArgs e) { (e.Argument as ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };

            Present.Search += new EventHandler<SearchEventArgs>(Course_Search);
            #endregion

            ListPaneField nameField = new ListPaneField("名稱");
            nameField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].Name;
            };
            this.AddListPaneField(nameField);

            ListPaneField domainField = new ListPaneField("領域");
            domainField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].Domain;
            };
            this.AddListPaneField(domainField);

            ListPaneField subjectField = new ListPaneField("科目名稱");
            subjectField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].Subject;
            };
            this.AddListPaneField(subjectField);

            ListPaneField classField = new ListPaneField("所屬班級");
            classField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    if (Items[e.Key].Class != null)
                        e.Value = Items[e.Key].Class.Name;
            };
            this.AddListPaneField(classField);

            ListPaneField sectField = new ListPaneField("節數/權數");
            sectField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                {
                    string period = Items[e.Key].Period;
                    string credit = Items[e.Key].Credit;
                    if (period == credit) e.Value = period;
                    else e.Value = period + "/" + credit;
                }
            };
            this.AddListPaneField(sectField);

            ListPaneField schoolyearField = new ListPaneField("學年度");
            schoolyearField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].SchoolYear;
            };
            this.AddListPaneField(schoolyearField);

            ListPaneField semesterField = new ListPaneField("學期");
            semesterField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                    e.Value = Items[e.Key].Semester;
            };
            this.AddListPaneField(semesterField);

            ListPaneField semScoreField = new ListPaneField("學期成績");
            semScoreField.GetVariable += delegate (object sender, GetVariableEventArgs e)
            {
                if (Items[e.Key] != null)
                {
                    if (Items[e.Key].CalculationFlag == "1")
                        e.Value = "列入";

                    if (Items[e.Key].CalculationFlag == "2")
                        e.Value = "不列入";
                }
            };
            this.AddListPaneField(semScoreField);

            //this.AddView(new AllItemView());

            this.AddView(new SubjectView());
            //this.AddView(new CategoryView()); //由類別模組提供
            this.AddView(new GradeYear_Class_View());

            //this.RequiredDescription += delegate(object sender, RequiredDescriptionEventArgs e)
            //{
            //    e.Result = this[e.PrimaryKey].GetDescription();
            //};

            Present.NavPaneContexMenu.GetChild("重新整理").Click += delegate
            {
                this.SyncAllBackground();
            };

            //處理產品社團,當使用轉入課程功能時
            //需要引發課程的更新事件
            //2019/9/10 - Dylan
            FISCA.InteractionService.SubscribeEvent("課程/重新整理", (sender, args) =>
            {
                this.SyncAllBackground();
            });

            //由類別模組提供
            //Present.SetDescriptionPaneBulider(new DescriptionPaneBulider<CourseDescription>());

            Present.FilterMenu.SupposeHasChildern = true;
            Present.FilterMenu.PopupOpen += delegate (object sender, PopupOpenEventArgs e)
            {
                List<SemesterInfo> semesterList = new List<SemesterInfo>();
                foreach (var item in Items)
                {
                    SemesterInfo semester = new SemesterInfo();
                    semester.SchoolYear = item.SchoolYear;
                    semester.Semester = item.Semester;
                    if (!semesterList.Contains(semester))
                        semesterList.Add(semester);
                }
                if (semesterList.Count <= 0)
                {
                    MenuButton mb = e.VirtualButtons[FiltedSemester.ToString()];
                    //mb.Visible = false;
                }
                semesterList.Sort();
                foreach (var item in semesterList)
                {
                    MenuButton mb = e.VirtualButtons[item.ToString()];
                    mb.AutoCheckOnClick = true;
                    mb.AutoCollapseOnClick = true;
                    mb.Checked = (item == FiltedSemester);
                    mb.Tag = item;
                    mb.CheckedChanged += delegate
                    {
                        if (mb.Checked)
                        {
                            FiltedSemester = (SemesterInfo)mb.Tag;
                            SetSource();
                        }
                    };
                }

            };

            MotherForm.AddPanel(K12.Presentation.NLDPanels.Course);
            _Initilized = true;
            //FillFilter();

            UseFilter = true;
            SetSource();
        }

        private MenuButton SearchName, SearchDomain, SearchSubject;
        private SearchEventArgs SearEvArgs = null;

        private bool _Initilized = false;

        private Course(NLDPanel present)
            : base(present)
        {
        }

        protected override Dictionary<string, CourseRecord> GetAllData()
        {
            Dictionary<string, CourseRecord> items = new Dictionary<string, CourseRecord>();
            foreach (var item in Feature.QueryCourse.GetAllCourses())
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        protected override Dictionary<string, CourseRecord> GetData(IEnumerable<string> primaryKeys)
        {
            Dictionary<string, CourseRecord> items = new Dictionary<string, CourseRecord>();
            foreach (var item in Feature.QueryCourse.GetCourses(primaryKeys))
            {
                items.Add(item.ID, item);
            }
            return items;
        }

        private void SetSource()
        {
            //資料載入中或資料未載入或畫面沒有設定完成就什麼都不做
            if (!_Initilized || !Loaded) return;
            if (UseFilter)
                FillFilter();
            else
            {
                Present.FilterMenu.Text = "所有學年度學期";
                Present.SetFilteredSource(new List<string>(Items.Keys));
            }
        }

        private void Course_Search(object sender, SearchEventArgs e)
        {
            SearEvArgs = e;
            Campus.Windows.BlockMessage.Display("資料搜尋中,請稍候....", new Campus.Windows.ProcessInvoker(ProcessSearch));
        }

        private void ProcessSearch(Campus.Windows.MessageArgs args)
        {
            try
            {
                List<CourseRecord> courses = new List<CourseRecord>(Course.Instance.Items);
                Dictionary<string, CourseRecord> results = new Dictionary<string, CourseRecord>();
                Regex rx = new Regex(SearEvArgs.Condition, RegexOptions.IgnoreCase);

                if (SearchName.Checked)
                {
                    foreach (CourseRecord each in courses)
                    {
                        string name = each.Name;
                        if (rx.Match(name).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                if (SearchDomain.Checked)
                {
                    foreach (CourseRecord each in courses)
                    {
                        string name = each.Domain;
                        if (rx.Match(name).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                if (SearchSubject.Checked)
                {
                    foreach (CourseRecord each in courses)
                    {
                        string name = each.Subject;
                        if (rx.Match(name).Success)
                        {
                            if (!results.ContainsKey(each.ID))
                                results.Add(each.ID, each);
                        }
                    }
                }

                SearEvArgs.Result.AddRange(results.Values.AsKeyList());
            }
            catch (Exception) { }
        }

        protected override void FillFilter()
        {   //資料載入中或資料未載入或畫面沒有設定完成就什麼都不做
            if (!_Initilized || !Loaded) return;
            Course.Instance.FilterMenu.Text = FiltedSemester.ToString();
            List<string> primaryKeys = new List<string>();
            foreach (var item in Items)
            {
                if (item.Semester == FiltedSemester.Semester && item.SchoolYear == FiltedSemester.SchoolYear)
                    primaryKeys.Add(item.ID.ToString());
            }
            Present.SetFilteredSource(primaryKeys);
        }

        protected override List<string> AsKeyList(List<CourseRecord> list)
        {
            return list.AsKeyList();
        }
    }
    public static class CourseMethod
    {
        public static IEnumerable<CourseRecord> GetSemesterCourses(this IEnumerable<CourseRecord> source, int schoolYear, int semester)
        {
            List<CourseRecord> result = new List<CourseRecord>();
            foreach (var item in source)
            {
                if (item.SchoolYear == schoolYear && item.Semester == semester)
                    result.Add(item);
            }
            return result;
        }
        /// <summary>
        /// 取得課程的簡短描述
        /// </summary>
        /// <param name="course">課程</param>
        /// <returns>簡短描述</returns>
        public static string GetDescription(this CourseRecord course)
        {
            return course.Name + "[" + course.SchoolYear + " " + course.Semester + "]";
        }

        /// <summary>
        /// 讀取課程的系統編號轉換成 List。
        /// </summary>
        public static List<string> AsKeyList(this IEnumerable<CourseRecord> courses)
        {
            List<string> keys = new List<string>();
            foreach (CourseRecord each in courses)
                keys.Add(each.ID);

            return keys;
        }
    }
}