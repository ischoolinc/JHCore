using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;

namespace JHSchool.InternalExtendControls.Tagging
{
    public partial class TagEditor : BaseForm
    {
        private JHTagConfigRecord _OldTagRec;
        private enum ManageMode
        {
            Update, Insert
        }

        private ManageMode _mode;
        
        private JHTagConfigRecord _current_tag;

        public TagEditor(K12.Data.TagCategory category)
        {
            InitializeComponent();

            _mode = ManageMode.Insert;
            _current_tag = new JHTagConfigRecord();
            _current_tag.Category = category.ToString ();
        }

        public TagEditor(JHTagConfigRecord tag)
        {
            InitializeComponent();

            _mode = ManageMode.Update;
            _current_tag = tag;
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult = DialogResult.None;
                _OldTagRec = new JHTagConfigRecord ();
                _OldTagRec.Prefix = _current_tag.Prefix;
                _OldTagRec.Name = _current_tag.Name;
                _OldTagRec.ColorCode = _current_tag.ColorCode;

                _current_tag.Prefix = cboGroups.Text.Trim();
                _current_tag.Name = txtName.Text.Trim();
                _current_tag.Color = cpColor.SelectedColor;

                if (string.IsNullOrEmpty(_current_tag.Name))
                {
                    FISCA.Presentation.Controls.MsgBox.Show("您必須輸入類別名稱。");
                    return;
                }

                foreach (JHTagConfigRecord each in JHTagConfig.SelectAll ())
                {
                    if (each == _current_tag)
                        continue;
                    
                    if(_mode == ManageMode.Insert )
                    if (_current_tag.FullName == each.FullName && _current_tag.Category.ToUpper() == each.Category.ToUpper())
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("名稱重覆，請選擇其他名稱。");
                        return;
                    }
                }
                PermRecLogProcess prlp = new PermRecLogProcess();

                if (_mode == ManageMode.Insert)
                {
                    // Log
                    prlp.SaveLog("學籍.類別管理", "類別管理新增類別", "新增 "+_current_tag.Category+" 類別,名稱:" + _current_tag.FullName);

                    JHTagConfig.Insert(_current_tag);
                }
                else
                {
                    // Log
                    bool checkEdit = false;
                    string strLogName = "更新 "+_current_tag.Category + " 類別,";
                    if (_OldTagRec.FullName != _current_tag.FullName)
                    {
                        strLogName += "名稱由「" + _OldTagRec.FullName + "」改為「" + _current_tag.FullName + "」,";
                        checkEdit = true;
                    }
                    if (_OldTagRec.ColorCode != _current_tag.ColorCode)
                    {
                        strLogName += "修改" + _current_tag.FullName + "顏色。";
                        checkEdit = true;
                    }

                    if(checkEdit)
                        prlp.SaveLog("學籍.類別管理", "類別管理更新類別", strLogName);

                    JHTagConfig.Update(_current_tag);
                }

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show(ex.Message);
                DialogResult = DialogResult.None;
            }
        }

        private void TagInsert_Load(object sender, EventArgs e)
        {
            if (_mode == ManageMode.Insert)
                Text = "新增類別";
            else
                Text = "修改類別";

            cboGroups.Text = string.Empty;
            txtName.Text = string.Empty;
            cpColor.SelectedColor = Color.White;

            InitGroupCombobox();

            if (_mode == ManageMode.Insert) return; //_current_item 是 null 代表是使用「Insert」模式。

            int group_index = -1;

            if (!string.IsNullOrEmpty(_current_tag.Prefix)) //如果 Prefix 是空字串的話，就不要選擇任何項目，因為這是為分類的 Prefix。
                group_index = cboGroups.FindString(_current_tag.Prefix);

            cboGroups.SelectedIndex = group_index;
            txtName.Text = _current_tag.Name;
            cpColor.SelectedColor = _current_tag.Color;
        }

        private void InitGroupCombobox()
        {
            cboGroups.SelectedItem = null;
            cboGroups.Items.Clear();

            List<string> groups = new List<string>();

            foreach (JHTagConfigRecord each in JHTagConfig.SelectAll ())
            {
                if (each.Category.ToUpper() == _current_tag.Category.ToUpper() && !groups.Contains(each.Prefix))
                    groups.Add(each.Prefix);
            }
            groups.Sort();

            cboGroups.Items.AddRange(groups.ToArray());
        }

        public static DialogResult ModifyTag(JHTagConfigRecord tag)
        {
            TagEditor editor = new TagEditor(tag);            
            return editor.ShowDialog();
        }

        public static DialogResult InsertTag(K12.Data.TagCategory category)
        {
            TagEditor editor = new TagEditor(category);

            return editor.ShowDialog();
        }
    }
}