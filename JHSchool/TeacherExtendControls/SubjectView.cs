using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;

namespace JHSchool.TeacherExtendControls
{
    public partial class SubjectView : NavView
    {
        public SubjectView()
        {
            InitializeComponent();
            this.Text = "類別檢視";
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

            DevComponents.AdvTree.Node rootNode = new DevComponents.AdvTree.Node();
            rootNode.Text = "所有教師(" + PrimaryKeys.Count + ")";
           

            SortedList<string, List<string>> categoryList = new SortedList<string, List<string>>();
            List<string> noCategroyList = new List<string>();

            foreach (var key in PrimaryKeys)
            {
                var teacherRec = Teacher.Instance.Items[key];

                string category = teacherRec.Category;

                if (!string.IsNullOrEmpty(category))
                {
                    if (!categoryList.ContainsKey(category))
                        categoryList.Add(category, new List<string>());
                    categoryList[category].Add(key);
                }
                else
                {
                    noCategroyList.Add(key);
                }
            }

            foreach (var categoryKey in categoryList.Keys)
            {
                DevComponents.AdvTree.Node categoryNode = new DevComponents.AdvTree.Node();
                categoryNode.Text = categoryKey+"(" + categoryList[categoryKey].Count+")";
                items.Add(categoryNode, categoryList[categoryKey]);
                rootNode.Nodes.Add(categoryNode);
            }

            if (noCategroyList.Count > 0)
            {
                DevComponents.AdvTree.Node categoryNode = new DevComponents.AdvTree.Node();
                categoryNode.Text = "未分類("+noCategroyList.Count+")";
                items.Add(categoryNode, noCategroyList);
                rootNode.Nodes.Add(categoryNode);
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

           // advTree1.Focus();
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

        public event EventHandler<ListPaneSourceChangedEventArgs> ListPaneSourceChanged;

        #endregion

        private void advTree1_AfterNodeSelect(object sender, DevComponents.AdvTree.AdvTreeNodeEventArgs e)
        {
            //if (ListPaneSourceChanged != null)
            //{
                if (e.Node != null)
                {
                    ListPaneSourceChangedEventArgs args = new ListPaneSourceChangedEventArgs(items[e.Node]);
                    args.SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                    args.AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                    ListPaneSourceChanged(this, args);
                }
                else
                {
                    ListPaneSourceChangedEventArgs args = new ListPaneSourceChangedEventArgs(new List<string>());
                    args.SelectedAll = false;
                    args.AddToTemp = false;
                    ListPaneSourceChanged(this, args);
                }
            //}
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
