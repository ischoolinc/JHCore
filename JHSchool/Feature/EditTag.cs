using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;
using FISCA.DSAUtil;
using Framework;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature
{
    internal class EditTag
    {
        public static void SaveTagRecordEditor(params TagRecordEditor[] editors)
        {
            SaveTagRecordEditor((IEnumerable<TagRecordEditor>)editors);
        }

        public static void SaveTagRecordEditor(IEnumerable<TagRecordEditor> editors)
        {
            MultiThreadWorker<TagRecordEditor> worker = new MultiThreadWorker<TagRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<TagRecordEditor> e)
            {
                DSXmlHelper updateHelper = new DSXmlHelper("Request");
                DSXmlHelper insertHelper = new DSXmlHelper("Request");
                DSXmlHelper deleteHelper = new DSXmlHelper("Request");
                List<string> synclist = new List<string>();
                bool hasUpdate = false, hasInsert = false, hasDelete = false;

                foreach (var editor in e.List)
                {
                    #region 更新
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                    {
                        updateHelper.AddElement("Tag");
                        updateHelper.AddElement("Tag", "Field");
                        updateHelper.AddElement("Tag/Field", "Prefix", editor.Prefix);
                        updateHelper.AddElement("Tag/Field", "Name", editor.Name);
                        updateHelper.AddElement("Tag/Field", "Color", editor.ColorCode.ToString());
                        updateHelper.AddElement("Tag", "Condition");
                        updateHelper.AddElement("Tag/Condition", "ID", editor.ID);

                        hasUpdate = true;
                        synclist.Add(editor.ID);
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "Field");
                        insertHelper.AddElement("Tag", "Prefix", editor.Prefix);
                        insertHelper.AddElement("Tag", "Name", editor.Name.ToString());
                        insertHelper.AddElement("Tag", "Color", editor.ColorCode.ToString());
                        insertHelper.AddElement("Tag", "Category", editor.Category);

                        hasInsert = true;
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Tag");
                        deleteHelper.AddElement("Tag", "ID", editor.ID);

                        hasDelete = true;
                        synclist.Add(editor.ID);
                    }
                    #endregion
                }

                if (hasUpdate)
                    DSAServices.CallService("SmartSchool.Tag.Update", new DSRequest(updateHelper.BaseElement));

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Tag.Insert", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Tag.Delete", new DSRequest(deleteHelper.BaseElement));

                Tag.Instance.SyncDataBackground(synclist);
            };
            List<PackageWorkEventArgs<TagRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<TagRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
