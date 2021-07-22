using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SmartSchool.Feature.Class;
using SmartSchool.Feature.Teacher;
using FISCA.DSAUtil;
using JHSchool.Legacy.ImportSupport;
using JHSchool.Feature.Legacy;

namespace JHSchool.CourseExtendControls.Ribbon.CourseImportWizardControls
{
    internal class ImportDataAccess : IDataAccess
    {
        public XmlElement GetImportFieldList()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_Course_ImportFieldList);
            return doc.DocumentElement;
            //return SmartSchool.Feature.Course.CourseBulkProcess.GetImportDescription();
        }

        public XmlElement GetValidateFieldRule()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(Properties.Resources.JH_Course_FieldValidationRule);
            return doc.DocumentElement;
            //return SmartSchool.Feature.Course.CourseBulkProcess.GetFieldValidationRule();
        }

        public XmlElement GetUniqueFieldData()
        {
            return SmartSchool.Feature.Course.CourseBulkProcess.GetPrimaryKeyList();
        }

        public XmlElement GetShiftCheckList(params string[] fieldNameList)
        {
            return SmartSchool.Feature.Course.CourseBulkProcess.GetShiftCheckList(fieldNameList);
        }

        public void InsertImportData(XmlElement data)
        {
            SmartSchool.Feature.Course.CourseBulkProcess.InsertImportCourse(data);
        }

        public void UpdateImportData(XmlElement data)
        {
            SmartSchool.Feature.Course.CourseBulkProcess.UpdateImportCourse(data);
        }

        public void AddCourseTeachers(XmlElement request)
        {
            EditCourse.AddCourseTeacher(new DSXmlHelper(request));
        }

        public void RemoveCourseTeachers(XmlElement request)
        {
            EditCourse.RemoveCourseTeachers(new DSXmlHelper(request));
        }

        public XmlElement GetCourseTeachers(IEnumerable<string> fieldNameList)
        {
            return SmartSchool.Feature.Course.CourseBulkProcess.GetCourseTeachers(fieldNameList);
        }
    }
}
