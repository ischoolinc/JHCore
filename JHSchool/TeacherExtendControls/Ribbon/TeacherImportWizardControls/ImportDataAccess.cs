using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using SmartSchool.Feature.Class;
using SmartSchool.Feature.Teacher;
using FISCA.DSAUtil;
using JHSchool.Legacy.ImportSupport;

namespace JHSchool.TeacherExtendControls.Ribbon.TeacherImportWizardControls
{
    internal class ImportDataAccess : IDataAccess
    {
        public XmlElement GetImportFieldList()
        {
            return TeacherBulkProcess.GetImportFieldList();
        }

        public XmlElement GetValidateFieldRule()
        {
            return TeacherBulkProcess.GetFieldValidationRule();
        }

        public XmlElement GetUniqueFieldData()
        {
            return TeacherBulkProcess.GetUniqueFieldData();
        }

        public XmlElement GetShiftCheckList(params string[] fieldNameList)
        {
            return TeacherBulkProcess.GetShiftCheckList(fieldNameList);
        }

        public void InsertImportData(XmlElement data)
        {
            TeacherBulkProcess.InsertImportTeacher(data);
        }

        public void UpdateImportData(XmlElement data)
        {
            TeacherBulkProcess.UpdateImportTeacher(data);
        }
    }
}
