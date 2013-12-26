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
    internal static class EditTeacher
    {
        public static void SaveTeacherRecordEditor(IEnumerable<TeacherRecordEditor> editors)
        {
            DSXmlHelper insertHelper = new DSXmlHelper("InsertRequest");
            DSXmlHelper updateHelper = new DSXmlHelper("UpdateRequest");
            //SmartSchool.Teacher.Delete 實際上是 update teacher set status='256' where @@Condition
            DSXmlHelper deleteHelper = new DSXmlHelper("DeleteRequest");

            List<string> synclist = new List<string>();
            bool hasInsert = false, hasUpdate = false, hasDelete = false;

            foreach (var editor in editors)
            {
                if (editor.EditorStatus != EditorStatus.NoChanged)
                    synclist.Add(editor.ID);

                if (editor.EditorStatus == EditorStatus.Insert)
                {
                    insertHelper.AddElement("Teacher");
                    insertHelper.AddElement("Teacher", "Field");
                    insertHelper.AddElement("Teacher/Field", "TeacherName", editor.Name);
                    insertHelper.AddElement("Teacher/Field", "Nickname", editor.Nickname);
                    insertHelper.AddElement("Teacher/Field", "Gender", editor.Gender);
                    insertHelper.AddElement("Teacher/Field", "IDNumber", editor.IDNumber);
                    insertHelper.AddElement("Teacher/Field", "Category", editor.Category);
                    insertHelper.AddElement("Teacher/Field", "Status", editor.Status);
                    insertHelper.AddElement("Teacher/Field", "ContactPhone", editor.ContactPhone);

                    hasInsert = true;
                }
                else if (editor.EditorStatus == EditorStatus.Update)
                {
                    updateHelper.AddElement("Teacher");
                    updateHelper.AddElement("Teacher", "Field");
                    updateHelper.AddElement("Teacher/Field", "TeacherName", editor.Name);
                    updateHelper.AddElement("Teacher/Field", "Nickname", editor.Nickname);
                    updateHelper.AddElement("Teacher/Field", "Gender", editor.Gender);
                    updateHelper.AddElement("Teacher/Field", "IDNumber", editor.IDNumber);
                    updateHelper.AddElement("Teacher/Field", "Category", editor.Category);
                    updateHelper.AddElement("Teacher/Field", "Status", editor.Status);
                    updateHelper.AddElement("Teacher/Field", "ContactPhone", editor.ContactPhone);
                    updateHelper.AddElement("Teacher", "Condition");
                    updateHelper.AddElement("Teacher/Condition", "ID", editor.ID);

                    hasUpdate = true;
                }
                else if (editor.EditorStatus == EditorStatus.Delete)
                {
                    deleteHelper.AddElement("Teacher");
                    deleteHelper.AddElement("Teacher", "ID", editor.ID);

                    hasDelete = true;
                }
            }

            if (hasInsert)
            {
                DSXmlHelper response = DSAServices.CallService("SmartSchool.Teacher.Insert", new DSRequest(insertHelper.BaseElement)).GetContent();
                foreach (XmlElement each in response.GetElements("NewID"))
                    synclist.Add(each.InnerText);
            }

            if (hasUpdate)
                DSAServices.CallService("SmartSchool.Teacher.Update", new DSRequest(updateHelper.BaseElement));

            if (hasDelete)
                DSAServices.CallService("SmartSchool.Teacher.Delete", new DSRequest(deleteHelper.BaseElement));

            Teacher.Instance.SyncDataBackground(synclist);
        }
        
        internal static string AddTeacher(string name)
        {
            string sid = "";
            DSXmlHelper req = new DSXmlHelper("InsertRequest");

            req.AddElement("Teacher");
            req.AddElement("Teacher", "Field");
            //req.AddElement("Student", "Field");
            req.AddElement("Teacher/Field", "TeacherName", name);
            DSXmlHelper rsp = DSAServices.CallService("SmartSchool.Teacher.Insert", new DSRequest(req.BaseElement)).GetContent();
            foreach (XmlElement xm in rsp.GetElements("NewID"))
                sid = xm.InnerText;
            return sid;
        }

        internal static void DelTeacher(string TeacherID)
        {   
            DSXmlHelper req = new DSXmlHelper("DeleteRequest");
            req.AddElement("Teacher");
            req.AddElement("Teacher", "Condition");
            req.AddElement("Teacher/Condition", "ID", TeacherID);
            DSAServices.CallService("SmartSchool.Teacher.Delete", new DSRequest(req.BaseElement));
        
        }

        internal static void SaveTeacherTagRecordEditor(IEnumerable<JHSchool.Editor.TeacherTagRecordEditor> editors)
        {
            MultiThreadWorker<JHSchool.Editor.TeacherTagRecordEditor> worker = new MultiThreadWorker<JHSchool.Editor.TeacherTagRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<JHSchool.Editor.TeacherTagRecordEditor> e)
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
                        deleteHelper.AddElement("Tag", "RefTeacherID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);

                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefTeacherID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefTeacherID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Tag");
                        deleteHelper.AddElement("Tag", "RefTeacherID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);
                    }
                    #endregion
                }

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Tag.AddTeacherTag", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Tag.RemoveTeacherTag", new DSRequest(deleteHelper.BaseElement));
            };
            List<PackageWorkEventArgs<JHSchool.Editor.TeacherTagRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<JHSchool.Editor.TeacherTagRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }    
}
