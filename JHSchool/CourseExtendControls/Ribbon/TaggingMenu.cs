using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.Presentation;
using JHSchool.Editor;
using Framework;

namespace JHSchool.CourseExtendControls.Ribbon
{
    internal class TaggingMenu
    {
        /// <summary>
        /// 指定類別的權限代碼。
        /// </summary>
        private string AssignCode { get; set; }
        /// <summary>
        /// 管理類別的權限代碼。
        /// </summary>
        private string ManageCode { get; set; }

        /// <param name="assignCode">指定類別的權限代碼。</param>
        /// <param name="manageCode">管理類別的權限代碼。</param>
        internal TaggingMenu(string assignCode, string manageCode)
        {
            AssignCode = assignCode;
            ManageCode = manageCode;
        }

        internal void MenuOpen(object sender, PopupOpenEventArgs e)
        {
            if (!Tag.Instance.Loaded)
                Tag.Instance.SyncAllBackground();

            if (Course.Instance.SelectedList.Count > 0)
            {
                Course.Instance.SelectedList.SyncTagCache();
                List<string> prefixes = new List<string>();
                foreach (TagRecord each in Tag.Instance.Items.GetTagsBy(TagCategory.Course))
                {
                    string prefix = string.IsNullOrEmpty(each.Prefix) ? "<未分類>" : each.Prefix;

                    MenuButton mb = e.VirtualButtons[prefix][each.Name];
                    mb.Tag = each;

                    mb.AutoCheckOnClick = true;
                    mb.AutoCollapseOnClick = false;

                    MenuButton prefixMenuButton = e.VirtualButtons[prefix];
                    if (!prefixes.Contains(prefix))
                    {
                        prefixes.Add(prefix);
                        prefixMenuButton.PopupOpen += delegate
                        {
                            if (string.IsNullOrEmpty("" + prefixMenuButton.Tag))
                            {
                                Dictionary<string, int> temp = new Dictionary<string, int>();
                                foreach (var item in Course.Instance.SelectedList)
                                {
                                    foreach (var tag in item.GetTags())
                                    {
                                        if (!temp.ContainsKey(tag.RefTagID))
                                            temp.Add(tag.RefTagID, 0);
                                        temp[tag.RefTagID]++;
                                    }
                                }
                                prefixMenuButton.Tag = temp;
                            }

                            Dictionary<string, int> tags = prefixMenuButton.Tag as Dictionary<string, int>;
                            int count = Course.Instance.SelectedList.Count;
                            foreach (var item in prefixMenuButton.Items)
                            {
                                string tagID = (item.Tag as TagRecord).ID;
                                if (tags.ContainsKey(tagID) && tags[tagID] == count)
                                    item.Checked = true;
                                else
                                    item.Checked = false;
                            }
                        };
                    }

                    e.VirtualButtons[prefix][each.Name].Enable = User.Acl[AssignCode].Executable;
                    e.VirtualButtons[prefix][each.Name].Click += delegate
                    {
                        List<CourseTagRecordEditor> editors = new List<CourseTagRecordEditor>();
                        TagRecord record = mb.Tag as TagRecord;

                        foreach (CourseRecord item in Course.Instance.SelectedList)
                        {
                            Dictionary<string, CourseTagRecord> list = new Dictionary<string, CourseTagRecord>();
                            foreach (CourseTagRecord tag in item.GetTags())
                                list.Add(tag.RefTagID, tag);

                            if (mb.Checked == true)
                            {
                                if (!list.ContainsKey(record.ID))
                                    editors.Add(item.AddTag(record));
                            }
                            else
                            {
                                if (list.ContainsKey(record.ID))
                                {
                                    CourseTagRecordEditor editor = list[record.ID].GetEditor();
                                    editor.Remove = true;
                                    editors.Add(editor);
                                }
                            }
                        }

                        if (editors.Count > 0)
                        {
                            editors.SaveAllEditors();

                            //發生更新事件
                            CourseTagEvents.RaiseTagChanged();

                            prefixMenuButton.Tag = null;
                            Course.Instance.SelectedList.SyncTagCache();
                            // 畫面重新整理
                            Course.Instance.SyncAllBackground();
                        }
                    };
                }
            }

            e.VirtualButtons["類別管理..."].BeginGroup = true;
            e.VirtualButtons["類別管理..."].Enable = User.Acl[ManageCode].Executable;
            e.VirtualButtons["類別管理..."].Click += delegate
            {
                new JHSchool.CourseExtendControls.Ribbon.CourseTagForm().ShowDialog();
            };

        }
    }
}
