using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using FCode = Framework.Security.FeatureCodeAttribute;
using JHSchool.Data;
using FISCA.Presentation;

namespace JHSchool.TeacherExtendControls
{
    [FCode("JHSchool.Teacher.Detail0010", "帶班班級")]
    internal partial class TeachStudentItem : FISCA.Presentation.DetailContent
    {
        private bool _isBGWorkBusy=false;
        private BackgroundWorker _BGWorker;
        private List<ClassRow> _ClassRows;
        private DetailContentLazyLoader _LazyLoader;

        /// <summary>
        /// 一個帶班班級的顯示資料（在背景執行緒計算，UI 執行緒只負責繫結）。
        /// </summary>
        private class ClassRow
        {
            public string ID;
            public string Name;
            public string GradeYear;
            public int Total;
            public int Boys;
            public int Girls;
        }

        public TeachStudentItem()
        {
            InitializeComponent();
            Group = "帶班班級";
            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            _ClassRows = new List<ClassRow>();
            _LazyLoader = new DetailContentLazyLoader(this, () => PrimaryKey, LoadCurrentRecord);
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

        private void BindDataToForm()
        {
            listView2.Items.Clear();
            listView2.Groups.Clear();
            Dictionary<string, ListViewGroup> groups = new Dictionary<string, ListViewGroup>();

            foreach (ClassRow row in _ClassRows)
            {
                string gradeYear = string.IsNullOrEmpty(row.GradeYear) ? " " : row.GradeYear;
                ListViewGroup group;

                string gKey = gradeYear + "年級";
                if (!groups.ContainsKey(gKey))
                {
                    group = new ListViewGroup(gKey, gKey);
                    groups.Add(gKey, group);
                    listView2.Groups.Add(group);
                }
                else
                    group = groups[gKey];

                ListViewItem item = listView2.Items.Add(gradeYear);

                item.SubItems.Add(row.Name);
                item.SubItems.Add(row.Total+"");
                item.SubItems.Add(row.Boys+"");
                item.SubItems.Add(row.Girls+"");
                item.Group = group;
                item.Tag = row.ID;
            }
            this.Loading = false;
        }


        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // 由記憶體快取取得帶班班級與班級學生，不再每次下載全部班級、再逐班查詢學生。
            // GetClassStudents 只包含「一般」與「輟學」的學生，與原本的人數計算條件相同。
            List<ClassRow> rows = new List<ClassRow>();
            foreach (ClassRecord classRec in Class.Instance.GetTeacherSupervisedClass(PrimaryKey))
            {
                ClassRow row = new ClassRow() { ID = classRec.ID, Name = classRec.Name, GradeYear = classRec.GradeYear };
                foreach (StudentRecord studRec in Student.Instance.GetClassStudents(classRec))
                {
                    if (studRec.Gender == "男")
                        row.Boys++;

                    if (studRec.Gender == "女")
                        row.Girls++;

                    row.Total++;
                }
                rows.Add(row);
            }
            _ClassRows = rows;
        }

        protected override void OnPrimaryKeyChanged(System.EventArgs e)
        {
            _LazyLoader.RequestLoad();
        }

        private void LoadCurrentRecord()
        {
            this.Loading = true;
            if (_BGWorker.IsBusy)
                _isBGWorkBusy = true;
            else
                _BGWorker.RunWorkerAsync();
        }

        public DetailContent GetContent()
        {
            return new TeachStudentItem();
        }

        private void listView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView2.FocusedItem == null) return;
            if (listView2.FocusedItem.Tag == null) return;
            string id = listView2.FocusedItem.Tag.ToString();
            string caption = listView2.FocusedItem.SubItems[1].Text;
            PopupClassForm(id, caption);
        }

        private void PopupClassForm(string classid,string caption)
        {
            //Class.Instance.PopupClassForm(classid,caption);
        }
    }
}
