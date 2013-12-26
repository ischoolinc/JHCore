using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Framework;
using FISCA.Presentation;

namespace JHSchool.StudentExtendControls
{
    public partial class Teacher_View :FISCA.Presentation.NavView
    {
        public Teacher_View()
        {
            InitializeComponent();
            this.Text = "年級班級檢視";
        }

        #region NavView 成員

        public bool Active
        {
            get;
            set;
        }

        public string Description
        {
            get { return ""; }
        }

        public Control DisplayPane
        {
            get { return this; }
        }

        //public Control ConfigurationPane
        //{
        //    get { throw new NotImplementedException(); }
        //}

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

            //第一維記錄教師分類，第二維記錄年級下的教師編號
            SortedList<string, List<string>> TeacherCategory = new SortedList<string, List<string>>();

            //用來記錄未分類的教師
            List<string> TeacherUnCategory = new List<string>();

            //取得所有教師編號
            foreach (var key in PrimaryKeys)
            {
                //根據學生記錄取得班級記錄
                TeacherRecord  TeacherRec = Teacher.Instance[key];

                //根據班級記錄取得年級，若是年級為null則年級為空白
                string strTeacherCategory = (TeacherRec == null ? "" : TeacherRec.Category);

                //將gradeYear轉型成int
                if (!strTeacherCategory.Equals(""))
                {
                    if (!TeacherCategory.ContainsKey(strTeacherCategory))
                        TeacherCategory.Add(strTeacherCategory, new List<string>());
                    TeacherCategory[strTeacherCategory].Add(key);
                }
                else
                    TeacherUnCategory.Add(key);
            }

            foreach (string strCategory in TeacherCategory.Keys)
            {
                DevComponents.AdvTree.Node TeacherCategoryNode = new DevComponents.AdvTree.Node();

                TeacherCategoryNode.Text = strCategory;

                TeacherCategoryNode.Cells.Add(new DevComponents.AdvTree.Cell("" + TeacherCategory[strCategory].Count));

                items.Add(TeacherCategoryNode,TeacherCategory[strCategory]);

                this.advTree1.Nodes.Add(TeacherCategoryNode);
            }

            if (TeacherUnCategory.Count > 0)
            {

                DevComponents.AdvTree.Node TeacherCategoryNode = new DevComponents.AdvTree.Node() { Text = "未分類" };
                
                TeacherCategoryNode.Cells.Add(new DevComponents.AdvTree.Cell("" + TeacherUnCategory.Count));

                items.Add(TeacherCategoryNode,TeacherUnCategory);
                this.advTree1.Nodes.Add(TeacherCategoryNode);
            }

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
            foreach ( var item in nodeCollection )
            {
                if ( item is DevComponents.AdvTree.Node )
                {
                    var node = (DevComponents.AdvTree.Node)item;
                    if ( node.Text == selectPath[level] )
                    {
                        if ( selectPath.Count - 1 == level )
                            return node;
                        else
                        {
                            var childNode = SelectNode(selectPath, level + 1, node.Nodes);
                            if ( childNode == null )
                                return node;
                            else
                                return childNode;
                        }
                    }
                }
            }
            return null;
        }

        public event EventHandler<ListPaneSourceChangedEventArgs> ListPaneSourceChanged;

        #endregion
        private void advTree1_AfterNodeSelect(object sender, DevComponents.AdvTree.AdvTreeNodeEventArgs e)
        {
            if ( ListPaneSourceChanged != null )
            {
                if ( e.Node != null )
                {
                    ListPaneSourceChangedEventArgs args = new ListPaneSourceChangedEventArgs(items[e.Node]);
                    args.SelectedAll = ( Control.ModifierKeys & Keys.Control ) == Keys.Control;
                    args.AddToTemp = ( Control.ModifierKeys & Keys.Shift ) == Keys.Shift;
                    ListPaneSourceChanged(this, args);
                }
                else
                {
                    ListPaneSourceChangedEventArgs args = new ListPaneSourceChangedEventArgs(new List<string>());
                    args.SelectedAll = false;
                    args.AddToTemp = false;
                    ListPaneSourceChanged(this, args);
                }
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
