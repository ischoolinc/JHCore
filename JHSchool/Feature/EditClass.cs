using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Editor;
using Framework;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.Authentication;

namespace JHSchool.Feature
{
    internal static class EditClass
    {
        public static void SaveClassRecordEditor(IEnumerable<ClassRecordEditor> editors)
        {
            MultiThreadWorker<ClassRecordEditor> worker = new MultiThreadWorker<ClassRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<ClassRecordEditor> e)
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
                        updateHelper.AddElement("Class");
                        updateHelper.AddElement("Class", "Field");
                        updateHelper.AddElement("Class/Field", "ClassName", editor.Name);
                        updateHelper.AddElement("Class/Field", "NamingRule", editor.NamingRule);
                        updateHelper.AddElement("Class/Field", "GradeYear", editor.GradeYear);
                        updateHelper.AddElement("Class/Field", "RefDepartmentID", editor.RefDepartmentID);
                        updateHelper.AddElement("Class/Field", "RefGraduationPlanID", editor.RefProgramPlanID);
                        updateHelper.AddElement("Class/Field", "RefScoreCalcRuleID", editor.RefScoreCalcRuleID);
                        updateHelper.AddElement("Class/Field", "RefTeacherID", editor.RefTeacherID);
                        updateHelper.AddElement("Class/Field", "DisplayOrder", editor.DisplayOrder);
                        updateHelper.AddElement("Class/Field", "ClassNumber", editor.DisplayOrder);
                        updateHelper.AddElement("Class", "Condition");
                        updateHelper.AddElement("Class/Condition", "ID", editor.ID);

                        hasUpdate = true;
                        synclist.Add(editor.ID);
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Class");
                        insertHelper.AddElement("Class", "Field");
                        insertHelper.AddElement("Class/Field", "ClassName", editor.Name);
                        insertHelper.AddElement("Class/Field", "NamingRule", editor.NamingRule);
                        insertHelper.AddElement("Class/Field", "GradeYear", editor.GradeYear);
                        insertHelper.AddElement("Class/Field", "RefDepartmentID", editor.RefDepartmentID);
                        insertHelper.AddElement("Class/Field", "RefGraduationPlanID", editor.RefProgramPlanID);
                        insertHelper.AddElement("Class/Field", "RefScoreCalcRuleID", editor.RefScoreCalcRuleID);
                        insertHelper.AddElement("Class/Field", "RefTeacherID", editor.RefTeacherID);
                        insertHelper.AddElement("Class/Field", "DisplayOrder", editor.DisplayOrder);
                        insertHelper.AddElement("Class/Field", "ClassNumber", editor.DisplayOrder);

                        hasInsert = true;
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Class");
                        deleteHelper.AddElement("Class", "ID", editor.ID);

                        hasDelete = true;
                        synclist.Add(editor.ID);
                    }
                    #endregion
                }

                if (hasUpdate)
                    DSAServices.CallService("SmartSchool.Class.Update", new DSRequest(updateHelper.BaseElement));

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Class.Insert", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Class.Delete", new DSRequest(deleteHelper.BaseElement));

                Class.Instance.SyncDataBackground(synclist);
            };
            List<PackageWorkEventArgs<ClassRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<ClassRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }

        internal static void SaveClassTagRecordEditor(IEnumerable<JHSchool.Editor.ClassTagRecordEditor> editors)
        {
            MultiThreadWorker<JHSchool.Editor.ClassTagRecordEditor> worker = new MultiThreadWorker<JHSchool.Editor.ClassTagRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<JHSchool.Editor.ClassTagRecordEditor> e)
            {
                DSXmlHelper updateHelper = new DSXmlHelper("Request");
                DSXmlHelper insertHelper = new DSXmlHelper("Request");
                DSXmlHelper deleteHelper = new DSXmlHelper("Request");
                List<string> synclist = new List<string>(); //這個目前沒作用
                bool hasInsert = false, hasDelete = false;

                foreach (var editor in e.List)
                {
                    #region 更新
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                    {
                        deleteHelper.AddElement("Tag");
                        deleteHelper.AddElement("Tag", "RefClassID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);

                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefClassID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefClassID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Tag");
                        deleteHelper.AddElement("Tag", "RefClassID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);
                    }
                    #endregion
                }

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Tag.AddClassTag", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Tag.RemoveClassTag", new DSRequest(deleteHelper.BaseElement));
            };
            List<PackageWorkEventArgs<JHSchool.Editor.ClassTagRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<JHSchool.Editor.ClassTagRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
