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
        private List<JHClassRecord> _ClassRecList;
        public TeachStudentItem()
        {
            InitializeComponent();
            Group = "帶班班級";
            _BGWorker = new BackgroundWorker();
            _BGWorker.DoWork += new DoWorkEventHandler(_BGWorker_DoWork);
            _BGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWorker_RunWorkerCompleted);
            _ClassRecList = new List<JHClassRecord>();
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

            foreach (JHClassRecord classRec in _ClassRecList)
            {
                string id = classRec.ID;
                string classname = classRec.Name;
                byte BoyCot=0,GirlCot=0,TotalCot=0;
                foreach (JHStudentRecord studRec in classRec.Students)
                {
                    // 一般或輟學
                    if (studRec.Status == K12.Data.StudentRecord.StudentStatus.一般 || studRec.Status == K12.Data.StudentRecord.StudentStatus.輟學)
                    {
                        if (studRec.Gender == "男")
                            BoyCot++;

                        if (studRec.Gender == "女")
                            GirlCot++;

                        TotalCot++;
                    }
                }
                string gradeYear=" ";
                ListViewGroup group;
                if (classRec.GradeYear.HasValue)
                    gradeYear = classRec.GradeYear.Value + "";

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

                item.SubItems.Add(classname);
                item.SubItems.Add(TotalCot+"");
                item.SubItems.Add(BoyCot+"");
                item.SubItems.Add(GirlCot+"");
                item.Group = group;
                item.Tag = id;
            }
            this.Loading = false;
        }


        void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _ClassRecList.Clear();
            foreach (JHClassRecord classRec in JHClass.SelectAll())
                if (classRec.RefTeacherID == PrimaryKey)
                    _ClassRecList.Add(classRec);
        }

        protected override void OnPrimaryKeyChanged(System.EventArgs e)
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
