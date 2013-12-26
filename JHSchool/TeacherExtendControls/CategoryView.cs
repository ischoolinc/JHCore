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
    public partial class CategoryView : NavView
    {
        public CategoryView()
        {
            InitializeComponent();
            NavText = "類別檢視";

            SourceChanged += new EventHandler(CategoryView_SourceChanged);
        }

        private void CategoryView_SourceChanged(object sender, EventArgs e)
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

            NavViewNode PrefixCategoryNode = new NavViewNode();

            PrefixCategoryNode.Name = "所有教師";

            NavViewNode NoPrefixCategoryNode = new NavViewNode();
            NavViewNode NoPrefixNoCategoryNode = new NavViewNode();
            NavViewNode DelPrefixNoCategoryNode = new NavViewNode();

            TeacherTag.Instance.SyncData(PrimaryKeys);

            List<JHSchool.Data.JHTeacherRecord> TeacherRecs= JHSchool.Data.JHTeacher.SelectAll();
            
            List<string> DeletedTeacherIDList = new List<string> ();
            foreach (JHSchool.Data.JHTeacherRecord tr in TeacherRecs)
            {
                if (tr.Status == K12.Data.TeacherRecord.TeacherStatus.刪除)
                    DeletedTeacherIDList.Add(tr.ID);
            }

            foreach (var key in PrimaryKeys)
            {
                // 過濾刪除教師
                if (DeletedTeacherIDList.Contains(key))
                {
                    DelPrefixNoCategoryNode["刪除教師"].PrimaryKeys.Add(key);
                    continue;
                }

                List<TeacherTagRecord> TagRecords = Teacher.Instance.Items[key].GetTags();
                
                if (TagRecords.Count == 0)
                    NoPrefixNoCategoryNode["未分類教師"].PrimaryKeys.Add(key);
                else
                {
                    foreach (TeacherTagRecord TagRecord in TagRecords)
                    {
                        string category = TagRecord.Name;
                        string prefix = TagRecord.Prefix;

                        if (!prefix.Equals(string.Empty) && !category.Equals(string.Empty))
                            PrefixCategoryNode[prefix][category].PrimaryKeys.Add(key);
                        else if (prefix.Equals(string.Empty) && !category.Equals(string.Empty))
                            NoPrefixCategoryNode[category].PrimaryKeys.Add(key);
                    }
                }
            }

            NavViewNode.NodePrimaryKeys.Clear();

            foreach (string key in NoPrefixCategoryNode.Nodes.Keys)
                PrefixCategoryNode[key].PrimaryKeys.AddRange(NoPrefixCategoryNode[key].PrimaryKeys);

            foreach (string key in NoPrefixNoCategoryNode.Nodes.Keys)
                PrefixCategoryNode[key].PrimaryKeys.AddRange(NoPrefixNoCategoryNode[key].PrimaryKeys);

            // 加入刪除篩選
            foreach (string key in DelPrefixNoCategoryNode.Nodes.Keys)
                PrefixCategoryNode[key].PrimaryKeys.AddRange(DelPrefixNoCategoryNode[key].PrimaryKeys);


            PrefixCategoryNode.UpdateInstance(false);

            PrefixCategoryNode.InstanceNode.Expand();

            advTree1.Nodes.Add(PrefixCategoryNode.InstanceNode);

            items = NavViewNode.NodePrimaryKeys;

            if (selectPath.Count != 0)
            {
                selectNode = SelectNode(selectPath, 0, advTree1.Nodes);
                if (selectNode != null)
                    advTree1.SelectedNode = selectNode;
            }
            //advTree1.Focus();
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
