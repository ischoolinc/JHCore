using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Framework;

namespace JHSchool.StudentExtendControls
{
    public partial class CategoryView : NavView
    {
        public CategoryView()
        {
            InitializeComponent();
            this.NavText = "類別檢視";

            SourceChanged += new EventHandler(CategoryView_SourceChanged);
        }

        private void CategoryView_SourceChanged(object sender, EventArgs e)
        {
            Layout(new List<string>(Source));
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

            NavViewNode PrefixCategoryNode = new NavViewNode();
            
            PrefixCategoryNode.Name = "所有學生";

            NavViewNode NoPrefixCategoryNode = new NavViewNode();
            NavViewNode NoCategoryNode = new NavViewNode();

            StudentTag.Instance.SyncData(PrimaryKeys);

            //List<string> tmpPrKeys = new List<string>();

            //bool checkHasTag = false;
            foreach (var key in PrimaryKeys)
            {
                //checkHasTag = false;

                List<StudentTagRecord> TagRecords = Student.Instance.Items[key].GetTags();

                if (TagRecords.Count == 0)
                    NoPrefixCategoryNode["未分類學生"].PrimaryKeys.Add(key);
                else
                {
                    foreach (StudentTagRecord TagRecord in TagRecords)
                    {
                        string category = TagRecord.Name;
                        string prefix = TagRecord.Prefix;

                        //if (!prefix.Equals(string.Empty))
                        //    PrefixCategoryNode[prefix][category].PrimaryKeys.Add(key);
                        //else
                        //    NoPrefixCategoryNode[category].PrimaryKeys.Add(key);

                        if (!prefix.Equals(string.Empty) && !category.Equals(string.Empty))
                            PrefixCategoryNode[prefix][category].PrimaryKeys.Add(key);
                        else if (prefix.Equals(string.Empty) && !category.Equals(string.Empty))
                            NoPrefixCategoryNode[category].PrimaryKeys.Add(key);
   
                    }
                }
            }


            //foreach (var key in PrimaryKeys)
            //    if (!tmpPrKeys.Contains(key))
            //        NoCategoryNode["未分類別"].PrimaryKeys.Add(key);

            NavViewNode.NodePrimaryKeys.Clear();

            //PrefixCategoryNode.UpdateInstance(true);
            //NoPrefixCategoryNode.UpdateInstance(true);
            //NoCategoryNode.UpdateInstance(true);


            //foreach (NavViewNode Node in PrefixCategoryNode.Nodes.Values)
            //    advTree1.Nodes.Add(Node.InstanceNode);

            foreach(string key in NoPrefixCategoryNode.Nodes.Keys)
                PrefixCategoryNode[key].PrimaryKeys.AddRange(NoPrefixCategoryNode[key].PrimaryKeys);

            foreach (string key in NoCategoryNode.Nodes.Keys)
                PrefixCategoryNode[key].PrimaryKeys.AddRange(NoCategoryNode[key].PrimaryKeys);

            PrefixCategoryNode.UpdateInstance(false);

            PrefixCategoryNode.InstanceNode.Expand();

            advTree1.Nodes.Add(PrefixCategoryNode.InstanceNode);

            //foreach (NavViewNode Node in NoPrefixCategoryNode.Nodes.Values)
            //    advTree1.Nodes.Add(Node.InstanceNode);

            //foreach (NavViewNode node in NoCategoryNode.Nodes.Values)
            //    advTree1.Nodes.Add(node.InstanceNode);

            
            items = NavViewNode.NodePrimaryKeys;

            if (selectPath.Count != 0)
            {
                var MySelectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                //selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (MySelectNode != null)
                    advTree1.SelectedNode = MySelectNode;
            }
         //   advTree1.Focus();
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
                bool selAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
                bool addToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
                SetListPaneSource(items[e.Node], selAll, addToTemp);
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
