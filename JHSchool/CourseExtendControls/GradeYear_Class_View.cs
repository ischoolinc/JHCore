using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;

namespace JHSchool.CourseExtendControls
{
    public partial class GradeYear_Class_View : NavView
    {
        public GradeYear_Class_View()
        {
            InitializeComponent();
            NavText = "班級檢視";

            SourceChanged += new EventHandler(GradeYear_Class_View_SourceChanged);
        }

        private void GradeYear_Class_View_SourceChanged(object sender, EventArgs e)
        {
            Layout(new List<string>(Source));
        }

        #region NavView 成員
        private Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();
        public new void Layout(List<string> PrimaryKeys)
        {
            //選取的結點的完整路徑

            List<string> selectPath = new List<string>();
            #region 記錄選取的結點的完整路徑
            var selectNode = advTree1.SelectedNode;
            if (selectNode != null)
            {
                while (selectNode != null)
                {
                    selectPath.Insert(0, selectNode.Text);
                    selectNode = selectNode.Parent;
                }
            }
            #endregion
            advTree1.Nodes.Clear();
            items.Clear();
            SortedList<int?, List<string>> gradeYearList = new SortedList<int?, List<string>>();
            List<string> nullGradeList = new List<string>();
            List<string> nullClassList = new List<string>();
            Dictionary<ClassRecord, List<string>> classList = new Dictionary<ClassRecord, List<string>>();
            Dictionary<ClassRecord, int?> classGradeYear = new Dictionary<ClassRecord, int?>();
            List<ClassRecord> classes = new List<ClassRecord>();

            DevComponents.AdvTree.Node rootNode = new DevComponents.AdvTree.Node();

            rootNode.Text = "所有科目(" + PrimaryKeys.Count + ")";

            foreach (var key in PrimaryKeys)
            {
                var courseRec = Course.Instance[key];

                ClassRecord classRec = courseRec.Class;

                string gradeYear = (classRec == null ? "" : classRec.GradeYear);
                int gyear = 0;
                int? g;
                if (int.TryParse(gradeYear, out gyear))
                {
                    g = gyear;
                    if (!gradeYearList.ContainsKey(g))
                        gradeYearList.Add(g, new List<string>());
                    gradeYearList[g].Add(key);
                }
                else
                {
                    g = null;
                    nullGradeList.Add(key);
                }
                if (classRec != null)
                {
                    if (!classList.ContainsKey(classRec))
                    {
                        classList.Add(classRec, new List<string>());
                        classes.Add(classRec);
                    }
                    classList[classRec].Add(key);
                    if (!classGradeYear.ContainsKey(classRec))
                        classGradeYear.Add(classRec, g);
                }
                else
                    nullClassList.Add(key);
            }
            classes.Sort();

            foreach (var gyear in gradeYearList.Keys)
            {
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();
                switch (gyear)
                {
                    //case 1:
                    //    gyearNode.Text = "一年級";
                    //    break;
                    //case 2:
                    //    gyearNode.Text = "二年級";
                    //    break;
                    //case 3:
                    //    gyearNode.Text = "三年級";
                    //    break;
                    //case 4:
                    //    gyearNode.Text = "四年級";
                    //    break;
                    default:
                        gyearNode.Text = "" + gyear + "年級";
                        break;

                }

                gyearNode.Text += "(" + gradeYearList[gyear].Count + ")";

                items.Add(gyearNode, gradeYearList[gyear]);

                rootNode.Nodes.Add(gyearNode);

                foreach (var classRec in classes)
                {
                    if (classGradeYear[classRec] == gyear)
                    {
                        DevComponents.AdvTree.Node classNode = new DevComponents.AdvTree.Node();

                        classNode.Text = classRec.Name + "(" + classList[classRec].Count + ")";

                        items.Add(classNode, classList[classRec]);

                        gyearNode.Nodes.Add(classNode);
                    }
                }
            }
            if (nullGradeList.Count > 0)
            {
                DevComponents.AdvTree.Node gyearNode = new DevComponents.AdvTree.Node();

                gyearNode.Text = "未分年級(" + nullGradeList.Count + ")";

                items.Add(gyearNode, nullGradeList);

                rootNode.Nodes.Add(gyearNode);

                foreach (var classRec in classes)
                {
                    if (classGradeYear[classRec] == null)
                    {
                        DevComponents.AdvTree.Node classNode = new DevComponents.AdvTree.Node();

                        classNode.Text = classRec.Name + "(" + classList[classRec].Count + ")";

                        items.Add(classNode, classList[classRec]);

                        gyearNode.Nodes.Add(classNode);
                    }
                }
                if (nullClassList.Count > 0)
                {
                    DevComponents.AdvTree.Node classNode = new DevComponents.AdvTree.Node();

                    classNode.Text = "未分班(" + nullClassList.Count + ")";

                    items.Add(classNode, nullClassList);

                    gyearNode.Nodes.Add(classNode);
                }
            }

            rootNode.Expand();

            advTree1.Nodes.Add(rootNode);
            items.Add(rootNode, PrimaryKeys);

            if (selectPath.Count != 0)
            {
                selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (selectNode != null)
                    advTree1.SelectedNode = selectNode;
            }


            advTree1.Focus();
        }

        private DevComponents.AdvTree.Node SelectNode(List<string> selectPath, int level, DevComponents.AdvTree.NodeCollection nodeCollection)
        {
            foreach (var item in nodeCollection)
            {
                if (item is DevComponents.AdvTree.Node)
                {
                    var node = (DevComponents.AdvTree.Node)item;
                    if (node.Text == selectPath[level])
                    {
                        if (selectPath.Count - 1 == level)
                            return node;
                        else
                        {
                            var childNode = SelectNode(selectPath, level + 1, node.Nodes);
                            if (childNode == null)
                                return node;
                            else
                                return childNode;
                        }
                    }
                }
            }
            return null;
        }

        #endregion
        private void advTree1_AfterNodeSelect(object sender, DevComponents.AdvTree.AdvTreeNodeEventArgs e)
        {
            if (e.Node != null)
            {
                bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], SelectedAll, AddToTemp);
            }
            else
            {
                SetListPaneSource(new List<string>(), false, false);
            }
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            try
            {
                bool selAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool addToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], selAll, addToTemp);
            }
            catch (Exception) { }
        }

        private void advTree1_NodeDoubleClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            try
            {
                bool selAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool addToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], selAll, addToTemp);
            }
            catch (Exception) { }
        }

    }
}
