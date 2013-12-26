using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
//using SmartSchool.Common;
//using SmartSchool.Feature.Teacher;
//using SmartSchool.Feature.Course;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Legacy;
using Framework.Security;
using JHSchool.Feature.Legacy;
//using SmartSchool.CouseRelated;
//using SmartSchool.AccessControl;
using FCode = Framework.Security.FeatureCodeAttribute;

namespace JHSchool.TeacherExtendControls
{
    [FCode("JHSchool.Teacher.Detail0030", "教授課程")]
    internal partial class TeachCourseItem : PalmerwormItem
    {
        private const string ALL_COURSE = "All";
        private SemsComboItemCollection _sems;

        public TeachCourseItem()
        {
            InitializeComponent();
            Title = "教授課程";
        }

        protected override object OnBackgroundWorkerWorking()
        {
            DSResponse dsrsp = QueryTeacher.GetCourseList(RunningID);
            return dsrsp;
        }

        protected override void OnBackgroundWorkerCompleted(object result)
        {
            DSResponse dsrsp = result as DSResponse;
            DSXmlHelper helper = dsrsp.GetContent();

            _sems = new SemsComboItemCollection();
            _sems.Add(ALL_COURSE, new SemsComboItem("", "", "所有教授課程"));

            foreach (XmlElement element in helper.GetElements("Course"))
            {
                string school_year = "0";
                string semester = "0";

                //ListViewItem item = listView.Items.Add(element.SelectSingleNode("CourseName").InnerText);
                ListViewItem item = new ListViewItem(element.SelectSingleNode("CourseName").InnerText);
                item.SubItems.Add(element.SelectSingleNode("Credit").InnerText);
                item.SubItems.Add(element.SelectSingleNode("StudentCount").InnerText);

                if (element.SelectSingleNode("SchoolYear") != null) //Service 可能是舊的，所以這樣判斷。
                {
                    school_year = element.SelectSingleNode("SchoolYear").InnerText;
                    semester = element.SelectSingleNode("Semester").InnerText;
                    item.SubItems.Add(school_year);
                    item.SubItems.Add(semester);
                }

                item.Tag = (element.GetAttribute("ID"));

                if(!_sems.ContainsKey(school_year+":"+semester))
                    _sems.Add(school_year+":"+semester, new SemsComboItem(school_year, semester, school_year + " 學年度  第 " + semester + " 學期"));
                _sems[school_year+":"+semester].AddCourse(item);
                _sems[ALL_COURSE].AddCourse(item);
            }

            cboSemsFilter.SelectedItem = null;
            cboSemsFilter.Items.Clear();
            foreach (SemsComboItem combo_item in _sems.Values)
                cboSemsFilter.Items.Add(combo_item);
            cboSemsFilter.DisplayMember = "DisplayText";
            cboSemsFilter.SelectedIndex = 0;
        }

        private void cboSemsFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SemsComboItem item = (SemsComboItem)cboSemsFilter.SelectedItem;
            if (item == null)
                return;
            ListCourses(item.GetCourseList());
        }

        private void ListCourses(List<ListViewItem> list)
        {
            listView.Items.Clear();

            if (list.Count <= 0)
                return;

            foreach (ListViewItem item in list)
                listView.Items.Add(item);
        }

        private void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (listView.FocusedItem == null) return;

            if (Control.ModifierKeys == Keys.Control && e.Item.Selected)
            {
                e.Item.Selected = false;
            }
        }

        //暫時註解吧
        //private void listView_DoubleClick(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (listView.FocusedItem == null) return;
        //        if (listView.FocusedItem.Tag == null) return;
        //        string id = listView.FocusedItem.Tag.ToString();
        //
        //        CourseEntity.Instance.DisplayDetail(int.Parse(id));
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        public override object Clone()
        {
            return new TeachCourseItem();
        }
    }

    class SemsComboItem
    {
        private string _display_text;
        public string DisplayText
        {
            get { return _display_text; }
            set { _display_text = value; }
        }

        private string _school_year;
        public string SchoolYear
        {
            get { return _school_year; }
            set { _school_year = value; }
        }

        private string _semester;
        public string Semester
        {
            get { return _semester; }
            set { _semester = value; }
        }

        private List<ListViewItem> _course_list;

        public SemsComboItem(string school_year, string semester, string display_text)
        {
            _display_text = display_text;
            _school_year = school_year;
            _semester = semester;
            _course_list = new List<ListViewItem>();
        }

        public void AddCourse(ListViewItem item)
        {
            _course_list.Add(item);
        }

        public List<ListViewItem> GetCourseList()
        {
            return _course_list;
        }
    }

    class SemsComboItemCollection : Dictionary<string, SemsComboItem>
    {
    }
}
