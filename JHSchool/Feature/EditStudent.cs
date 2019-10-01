using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using Framework;
using System.Xml;
using FISCA.Authentication;
using JHSchool.Feature.Legacy;

namespace JHSchool.Feature
{
    public static class EditStudent
    {
        public static void SaveStudentRecordEditor(IEnumerable<JHSchool.Editor.StudentRecordEditor> editors)
        {
            DSXmlHelper updateHelper = new DSXmlHelper("UpdateStudentList");
            DSXmlHelper insertHelper = new DSXmlHelper("InsertRequest");
            List<LogInfo> logs = new List<LogInfo>();
            List<string> reflashList = new List<string>();
            bool hasUpdate = false;
            foreach (var editor in editors)
            {
                if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Update)
                {
                    #region 更新
                    LogInfo log = new LogInfo() { Action = "修改學生資料", Entity = "Student", EntityID = editor.ID };
                    string description = "";
                    updateHelper.AddElement("Student");
                    updateHelper.AddElement("Student", "Field");
                    if (editor.Birthday != editor.Student.Birthday)
                    {
                        updateHelper.AddElement("Student/Field", "Birthdate", editor.Birthday);
                        description += "生日 由\"" + editor.Student.Birthday + "\"變更為\"" + editor.Birthday + "\"";
                    }
                    if (editor.Gender != editor.Student.Gender)
                    {
                        updateHelper.AddElement("Student/Field", "Gender", editor.Gender);
                        description += "性別 由\"" + editor.Student.Gender + "\"變更為\"" + editor.Gender + "\"";
                    }
                    if (editor.IDNumber != editor.Student.IDNumber)
                    {
                        updateHelper.AddElement("Student/Field", "IDNumber", editor.IDNumber);
                        description += "身分證號 由\"" + editor.Student.IDNumber + "\"變更為\"" + editor.IDNumber + "\"";
                    }
                    if (editor.Name != editor.Student.Name)
                    {
                        updateHelper.AddElement("Student/Field", "Name", editor.Name);
                        description += "姓名 由\"" + editor.Student.Name + "\"變更為\"" + editor.Name + "\"";
                    }
                    if (editor.SeatNo != editor.Student.SeatNo)
                    {
                        updateHelper.AddElement("Student/Field", "SeatNo", editor.SeatNo);
                        description += "座號 由\"" + editor.Student.SeatNo + "\"變更為\"" + editor.SeatNo + "\"";
                    }
                    if (editor.Status != editor.Student.Status)
                    {
                        updateHelper.AddElement("Student/Field", "Status", editor.Status);
                        description += "狀態 由\"" + editor.Student.Status + "\"變更為\"" + editor.Status + "\"";
                    }
                    if (editor.StudentNumber != editor.Student.StudentNumber)
                    {
                        updateHelper.AddElement("Student/Field", "StudentNumber", editor.StudentNumber);
                        description += "學號 由\"" + editor.Student.StudentNumber + "\"變更為\"" + editor.StudentNumber + "\"";
                    }
                    if (editor.RefClassID != editor.Student.RefClassID)
                    {
                        updateHelper.AddElement("Student/Field", "RefClassID", editor.RefClassID);
                        description += "班級 由\"" + (editor.Student.Class == null ? "" : editor.Student.Class.Name) + "\"變更為\"" + (JHSchool.Class.Instance.Items[editor.RefClassID] == null ? "" : JHSchool.Class.Instance.Items[editor.RefClassID].Name) + "\"";
                    }
                    //if (editor.OverrideDepartmentID != editor.Student.OverrideDepartmentID)
                    //{
                    //    updateHelper.AddElement("Student/Field", "OverrideDeptID", editor.OverrideDepartmentID);
                    //    description += "指定科別 由\"" + (JHSchool.Department.Instance[editor.Student.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.Student.OverrideDepartmentID].FullName) + "\"變更為\"" + (JHSchool.Department.Instance[editor.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.OverrideDepartmentID].FullName) + "\"";
                    //}
                    if (editor.Nationality != editor.Student.Nationality)
                    {
                        updateHelper.AddElement("Student/Field", "Nationality", editor.Nationality);
                        description += "國籍 由\"" + editor.Student.Nationality + "\"變更為\"" + editor.Nationality + "\"";
                    }


                    //天啊，這個 Log 怎麼寫比較好。

                    //if (editor.OverrideProgramPlanID != editor.Student.OverrideProgramPlanID)
                    //{
                    //    updateHelper.AddElement("Student/Field", "RefGraduationPlanID", editor.OverrideProgramPlanID);
                    //    description += "指定課程規劃 由\"" + (JHSchool.Department.Instance[editor.Student.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.Student.OverrideDepartmentID].FullName) + "\"變更為\"" + (JHSchool.Department.Instance[editor.OverrideDepartmentID] == null ? "" : JHSchool.Department.Instance[editor.OverrideDepartmentID].FullName) + "\"";
                    //}
                    updateHelper.AddElement("Student/Field", "RefGraduationPlanID", "" + editor.OverrideProgramPlanID);
                    updateHelper.AddElement("Student/Field", "RefScoreCalcRuleID", "" + editor.OverrideScoreCalcRuleID);

                    updateHelper.AddElement("Student", "Condition");
                    updateHelper.AddElement("Student/Condition", "ID", editor.ID);
                    log.Description = description;
                    logs.Add(log);
                    hasUpdate = true;
                    reflashList.Add(editor.ID);
                    #endregion
                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        throw new NotImplementedException("新增學生尚未實作");
                    }
                    #endregion
                }
                if (hasUpdate)
                {
                    DSAServices.CallService("SmartSchool.Student.Update", new DSRequest(updateHelper.BaseElement));
                    Framework.LogInfoExtendFunctions.SaveAll(logs);
                    Student.Instance.SyncDataBackground(reflashList);
                }
            }
        }
        public static void SaveSemesterHistoryRecordEditor(IEnumerable<JHSchool.Editor.SemesterHistoryRecordEditor> editors)
        {
            Dictionary<string, List<JHSchool.Editor.SemesterHistoryRecordEditor>> grouped = new Dictionary<string, List<JHSchool.Editor.SemesterHistoryRecordEditor>>();
            foreach (var editor in editors)
            {
                if (!grouped.ContainsKey(editor.RefStudentID))
                    grouped.Add(editor.RefStudentID, new List<JHSchool.Editor.SemesterHistoryRecordEditor>());
                grouped[editor.RefStudentID].Add(editor);
            }
            SemesterHistory.Instance.SyncData(grouped.Keys);

            DSXmlHelper helper = new DSXmlHelper("UpdateStudentList");
            bool hasChanged = false;
            
            List<LogInfo> logs = new List<LogInfo>();
            foreach (var primaryKey in grouped.Keys)
            {
                List<SemesterInfo> contentsSemester = new List<SemesterInfo>();
                Dictionary<SemesterInfo, JHSchool.Editor.SemesterHistoryRecordEditor> changedValues = new Dictionary<SemesterInfo, JHSchool.Editor.SemesterHistoryRecordEditor>();
                Dictionary<SemesterInfo, SemesterHistoryRecord> values = new Dictionary<SemesterInfo, SemesterHistoryRecord>();

                foreach (var item in grouped[primaryKey])//把這個人的變更資料依學期整裡好
                {
                    if (item.EditorStatus == JHSchool.Editor.EditorStatus.NoChanged) continue;//沒變更的就跳過

                    SemesterInfo semester = new SemesterInfo() { SchoolYear = item.SchoolYear, Semester = item.Semester };
                    if (!contentsSemester.Contains(semester))
                        contentsSemester.Add(semester);
                    if (!changedValues.ContainsKey(semester))
                        changedValues.Add(semester, item);
                    else
                        changedValues[semester] = item;
                }
                if (changedValues.Count == 0) continue;//更本沒有資料有變更就換下一個人

                helper.AddElement("Student");

                foreach (var item in SemesterHistory.Instance[primaryKey])//把原來就有的學期資料整裡好
                {
                    SemesterInfo semester = new SemesterInfo() { SchoolYear = item.SchoolYear, Semester = item.Semester };
                    if (!contentsSemester.Contains(semester))
                        contentsSemester.Add(semester);
                    if (!values.ContainsKey(semester))
                        values.Add(semester, item);
                    else
                        values[semester] = item;
                }
                contentsSemester.Sort();
                hasChanged = true;
                helper.AddElement("Student", "Field");
                helper.AddElement("Student/Field", "SemesterHistory");
                helper.AddElement("Student", "Condition");
                helper.AddElement("Student/Condition", "ID", primaryKey);
                foreach (var semester in contentsSemester)
                {
                    if (changedValues.ContainsKey(semester))
                    {
                        if (changedValues[semester].EditorStatus != JHSchool.Editor.EditorStatus.Delete)
                        {
                            XmlElement element = helper.AddElement("Student/Field/SemesterHistory", "History");
                            element.SetAttribute("SchoolYear", "" + semester.SchoolYear);
                            element.SetAttribute("Semester", "" + semester.Semester);
                            element.SetAttribute("GradeYear", "" + changedValues[semester].GradeYear);
                            element.SetAttribute("SeatNo", "" + changedValues[semester].SeatNo);
                            element.SetAttribute("ClassName", "" + changedValues[semester].ClassName);
                            element.SetAttribute("Teacher", "" + changedValues[semester].Teacher);
                            element.SetAttribute("SchoolDayCount", "" + changedValues[semester].SchoolDayCount);
                            if (values.ContainsKey(semester))
                            {
                                LogInfo logInfo = new LogInfo() { Action = "修改學期歷程", Entity = "Student", EntityID = primaryKey };
                                logInfo.Description = "修改 「" + semester.SchoolYear + "」學年度 第「" + semester.Semester + "」學期" +
                                    (changedValues[semester].GradeYear != values[semester].GradeYear ? ("\n\t年級由「" + values[semester].GradeYear + "」年級變更為「" + changedValues[semester].GradeYear + "」年級") : "") +
                                    (changedValues[semester].ClassName != values[semester].ClassName ? ("\n\t班級由「" + values[semester].ClassName + "」變更為「" + changedValues[semester].ClassName + "」") : "") +
                                    (changedValues[semester].SeatNo != values[semester].SeatNo ? ("\n\t座號由「" + values[semester].SeatNo + "」變更為「" + changedValues[semester].SeatNo + "」") : "") +
                                    (changedValues[semester].Teacher != values[semester].Teacher ? ("\n\t班導師由「" + values[semester].Teacher + "」變更為「" + changedValues[semester].Teacher + "」") : "") + (changedValues[semester].SchoolDayCount  != values[semester].SchoolDayCount  ? ("\n\t上課天數由「" + values[semester].SchoolDayCount  + "」變更為「" + changedValues[semester].SchoolDayCount  + "」") : "");
                                logs.Add(logInfo);
                            }
                            else
                            {
                                LogInfo logInfo = new LogInfo() { Action = "新增學期歷程", Entity = "Student", EntityID = primaryKey };
                                logInfo.Description = "新增 「" + semester.SchoolYear + "」學年度 第「" + semester.Semester + "」學期" +
                                   "\n\t年級為「" + changedValues[semester].GradeYear + "」年級" +
                                    "\n\t班級為「" + changedValues[semester].ClassName + "」" +
                                    "\n\t座號為「" + changedValues[semester].SeatNo + "」" +
                                    "\n\t班導師為「" + changedValues[semester].Teacher + "」"+
                                    "\n\t上課天數為「" + changedValues[semester].SchoolDayCount + "」";
                                logs.Add(logInfo);
                            }
                        }
                        else
                        {
                            logs.Add(new LogInfo() { Action = "刪除學期歷程", Entity = "Student", EntityID = primaryKey, Description = "刪除 「" + semester.SchoolYear + "」學年度 第「" + semester.Semester + "」學期學期歷程。" });
                        }
                    }
                    else
                    {
                        XmlElement element = helper.AddElement("Student/Field/SemesterHistory", "History");
                        element.SetAttribute("SchoolYear", "" + semester.SchoolYear);
                        element.SetAttribute("Semester", "" + semester.Semester);
                        element.SetAttribute("GradeYear", "" + values[semester].GradeYear);
                        element.SetAttribute("SeatNo", "" + values[semester].SeatNo);
                        element.SetAttribute("ClassName", "" + values[semester].ClassName);
                        element.SetAttribute("Teacher", "" + values[semester].Teacher);
                        element.SetAttribute("SchoolDayCount", "" + values[semester].SchoolDayCount);
                    }
                }
            }
            if (hasChanged)
            {
                DSAServices.CallService("SmartSchool.Student.Update", new DSRequest(helper.BaseElement));
                Framework.LogInfoExtendFunctions.SaveAll(logs);
                SemesterHistory.Instance.SyncDataBackground(grouped.Keys);
            }
        }

        internal static string AddStudent(string name)
        {
            string sid = "";
            DSXmlHelper req = new DSXmlHelper("InsertRequest");

            req.AddElement("Student");
            req.AddElement("Student", "Field");
            //req.AddElement("Student", "Field");
            req.AddElement("Student/Field", "Name", name);
            DSXmlHelper rsp = DSAServices.CallService("SmartSchool.Student.Insert", new DSRequest(req.BaseElement)).GetContent();
            foreach (XmlElement xm in rsp.GetElements("NewID"))
                sid = xm.InnerText;
            return sid;

            //SmartSchool.student.insert
            //Student.Instance.SyncDataBackground()
        }
        internal static void DelStudent(string StudentID)
        {

            DSXmlHelper req = new DSXmlHelper("UpdateStudentList");
            req.AddElement("Student");
            req.AddElement("Student", "Field");
            req.AddElement("Student/Field", "Status", "刪除");
            req.AddElement("Student", "Condition");
            req.AddElement("Student/Condition", "ID", StudentID);
            DSAServices.CallService("SmartSchool.Student.Update", new DSRequest(req.BaseElement));
        }

        internal static void SaveStudentTagRecordEditor(IEnumerable<JHSchool.Editor.StudentTagRecordEditor> editors)
        {
            MultiThreadWorker<JHSchool.Editor.StudentTagRecordEditor> worker = new MultiThreadWorker<JHSchool.Editor.StudentTagRecordEditor>();
            worker.MaxThreads = 3;
            worker.PackageSize = 100;
            worker.PackageWorker += delegate(object sender, PackageWorkEventArgs<JHSchool.Editor.StudentTagRecordEditor> e)
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
                        deleteHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);

                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 新增
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Insert)
                    {
                        insertHelper.AddElement("Tag");
                        insertHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
                        insertHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasInsert = true;
                    }
                    #endregion

                    #region 刪除
                    if (editor.EditorStatus == JHSchool.Editor.EditorStatus.Delete)
                    {
                        deleteHelper.AddElement("Tag");
                        deleteHelper.AddElement("Tag", "RefStudentID", editor.RefEntityID);
                        deleteHelper.AddElement("Tag", "RefTagID", editor.RefTagID);

                        hasDelete = true;
                        synclist.Add(editor.RefEntityID);
                    }
                    #endregion
                }

                if (hasInsert)
                {
                    DSXmlHelper response = DSAServices.CallService("SmartSchool.Tag.AddStudentTag", new DSRequest(insertHelper.BaseElement)).GetContent();
                    foreach (XmlElement each in response.GetElements("NewID"))
                        synclist.Add(each.InnerText);
                }

                if (hasDelete)
                    DSAServices.CallService("SmartSchool.Tag.RemoveStudentTag", new DSRequest(deleteHelper.BaseElement));
            };
            List<PackageWorkEventArgs<JHSchool.Editor.StudentTagRecordEditor>> packages = worker.Run(editors);
            foreach (PackageWorkEventArgs<JHSchool.Editor.StudentTagRecordEditor> each in packages)
            {
                if (each.HasException)
                    throw each.Exception;
            }
        }

        [QueryRequest()]
        public static void ChangeStudentStatus(string newStatus, params string[] ids)
        {
            if (ids.Length > 0)
            {
                string req = "<ChangeStatusRequest><Student><Field><Status>" + newStatus + "</Status></Field><Condition>";
                foreach (string id in ids)
                {
                    req += "<ID>" + id + "</ID>";
                }
                req += "</Condition></Student></ChangeStatusRequest>";
                DSRequest dsreq = new DSRequest(req);
                DSResponse dsrsp = DSAServices.CallService("SmartSchool.Student.Update", dsreq);
            }
        }
    }
}
