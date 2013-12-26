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
    public class EditCourse
    {
        public static void SaveCourseRecordEditor(IEnumerable<CourseRecordEditor> editors)
        {
            MultiThreadWorker<CourseRecordEditor> worker = new MultiThreadWorker<CourseRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<CourseRecordEditor> e)
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
                        updateHelper.AddElement("Course");
                        updateHelper.AddElement("Course", "Field");
                        updateHelper.AddElement("Course/Field", "CourseName", editor.Name);
                        updateHelper.AddElement("Course/Field", "SchoolYear", editor.SchoolYear.ToString());
                        updateHelper.AddElement("Course/Field", "Semester", editor.Semester.ToString());
                        updateHelper.AddElement("Course/Field", "Subject", editor.Subject);
                        updateHelper.AddElement("Course/Field", "Domain", editor.Domain);
                        updateHelper.AddElement("Course/Field", "Period", editor.Period);
                        updateHelper.AddElement("Course/Field", "Credit", editor.Credit);
                        updateHelper.AddElement("Course/Field", "RefClassID", editor.RefClassID);
                        updateHelper.AddElement("Course/Field", "RefExamTemplateID", editor.RefAssessmentSetupID);
                        updateHelper.AddElement("Course/Field", "ScoreCalcFlag", editor.CalculationFlag);
                        updateHelper.AddElement("Course", "Condition");
                        updateHelper.AddElement("Course/Condition", "ID", editor.ID);

                        hasUpdate = true;
                        synclist.Add(editor.ID);
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Course");
                        //insertHelper.AddElement("Course", "Field");
                        insertHelper.AddElement("Course", "CourseName", editor.Name);
                        insertHelper.AddElement("Course", "SchoolYear", editor.SchoolYear.ToString());
                        insertHelper.AddElement("Course", "Semester", editor.Semester.ToString());
                        insertHelper.AddElement("Course", "Subject", editor.Subject);
                        insertHelper.AddElement("Course", "Domain", editor.Domain);
                        insertHelper.AddElement("Course", "Period", editor.Period);
                        insertHelper.AddElement("Course", "Credit", editor.Credit);
                        insertHelper.AddElement("Course", "RefClassID", editor.RefClassID);
                        insertHelper.AddElement("Course", "RefExamTemplateID", editor.RefAssessmentSetupID);
                        insertHelper.AddElement("Course", "ScoreCalcFlag", editor.CalculationFlag);

                        hasInsert = true;
                        //synclist.Add(editor.ID);
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Course");
                        deleteHelper.AddElement("Course", "ID", editor.ID);

                        hasDelete = true;
                        synclist.Add(editor.ID);
                    }
                    #endregion
                }

                if (hasUpdate)
                    DSAServices.CallService("SmartSchool.Course.Update", new DSRequest(updateHelper.BaseElement));

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Course.Insert", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Course.Delete", new DSRequest(deleteHelper.BaseElement));

                Course.Instance.SyncDataBackground(synclist);
            };
            List<PackageWorkEventArgs<CourseRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<CourseRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }

        internal static void SaveCourseTagRecordEditor(IEnumerable<JHSchool.Editor.CourseTagRecordEditor> editors)
        {
            MultiThreadWorker<JHSchool.Editor.CourseTagRecordEditor> worker = new MultiThreadWorker<JHSchool.Editor.CourseTagRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<JHSchool.Editor.CourseTagRecordEditor> e)
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
                        deleteHelper.AddElement("Tag", "RefCourseID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);

                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefCourseID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefCourseID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Tag");
                        deleteHelper.AddElement("Tag", "RefCourseID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);
                    }
                    #endregion
                }

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Tag.AddCourseTag", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Tag.RemoveCourseTag", new DSRequest(deleteHelper.BaseElement));
            };
            List<PackageWorkEventArgs<JHSchool.Editor.CourseTagRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<JHSchool.Editor.CourseTagRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }
    }
}
